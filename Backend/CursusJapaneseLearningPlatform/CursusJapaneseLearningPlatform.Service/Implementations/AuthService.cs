using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Interfaces;
using CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Requests;
using CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Responses;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using CursusJapaneseLearningPlatform.Service.Commons.Interfaces;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using CursusJapaneseLearningPlatform.Service.Commons.Implementations;
using Newtonsoft.Json.Linq;

namespace CursusJapaneseLearningPlatform.Service.Implementations;
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITimeService _timeService;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly int _exAccessToken;
    private readonly int _exRefreshToken;
    private readonly IRedisService _redisService;
    private readonly IFirebaseService _firebaseService;
    private readonly IEmailService _mailService;
    private readonly IAWSS3Service _awsS3Service;
    public AuthService(IUnitOfWork unitOfWork, ITimeService timeService, IMapper mapper, UserManager<User> userManager, RoleManager<Role> roleManager, IConfiguration configuration, IRedisService redisService, IFirebaseService firebaseService, IEmailService mailService, IAWSS3Service awsS3Service)
    {
        _unitOfWork = unitOfWork;
        _timeService = timeService;
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _redisService = redisService;
        _exAccessToken = int.Parse(_configuration["JwtSettings:ExpirationAccessToken"]!);
        _exRefreshToken = int.Parse(_configuration["JwtSettings:ExpirationRefreshToken"]!);
        _firebaseService = firebaseService;
        _mailService = mailService;
        _awsS3Service = awsS3Service;
    }

    public async Task<LoginResponse> CreateUserAsync(UserRequestModel userRequestModel)
    {
        try
        {
            if (await _userManager.Users.AnyAsync(x => x.UserName == userRequestModel.EmailAddress))
                throw new CustomException(StatusCodes.Status400BadRequest, ResponseCodeConstants.DUPLICATE, "User đã tồn tại.");

            var newUser = _mapper.Map<User>(userRequestModel);
            newUser.IsActive = false;
            newUser.IsDelete = false;
            IdentityResult resultCreateUser = await _userManager.CreateAsync(newUser, userRequestModel.Password);
            if (!resultCreateUser.Succeeded)
                throw new CustomException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "Đã có lỗi xảy ra");

            // Gán vai trò admin cho tài khoản
            IdentityResult resultAddRole = await _userManager.AddToRoleAsync(newUser, "Learner");
            if (!resultAddRole.Succeeded)
                throw new CustomException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "Đã có lỗi xảy ra");

            var user = await _userManager.FindByNameAsync(userRequestModel.EmailAddress) ??
                    throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tên đăng nhập hoặc mật khẩu không đúng");

            var checkPassword = await _userManager.CheckPasswordAsync(user, userRequestModel.Password);
            if (!checkPassword)
            {
                throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "Tên đăng nhập hoặc mật khẩu không đúng");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault();
            if (userRole == null)
            {
                throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.UNAUTHORIZED, "Nguời dùng chưa được cấp quyền");
            }

            // Generate access token
            var accessToken = GenerateToken(user, userRole, false);
            // Generate refresh token
            var refreshToken = GenerateToken(user, userRole, true);

            // Save tokens to Redis with expiration
            await _redisService.SetValueAsync($"AccessToken:{user.Id}", accessToken);
            await _redisService.SetValueAsync($"RefreshToken:{user.Id}", refreshToken);

            // Set expiration times for the tokens in Redis
            await _redisService.SetValueAsync($"AccessToken:{user.Id}:Expiry", _timeService.SystemTimeNow.AddMinutes(_exAccessToken).ToString());
            await _redisService.SetValueAsync($"RefreshToken:{user.Id}:Expiry", _timeService.SystemTimeNow.AddHours(_exRefreshToken).ToString());

            string domainFrontend = _configuration["Domain:UrlFrontEnd"];
            string url = $"{domainFrontend}authenticate/verify/{accessToken}";
            bool sendMail = await _mailService.SendVerifyEmailAsync(user.EmailAddress, "Xác thực tài khoản", user.FullName, user.EmailAddress, url);

            if (sendMail)
            {
                return new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    UserId = user.Id,
                    EmailAddress = user.EmailAddress,
                    FullName = user.FullName,
                    Role = userRole
                };
            }

            else
            {
                throw new CustomException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "Đã có lỗi xảy ra");
            }
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception)
        {
            _unitOfWork.RollBack();
            throw new CustomException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, ResponseMessages.INTERNAL_SERVER_ERROR);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
    }


    public async Task<LoginResponse> LoginAsync(LoginModel loginModel)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(loginModel.EmailAddress) ??
                    throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tên đăng nhập hoặc mật khẩu không đúng");

            var checkPassword = await _userManager.CheckPasswordAsync(user, loginModel.Password);
            if (!checkPassword)
            {
                throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "Tên đăng nhập hoặc mật khẩu không đúng");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault();
            if (userRole == null)
            {
                throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.UNAUTHORIZED, "Nguời dùng chưa được cấp quyền");
            }

            // Generate access token
            var accessToken = GenerateToken(user, userRole, false);
            // Generate refresh token
            var refreshToken = GenerateToken(user, userRole, true);

            // Save tokens to Redis with expiration
            await _redisService.SetValueAsync($"AccessToken:{user.Id}", accessToken);
            await _redisService.SetValueAsync($"RefreshToken:{user.Id}", refreshToken);

            // Set expiration times for the tokens in Redis
            await _redisService.SetValueAsync($"AccessToken:{user.Id}:Expiry", _timeService.SystemTimeNow.AddMinutes(_exAccessToken).ToString());
            await _redisService.SetValueAsync($"RefreshToken:{user.Id}:Expiry", _timeService.SystemTimeNow.AddHours(_exRefreshToken).ToString());

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
                EmailAddress = user.EmailAddress,
                FullName = user.FullName,
                Role = userRole
            };
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception)
        {
            _unitOfWork.RollBack();
            throw new CustomException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, ResponseMessages.INTERNAL_SERVER_ERROR);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
    }

    public async Task<LoginResponse> LoginWithGoogleAsync(string firebaseToken)
    {
        try
        {
            // Verify Firebase token
            var decodedToken = await _firebaseService.VerifyTokenAsync(firebaseToken);
            string email = decodedToken.Claims["email"].ToString();

            // Find user by email
            var user = await _userManager.FindByNameAsync(email);

            if (user != null)
            {
                // Existing user - proceed directly to login
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault();

                if (userRole == null)
                {
                    throw new CustomException(
                        StatusCodes.Status403Forbidden,
                        ResponseCodeConstants.UNAUTHORIZED,
                        "Người dùng chưa được cấp quyền"
                    );
                }

                // Generate tokens and proceed with login
                var accessToken = GenerateToken(user, userRole, false);
                var refreshToken = GenerateToken(user, userRole, true);

                // Save to Redis
                await _redisService.SetValueAsync($"AccessToken:{user.Id}", accessToken);
                await _redisService.SetValueAsync($"RefreshToken:{user.Id}", refreshToken);
                await _redisService.SetValueAsync($"AccessToken:{user.Id}:Expiry",
                    _timeService.SystemTimeNow.AddMinutes(_exAccessToken).ToString());
                await _redisService.SetValueAsync($"RefreshToken:{user.Id}:Expiry",
                    _timeService.SystemTimeNow.AddHours(_exRefreshToken).ToString());


                return new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    UserId = user.Id,
                    EmailAddress = user.EmailAddress,
                    FullName = user.FullName,
                    Role = userRole
                };
            }
            else
            {
                // New user - proceed with onboarding
                // Find default role for Google users
                IdentityRole<Guid>? role = await _roleManager.FindByNameAsync("Learner");
                if (role == null)
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Vai trò không tồn tại.");

                // Create new user with proper initialization
                user = new User
                {
                    UserName = email,
                    Email = email,
                    EmailAddress = email,
                    FullName = decodedToken.Claims["name"]?.ToString() ?? email,
                    IsActive = true,
                    IsDelete = false,
                    CreatedTime = _timeService.SystemTimeNow,
                    LastUpdatedTime = _timeService.SystemTimeNow,
                    CreatedBy = "Google Auth",
                    LastUpdatedBy = "Google Auth",
                    ImagePath = decodedToken.Claims["picture"]?.ToString()
                };

                // Create user without password for Google auth
                IdentityResult resultCreateUser = await _userManager.CreateAsync(user);
                if (!resultCreateUser.Succeeded)
                    throw new CustomException(StatusCodes.Status500InternalServerError,
                        ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                        "Đã có lỗi xảy ra");

                // Assign role
                IdentityResult resultAddRole = await _userManager.AddToRoleAsync(user, role.Name!);
                if (!resultAddRole.Succeeded)
                    throw new CustomException(StatusCodes.Status500InternalServerError,
                        ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                        "Đã có lỗi xảy ra");

                // Generate tokens for new user
                var accessToken = GenerateToken(user, role.Name!, false);
                var refreshToken = GenerateToken(user, role.Name!, true);

                // Save to Redis
                await _redisService.SetValueAsync($"AccessToken:{user.Id}", accessToken);
                await _redisService.SetValueAsync($"RefreshToken:{user.Id}", refreshToken);
                await _redisService.SetValueAsync($"AccessToken:{user.Id}:Expiry",
                    _timeService.SystemTimeNow.AddMinutes(_exAccessToken).ToString());
                await _redisService.SetValueAsync($"RefreshToken:{user.Id}:Expiry",
                    _timeService.SystemTimeNow.AddHours(_exRefreshToken).ToString());


                return new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    UserId = user.Id,
                    EmailAddress = user.EmailAddress,
                    FullName = user.FullName,
                    Role = role.Name!
                };
            }
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception)
        {
            _unitOfWork.RollBack();
            throw new CustomException(
                StatusCodes.Status500InternalServerError,
                ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                ResponseMessages.INTERNAL_SERVER_ERROR
            );
        }
        finally
        {
            _unitOfWork.Dispose();
        }
    }


    private string GenerateToken(User user, string role, bool isRefreshToken)
    {
        try
        {
            var keyString = _configuration["JwtSettings:SecretKey"]
                           ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "JWT key is not configured.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Email", user.EmailAddress.ToString()),
                    new Claim("Role", role),
                    new Claim(ClaimTypes.Role, role) 
                    //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            if (isRefreshToken)
            {
                claims.Add(new Claim("isRefreshToken", "true"));
            }

            DateTime expiresDateTime;
            if (isRefreshToken)
            {
                expiresDateTime = _timeService.SystemTimeNow.AddHours(_exRefreshToken).DateTime;
            }
            else
            {
                expiresDateTime = _timeService.SystemTimeNow.AddMinutes(_exAccessToken).DateTime;
            }


            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expiresDateTime,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new CustomException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "Đã xảy ra lỗi không mong muốn khi lưu.");
        }
    }

    public async Task<bool> IsTokenValid(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true
            }, out SecurityToken validatedToken);

            return true; // Token hợp lệ
        }
        catch
        {
            return false; // Token không hợp lệ hoặc hết hạn
        }
    }

    public ClaimsPrincipal DecodeJWT(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (handler.CanReadToken(token))
        {
            // Đọc và kiểm tra JWT token
            var jwtToken = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtToken.Claims);
            return new ClaimsPrincipal(identity);
        }

        throw new SecurityTokenException("Invalid token");
    }

    public async Task<LoginResponse> VerifyEmail(string token)
    {
        try
        {
            if (token == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Token không hợp lệ");
            }
            //Decode token
            Guid id = Guid.Parse(DecodeJWT(token).FindFirst("Id")?.Value ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Token không hợp lệ"));
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id) ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người dùng không tồn tại");

            if (user == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người dùng không tồn tại");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault();

            string accessToken = await _redisService.GetValueAsync($"AccessToken:{user.Id}");
            string refreshToken = await _redisService.GetValueAsync($"RefreshToken:{user.Id}");
            string newAccessToken;
            string newRefreshToken;
            //Nếu access token còn tồn tại thì trả về cả 2 token nếu không thì kiểm tra RefreshToken
            if (accessToken != null)
            {
                newAccessToken = accessToken;
                newRefreshToken = refreshToken;
            }
            else
            {
                //Nếu RefreshToken còn tồn tại thì taọ ra 2 token mới
                if (refreshToken != null)
                {
                    if (userRole == null)
                    {
                        throw new CustomException(
                            StatusCodes.Status403Forbidden,
                            ResponseCodeConstants.UNAUTHORIZED,
                            "Người dùng chưa được cấp quyền"
                        );
                    }
                    // Generate access token
                    newAccessToken = GenerateToken(user, userRole, false);
                    // Generate refresh token
                    newRefreshToken = GenerateToken(user, userRole, true);

                    // Save tokens to Redis with expiration
                    await _redisService.SetValueAsync($"AccessToken:{user.Id}", newAccessToken);
                    await _redisService.SetValueAsync($"RefreshToken:{user.Id}", newRefreshToken);

                    // Set expiration times for the tokens in Redis
                    await _redisService.SetValueAsync($"AccessToken:{user.Id}:Expiry", _timeService.SystemTimeNow.AddMinutes(_exAccessToken).ToString());
                    await _redisService.SetValueAsync($"RefreshToken:{user.Id}:Expiry", _timeService.SystemTimeNow.AddHours(_exRefreshToken).ToString());

                }
                else
                {
                    throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bạn cần phải đăng nhặp lại bởi vì bạn đã hết hạn truy cặp.");
                }
            }

            // Save Database và chỉ is active = true
            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                UserId = user.Id,
                EmailAddress = user.EmailAddress,
                FullName = user.FullName,
                Role = userRole
            };
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception)
        {
            _unitOfWork.RollBack();
            throw new CustomException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, ResponseMessages.INTERNAL_SERVER_ERROR);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
    }

    public async Task<LoginResponse> RefreshAccessToken(string oldAccessToken, string oldRefreshToken)
    {
        try
        {
            //Decode token
            Guid id = Guid.Parse(DecodeJWT(oldRefreshToken).FindFirst("Id")?.Value ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Token không hợp lệ"));
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id) ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người dùng không tồn tại");

            if (user == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người dùng không tồn tại");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault();

            string accessToken = await _redisService.GetValueAsync($"AccessToken:{user.Id}");
            string refreshToken = await _redisService.GetValueAsync($"RefreshToken:{user.Id}");
            string newAccessToken;
            string newRefreshToken;
            //Nếu access token còn tồn tại thì trả về cả 2 token nếu không thì kiểm tra RefreshToken
            if (accessToken != null)
            {
                newAccessToken = accessToken;
                newRefreshToken = refreshToken;
            }
            else
            {
                //Nếu RefreshToken còn tồn tại thì taọ ra 2 token mới
                if (refreshToken != null)
                {
                    if (userRole == null)
                    {
                        throw new CustomException(
                            StatusCodes.Status403Forbidden,
                            ResponseCodeConstants.UNAUTHORIZED,
                            "Người dùng chưa được cấp quyền"
                        );
                    }
                    // Generate access token
                    newAccessToken = GenerateToken(user, userRole, false);
                    // Generate refresh token
                    newRefreshToken = GenerateToken(user, userRole, true);

                    // Save tokens to Redis with expiration
                    await _redisService.SetValueAsync($"AccessToken:{user.Id}", newAccessToken);
                    await _redisService.SetValueAsync($"RefreshToken:{user.Id}", newRefreshToken);

                    // Set expiration times for the tokens in Redis
                    await _redisService.SetValueAsync($"AccessToken:{user.Id}:Expiry", _timeService.SystemTimeNow.AddMinutes(_exAccessToken).ToString());
                    await _redisService.SetValueAsync($"RefreshToken:{user.Id}:Expiry", _timeService.SystemTimeNow.AddHours(_exRefreshToken).ToString());

                }
                else
                {
                    throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bạn cần phải đăng nhặp lại bởi vì bạn đã hết hạn truy cặp.");
                }
            }


            // Save Database
            await _userManager.UpdateAsync(user);

            return new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                UserId = user.Id,
                EmailAddress = user.EmailAddress,
                FullName = user.FullName,
                Role = userRole
            };
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception)
        {
            _unitOfWork.RollBack();
            throw new CustomException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, ResponseMessages.INTERNAL_SERVER_ERROR);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
    }

    public async Task InitializeAdminAccountAsync()
    {
        string adminEmail = _configuration["AccountAdmin:AdminEmail"];
        string adminPassword = _configuration["AccountAdmin:AdminPassword"];

        var adminUser = await _userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            // Tạo tài khoản admin mới
            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailAddress = adminEmail,
                FullName = "Admin User", // Tên đầy đủ có thể được tùy chỉnh
                IsActive = true,
                IsDelete = false,
                CreatedTime = DateTimeOffset.UtcNow,
                LastUpdatedTime = DateTimeOffset.UtcNow,
                CreatedBy = "System", // Ai tạo ra
                LastUpdatedBy = "System", // Ai cập nhật lần cuối
            };

            var result = await _userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
            {
                // Xử lý lỗi nếu cần
                throw new Exception("Failed to create admin account: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Gán vai trò admin cho tài khoản
            await _userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    public async Task<UserDetailResponse> GetUserDetail(Guid id)
    {
        try
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id) ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người dùng không tồn tại");
            if (user == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, ResponseMessages.NOT_FOUND);
            }
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault();
            if (userRole == null)
            {
                throw new CustomException(StatusCodes.Status403Forbidden, ResponseCodeConstants.UNAUTHORIZED, "Nguời dùng chưa được cấp quyền");
            }

            string url = null;
            if (_awsS3Service.IsImagePathValid(user.ImagePath))
            {
                url = await _awsS3Service.GetFileUrl(user.ImagePath, 60);
            }
            if (url != null)
            {
                return new UserDetailResponse
                {
                    Id = user.Id,
                    ImagePath = url,
                    EmailAddress = user.EmailAddress,
                    FullName = user.FullName,
                    Role = userRole,
                    IsActive = user.IsActive,
                    IsDelete = user.IsDelete
                };
            }
            else
            {
                return new UserDetailResponse
                {
                    Id = user.Id,
                    ImagePath = user.ImagePath,
                    EmailAddress = user.EmailAddress,
                    FullName = user.FullName,
                    Role = userRole
                };
            }
            
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception)
        {
            _unitOfWork.RollBack();
            throw new CustomException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, ResponseMessages.INTERNAL_SERVER_ERROR);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
    }

    public async Task<bool> ForgotPassword(string email)
    {
        var user = await _userManager.FindByNameAsync(email);

        if (user != null)
        {
            // Existing user - proceed directly to login
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault();

            if (userRole == null)
            {
                throw new CustomException(
                    StatusCodes.Status403Forbidden,
                    ResponseCodeConstants.UNAUTHORIZED,
                    "Người dùng chưa được cấp quyền"
                );
            }

            // Generate tokens and proceed with login
            string accessToken = await _redisService.GetValueAsync($"AccessToken:{user.Id}");
            string refreshToken = await _redisService.GetValueAsync($"RefreshToken:{user.Id}");
            string newAccessToken;
            string newRefreshToken;
            //Nếu access token còn tồn tại thì trả về cả 2 token nếu không thì kiểm tra RefreshToken
            if (accessToken != null)
            {
                newAccessToken = accessToken;
                newRefreshToken = refreshToken;
            }
            else
            {
                newAccessToken = GenerateToken(user, userRole, false);
                // Generate refresh token
                newRefreshToken = GenerateToken(user, userRole, true);

                // Save tokens to Redis with expiration
                await _redisService.SetValueAsync($"AccessToken:{user.Id}", newAccessToken);
                await _redisService.SetValueAsync($"RefreshToken:{user.Id}", newRefreshToken);

                // Set expiration times for the tokens in Redis
                await _redisService.SetValueAsync($"AccessToken:{user.Id}:Expiry", _timeService.SystemTimeNow.AddMinutes(_exAccessToken).ToString());
                await _redisService.SetValueAsync($"RefreshToken:{user.Id}:Expiry", _timeService.SystemTimeNow.AddHours(_exRefreshToken).ToString());
            }

            string domainFrontend = _configuration["Domain:UrlFrontEnd"];
            string url = $"{domainFrontend}/authenticate/forgot-password/{newAccessToken}";
            bool sendMail = await _mailService.SendForgotPasswordEmailAsync(user.EmailAddress, "Password Reset", user.FullName, user.EmailAddress, url);
            if (sendMail) {
                return true;
            }
            else
            {
                throw new CustomException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "Đã có lỗi xảy ra");
            }
        } 
        else
        {
            throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người dùng không tồn tại");
        }
    }

    public async Task<bool> ResetPassword(ResetPasswordRequestModel model)
    {
        try
        {
            //Decode token
            Guid id = Guid.Parse(DecodeJWT(model.VerificationToken).FindFirst("Id")?.Value ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Token không hợp lệ"));
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id) ?? throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người dùng không tồn tại");

            if (user == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người dùng không tồn tại");
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, model.Password);
            if (result.Succeeded)
            {
                //user.EmailCode = null; 
                await _userManager.UpdateAsync(user);
                var selectedEmail = new List<string> { user.EmailAddress };
                var time = _timeService.SystemTimeNow.ToString("yyyy-MM-dd HH:mm:ss");
                await _mailService.SendEmailAsync(selectedEmail, "THAY ĐỔI MẬT KHẨU THÀNH CÔNG", $"Your password has changed successfully at: {time}");
                return true;
            }
            return false;
        }
        catch (CustomException)
        {
            _unitOfWork.RollBack();
            throw;
        }
        catch (Exception)
        {
            _unitOfWork.RollBack();
            throw CustomExceptionFactory.CreateInternalServerError();
        }
        finally
        {
            _unitOfWork.Dispose();
        }
    }
}
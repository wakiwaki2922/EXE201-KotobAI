using CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Requests;
using CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Responses;
using FirebaseAdmin.Auth;

namespace CursusJapaneseLearningPlatform.Service.Interfaces;
public interface IAuthService
{
    Task<LoginResponse> CreateUserAsync(UserRequestModel userRequestModel);
    Task<LoginResponse> LoginAsync(LoginModel loginModel);
    Task<LoginResponse> LoginWithGoogleAsync(string firebaseToken);
    Task<bool> IsTokenValid(string token);
    Task<LoginResponse> VerifyEmail(string token);
    Task<LoginResponse> RefreshAccessToken(string accessToken, string refreshToken);
    Task InitializeAdminAccountAsync();
    Task<UserDetailResponse> GetUserDetail(Guid id);
    Task<bool> ForgotPassword(string email);
    Task<bool> ResetPassword(ResetPasswordRequestModel model);
}

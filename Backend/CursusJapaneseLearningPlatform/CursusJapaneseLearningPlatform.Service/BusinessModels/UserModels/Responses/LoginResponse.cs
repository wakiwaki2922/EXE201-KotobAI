namespace CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Responses;
public class LoginResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public Guid UserId { get; set; }
    public string EmailAddress { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
}

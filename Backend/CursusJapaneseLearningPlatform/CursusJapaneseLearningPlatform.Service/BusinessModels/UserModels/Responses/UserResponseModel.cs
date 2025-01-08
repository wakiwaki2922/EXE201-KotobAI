namespace CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Responses;
public class UserResponseModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string? ImagePath { get; set; }
    public string EmailAddress { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset LastUpdatedTime { get; set; }
    public DateTimeOffset? DeletedTime { get; set; }
    public required string CreatedBy { get; set; } = string.Empty;
    public required string LastUpdatedBy { get; set; } = string.Empty;
    public string? DeletedBy { get; set; }
    public bool IsActive { get; set; }
    public bool IsDelete { get; set; }
}

namespace CursusJapaneseLearningPlatform.Repository.Bases.BaseEntitys;

public class BaseEntity : IBaseEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset LastUpdatedTime { get; set; }
    public DateTimeOffset? DeletedTime { get; set; }

    public required string CreatedBy { get; set; }
    public required string LastUpdatedBy { get; set; }
    public string? DeletedBy { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDelete { get; set; } = false;
}

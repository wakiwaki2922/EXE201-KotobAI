using Microsoft.AspNetCore.Identity;
using CursusJapaneseLearningPlatform.Repository.Bases.BaseEntitys;
using System;

namespace CursusJapaneseLearningPlatform.Repository.Entities;

public sealed class User : IdentityUser<Guid>, IBaseEntity
{
    public required string FullName { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public string? EmailAddress { get; set; }

    public ICollection<Subscription> Subscriptions { get; set; }
    public ICollection<Payment> Payments { get; set; }
    public ICollection<Collection> Collections { get; set; }
    public ICollection<Chat> Chats { get; set; }


    // Base Entity
    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset LastUpdatedTime { get; set; }
    public DateTimeOffset? DeletedTime { get; set; }
    public required string CreatedBy { get; set; } = string.Empty;
    public required string LastUpdatedBy { get; set; } = string.Empty;
    public string? DeletedBy { get; set; }
    public bool IsActive { get; set; }
    public bool IsDelete { get; set; }
}

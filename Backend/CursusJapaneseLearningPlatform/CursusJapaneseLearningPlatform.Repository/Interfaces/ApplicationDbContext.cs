using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CursusJapaneseLearningPlatform.Repository.Entities;

namespace CursusJapaneseLearningPlatform.Repository;

public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentHistory> PaymentHistories { get; set; }
    public DbSet<Collection> Collections { get; set; }
    public DbSet<Flashcard> Flashcards { get; set; }
    public DbSet<Vocabulary> Vocabularies { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}

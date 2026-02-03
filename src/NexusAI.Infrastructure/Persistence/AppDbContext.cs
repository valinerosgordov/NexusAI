using Microsoft.EntityFrameworkCore;
using NexusAI.Domain.Entities;

namespace NexusAI.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTask> Tasks => Set<ProjectTask>();
    public DbSet<ProjectFile> Files => Set<ProjectFile>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.Username).IsUnique();

            entity.HasMany(e => e.Projects)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Projects");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.GitHubRepoUrl).HasMaxLength(500);
            entity.Property(e => e.UserId).IsRequired();

            entity.HasMany(e => e.Tasks)
                .WithOne(e => e.Project)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Files)
                .WithOne(e => e.Project)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.ToTable("ProjectTasks");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProjectId).IsRequired();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Status).IsRequired().HasConversion<string>();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EstimatedHours).HasPrecision(8, 2);
            entity.Property(e => e.OrderIndex).IsRequired();

            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.ProjectId, e.OrderIndex });
        });

        modelBuilder.Entity<ProjectFile>(entity =>
        {
            entity.ToTable("ProjectFiles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProjectId).IsRequired();
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Language).IsRequired().HasMaxLength(50);

            entity.HasIndex(e => e.ProjectId);
        });

        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.ToTable("ChatSessions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);

            entity.HasMany(e => e.Messages)
                .WithOne(e => e.ChatSession)
                .HasForeignKey(e => e.ChatSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.LastActivityAt);
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.ToTable("ChatMessages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ChatSessionId).IsRequired();
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);

            entity.HasIndex(e => e.ChatSessionId);
            entity.HasIndex(e => e.Timestamp);
        });
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace EventHubLibrary.Models;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationSetting> NotificationSettings { get; set; }

    public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    public virtual DbSet<ScheduledJob> ScheduledJobs { get; set; }

    public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=EventHubDB;Trusted_Connection=True;");
    protected override void OnConfiguring(DbContextOptionsBuilder
optionsBuilder)
    {
        var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json");
        var config = builder.Build();
        var connectionString =
       config.GetConnectionString("DefaultConnection");
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__7944C810B96A041D");

            entity.HasIndex(e => e.UserId, "idx_event_user");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EventName).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.Events)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Events__UserId__29572725");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12C39D4630");

            entity.HasIndex(e => new { e.UserId, e.IsRead }, "idx_user_notifications");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(10);

            entity.HasOne(d => d.Event).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK__Notificat__Event__3C69FB99");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__3B75D760");
        });
        var converter = new EnumToStringConverter<NotificationChannel>();
        modelBuilder.Entity<Notification>()
            .Property(n => n.Type)
            .HasConversion(converter);
        modelBuilder.Entity<NotificationSetting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("PK__Notifica__54372B1D7A09735B");

            entity.Property(e => e.IsEmailEnabled).HasDefaultValue(true);
            entity.Property(e => e.IsSmsEnabled).HasDefaultValue(true);
            entity.Property(e => e.MaxSendAttempts).HasDefaultValue(5);
            entity.Property(e => e.Reminder1hrEnabled).HasDefaultValue(true);
            entity.Property(e => e.Reminder24hrEnabled).HasDefaultValue(true);
            entity.Property(e => e.ReminderHoursBeforeEvent).HasDefaultValue(24);
            entity.Property(e => e.ReminderHoursBeforeEvent1).HasDefaultValue(1);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.HasKey(e => e.TemplateId).HasName("PK__Notifica__F87ADD2748952728");

            entity.HasIndex(e => e.TemplateName, "UQ__Notifica__A6C2DA66A5387510").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Subject).HasMaxLength(200);
            entity.Property(e => e.TemplateName).HasMaxLength(100);
        });

        modelBuilder.Entity<ScheduledJob>(entity =>
        {
            entity.HasKey(e => e.JobId).HasName("PK__Schedule__056690C2AD9D4523");

            entity.HasIndex(e => new { e.ScheduledTime, e.IsTriggered }, "idx_scheduled_time");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsTriggered).HasDefaultValue(false);
            entity.Property(e => e.NotificationType).HasMaxLength(20);

            entity.HasOne(d => d.Event).WithMany(p => p.ScheduledJobs)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Scheduled__Event__4222D4EF");

            entity.HasOne(d => d.User).WithMany(p => p.ScheduledJobs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Scheduled__UserI__412EB0B6");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CD3584B9B");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053448BAB8EB").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

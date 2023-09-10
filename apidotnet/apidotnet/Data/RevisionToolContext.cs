using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RevisionTool.Entity;

namespace RevisionTool.Data;

public partial class RevisionToolContext : DbContext
{
    public RevisionToolContext(DbContextOptions<RevisionToolContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Point> Points { get; set; }

    public virtual DbSet<PointsHistory> PointsHistories { get; set; }

    public virtual DbSet<PointsInRevision> PointsInRevisions { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Verificationcode> Verificationcodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8_general_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.ModuleId).HasName("PRIMARY");

            entity.ToTable("modules");

            entity.HasIndex(e => e.ModuleId, "module_id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.CreatedBy, "module_users_user_id_idx");

            entity.Property(e => e.ModuleId)
                .HasColumnType("int(11)")
                .HasColumnName("module_id");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("create_time");
            entity.Property(e => e.CreatedBy)
                .HasColumnType("int(11)")
                .HasColumnName("created_by");
            entity.Property(e => e.ModuleName)
                .HasMaxLength(50)
                .HasColumnName("module_name");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Modules)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("modules_users_user_id");
        });

        modelBuilder.Entity<Point>(entity =>
        {
            entity.HasKey(e => e.PointId).HasName("PRIMARY");

            entity.ToTable("points");

            entity.HasIndex(e => e.PointId, "point_id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.TopicId, "points_modules_module_id_idx");

            entity.HasIndex(e => e.CreateBy, "points_users_user_id_idx");

            entity.Property(e => e.PointId)
                .HasColumnType("int(11)")
                .HasColumnName("point_id");
            entity.Property(e => e.CreateBy)
                .HasColumnType("int(11)")
                .HasColumnName("create_by");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("create_time");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Point1)
                .HasMaxLength(255)
                .HasColumnName("point");
            entity.Property(e => e.TopicId)
                .HasColumnType("int(11)")
                .HasColumnName("topic_id");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Points)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("points_users_user_id");

            entity.HasOne(d => d.Topic).WithMany(p => p.Points)
                .HasForeignKey(d => d.TopicId)
                .HasConstraintName("points_topics_topic_id");
        });

        modelBuilder.Entity<PointsHistory>(entity =>
        {
            entity.HasKey(e => e.PointsHistoryId).HasName("PRIMARY");

            entity.ToTable("points_history");

            entity.HasIndex(e => e.PointId, "points_history_points_point_id_idx");

            entity.HasIndex(e => e.UserId, "points_history_users_user_id_idx");

            entity.Property(e => e.PointsHistoryId)
                .HasColumnType("int(11)")
                .HasColumnName("points_history_id");
            entity.Property(e => e.AskedTime)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("asked_time");
            entity.Property(e => e.NextTime)
                .HasColumnType("datetime")
                .HasColumnName("next_time");
            entity.Property(e => e.PointId)
                .HasColumnType("int(11)")
                .HasColumnName("point_id");
            entity.Property(e => e.TimeTakenToAnswer)
                .HasColumnType("time")
                .HasColumnName("time_taken_to_answer");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");

            entity.HasOne(d => d.Point).WithMany(p => p.PointsHistories)
                .HasForeignKey(d => d.PointId)
                .HasConstraintName("points_history_points_point_id");

            entity.HasOne(d => d.User).WithMany(p => p.PointsHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("points_history_users_user_id");
        });

        modelBuilder.Entity<PointsInRevision>(entity =>
        {
            entity.HasKey(e => e.PointsInRevisionId).HasName("PRIMARY");

            entity.ToTable("points_in_revision");

            entity.HasIndex(e => e.PointId, "points_In_revision_points_point_id_idx");

            entity.HasIndex(e => e.UserId, "points_in_revision_users_user_id_idx");

            entity.Property(e => e.PointsInRevisionId)
                .HasColumnType("int(11)")
                .HasColumnName("points_in_revision_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName(" is_active");
            entity.Property(e => e.PointId)
                .HasColumnType("int(11)")
                .HasColumnName("point_id");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");

            entity.HasOne(d => d.Point).WithMany(p => p.PointsInRevisions)
                .HasForeignKey(d => d.PointId)
                .HasConstraintName("points_In_revision_points_point_id");

            entity.HasOne(d => d.User).WithMany(p => p.PointsInRevisions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("points_in_revision_users_user_id");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PRIMARY");

            entity.ToTable("token");

            entity.HasIndex(e => e.TokenId, "active_id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.UserId, "active_logins_users_user_id_idx");

            entity.HasIndex(e => e.RefreshToken, "session_key_UNIQUE").IsUnique();

            entity.Property(e => e.TokenId)
                .HasColumnType("int(11)")
                .HasColumnName("token_id");
            entity.Property(e => e.ExpireTime)
                .HasColumnType("datetime")
                .HasColumnName("expire_time");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(64)
                .HasColumnName("refresh_token");
            entity.Property(e => e.RememberMe)
                .HasColumnType("tinyint(4)")
                .HasColumnName("remember_me");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("active_logins_users_user_id");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("PRIMARY");

            entity.ToTable("topics");

            entity.HasIndex(e => e.TopicId, "category_id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.ModuleId, "topic_modules_module_id_idx");

            entity.HasIndex(e => e.CreateBy, "topics_users_user_id_idx");

            entity.Property(e => e.TopicId)
                .HasColumnType("int(11)")
                .HasColumnName("topic_id");
            entity.Property(e => e.CreateBy)
                .HasColumnType("int(11)")
                .HasColumnName("create_ by");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("create_time");
            entity.Property(e => e.ModuleId)
                .HasColumnType("int(11)")
                .HasColumnName("module_id");
            entity.Property(e => e.TopicName)
                .HasMaxLength(255)
                .HasColumnName("topic_name");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Topics)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("topics_users_user_id");

            entity.HasOne(d => d.Module).WithMany(p => p.Topics)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("topics_modules_module_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.MobileNumber, "contact_number_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Email, "email_UNIQUE").IsUnique();

            entity.HasIndex(e => e.UserId, "user_id_UNIQUE").IsUnique();

            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(45)
                .HasColumnName("first_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("is_active");
            entity.Property(e => e.IsEmailVerify)
                .HasColumnType("tinyint(4)")
                .HasColumnName("is_email_verify");
            entity.Property(e => e.IsMobileNumberVeirfy)
                .HasDefaultValueSql("'0'")
                .HasColumnType("tinyint(4)")
                .HasColumnName("is_mobile_number_veirfy");
            entity.Property(e => e.LastName)
                .HasMaxLength(45)
                .HasColumnName("last_name");
            entity.Property(e => e.LastWrongAttempt)
                .HasColumnType("datetime")
                .HasColumnName("last_wrong_attempt");
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(20)
                .HasColumnName("mobile_number");
            entity.Property(e => e.Notification)
                .HasDefaultValueSql("'none'")
                .HasColumnType("enum('none','email','whatsapp','both')")
                .HasColumnName("notification");
            entity.Property(e => e.Password)
                .HasMaxLength(1000)
                .HasColumnName("password");
            entity.Property(e => e.ProfilePic)
                .HasMaxLength(255)
                .HasColumnName("profile_pic");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'student'")
                .HasColumnType("enum('student','teacher','admin')")
                .HasColumnName("role");
            entity.Property(e => e.WrongAttempts)
                .HasColumnType("int(11)")
                .HasColumnName("wrong_attempts");
        });

        modelBuilder.Entity<Verificationcode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("verificationcode");

            entity.HasIndex(e => e.UserId, "verificationcode_user_user_id_idx");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpirationTime).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnType("int(11)");

            entity.HasOne(d => d.User).WithMany(p => p.Verificationcodes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("verificationcode_user_user_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Zamify.Models;

public partial class ExamMasterDbContext : DbContext
{
    public ExamMasterDbContext()
    {
    }

    public ExamMasterDbContext(DbContextOptions<ExamMasterDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExamLink> ExamLinks { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionExam> QuestionExams { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }
    ////public object QuestionExam { get; internal set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) { }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Admin__3214EC0779F5797B");

            entity.ToTable("Admin");

            entity.HasIndex(e => e.Email, "UQ__Admin__A9D10534F0928BC1").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Exam__3214EC075EA11B31");

            entity.ToTable("Exam");

            entity.HasIndex(e => new { e.SubjectId, e.TeacherId }, "IX_Exam_SubjectTeacher");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Subject).WithMany(p => p.Exams)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exam__SubjectId__59063A47");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Exams)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exam__TeacherId__59FA5E80");
        });

        modelBuilder.Entity<ExamLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExamLink__3214EC074DA12216");

            entity.ToTable("ExamLink");

            entity.HasIndex(e => e.Token, "IX_ExamLink_Token").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExamDate).HasColumnType("datetime");
            entity.Property(e => e.Token).HasDefaultValueSql("(newid())");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamLinks)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExamLink__ExamId__6477ECF3");

            entity.HasOne(d => d.Student).WithMany(p => p.ExamLinks)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExamLink__Studen__656C112C");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC07A37F352D");

            entity.ToTable("Question");

            entity.Property(e => e.CorrectOption).HasMaxLength(10);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OptionA).HasMaxLength(255);
            entity.Property(e => e.OptionB).HasMaxLength(255);
            entity.Property(e => e.OptionC).HasMaxLength(255);
            entity.Property(e => e.OptionD).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Subject).WithMany(p => p.Questions)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK__Question__Subjec__5FB337D6");
        });

        modelBuilder.Entity<QuestionExam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC07AE64C8B3");

            entity.ToTable("QuestionExam");

            entity.HasIndex(e => new { e.ExamId, e.QuestionId }, "UQ_Exam_Question").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Exam).WithMany(p => p.QuestionExams)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("FK__QuestionE__ExamI__6D0D32F4");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionExams)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK__QuestionE__Quest__6E01572D");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Student__3214EC07A387BB5B");

            entity.ToTable("Student");

            entity.HasIndex(e => e.Email, "UQ__Student__A9D1053475EC861D").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subject__3214EC076AED9792");

            entity.ToTable("Subject");

            entity.HasIndex(e => e.Name, "UQ__Subject__737584F6298972BA").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Teacher__3214EC0792C3DE42");

            entity.ToTable("Teacher");

            entity.HasIndex(e => e.Email, "UQ__Teacher__A9D10534153B4903").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

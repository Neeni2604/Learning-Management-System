using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class LMSContext : DbContext
    {
        public LMSContext()
        {
        }

        public LMSContext(DbContextOptions<LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrators { get; set; } = null!;
        public virtual DbSet<Assignment> Assignments { get; set; } = null!;
        public virtual DbSet<AssignmentCategory> AssignmentCategories { get; set; } = null!;
        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Enrolled> Enrolleds { get; set; } = null!;
        public virtual DbSet<Professor> Professors { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Submission> Submissions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("name=LMS:LMSConnectionString", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.3-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("latin1_swedish_ci")
                .HasCharSet("latin1");

            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.AdminId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.UId, "uID")
                    .IsUnique();

                entity.Property(e => e.AdminId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("adminID");

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FName)
                    .HasMaxLength(100)
                    .HasColumnName("fName");

                entity.Property(e => e.LName)
                    .HasMaxLength(100)
                    .HasColumnName("lName");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.AId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.AName, e.AcId }, "AssignmentName_AssignmentCategory")
                    .IsUnique();

                entity.HasIndex(e => e.AcId, "acID");

                entity.Property(e => e.AId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("aID");

                entity.Property(e => e.AName)
                    .HasMaxLength(100)
                    .HasColumnName("aName");

                entity.Property(e => e.AcId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("acID");

                entity.Property(e => e.Contents).HasMaxLength(8192);

                entity.Property(e => e.DueDate)
                    .HasColumnType("datetime")
                    .HasColumnName("due_date");

                entity.Property(e => e.Points).HasColumnType("int(11)");

                entity.HasOne(d => d.Ac)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.AcId)
                    .HasConstraintName("Assignments_ibfk_1");
            });

            modelBuilder.Entity<AssignmentCategory>(entity =>
            {
                entity.HasKey(e => e.AcId)
                    .HasName("PRIMARY");

                entity.ToTable("Assignment_Categories");

                entity.HasIndex(e => new { e.ClassId, e.Name }, "name_class")
                    .IsUnique();

                entity.Property(e => e.AcId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("acID");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Weight).HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("Assignment_Categories_ibfk_1");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasIndex(e => new { e.CId, e.SemesterYear, e.SemesterSeason }, "semester_course")
                    .IsUnique();

                entity.HasIndex(e => e.TaughtBy, "taught_by");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.CId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("cID");

                entity.Property(e => e.EndTime)
                    .HasColumnType("time")
                    .HasColumnName("end_time");

                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .HasColumnName("location");

                entity.Property(e => e.SemesterSeason)
                    .HasMaxLength(6)
                    .HasColumnName("semester_season");

                entity.Property(e => e.SemesterYear)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("semester_year");

                entity.Property(e => e.StartTime)
                    .HasColumnType("time")
                    .HasColumnName("start_time");

                entity.Property(e => e.TaughtBy)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("taught_by");

                entity.HasOne(d => d.CIdNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CId)
                    .HasConstraintName("Classes_ibfk_1");

                entity.HasOne(d => d.TaughtByNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.TaughtBy)
                    .HasConstraintName("Classes_ibfk_2");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.Num, e.DId }, "course_number")
                    .IsUnique();

                entity.HasIndex(e => e.DId, "dID");

                entity.Property(e => e.CId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("cID");

                entity.Property(e => e.DId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("dID");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Num).HasColumnType("int(11) unsigned");

                entity.HasOne(d => d.DIdNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.DId)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Subject, "UQ_Subject_Name")
                    .IsUnique();

                entity.Property(e => e.DId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("dID");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Subject).HasMaxLength(4);
            });

            modelBuilder.Entity<Enrolled>(entity =>
            {
                entity.HasKey(e => new { e.SId, e.ClassId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("Enrolled");

                entity.HasIndex(e => e.ClassId, "classID");

                entity.Property(e => e.SId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("sID");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.Grade).HasMaxLength(2);

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrolleds)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("Enrolled_ibfk_2");

                entity.HasOne(d => d.SIdNavigation)
                    .WithMany(p => p.Enrolleds)
                    .HasForeignKey(d => d.SId)
                    .HasConstraintName("Enrolled_ibfk_1");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.PId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.WorksIn, "WorksIn");

                entity.HasIndex(e => e.UId, "uID")
                    .IsUnique();

                entity.Property(e => e.PId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("pID");

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FName)
                    .HasMaxLength(100)
                    .HasColumnName("fName");

                entity.Property(e => e.LName)
                    .HasMaxLength(100)
                    .HasColumnName("lName");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.WorksIn).HasColumnType("int(11) unsigned");

                entity.HasOne(d => d.WorksInNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.WorksIn)
                    .HasConstraintName("Professors_ibfk_1");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.SId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Major, "Major");

                entity.HasIndex(e => e.UId, "uID")
                    .IsUnique();

                entity.Property(e => e.SId)
                    .HasColumnType("int(11) unsigned")
                    .ValueGeneratedNever()
                    .HasColumnName("sID");

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FName)
                    .HasMaxLength(100)
                    .HasColumnName("fName");

                entity.Property(e => e.LName)
                    .HasMaxLength(100)
                    .HasColumnName("lName");

                entity.Property(e => e.Major).HasColumnType("int(11) unsigned");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.HasOne(d => d.MajorNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Major)
                    .HasConstraintName("Students_ibfk_1");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => e.SubId)
                    .HasName("PRIMARY");

                entity.ToTable("Submission");

                entity.HasIndex(e => e.AId, "aID");

                entity.HasIndex(e => new { e.SId, e.AId }, "student_assignment")
                    .IsUnique();

                entity.Property(e => e.SubId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("subID");

                entity.Property(e => e.AId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("aID");

                entity.Property(e => e.Contents).HasMaxLength(8192);

                entity.Property(e => e.SId)
                    .HasColumnType("int(11) unsigned")
                    .HasColumnName("sID");

                entity.Property(e => e.Score)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("score");

                entity.Property(e => e.SubmissionTime)
                    .HasColumnType("datetime")
                    .HasColumnName("submission_time");

                entity.HasOne(d => d.AIdNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.AId)
                    .HasConstraintName("Submission_ibfk_1");

                entity.HasOne(d => d.SIdNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.SId)
                    .HasConstraintName("Submission_ibfk_2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

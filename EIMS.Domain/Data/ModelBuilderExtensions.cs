using System;
using EIMS.Domain.Entities;
using EIMS.Domain.Entities.Enum;
using Microsoft.EntityFrameworkCore;

namespace EIMS.Domain.Data
{
	public static class ModelBuilderExtensions
	{
        public static void GenerateTables(this ModelBuilder modelBuilder)
        {
            // Model for teacher-to-subject many-to-many relationship
            modelBuilder.Entity<TeachersSubjects>()
                .HasKey(ts => new {ts.SubjectId, ts.TeacherId});

            modelBuilder.Entity<TeachersSubjects>()
                .HasOne(ts => ts.Teacher)
                .WithMany(t => t.TeachersSubjects)
                .HasForeignKey(ts => ts.TeacherId);

            modelBuilder.Entity<TeachersSubjects>()
                .HasOne(ts => ts.Subject)
                .WithMany(ts => ts.TeachersSubjects)
                .HasForeignKey(ts => ts.SubjectId);

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.User)
                .WithOne(u => u.Teacher)
                .HasForeignKey<Teacher>(t=>t.UserId);


            //Teacher-UserProfile one-to-one relationship
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.UserProfile)
                .WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(t => t.ProfileId);

            modelBuilder.Entity<UserProfile>()
                .HasOne(p => p.User)
                .WithOne(u => u.UserProfile)
                .HasForeignKey<UserProfile>(p => p.UserId);


            //Local Government  relationships 
            modelBuilder.Entity<LocalGovernment>()
                .HasMany(l => l.Schools)
                .WithOne(s => s.LocalGovernment)
                .HasForeignKey(s => s.LocalGovernmentId);

            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Qualifications);

            // Model for classes-to-subject many-to-many relationship
            modelBuilder.Entity<SubjectClasses>()
	            .HasKey(ts => new { ts.SubjectId, ts.ClassesId });

            modelBuilder.Entity<SubjectClasses>()
	            .HasOne(ts => ts.Classes)
	            .WithMany(t => t.SubjectsClasses)
	            .HasForeignKey(ts => ts.ClassesId);

            modelBuilder.Entity<SubjectClasses>()
	            .HasOne(ts => ts.Subject)
	            .WithMany(ts => ts.SubjectsClasses)
	            .HasForeignKey(ts => ts.SubjectId);

            modelBuilder.Entity<ExaminationsSubjects>()
                .HasKey(es => new {es.ExaminationId, es.SubjectId});

            modelBuilder.Entity<ExaminationsSubjects>()
                .HasOne(es => es.Examination)
                .WithMany(e => e.ExaminationsSubjects)
                .HasForeignKey(ts => ts.ExaminationId);
        }

        public static void ConfigureEntities(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>()
                .Property(t => t.Status)
                .HasDefaultValue(Status.Active);

            modelBuilder.Entity<Teacher>()
                .Property(t => t.CreatedAt)
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<Examination>()
                .Property(ex => ex.ExamStatus)
                .HasDefaultValue(Status.Active);
        }

        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subject>()
                .HasData(new Subject
                {
                    Id = 9999999,
                    LevelCategory = SchoolCategory.COMBINED,
                    SubjectName = "Mathematics"
                },
                new Subject
                {
                    Id = 999990,
                    LevelCategory = SchoolCategory.PRIMARY,
                    SubjectName = "Social Studies"
                });
        }

        public static void ExamSettings(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Examination>()
                .Property(ex => ex.UpdatedAt)
                .HasDefaultValue(DateTime.Now)
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Examination>()
                .Property(ex => ex.CreatedAt)
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<Examination>()
                .Property(ex => ex.ExamFee)
                .HasColumnType("decimal(14, 2)");

        }

        public static void SeedLGAs(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LocalGovernment>()
                .HasData(new LocalGovernment
                    {
                        Id = 1,
                        Name = "Akoko Edo"
                    },
                    new LocalGovernment
                    {
                        Id = 2,
                        Name = "Egor"
                    },
                    new LocalGovernment
                    {
                        Id = 3,
                        Name = "Esan Central"
                    },
                    new LocalGovernment
                    {
                        Id = 4,
                        Name = "Esan North-East"
                    },
                    new LocalGovernment
                    {
                        Id = 5,
                        Name = "Esan South-East"
                    },
                    new LocalGovernment
                    {
                        Id = 6,
                        Name = "Esan West"
                    },
                    new LocalGovernment
                    {
                        Id = 7,
                        Name = "Etsako Central"
                    },
                    new LocalGovernment
                    {
                        Id = 8,
                        Name = "Etsako East"
                    },
                    new LocalGovernment
                    {
                        Id = 9,
                        Name = "Etsako West"
                    },
                    new LocalGovernment
                    {
                        Id = 10,
                        Name = "Etsako West"
                    },
                    new LocalGovernment
                    {
                        Id = 11,
                        Name = "Igueben"
                    },
                    new LocalGovernment
                    {
                        Id = 12,
                        Name = "Ikpoba Okha"
                    },
                    new LocalGovernment
                    {
                        Id = 13,
                        Name = "Oredo"
                    },
                    new LocalGovernment
                    {
                        Id = 14,
                        Name = "Orhionmwon"
                    },
                    new LocalGovernment
                    {
                        Id = 15,
                        Name = "Ovia North East"
                    },
                    new LocalGovernment
                    {
                        Id = 16,
                        Name = "Ovia South East"
                    },
                    new LocalGovernment
                    {
                        Id = 17,
                        Name = "Owan East"
                    },
                    new LocalGovernment
                    {
                        Id = 18,
                        Name = "Owan West"
                    },
                    new LocalGovernment
                    {
                        Id = 19,
                        Name = "Uhunmwonde"
                    }
                );
        }
    }
}

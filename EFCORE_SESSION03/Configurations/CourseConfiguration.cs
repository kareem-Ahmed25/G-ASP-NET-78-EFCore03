using EFCORE_SESSION03.Enums;
using EFCORE_SESSION03.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFCORE_SESSION03.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Description).HasMaxLength(1000);
            builder.Property(c => c.Price).HasPrecision(10, 2);
            builder.Property(c => c.Level).IsRequired();
            builder.Property(c => c.IsPublished).IsRequired();
            builder.HasData(
    new Course { Id = 1, Title = "C# Fundamentals", Description = "Learn C# from scratch", Price = 49.99m, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2024, 1, 15), InstructorId = 1, CategoryId = 1 },
    new Course { Id = 2, Title = "ASP.NET Core MVC", Price = 89.99m, Level = CourseLevel.Intermediate, IsPublished = true, PublishedDate = new DateTime(2024, 3, 1), InstructorId = 1, CategoryId = 3 },
    new Course { Id = 3, Title = "EF Core Mastery", Price = 120.00m, Level = CourseLevel.Advanced, IsPublished = false, PublishedDate = null, InstructorId = 2, CategoryId = 2 },
    new Course { Id = 4, Title = "UI Design Basics", Price = 35.00m, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2024, 5, 20), InstructorId = 2, CategoryId = 4 }
);
        }
    }
}

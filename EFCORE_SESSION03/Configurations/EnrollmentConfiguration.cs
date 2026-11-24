using EFCORE_SESSION03.Enums;
using EFCORE_SESSION03.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFCORE_SESSION03.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasKey(e => new { e.StudentId, e.CourseId });

            builder.Property(e => e.EnrollmentDate).IsRequired();
            builder.Property(e => e.ProgressPercentage).HasPrecision(5, 2);
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.FinalGrade).HasPrecision(5, 2);

            builder.HasOne(e => e.Student)
                   .WithMany(s => s.Enrollments)
                   .HasForeignKey(e => e.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Course)
                   .WithMany(c => c.Enrollments)
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasData(
    new Enrollment { StudentId = 1, CourseId = 1, EnrollmentDate = new DateTime(2024, 2, 1), ProgressPercentage = 100m, Status = EnrollmentStatus.Completed, FinalGrade = 92.50m },
    new Enrollment { StudentId = 1, CourseId = 2, EnrollmentDate = new DateTime(2024, 3, 10), ProgressPercentage = 40m, Status = EnrollmentStatus.Active },
    new Enrollment { StudentId = 2, CourseId = 1, EnrollmentDate = new DateTime(2024, 2, 5), ProgressPercentage = 15m, Status = EnrollmentStatus.Active }
);
        }
    }
}

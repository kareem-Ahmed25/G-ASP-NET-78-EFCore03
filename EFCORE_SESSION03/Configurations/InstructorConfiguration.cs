using EFCORE_SESSION03.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFCORE_SESSION03.Configurations
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.FullName).IsRequired().HasMaxLength(100);
            builder.Property(i => i.Email).IsRequired().HasMaxLength(150);
            builder.Property(i => i.Bio).HasMaxLength(500);
            builder.Property(i => i.JoinedDate).IsRequired();

            builder.HasIndex(i => i.Email).IsUnique();

            builder.HasMany(i => i.Courses)
                   .WithOne(c => c.Instructor)
                   .HasForeignKey(c => c.InstructorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
    new Instructor { Id = 1, FullName = "Ahmed Hassan", Email = "ahmed.hassan@example.com", Bio = "Backend developer and trainer", JoinedDate = new DateTime(2023, 1, 10) },
    new Instructor { Id = 2, FullName = "Sara Mostafa", Email = "sara.mostafa@example.com", JoinedDate = new DateTime(2023, 6, 5) }
);

        }
    }
}

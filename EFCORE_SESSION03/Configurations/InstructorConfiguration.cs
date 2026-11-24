using EFCORE_SESSION03.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFCORE_SESSION03.Configurations
{
    public class InstructorConfiguration:IEntityTypeConfiguration<Instructor>
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
        }
    }
}

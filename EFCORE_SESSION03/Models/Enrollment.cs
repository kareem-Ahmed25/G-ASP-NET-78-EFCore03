using EFCORE_SESSION03.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFCORE_SESSION03.Models
{
    public class Enrollment
    {
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public int CourseId { get; set; }
        public Course? Course { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public decimal ProgressPercentage { get; set; }
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
        public decimal? FinalGrade { get; set; }
    }
}

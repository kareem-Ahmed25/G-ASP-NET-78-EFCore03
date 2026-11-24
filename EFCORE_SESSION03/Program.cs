using EFCORE_SESSION03.Data;
using EFCORE_SESSION03.Enums;
using EFCORE_SESSION03.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCORE_SESSION03
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new AppDbContext();

            Console.WriteLine("=== CREATE ===");
            AddInstructor(context);
            AddCourse(context, instructorId: 1, categoryId: 2);
            AddStudents(context);
            EnrollStudent(context, studentId: 2, courseId: 2);

            Console.WriteLine("\n=== READ ===");
            FindCourse(context, id: 1, title: "ASP.NET Core MVC");
            FindCourse(context, id: 999, title: "Does Not Exist");
            FilterCourses(context, minPrice: 40m, maxPrice: 100m);
            PrintSubCategories(context, categoryId: 1);
            PrintTopLevelCategories(context);
            CourseRoster(context, courseId: 1);

            Console.WriteLine("\n=== UPDATE ===");
            ChangePrice(context, courseId: 1, newPrice: 59.99m);
            ChangePrice(context, courseId: 999, newPrice: 10m);
            UpdateProgress(context, studentId: 2, courseId: 1, newProgress: 60m);
            UpdateProgress(context, studentId: 2, courseId: 1, newProgress: 150m);
            UpdateProgress(context, studentId: 2, courseId: 1, newProgress: 100m);
            PublishCourse(context, courseId: 3);
            PublishCourse(context, courseId: 3);

            Console.WriteLine("\n=== DELETE ===");

            EnrollStudent(context, studentId: 1, courseId: 2);
            Unenroll(context, studentId: 1, courseId: 2);
            Unenroll(context, studentId: 1, courseId: 2);

            var (parentId, childId) = AddTempCategoryTree(context);
            DeleteCategoryUnsafe(context, parentId);
            DeleteCategorySafe(context, parentId);
            DeleteCategorySafe(context, childId);
            DeleteCategorySafe(context, parentId);

            int tempCourseId = AddTempCourseWithEnrollments(context);
            DeleteCourse(context, tempCourseId);
            DeleteCourse(context, tempCourseId);
        }

        static void AddInstructor(AppDbContext context)
        {
            var instructor = new Instructor
            {
                FullName = "Khaled Samir",
                Email = $"khaled.{Guid.NewGuid():N}@example.com",
                Bio = "Cloud and DevOps instructor",
                JoinedDate = DateTime.Now
            };

            context.Instructors.Add(instructor);
            context.SaveChanges();

            Console.WriteLine($"Instructor added with Id = {instructor.Id}");
        }

        static void AddCourse(AppDbContext context, int instructorId, int categoryId)
        {
            if (!context.Instructors.Any(i => i.Id == instructorId))
            {
                Console.WriteLine($"Cannot add course: instructor {instructorId} does not exist.");
                return;
            }

            if (!context.Categories.Any(c => c.Id == categoryId))
            {
                Console.WriteLine($"Cannot add course: category {categoryId} does not exist.");
                return;
            }

            var course = new Course
            {
                Title = "Docker for Developers",
                Price = 75.00m,
                Level = CourseLevel.Intermediate,
                IsPublished = false,
                InstructorId = instructorId,
                CategoryId = categoryId
            };

            context.Courses.Add(course);
            context.SaveChanges();

            Console.WriteLine($"Course added with Id = {course.Id}");
        }

        static void AddStudents(AppDbContext context)
        {
            var students = new List<Student>
            {
                new Student { FullName = "Youssef Adel", Email = $"youssef.{Guid.NewGuid():N}@example.com", DateOfBirth = new DateTime(1999, 2, 14) },
                new Student { FullName = "Nour Tarek", Email = $"nour.{Guid.NewGuid():N}@example.com", DateOfBirth = new DateTime(2002, 7, 21) },
                new Student { FullName = "Hana Fathy", Email = $"hana.{Guid.NewGuid():N}@example.com", DateOfBirth = new DateTime(2000, 11, 30) }
            };

            context.Students.AddRange(students);
            context.SaveChanges();

            foreach (var s in students)
                Console.WriteLine($"Student '{s.FullName}' added with Id = {s.Id}");
        }

        static void EnrollStudent(AppDbContext context, int studentId, int courseId)
        {
            var student = context.Students.Find(studentId);
            if (student == null)
            {
                Console.WriteLine($"Enrollment refused: student {studentId} does not exist.");
                return;
            }

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Enrollment refused: course {courseId} does not exist.");
                return;
            }

            if (!course.IsPublished)
            {
                Console.WriteLine($"Enrollment refused: course '{course.Title}' is not published.");
                return;
            }

            if (context.Enrollments.Find(studentId, courseId) != null)
            {
                Console.WriteLine($"Enrollment refused: {student.FullName} is already enrolled in '{course.Title}'.");
                return;
            }

            context.Enrollments.Add(new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                ProgressPercentage = 0,
                Status = EnrollmentStatus.Active
            });
            context.SaveChanges();

            Console.WriteLine($"{student.FullName} enrolled in '{course.Title}'.");
        }

        static void FindCourse(AppDbContext context, int id, string title)
        {
            var byId = context.Courses.Find(id);
            if (byId == null)
                Console.WriteLine($"Course with Id {id}: not found");
            else
                Console.WriteLine($"Course with Id {id}: {byId.Title} ({byId.Level}, {byId.Price:C})");

            var byTitle = context.Courses.FirstOrDefault(c => c.Title == title);
            if (byTitle == null)
                Console.WriteLine($"Course titled '{title}': not found");
            else
                Console.WriteLine($"Course titled '{title}': Id = {byTitle.Id}, {byTitle.Level}, {byTitle.Price:C}");
        }

        static void FilterCourses(AppDbContext context, decimal minPrice, decimal maxPrice)
        {
            if (minPrice > maxPrice)
            {
                Console.WriteLine("Invalid range: the minimum price is greater than the maximum price.");
                return;
            }

            var courses = context.Courses
                .Where(c => c.Price >= minPrice && c.Price <= maxPrice)
                .OrderByDescending(c => c.Price)
                .Select(c => new { c.Title, c.Level, c.Price })
                .ToList();

            Console.WriteLine($"Courses priced between {minPrice} and {maxPrice}:");
            if (courses.Count == 0)
            {
                Console.WriteLine("  No courses found in this range.");
                return;
            }

            foreach (var c in courses)
                Console.WriteLine($"  {c.Title} | {c.Level} | {c.Price:C}");
        }

        static void PrintSubCategories(AppDbContext context, int categoryId)
        {
            var category = context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine($"Category {categoryId} not found.");
                return;
            }

            var subCategories = context.Categories
                .Where(c => c.ParentCategoryId == categoryId)
                .ToList();

            Console.WriteLine($"Direct sub-categories of '{category.Name}':");
            if (subCategories.Count == 0)
                Console.WriteLine("  (none)");
            else
                foreach (var sub in subCategories)
                    Console.WriteLine($"  {sub.Id}: {sub.Name}");
        }

        static void PrintTopLevelCategories(AppDbContext context)
        {
            var topLevel = context.Categories
                .Where(c => c.ParentCategoryId == null)
                .ToList();

            Console.WriteLine("Top-level categories:");
            foreach (var c in topLevel)
                Console.WriteLine($"  {c.Id}: {c.Name}");
        }

        static void CourseRoster(AppDbContext context, int courseId)
        {
            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Course {courseId} not found.");
                return;
            }

            var studentIds = context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.StudentId)
                .ToList();

            Console.WriteLine($"'{course.Title}' has {studentIds.Count} enrolled student(s):");

            var students = context.Students
                .Where(s => studentIds.Contains(s.Id))
                .ToList();

            foreach (var s in students)
                Console.WriteLine($"  {s.FullName}");
        }

        static void ChangePrice(AppDbContext context, int courseId, decimal newPrice)
        {
            if (newPrice < 0)
            {
                Console.WriteLine("Invalid price: it cannot be negative.");
                return;
            }

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Cannot change price: course {courseId} not found.");
                return;
            }

            var oldPrice = course.Price;
            course.Price = newPrice;
            context.SaveChanges();

            Console.WriteLine($"Price of '{course.Title}' changed from {oldPrice:F2} to {newPrice:F2}");
        }

        static void UpdateProgress(AppDbContext context, int studentId, int courseId, decimal newProgress)
        {
            if (newProgress < 0 || newProgress > 100)
            {
                Console.WriteLine($"Progress {newProgress} rejected: it must be between 0 and 100.");
                return;
            }

            var enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment == null)
            {
                Console.WriteLine($"Cannot update progress: student {studentId} is not enrolled in course {courseId}.");
                return;
            }

            enrollment.ProgressPercentage = newProgress;

            if (newProgress == 100)
                enrollment.Status = EnrollmentStatus.Completed;
            else if (enrollment.Status == EnrollmentStatus.Completed)
                enrollment.Status = EnrollmentStatus.Active;

            context.SaveChanges();

            Console.WriteLine($"Progress updated to {newProgress}% (status: {enrollment.Status})");
        }

        static void PublishCourse(AppDbContext context, int courseId)
        {
            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Cannot publish: course {courseId} not found.");
                return;
            }

            if (course.IsPublished)
            {
                Console.WriteLine($"Course '{course.Title}' is already published.");
                return;
            }

            course.IsPublished = true;
            course.PublishedDate = DateTime.Now;
            context.SaveChanges();

            Console.WriteLine($"Course '{course.Title}' published on {course.PublishedDate:yyyy-MM-dd}");
        }

        static void Unenroll(AppDbContext context, int studentId, int courseId)
        {
            var enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment == null)
            {
                Console.WriteLine($"Cannot unenroll: student {studentId} is not enrolled in course {courseId}.");
                return;
            }

            context.Enrollments.Remove(enrollment);
            context.SaveChanges();

            Console.WriteLine($"Student {studentId} unenrolled from course {courseId}.");
        }

        static void DeleteCategoryUnsafe(AppDbContext context, int categoryId)
        {
            context.ChangeTracker.Clear();

            var category = context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine($"Category {categoryId} not found.");
                return;
            }

            try
            {
                context.Categories.Remove(category);
                context.SaveChanges();
                Console.WriteLine($"Category '{category.Name}' deleted.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Delete failed: {ex.Message}");
                Console.WriteLine($"Reason: {ex.InnerException?.Message}");

                context.ChangeTracker.Clear();
            }
        }

        static (int parentId, int childId) AddTempCategoryTree(AppDbContext context)
        {
            var parent = new Category { Name = "Temp Parent" };
            context.Categories.Add(parent);
            context.SaveChanges();

            var child = new Category { Name = "Temp Child", ParentCategoryId = parent.Id };
            context.Categories.Add(child);
            context.SaveChanges();

            Console.WriteLine($"Temporary categories added: parent Id = {parent.Id}, child Id = {child.Id}");
            return (parent.Id, child.Id);
        }

        static void DeleteCategorySafe(AppDbContext context, int categoryId)
        {
            var category = context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine($"Category {categoryId} not found.");
                return;
            }

            if (context.Categories.Any(c => c.ParentCategoryId == categoryId))
            {
                Console.WriteLine($"Cannot delete '{category.Name}': it still has sub-categories.");
                return;
            }

            if (context.Courses.Any(c => c.CategoryId == categoryId))
            {
                Console.WriteLine($"Cannot delete '{category.Name}': it still has courses.");
                return;
            }

            context.Categories.Remove(category);
            context.SaveChanges();

            Console.WriteLine($"Category '{category.Name}' deleted safely.");
        }

        static int AddTempCourseWithEnrollments(AppDbContext context)
        {
            var course = new Course
            {
                Title = "Temp Course",
                Price = 10m,
                Level = CourseLevel.Beginner,
                IsPublished = true,
                PublishedDate = DateTime.Now,
                InstructorId = 1,
                CategoryId = 1
            };
            context.Courses.Add(course);
            context.SaveChanges();

            EnrollStudent(context, studentId: 1, courseId: course.Id);
            EnrollStudent(context, studentId: 2, courseId: course.Id);

            return course.Id;
        }

        static void DeleteCourse(AppDbContext context, int courseId)
        {
            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Cannot delete: course {courseId} not found.");
                return;
            }

            int before = context.Enrollments.Count(e => e.CourseId == courseId);
            Console.WriteLine($"Enrollments of '{course.Title}' before delete: {before}");

            context.Courses.Remove(course);
            context.SaveChanges();

            int after = context.Enrollments.Count(e => e.CourseId == courseId);
            Console.WriteLine($"Enrollments of course {courseId} after delete: {after}");

    
        }
    }
}
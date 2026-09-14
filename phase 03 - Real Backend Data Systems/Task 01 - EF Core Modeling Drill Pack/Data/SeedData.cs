using Microsoft.EntityFrameworkCore;
using Task_01___EF_Core_Modeling_Drill_Pack.Entities;
using Task_01___EF_Core_Modeling_Drill_Pack.Entities.Enums;

namespace Task_01___EF_Core_Modeling_Drill_Pack.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            if (await db.Students.AnyAsync())
            {
                return;
            }

            var instructors = new List<Instructor>
            {
                new() { FName = "Ahmed",LName = "Hossam", Email = "ahmed@gmail.com" },
                new() { FName = "Amr",LName = "Ali", Email = "amr@gmail.com" },
            };
            db.Instructors.AddRange(instructors);
            await db.SaveChangesAsync();

            var tracks = new List<TrainingTrack>
            {
                new() { Name = "ASP.NET Core Backend", Description = "REST APIs, EF Core, auth.", InstructorId = instructors[0].Id },
                new() { Name = "Cloud Fundamentals", Description = "Azure basics for backend devs.", InstructorId = instructors[1].Id },
                new() { Name = "Database Design", Description = "Relational modeling and SQL performance.", InstructorId = instructors[0].Id },
            };
            db.TrainingTracks.AddRange(tracks);
            await db.SaveChangesAsync();

            var students = new List<Student>
            {
                new() { FName = "Ahmed",LName = "Ali", Email = "ahmed2@gmail.com" },
                new() {FName = "Abduallah",LName = "Mohamed", Email = "aboda@gmail.com" },
                new() { FName = "Zeyad",LName = "Salem", Email = "zezo@gmail.com" },
                new() { FName = "Osama",LName = "Hasan", Email = "osos@gmail.com" },
                new() {FName = "Ali",LName = "Ali", Email = "ali@gmail.com" },
            };
            db.Students.AddRange(students);
            await db.SaveChangesAsync();

            var enrollments = new List<Enrollment>
            {
                new() { StudentId = students[0].Id, TrainingTrackId = tracks[0].Id, Status = EnrollmentStatus.Active },
                new() { StudentId = students[1].Id, TrainingTrackId = tracks[0].Id, Status = EnrollmentStatus.Active },
                new() { StudentId = students[2].Id, TrainingTrackId = tracks[1].Id, Status = EnrollmentStatus.Active },
                new() { StudentId = students[3].Id, TrainingTrackId = tracks[2].Id, Status = EnrollmentStatus.Completed, FinalGrade = 92.5 },
                new() { StudentId = students[4].Id, TrainingTrackId = tracks[0].Id, Status = EnrollmentStatus.Active },
            };
            db.Enrollments.AddRange(enrollments);
            await db.SaveChangesAsync();
        }
    }
}

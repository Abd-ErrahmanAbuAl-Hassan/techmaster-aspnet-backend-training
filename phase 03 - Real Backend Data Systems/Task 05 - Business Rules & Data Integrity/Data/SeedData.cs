using Microsoft.EntityFrameworkCore;
using Task_05_Business_Rules_Data_Integrity.Data;
using Task_05_Business_Rules_Data_Integrity.Entities;
using Task_05_Business_Rules_Data_Integrity.Utilities.Enums;

namespace Task_05_Business_Rules_Data_Integrity.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Instructors.AnyAsync())
            {
                logger.LogInformation("Database already contains data. Skipping seed.");
                return;
            }

            logger.LogInformation("Seeding database...");

            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var instructors = SeedInstructors();
                await context.Instructors.AddRangeAsync(instructors);
                await context.SaveChangesAsync(); // needed so Instructor.Id is populated

                var tracks = SeedTrainingTracks(instructors);
                await context.TrainingTracks.AddRangeAsync(tracks);
                await context.SaveChangesAsync(); // needed so TrainingTrack.Id is populated

                var students = SeedStudents();
                await context.Students.AddRangeAsync(students);
                await context.SaveChangesAsync(); // needed so Student.Id is populated

                var enrollments = SeedEnrollments(students, tracks);
                await context.Enrollments.AddRangeAsync(enrollments);
                await context.SaveChangesAsync(); // needed so Enrollment.Id is populated

                var payments = SeedPayments(enrollments, tracks);
                await context.Payments.AddRangeAsync(payments);
                await context.SaveChangesAsync();

                // Now apply the same status logic the runtime would have applied:
                // any enrollment whose payments fully cover the track price → Active.
                await ActivateFullyPaidEnrollmentsAsync(context);

                await transaction.CommitAsync();
                logger.LogInformation(
                    "Seed complete: {Instructors} instructors, {Tracks} tracks, {Students} students, {Enrollments} enrollments, {Payments} payments.",
                    instructors.Count, tracks.Count, students.Count, enrollments.Count, payments.Count);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex, "Seeding failed. Transaction rolled back.");
                throw;
            }
        }

        // --------------------------------------------------------------------
        // Instructors
        // --------------------------------------------------------------------
        private static List<Instructor> SeedInstructors() => new()
        {
            new Instructor
            {
                FName = "Ahmed", LName = "Hassan",
                Email = "ahmed.hassan@techmaster.test",
                PhoneNumber = "01012345678",
                Specialization = "Backend Development",
                Bio = "Senior .NET engineer with 10+ years building enterprise APIs.",
                IsActive = true
            },
            new Instructor
            {
                FName = "Sara", LName = "Mahmoud",
                Email = "sara.mahmoud@techmaster.test",
                PhoneNumber = "01112345678",
                Specialization = "Frontend Development",
                Bio = "React & TypeScript specialist. Loves clean UI and accessibility.",
                IsActive = true
            },
            new Instructor
            {
                FName = "Omar", LName = "Ibrahim",
                Email = "omar.ibrahim@techmaster.test",
                PhoneNumber = "01212345678",
                Specialization = "Cloud & DevOps",
                Bio = "Azure-certified architect. Focused on scalable cloud-native systems.",
                IsActive = true
            },
            new Instructor
            {
                FName = "Mona", LName = "Khaled",
                Email = "mona.khaled@techmaster.test",
                PhoneNumber = "01512345678",
                Specialization = "Data Engineering",
                Bio = "Data pipelines, warehousing, and analytics at scale.",
                IsActive = false // one inactive for filter tests
            }
        };

        // --------------------------------------------------------------------
        // Training Tracks
        // --------------------------------------------------------------------
        private static List<TrainingTrack> SeedTrainingTracks(List<Instructor> instructors)
        {
            var ahmed = instructors[0];
            var sara = instructors[1];
            var omar = instructors[2];

            return new List<TrainingTrack>
            {
                new TrainingTrack
                {
                    Title = "ASP.NET Core Web API",
                    Code = "API101",
                    Description = "Build production-ready REST APIs with .NET 8, EF Core, and clean architecture.",
                    Price = 3500m,
                    Level = TrackLevel.Intermediate,
                    Capacity = 20,
                    StartDate = DateTime.UtcNow.AddDays(7),
                    EndDate   = DateTime.UtcNow.AddDays(67),
                    Status = TrackStatus.Published,
                    IsActive = true,
                    InstructorId = ahmed.Id
                },
                new TrainingTrack
                {
                    Title = "Advanced Entity Framework Core",
                    Code = "EFC201",
                    Description = "Deep dive into EF Core: performance, migrations, and advanced mappings.",
                    Price = 2800m,
                    Level = TrackLevel.Advanced,
                    Capacity = 15,
                    StartDate = DateTime.UtcNow.AddDays(14),
                    EndDate   = DateTime.UtcNow.AddDays(74),
                    Status = TrackStatus.Published,
                    IsActive = true,
                    InstructorId = ahmed.Id
                },
                new TrainingTrack
                {
                    Title = "React & TypeScript Fundamentals",
                    Code = "RTS101",
                    Description = "Modern frontend development with React 18, hooks, and TypeScript.",
                    Price = 3000m,
                    Level = TrackLevel.Beginner,
                    Capacity = 30,
                    StartDate = DateTime.UtcNow.AddDays(3),
                    EndDate   = DateTime.UtcNow.AddDays(63),
                    Status = TrackStatus.Published,
                    IsActive = true,
                    InstructorId = sara.Id
                },
                new TrainingTrack
                {
                    Title = "Azure for Developers",
                    Code = "AZR201",
                    Description = "Deploy, scale, and monitor .NET apps on Azure App Service, SQL, and Key Vault.",
                    Price = 4000m,
                    Level = TrackLevel.Intermediate,
                    Capacity = 25,
                    StartDate = DateTime.UtcNow.AddDays(10),
                    EndDate   = DateTime.UtcNow.AddDays(70),
                    Status = TrackStatus.Published,
                    IsActive = true,
                    InstructorId = omar.Id
                },
                new TrainingTrack
                {
                    Title = "Docker & Kubernetes Essentials",
                    Code = "K8S101",
                    Description = "Containerize .NET apps and orchestrate them on Kubernetes.",
                    Price = 3200m,
                    Level = TrackLevel.Intermediate,
                    Capacity = 10,
                    StartDate = DateTime.UtcNow.AddDays(-30), // started
                    EndDate   = DateTime.UtcNow.AddDays(30),
                    Status = TrackStatus.Published,
                    IsActive = true,
                    InstructorId = omar.Id
                },
                new TrainingTrack
                {
                    Title = "Legacy .NET Framework Migration",
                    Code = "MIG999",
                    Description = "Archived course — no longer accepting enrollments.",
                    Price = 2500m,
                    Level = TrackLevel.Advanced,
                    Capacity = 20,
                    StartDate = DateTime.UtcNow.AddDays(-200),
                    EndDate   = DateTime.UtcNow.AddDays(-100),
                    Status = TrackStatus.Archived,
                    IsActive = false,
                    InstructorId = ahmed.Id
                }
            };
        }

        // --------------------------------------------------------------------
        // Students
        // --------------------------------------------------------------------
        private static List<Student> SeedStudents() => new()
        {
            new Student { FName = "Youssef", LName = "Ali",     Email = "youssef.ali@student.test",     PhoneNumber = "01011111111", IsActive = true  },
            new Student { FName = "Nour",    LName = "Samir",   Email = "nour.samir@student.test",      PhoneNumber = "01111111111", IsActive = true  },
            new Student { FName = "Khaled",  LName = "Mostafa", Email = "khaled.mostafa@student.test",  PhoneNumber = "01211111111", IsActive = true  },
            new Student { FName = "Layla",   LName = "Adel",    Email = "layla.adel@student.test",      PhoneNumber = "01511111111", IsActive = true  },
            new Student { FName = "Hassan",  LName = "Tarek",   Email = "hassan.tarek@student.test",    PhoneNumber = "01022222222", IsActive = true  },
            new Student { FName = "Dina",    LName = "Fathy",   Email = "dina.fathy@student.test",      PhoneNumber = "01122222222", IsActive = true  },
            new Student { FName = "Karim",   LName = "Nabil",   Email = "karim.nabil@student.test",     PhoneNumber = "01222222222", IsActive = true  },
            new Student { FName = "Aya",     LName = "Saeed",   Email = "aya.saeed@student.test",       PhoneNumber = "01522222222", IsActive = true  },
            new Student { FName = "Mahmoud", LName = "Roshdy",  Email = "mahmoud.roshdy@student.test",  PhoneNumber = "01033333333", IsActive = false },
            new Student { FName = "Salma",   LName = "Hany",    Email = "salma.hany@student.test",      PhoneNumber = "01133333333", IsActive = true  },
            // Soft-deleted student for filter tests
            new Student
            {
                FName = "Deleted", LName = "Student",
                Email = "deleted@student.test",
                PhoneNumber = "01233333333",
                IsActive = false,
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow.AddDays(-10)
            }
        };

        // --------------------------------------------------------------------
        // Enrollments
        // Builds a variety of scenarios: draft, active, completed, cancelled.
        // --------------------------------------------------------------------
        private static List<Enrollment> SeedEnrollments(List<Student> students, List<TrainingTrack> tracks)
        {
            var youssef = students[0];
            var nour = students[1];
            var khaled = students[2];
            var layla = students[3];
            var hassan = students[4];
            var dina = students[5];
            var karim = students[6];
            var aya = students[7];
            var salma = students[9];

            var api = tracks[0];
            var efc = tracks[1];
            var rts = tracks[2];
            var azr = tracks[3];
            var k8s = tracks[4];

            return new List<Enrollment>
            {
                // Draft — no payments yet
                new Enrollment { StudentId = youssef.Id, TrainingTrackId = api.Id,  Status = EnrollmentStatus.Draft, EnrollmentDate = DateTime.UtcNow.AddDays(-2) },
                // Draft — partial payment (will be added)
                new Enrollment { StudentId = nour.Id,    TrainingTrackId = api.Id,  Status = EnrollmentStatus.Draft, EnrollmentDate = DateTime.UtcNow.AddDays(-1) },
                // Draft — partial payment
                new Enrollment { StudentId = khaled.Id,  TrainingTrackId = rts.Id,  Status = EnrollmentStatus.Draft, EnrollmentDate = DateTime.UtcNow.AddDays(-3) },

                // Active — fully paid (activated by seeder after payments)
                new Enrollment { StudentId = layla.Id,   TrainingTrackId = api.Id,  Status = EnrollmentStatus.Active, EnrollmentDate = DateTime.UtcNow.AddDays(-20) },
                new Enrollment { StudentId = hassan.Id,  TrainingTrackId = efc.Id,  Status = EnrollmentStatus.Active, EnrollmentDate = DateTime.UtcNow.AddDays(-15) },
                new Enrollment { StudentId = dina.Id,    TrainingTrackId = rts.Id,  Status = EnrollmentStatus.Active, EnrollmentDate = DateTime.UtcNow.AddDays(-10) },
                new Enrollment { StudentId = karim.Id,   TrainingTrackId = azr.Id,  Status = EnrollmentStatus.Active, EnrollmentDate = DateTime.UtcNow.AddDays(-8) },

                // Completed — fully paid and finished
                new Enrollment
                {
                    StudentId = aya.Id, TrainingTrackId = k8s.Id,
                    Status = EnrollmentStatus.Completed, EnrollmentDate = DateTime.UtcNow.AddDays(-60),
                    ProgressPercentage = 100m, FinalGrade = 88.5m
                },
                new Enrollment
                {
                    StudentId = salma.Id, TrainingTrackId = k8s.Id,
                    Status = EnrollmentStatus.Completed, EnrollmentDate = DateTime.UtcNow.AddDays(-55),
                    ProgressPercentage = 100m, FinalGrade = 92.0m
                },

                // Cancelled — refund will be added by seeder
                new Enrollment { StudentId = youssef.Id, TrainingTrackId = efc.Id, Status = EnrollmentStatus.Cancelled, EnrollmentDate = DateTime.UtcNow.AddDays(-25) }
            };
        }

        // --------------------------------------------------------------------
        // Payments
        // Aligns with the cumulative-payment model:
        //   - All but the final payment for an enrollment: PartiallyPaid
        //   - Final payment that completes the enrollment: Paid
        //   - Refund row for the cancelled enrollment: Refunded
        // --------------------------------------------------------------------
        private static List<Payment> SeedPayments(List<Enrollment> enrollments, List<TrainingTrack> tracks)
        {
            var payments = new List<Payment>();
            var rng = new Random(20240924); // deterministic for repeatable test data

            for (int i = 0; i < enrollments.Count; i++)
            {
                var enrollment = enrollments[i];
                var track = tracks.First(t => t.Id == enrollment.TrainingTrackId);

                switch (enrollment.Status)
                {
                    case EnrollmentStatus.Draft:
                        // Some drafts get a partial payment, one gets none.
                        if (i == 1) // nour → 1000 of 3500
                            payments.Add(BuildPayment(enrollment, 1000m, PaymentStatus.PartiallyPaid, rng, DateTime.UtcNow.AddDays(-1)));
                        else if (i == 2) // khaled → 1500 of 3000
                            payments.Add(BuildPayment(enrollment, 1500m, PaymentStatus.PartiallyPaid, rng, DateTime.UtcNow.AddDays(-2)));
                        // i == 0 (youssef) → no payments
                        break;

                    case EnrollmentStatus.Active:
                        // Fully paid. Split into 2 or 3 partials, final = Paid.
                        if (track.Price == 3500m) // api
                        {
                            payments.Add(BuildPayment(enrollment, 1500m, PaymentStatus.PartiallyPaid, rng, DateTime.UtcNow.AddDays(-19)));
                            payments.Add(BuildPayment(enrollment, 2000m, PaymentStatus.Paid, rng, DateTime.UtcNow.AddDays(-18)));
                        }
                        else if (track.Price == 2800m) // efc
                        {
                            payments.Add(BuildPayment(enrollment, 2800m, PaymentStatus.Paid, rng, DateTime.UtcNow.AddDays(-14)));
                        }
                        else if (track.Price == 3000m) // rts
                        {
                            payments.Add(BuildPayment(enrollment, 1000m, PaymentStatus.PartiallyPaid, rng, DateTime.UtcNow.AddDays(-9)));
                            payments.Add(BuildPayment(enrollment, 1000m, PaymentStatus.PartiallyPaid, rng, DateTime.UtcNow.AddDays(-7)));
                            payments.Add(BuildPayment(enrollment, 1000m, PaymentStatus.Paid, rng, DateTime.UtcNow.AddDays(-6)));
                        }
                        else if (track.Price == 4000m) // azr
                        {
                            payments.Add(BuildPayment(enrollment, 4000m, PaymentStatus.Paid, rng, DateTime.UtcNow.AddDays(-7)));
                        }
                        break;

                    case EnrollmentStatus.Completed:
                        payments.Add(BuildPayment(enrollment, track.Price, PaymentStatus.Paid, rng, enrollment.EnrollmentDate.AddDays(1)));
                        break;

                    case EnrollmentStatus.Cancelled:
                        // Student had paid 2800 in full, then cancelled → refund row.
                        payments.Add(BuildPayment(enrollment, 2800m, PaymentStatus.Paid, rng, DateTime.UtcNow.AddDays(-24)));
                        payments.Add(BuildRefund(enrollment, 2800m, DateTime.UtcNow.AddDays(-20)));
                        break;
                }
            }

            return payments;
        }

        private static Payment BuildPayment(
            Enrollment enrollment, decimal amount, PaymentStatus status, Random rng, DateTime date) => new()
            {
                EnrollmentId = enrollment.Id,
                Amount = amount,
                PaymentMethod = (PaymentMethod)rng.Next(0, 6),
                PaymentDate = date,
                PaymentStatus = status,
                ReferenceNumber = $"SEED-{enrollment.Id}-{Guid.NewGuid():N}",
                Notes = status == PaymentStatus.Paid
                ? "Final payment — enrollment completed."
                : "Installment payment."
            };

        private static Payment BuildRefund(Enrollment enrollment, decimal amount, DateTime date) => new()
        {
            EnrollmentId = enrollment.Id,
            Amount = amount,
            PaymentMethod = PaymentMethod.BankTransfer,
            PaymentDate = date,
            PaymentStatus = PaymentStatus.Refunded,
            ReferenceNumber = $"SEED-{enrollment.Id}-{Guid.NewGuid():N}",
            Notes = $"Refund for cancelled enrollment #{enrollment.Id}."
        };

        // --------------------------------------------------------------------
        // Reconciliation — apply the same state transition the runtime applies:
        // fully paid draft enrollments become Active.
        // We only do this for enrollments that the seed marked as Draft AND
        // have payments covering the price. The cancelled + refunded one is
        // left as Cancelled.
        // --------------------------------------------------------------------
        private static async Task ActivateFullyPaidEnrollmentsAsync(ApplicationDbContext context)
        {
            var drafts = await context.Enrollments
                .Include(e => e.TrainingTrack)
                .Include(e => e.Payments)
                .Where(e => e.Status == EnrollmentStatus.Draft)
                .ToListAsync();

            foreach (var e in drafts)
            {
                var paid = e.Payments
                    .Where(p => p.PaymentStatus == PaymentStatus.Paid
                             || p.PaymentStatus == PaymentStatus.PartiallyPaid)
                    .Sum(p => p.Amount);

                if (paid >= (e.TrainingTrack?.Price ?? 0))
                    e.Status = EnrollmentStatus.Active;
            }
        }
    }
}
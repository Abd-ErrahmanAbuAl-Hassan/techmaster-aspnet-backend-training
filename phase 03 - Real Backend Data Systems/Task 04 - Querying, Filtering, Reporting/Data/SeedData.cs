using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task_04_Querying_Filtering_Reporting.Entities;
using Task_04_Querying_Filtering_Reporting.Utilities.Enums;

namespace Task_04_Querying_Filtering_Reporting.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            try
            {
                // Seed Instructors
                if (!context.Instructors.Any())
                {
                    var instructors = new List<Instructor>
                    {
                        new Instructor
                        {
                            FName = "Mohamed",
                            LName = "Ali",
                            Email = "mohammad.ali@training.com",
                            PhoneNumber = "01001234567",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Instructor
                        {
                            FName = "Fatima",
                            LName = "Ahmed",
                            Email = "fatima.ahmed@training.com",
                            PhoneNumber = "01102345678",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Instructor
                        {
                            FName = "Ali",
                            LName = "Mahmoud",
                            Email = "ali.mahmoud@training.com",
                            PhoneNumber = "01201234567",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Instructor
                        {
                            FName = "Nour",
                            LName = "Khaled",
                            Email = "nour.khaled@training.com",
                            PhoneNumber = "01001567890",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Instructor
                        {
                            FName = "Layla",
                            LName = "Hassan",
                            Email = "layla.hassan@training.com",
                            PhoneNumber = "01102567890",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        }
                    };

                    await context.Instructors.AddRangeAsync(instructors);
                    await context.SaveChangesAsync();
                }

                // Seed Training Tracks
                if (!context.TrainingTracks.Any())
                {
                    var instructors = await context.Instructors.ToListAsync();
                    var tracks = new List<TrainingTrack>
                    {
                        new TrainingTrack
                        {
                            Title = "C# Fundamentals",
                            Code = "CSE-456",
                            Description = "Complete programming course to learn C# from the beginning",
                            InstructorId = instructors[0].Id,
                            Capacity = 30,
                            Price = 500,
                            Level = TrackLevel.Beginner,
                            Status = TrackStatus.Published,
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        },
                        new TrainingTrack
                        {
                            Title = "Advanced ASP.NET Core",
                            Code = "CSE-986",
                            Description = "Advanced course to learn how to build web application using ASP.NET Core",
                            InstructorId = instructors[1].Id,
                            Capacity = 25,
                            Price = 800,
                            Level = TrackLevel.Advanced,
                            Status = TrackStatus.Published,
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        },
                        new TrainingTrack
                        {
                            Title = "SQL Database",
                            Code = "CSE-115",
                            Description = "Learning design and querying relational databases",
                            InstructorId = instructors[2].Id,
                            Capacity = 35,
                            Price = 600,
                            Level = TrackLevel.Intermidate,
                            Status = TrackStatus.Published,
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        },
                        new TrainingTrack
                        {
                            Title = "Entity Framework Core",
                            Code = "CSE-489",
                            Description = "Comprehensive course with Entity Framework Core using ASP.NET",
                            InstructorId = instructors[3].Id,
                            Capacity = 28,
                            Price = 700,
                            Level = TrackLevel.Intermidate,
                            Status = TrackStatus.Published,
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        },
                        new TrainingTrack
                        {
                            Title = "RESTful APIs",
                            Code = "CSE-987",
                            Description = "Building a powerful and secure web interfaces using .NET",
                            InstructorId = instructors[4].Id,
                            Capacity = 32,
                            Price = 750,
                            Level = TrackLevel.Advanced,
                            Status = TrackStatus.Published,
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        },
                        new TrainingTrack
                        {
                            Title = "Unit Testing و TDD",
                            Code = "CSE-123",
                            Description = "Developing high quality application through testing",
                            InstructorId = instructors[0].Id,
                            Capacity = 20,
                            Price = 550,
                            Level = TrackLevel.Intermidate,
                            Status = TrackStatus.Draft,
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        },
                        new TrainingTrack
                        {
                            Title = "LINQ و Collections",
                            Code = "CSE-065",
                            Description = "Mastery Language Integrated Query and working with groups",
                            InstructorId = instructors[1].Id,
                            Capacity = 30,
                            Price = 450,
                            Level = TrackLevel.Intermidate,
                            Status = TrackStatus.Published,
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        }
                    };

                    await context.TrainingTracks.AddRangeAsync(tracks);
                    await context.SaveChangesAsync();
                }

                // Seed Students
                if (!context.Students.Any())
                {
                    var students = new List<Student>
                    {
                        new Student
                        {
                            FName = "Ahmed",
                            LName = "Mohamed",
                            Email = "ahmed.mohammad@student.com",
                            PhoneNumber = "01001234567",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Student
                        {
                            FName = "Sarah",
                            LName = "Ali",
                            Email = "sarah.ali@student.com",
                            PhoneNumber = "01102345678",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Student
                        {
                            FName = "Mahmoud",
                            LName = "Hassan",
                            Email = "mahmoud.hassan@student.com",
                            PhoneNumber = "01201234567",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Student
                        {
                            FName = "Rana",
                            LName = "Khaled",
                            Email = "rana.khaled@student.com",
                            PhoneNumber = "01001567890",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Student
                        {
                            FName = "Omar",
                            LName = "Ibrahim",
                            Email = "omar.ibrahim@student.com",
                            PhoneNumber = "01102567890",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Student
                        {
                            FName = "Lama",
                            LName = "Mohamed",
                            Email = "lama.mohammad@student.com",
                            PhoneNumber = "01201567890",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Student
                        {
                            FName = "Khaled",
                            LName = "Ahmed",
                            Email = "khaled.ahmed@student.com",
                            PhoneNumber = "01001890123",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Student
                        {
                            FName = "Mira",
                            LName = "Ali",
                            Email = "mira.ali@student.com",
                            PhoneNumber = "01102890123",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Student
                        {
                            FName = "Ziad",
                            LName = "Farouk",
                            Email = "ziad.farouk@student.com",
                            PhoneNumber = "01201890123",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Student
                        {
                            FName = "Hana",
                            LName = "Youssef",
                            Email = "hana.youssef@student.com",
                            PhoneNumber = "01001234890",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Student
                        {
                            FName = "Youssef",
                            LName = "Mahmoud",
                            Email = "youssef.mahmoud@student.com",
                            PhoneNumber = "01102234890",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Student
                        {
                            FName = "Leah",
                            LName = "Mohamed",
                            Email = "leah.mohammad@student.com",
                            PhoneNumber = "01201234890",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Student
                        {
                            FName = "Nour",
                            LName = "Ahmed",
                            Email = "nour.ahmed@student.com",
                            PhoneNumber = "01001345678",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Student
                        {
                            FName = "Ghada",
                            LName = "Ali",
                            Email = "ghada.ali@student.com",
                            PhoneNumber = "01102345678",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Student
                        {
                            FName = "Sami",
                            LName = "Mohamed",
                            Email = "sami.mohammad@student.com",
                            PhoneNumber = "01201345678",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        }
                    };

                    await context.Students.AddRangeAsync(students);
                    await context.SaveChangesAsync();
                }

                // Seed Enrollments
                if (!context.Enrollments.Any())
                {
                    var students = await context.Students.ToListAsync();
                    var tracks = await context.TrainingTracks.ToListAsync();
                    var enrollments = new List<Enrollment>();

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[0].Id,
                        TrainingTrackId = tracks[0].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-10),
                        FinalResult = null
                    });
                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[0].Id,
                        TrainingTrackId = tracks[1].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-5),
                        FinalResult = null
                    });

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[1].Id,
                        TrainingTrackId = tracks[0].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-8),
                        FinalResult = null
                    });
                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[1].Id,
                        TrainingTrackId = tracks[2].Id,
                        Status = EnrollmentStatus.Completed,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-30),
                        FinalResult = 92.5
                    });

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[2].Id,
                        TrainingTrackId = tracks[1].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-12),
                        FinalResult = null
                    });

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[3].Id,
                        TrainingTrackId = tracks[2].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-15),
                        FinalResult = null
                    });
                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[3].Id,
                        TrainingTrackId = tracks[3].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-3),
                        FinalResult = null
                    });

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[4].Id,
                        TrainingTrackId = tracks[3].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-7),
                        FinalResult = null
                    });

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[5].Id,
                        TrainingTrackId = tracks[4].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-20),
                        FinalResult = null
                    });
                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[5].Id,
                        TrainingTrackId = tracks[5].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-1),
                        FinalResult = null
                    });

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[6].Id,
                        TrainingTrackId = tracks[0].Id,
                        Status = EnrollmentStatus.Cancelled,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-25),
                        FinalResult = null
                    });

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[7].Id,
                        TrainingTrackId = tracks[1].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-9),
                        FinalResult = null
                    });
                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[7].Id,
                        TrainingTrackId = tracks[4].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-4),
                        FinalResult = null
                    });

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[8].Id,
                        TrainingTrackId = tracks[2].Id,
                        Status = EnrollmentStatus.Completed,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-35),
                        FinalResult = 88.0
                    });
                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[8].Id,
                        TrainingTrackId = tracks[6].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-2),
                        FinalResult = null
                    });

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[9].Id,
                        TrainingTrackId = tracks[3].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-6),
                        FinalResult = null
                    });

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[10].Id,
                        TrainingTrackId = tracks[0].Id,
                        Status = EnrollmentStatus.Completed,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-40),
                        FinalResult = 95.0
                    });
                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[10].Id,
                        TrainingTrackId = tracks[6].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-11),
                        FinalResult = null
                    });

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[11].Id,
                        TrainingTrackId = tracks[4].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-14),
                        FinalResult = null
                    });

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[12].Id,
                        TrainingTrackId = tracks[1].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-18),
                        FinalResult = null
                    });
                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[12].Id,
                        TrainingTrackId = tracks[5].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-5),
                        FinalResult = null
                    });

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[13].Id,
                        TrainingTrackId = tracks[2].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-22),
                        FinalResult = null
                    });

                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[14].Id,
                        TrainingTrackId = tracks[0].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-13),
                        FinalResult = null
                    });
                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[14].Id,
                        TrainingTrackId = tracks[3].Id,
                        Status = EnrollmentStatus.Active,
                        EnrollmentDate = DateTime.UtcNow.AddDays(-8),
                        FinalResult = null
                    });

                    await context.Enrollments.AddRangeAsync(enrollments);
                    await context.SaveChangesAsync();
                }

                // Seed Payments
                if (!context.Payments.Any())
                {
                    var enrollments = await context.Enrollments
                        .Include(e => e.TrainingTrack)
                        .ToListAsync();

                    var payments = new List<Payment>();

                    // Add payments for various enrollments
                    foreach (var enrollment in enrollments.Take(10))
                    {
                        // First payment - partial
                        payments.Add(new Payment
                        {
                            EnrollmentId = enrollment.Id,
                            Amount = enrollment.TrainingTrack!.Price / 2,
                            PaymentMethod = PaymentMethod.CreditCard,
                            PaymentDate = DateTime.UtcNow.AddDays(-5),
                            PaymentStatus = PaymentStatus.Paid,
                            ReferenceNumber = $"REF-{enrollment.Id}-001",
                            Notes = "The first payment"
                        });

                        // Some get fully paid
                        if (enrollment.StudentId % 2 == 0)
                        {
                            payments.Add(new Payment
                            {
                                EnrollmentId = enrollment.Id,
                                Amount = enrollment.TrainingTrack!.Price / 2,
                                PaymentMethod = PaymentMethod.BankTransfer,
                                PaymentDate = DateTime.UtcNow.AddDays(-2),
                                PaymentStatus = PaymentStatus.Paid,
                                ReferenceNumber = $"REF-{enrollment.Id}-002",
                                Notes = "The final payment"
                            });
                        }
                    }

                    // Add some pending payments
                    foreach (var enrollment in enrollments.Skip(10).Take(5))
                    {
                        payments.Add(new Payment
                        {
                            EnrollmentId = enrollment.Id,
                            Amount = 100,
                            PaymentMethod = PaymentMethod.Cash,
                            PaymentDate = DateTime.UtcNow,
                            PaymentStatus = PaymentStatus.Pending,
                            ReferenceNumber = $"REF-{enrollment.Id}-001",
                            Notes = "Pending batch"
                        });
                    }

                    await context.Payments.AddRangeAsync(payments);
                    await context.SaveChangesAsync();
                }

                Console.WriteLine("Training data has been added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while adding the data: {ex.Message}");
                throw;
            }
        }
    }
}

using System.Text.RegularExpressions;
using Task_02___Student_Management_API.DTOs;
using Task_02___Student_Management_API.Entities;
using Task_02___Student_Management_API.Utilities;

namespace Task_02___Student_Management_API.Services
{
    public class StudentService
    {
        private static List<Student> _students = new List<Student>();

        public Result<Student> Create(CreateStudentRequest model)
        {
            if (model == null) return new Result<Student>
            {
                Success = false,
                Message = "Validation error.",
                Errors = { "Student model is null." },
                ErrorCode = 400
            };

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(model.FName)) errors.Add("First name is required.");
            if (string.IsNullOrWhiteSpace(model.LName)) errors.Add("Last name is required.");
            if (string.IsNullOrWhiteSpace(model.Email)) errors.Add("Email is required.");
            if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) errors.Add("Invalid email address.");
            if (string.IsNullOrWhiteSpace(model.PhoneNumber)) errors.Add("Phone number is required.");
            if (!Regex.IsMatch(model.PhoneNumber, @"^01[0125]\d{8}$")) errors.Add("Phone number must be EGY phone number.");
            if (string.IsNullOrWhiteSpace(model.TrackName)) errors.Add("Track name is required.");

            if (errors.Any()) return new Result<Student>
            {
                Success = false,
                Message = "Validation Errors.",
                Errors = errors,
                ErrorCode=400
            };

            var student = new Student
            {
                FName = model.FName,
                LName = model.LName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                TrackName = model.TrackName,
                EnrollmentDate = DateTime.Now,
                IsActive = true,
                LinkedInURL = model.LinkedInURL,
                GithubURL = model.GithubURL
            };

            _students.Add(student);

            return new Result<Student>
            {
                Success = true,
                Message = "Student created Successfully",
                Data = student
            };

        }
        public Result<List<StudentResponse>> GetAll(Filter? filter = null)
        {
            if (!_students.Any())
                return new Result<List<StudentResponse>>
                {
                    Success = false,
                    Message = "No Students Exists, create the first one.",
                    ErrorCode = 404
                };

            var errors = new List<string>();
            var students = _students.Select(s => new StudentResponse
            {
                Id = s.Id,
                FullName = s.FullName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                TrackName = s.TrackName,
                EnrollmentDate = s.EnrollmentDate,
                IsActive = s.IsActive,
                LinkedInURL = s.LinkedInURL,
                GithubURL = s.GithubURL
            }).ToList();

            
            if (filter.SearchTerm is not null)
            {
                students = students.Where(s => s.FullName.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)
                                            || s.Email.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (filter.TrackName is not null)
            {
                students = students.Where(s => s.TrackName.Contains(filter.TrackName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (filter.IsActive is not null)
            {
                students = students.Where(s => s.IsActive == filter.IsActive).ToList();
            }

            if (filter.PageSize is not null && filter.Page is not null)
            {
                if (filter.Page < 1) errors.Add("Page number must be greater than 1.");
                if (filter.PageSize < 1) errors.Add("Page size must be greater than 1.");
                if (filter.PageSize > 50) errors.Add("Page size must be less than 50.");
                if (errors.Any()) return new Result<List<StudentResponse>>
                {
                    Success = false,
                    Message = "Pagination parameters are not valid.",
                    Errors = errors,
                    ErrorCode = 400
                };

                students = students.Skip((int)((filter.Page - 1) * filter.PageSize))
                                   .Take((int)filter.PageSize).ToList();

            }

            if (!students.Any())
                return new Result<List<StudentResponse>>
                {
                    Success = false,
                    Message = "No Students found.",
                    ErrorCode = 404
                };

            return new Result<List<StudentResponse>>
            {
                Success = true,
                Message = $"Successfully retrieve ({students.Count}) students.",
                Data = students
            };
        }
        public Result<StudentStatsResponse> GetStats()
        {
            if (!_students.Any()) return new Result<StudentStatsResponse>()
            {
                Success = false,
                Message = "No students exists, create the first one.",
                Data = new StudentStatsResponse(),
                ErrorCode = 404
            };

            int total = _students.Count;
            int active = _students.Where(s => s.IsActive).Count();
            int inactive = _students.Where(s => !s.IsActive).Count();
            var trackCount = _students.GroupBy(s => s.TrackName).Select(s => new Dictionary<string, int>
            {
                { s.Select(t=>t.TrackName).First(),s.Count() }
            }).ToList();

            return new Result<StudentStatsResponse>
            {
                Success = true,
                Message = "Calculate Statistics Successfully.",
                Data = new StudentStatsResponse()
                {
                    TotalStudents = total,
                    ActiveStudents = active,
                    InActiveStudents = inactive,
                    CountByTrack = trackCount
                }
            };
        }
        public Result<Student> GetById(Guid id)
        {
            if (!_students.Any())
                return new Result<Student>
                {
                    Success = false,
                    Message = "No Students Exists, create the first one.",
                    ErrorCode = 404
                };

            var student = _students.FirstOrDefault(s => s.Id == id);

            if (student is null) return new Result<Student>
            {
                Success = false,
                Message = "Invalid student id",
                Errors = { $"Student with 'id:{id}' not found." },
                ErrorCode = 404
            };

            return new Result<Student>
            {
                Success = true,
                Message = "Successfully retrieval",
                Data = student
            };
        }
        public Result<Student> Update(Guid id, UpdateStudentRequest model)
        {
            if (model == null) return new Result<Student>
            {
                Success = false,
                Message = "Validation error, the update model must at least one field be provided.",
                ErrorCode = 400

            };

            var errors = new List<string>();

            if (model.Email is not null && !Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Invalid email address.");

            if (model.PhoneNumber is not null && !Regex.IsMatch(model.PhoneNumber, @"^01[0125]\d{8}$"))
                errors.Add("Phone number must be EGY phone number.");

            if (errors.Any()) return new Result<Student>
            {
                Success = false,
                Message = "Validation Errors.",
                Errors = errors,
                ErrorCode = 400
            };

            if (!_students.Any())
                return new Result<Student>
                {
                    Success = false,
                    Message = "No Students Exists, create the first one.",
                    ErrorCode = 404
                };

            var student = _students.FirstOrDefault(s => s.Id == id);


            if (student is null) return new Result<Student>
            {
                Success = false,
                Message = "Invalid student id",
                Errors = { $"Student with 'id:{id}' not found." },
                ErrorCode = 404
            };

            var index = _students.IndexOf(student);

            if (model.FName is not null) student.FName = model.FName;
            if (model.LName is not null) student.LName = model.LName;
            if (model.Email is not null) student.Email = model.Email;
            if (model.PhoneNumber is not null) student.PhoneNumber = model.PhoneNumber;
            if (model.TrackName is not null) student.TrackName = model.TrackName;
            if (model.LinkedInURL is not null) student.LinkedInURL = model.LinkedInURL;
            if (model.GithubURL is not null) student.GithubURL = model.GithubURL;

            _students[index] = student;

            return new Result<Student>
            {
                Success = true,
                Message = $"Successfully update student with id:{student.Id}.",
                Data = student
            };
        }
        public Result<Student> Update(Guid id, UpdateStudentStatusRequest model)
        {
            if (model == null) return new Result<Student>
            {
                Success = false,
                Message = "Validation error, the status must be provided.",
                Errors = { "Status is null" },
                ErrorCode = 400
            };

            if (!_students.Any())
                return new Result<Student>
                {
                    Success = false,
                    Message = "No Students Exists, create the first one.",
                    ErrorCode = 404
                };

            var student = _students.FirstOrDefault(s => s.Id == id);

            if (student is null) return new Result<Student>
            {
                Success = false,
                Message = "Invalid student id",
                Errors = { $"Student with 'id:{id}' not found." },
                ErrorCode = 404
            };

            if (student.IsActive == model.NewStatus) return model.NewStatus switch
            {
                true => new Result<Student>
                {
                    Success = false,
                    Message = "The student is already active.",
                    ErrorCode = 409 // conflict
                },
                false => new Result<Student>
                {
                    Success = false,
                    Message = "The student is already inactive.",
                    ErrorCode = 409
                }
            };
            var index = _students.IndexOf(student);

            student.IsActive = model.NewStatus;

            _students[index] = student;
            return new Result<Student>
            {
                Success = true,
                Message = $"Successfully update student status.",
                Data = student
            };
        }
        public Result<Student> Delete(Guid id)
        {
            if (!_students.Any())
                return new Result<Student>
                {
                    Success = false,
                    Message = "No Students Exists, create the first one.",
                    ErrorCode = 404
                };

            var student = _students.FirstOrDefault(s => s.Id == id);

            if (student is null) return new Result<Student>
            {
                Success = false,
                Message = "Invalid student id",
                Errors = { $"Student with 'id:{id}' not found." },
                ErrorCode = 404
            };

           
            bool result = _students.Remove(student);
            if(!result) return new Result<Student>
            {
                Success = false,
                Message = "Something went wrong, Try again.",
                Errors = {"Internal Server Error."},
                ErrorCode = 500
            };

            return new Result<Student>
            {
                Success = true,
                Message = "Successfully Deleted",
            };
        }
    }
}

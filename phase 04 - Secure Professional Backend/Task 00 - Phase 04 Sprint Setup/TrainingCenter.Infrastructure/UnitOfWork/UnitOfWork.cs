using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TrainingCenter.Application.Interfaces.Persistence;
using TrainingCenter.Domain.Interfaces.Repositories;
using TrainingCenter.Infrastructure.Data;
using TrainingCenter.Infrastructure.Repositories;

namespace TrainingCenter.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IUserRepository _users;
        private IAdminRepository _admins;
        private IInstructorRepository _instructors;
        private IStudentRepository _students;
        private IPaymentRepository _payments;
        private ITrackRepository _tracks;
        private IEnrollmentRepository _enrollments;
        private IRefreshTokenRepository _refreshTokens;
        private ITrackSessionRepository _trackSessions;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public IUserRepository Users
        {
            get
            {
                if (_users is null)
                {
                    _users = new UserRepository(_context);
                }

                return _users;
            }
        }
        public IAdminRepository Admins
        {
            get
            {
                if (_admins is null)
                {
                    _admins = new AdminRepository(_context);
                }

                return _admins;
            }
        }
        public IInstructorRepository Instructors
        {
            get
            {
                if (_instructors is null)
                {
                    _instructors = new InstructorRepository(_context);
                }

                return _instructors;
            }
        }
        public IStudentRepository Students
        {
            get
            {
                if (_students is null)
                {
                    _students = new StudentRepository(_context);
                }

                return _students;
            }
        }
        public IPaymentRepository Payments
        {
            get
            {
                if (_payments is null)
                {
                    _payments = new PaymentRepository(_context);
                }

                return _payments;
            }
        }
        public ITrackRepository Tracks
        {
            get
            {
                if (_tracks is null)
                {
                    _tracks = new TrackRepository(_context);
                }

                return _tracks;
            }
        }
        public IEnrollmentRepository Enrollments
        {
            get
            {
                if (_enrollments is null)
                {
                    _enrollments = new EnrollmentRepository(_context);
                }

                return _enrollments;
            }
        }
        public IRefreshTokenRepository RefreshTokens
        {
            get
            {
                if (_refreshTokens is null)
                {
                    _refreshTokens = new RefreshTokenRepository(_context);
    }

                return _refreshTokens;
            }
        }
        public ITrackSessionRepository TrackSessions
        {
            get
            {
                if (_trackSessions is null)
                {
                    _trackSessions = new TrackSessionRepository(_context);
                }

                return _trackSessions;
            }
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        }
        public IExecutionStrategy CreateExecutionStrategy()
        {
            return _context.Database.CreateExecutionStrategy();
        }
    }
}

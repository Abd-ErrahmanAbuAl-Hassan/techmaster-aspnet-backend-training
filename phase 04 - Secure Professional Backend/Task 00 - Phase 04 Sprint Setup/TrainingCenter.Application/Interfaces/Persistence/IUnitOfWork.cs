using Microsoft.EntityFrameworkCore.Storage;
using TrainingCenter.Domain.Interfaces.Repositories;

namespace TrainingCenter.Application.Interfaces.Persistence
{
    public interface IUnitOfWork: IDisposable
    {
        IUserRepository Users { get; }
        IAdminRepository Admins { get; }
        IInstructorRepository Instructors { get; }
        IStudentRepository Students { get; }
        IPaymentRepository Payments { get; }
        ITrackRepository Tracks { get; }
        IEnrollmentRepository Enrollments { get; }
        IRefreshTokenRepository RefreshTokens { get; }

        Task<int> SaveAsync();

        Task<IDbContextTransaction> BeginTransactionAsync();
        IExecutionStrategy CreateExecutionStrategy();
    }
}


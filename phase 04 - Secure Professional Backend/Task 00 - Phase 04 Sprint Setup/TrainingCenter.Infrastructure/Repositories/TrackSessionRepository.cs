using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Interfaces.Repositories;
using TrainingCenter.Infrastructure.Data;

namespace TrainingCenter.Infrastructure.Repositories
{
    public class TrackSessionRepository: GenericRepository<TrackSession>, ITrackSessionRepository
    {
        public TrackSessionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

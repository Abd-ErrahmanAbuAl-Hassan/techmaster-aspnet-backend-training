using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Interfaces.Repositories;
using TrainingCenter.Infrastructure.Data;

namespace TrainingCenter.Infrastructure.Repositories
{
    public class TrackRepository: GenericRepository<TrainingTrack>, ITrackRepository
    {
        public TrackRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

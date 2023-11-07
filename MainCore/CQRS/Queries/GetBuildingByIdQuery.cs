using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.CQRS.Queries
{
    public class GetBuildingByIdQuery : IRequest<BuildingDto>
    {
        public BuildingId BuildingId { get; }

        public GetBuildingByIdQuery(BuildingId buildingId)
        {
            BuildingId = buildingId;
        }
    }

    public class GetBuildingByIdQueryHandler : IRequestHandler<GetBuildingByIdQuery, BuildingDto>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public GetBuildingByIdQueryHandler(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<BuildingDto> Handle(GetBuildingByIdQuery request, CancellationToken cancellationToken)
        {
            var building = await Task.Run(() => Get(request.BuildingId), cancellationToken);
            return building;
        }

        public BuildingDto Get(BuildingId buildingId)
        {
            using var context = _contextFactory.CreateDbContext();

            var building = context.Buildings
                .AsNoTracking()
                .Where(x => x.Id == buildingId.Value)
                .ToDto()
                .FirstOrDefault();
            return building;
        }
    }
}
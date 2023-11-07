using Humanizer;
using MainCore.Common.Enums;
using MainCore.Common.Models;
using MainCore.CQRS.Base;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MainCore.UI.Models.Output;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MainCore.CQRS.Queries
{
    public class GetJobListBoxItemsQuery : ByVillageIdRequestBase, IRequest<List<ListBoxItem>>
    {
        public GetJobListBoxItemsQuery(VillageId villageId) : base(villageId)
        {
        }
    }

    public class GetJobListBoxItemsQueryHandler : IRequestHandler<GetJobListBoxItemsQuery, List<ListBoxItem>>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public GetJobListBoxItemsQueryHandler(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ListBoxItem>> Handle(GetJobListBoxItemsQuery request, CancellationToken cancellationToken)
        {
            var items = await Task.Run(() => GetItems(request.VillageId), cancellationToken);
            return items;
        }

        private List<ListBoxItem> GetItems(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();

            var items = context.Jobs
                .AsNoTracking()
                .Where(x => x.VillageId == villageId.Value)
                .OrderBy(x => x.Position)
                .ToDto()
                .AsEnumerable()
                .Select(x => new ListBoxItem()
                {
                    Id = x.Id.Value,
                    Content = GetContent(x),
                })
                .ToList();

            return items;
        }

        private static string GetContent(JobDto job)
        {
            switch (job.Type)
            {
                case JobTypeEnums.NormalBuild:
                    {
                        var plan = JsonSerializer.Deserialize<NormalBuildPlan>(job.Content);
                        return $"Build {plan.Type.Humanize()} to level {plan.Level} at location {plan.Location}";
                    }
                case JobTypeEnums.ResourceBuild:
                    {
                        var plan = JsonSerializer.Deserialize<ResourceBuildPlan>(job.Content);
                        return $"Build {plan.Plan.Humanize()} to level {plan.Level}";
                    }
                default:
                    return job.Content;
            }
        }
    }
}
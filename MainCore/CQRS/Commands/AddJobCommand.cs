using MainCore.Common.Enums;
using MainCore.Common.Models;
using MainCore.Notification;
using MainCore.CQRS.Base;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MainCore.CQRS.Commands
{
    public class AddJobCommand<T> : ByVillageIdRequestBase, IRequest
    {
        public T Content { get; }

        public AddJobCommand(VillageId villageId, T content) : base(villageId)
        {
            Content = content;
        }
    }

    public class AddJobCommandHandler<T> : IRequestHandler<AddJobCommand<T>>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IMediator _mediator;

        private readonly Dictionary<Type, JobTypeEnums> _jobTypes = new()
        {
            { typeof(NormalBuildPlan),JobTypeEnums.NormalBuild  },
            { typeof(ResourceBuildPlan),JobTypeEnums.ResourceBuild },
        };

        public AddJobCommandHandler(IDbContextFactory<AppDbContext> contextFactory, IMediator mediator)
        {
            _contextFactory = contextFactory;
            _mediator = mediator;
        }

        public async Task Handle(AddJobCommand<T> request, CancellationToken cancellationToken)
        {
            await Task.Run(() => Add(request.VillageId, request.Content), cancellationToken);
            await _mediator.Publish(new JobUpdated(request.VillageId), cancellationToken);
        }

        public void Add(VillageId villageId, T content)
        {
            using var context = _contextFactory.CreateDbContext();
            var count = context.Jobs
                .AsNoTracking()
                .Where(x => x.VillageId == villageId.Value)
                .Count();

            var job = new Job()
            {
                Position = count,
                VillageId = villageId.Value,
                Type = _jobTypes[typeof(T)],
                Content = JsonSerializer.Serialize(content),
            };

            context.Add(job);
            context.SaveChanges();
        }
    }
}
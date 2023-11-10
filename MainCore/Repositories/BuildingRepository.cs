﻿using Humanizer;
using MainCore.Common.Enums;
using MainCore.Common.Extensions;
using MainCore.Common.Models;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MainCore.UI.Models.Output;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace MainCore.Repositories
{
    [RegisterAsTransient]
    public class BuildingRepository : IBuildingRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public BuildingRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public BuildingDto GetBuilding(VillageId villageId, int location)
        {
            using var context = _contextFactory.CreateDbContext();
            var building = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Location == location)
                .ToDto()
                .FirstOrDefault();
            return building;
        }

        public int CountQueueBuilding(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var count = context.QueueBuildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type != BuildingEnums.Site)
                .Count();
            return count;
        }

        public int CountResourceQueueBuilding(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var resourceTypes = new List<BuildingEnums>()
            {
                BuildingEnums.Woodcutter,
                BuildingEnums.ClayPit,
                BuildingEnums.IronMine,
                BuildingEnums.Cropland
            };

            var count = context.QueueBuildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => resourceTypes.Contains(x.Type))
                .Count();
            return count;
        }

        public bool IsRallyPointExists(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var isExists = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type == BuildingEnums.RallyPoint)
                .Where(x => x.Level > 0)
                .Any();
            return isExists;
        }

        public bool IsEmptySite(VillageId villageId, int location)
        {
            using var context = _contextFactory.CreateDbContext();
            var isEmptySite = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type == BuildingEnums.Site)
                .Where(x => x.Location == location)
                .Any();
            return isEmptySite;
        }

        public bool IsJobComplete(VillageId villageId, JobDto job)
        {
            if (job.Type == JobTypeEnums.ResourceBuild) return false;
            var plan = JsonSerializer.Deserialize<NormalBuildPlan>(job.Content);

            using var context = _contextFactory.CreateDbContext();

            var queueBuilding = context.QueueBuildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Location == plan.Location)
                .OrderByDescending(x => x.Level)
                .FirstOrDefault();

            if (queueBuilding is not null && queueBuilding.Level >= plan.Level) return true;

            var villageBuilding = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Location == plan.Location)
                .FirstOrDefault();
            if (villageBuilding is not null && villageBuilding.Level >= plan.Level) return true;

            return false;
        }

        public Building GetCropland(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var building = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type == BuildingEnums.Cropland)
                .OrderBy(x => x.Level)
                .FirstOrDefault();
            return building;
        }

        public NormalBuildPlan GetNormalBuildPlan(VillageId villageId, ResourceBuildPlan plan)
        {
            using var context = _contextFactory.CreateDbContext();

            var resourceTypes = new List<BuildingEnums>();

            switch (plan.Plan)
            {
                case ResourcePlanEnums.AllResources:
                    resourceTypes.AddRange(new[]
                    {
                         BuildingEnums.Woodcutter,
                         BuildingEnums.ClayPit,
                         BuildingEnums.IronMine,
                         BuildingEnums.Cropland,
                    });
                    break;

                case ResourcePlanEnums.ExcludeCrop:
                    resourceTypes.AddRange(new[]
                    {
                         BuildingEnums.Woodcutter,
                         BuildingEnums.ClayPit,
                         BuildingEnums.IronMine,
                    });
                    break;

                case ResourcePlanEnums.OnlyCrop:
                    resourceTypes.AddRange(new[]
                    {
                         BuildingEnums.Cropland,
                    });
                    break;

                default:
                    break;
            }
            var buildings = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => resourceTypes.Contains(x.Type))
                .Where(x => x.Level < plan.Level)
                .ToList();

            var queueBuildings = context.QueueBuildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => resourceTypes.Contains(x.Type))
                .GroupBy(x => x.Location)
                .Select(x => new
                {
                    Location = x.Key,
                    UpgradingLevel = x.OrderBy(x => x.Level).Count()
                })
                .AsEnumerable();

            foreach (var queueBuilding in queueBuildings)
            {
                var building = buildings.FirstOrDefault(x => x.Location == queueBuilding.Location);
                building.Level += queueBuilding.UpgradingLevel;
            }

            buildings = buildings
                .Where(x => x.Level < plan.Level)
                .ToList();
            if (!buildings.Any()) return null;
            var chosenOne = buildings
                .OrderBy(x => x.Level)
                .FirstOrDefault();

            var normalBuildPlan = new NormalBuildPlan()
            {
                Type = chosenOne.Type,
                Level = chosenOne.Level + 1,
                Location = chosenOne.Location,
            };
            return normalBuildPlan;
        }

        public (BuildingEnums, int) GetBuildingInfo(BuildingId buildingId)
        {
            using var context = _contextFactory.CreateDbContext();

            var building = context.Buildings
                .AsNoTracking()
                .Where(x => x.Id == buildingId.Value)
                .Select(x => new
                {
                    x.Type,
                    x.Level,
                })
                .FirstOrDefault();

            return (building.Type, building.Level);
        }

        public List<ListBoxItem> GetItems(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.Buildings
                .AsNoTracking()
                .Where(x => x.VillageId == villageId.Value)
                .OrderBy(x => x.Location)
                .AsEnumerable()
                .Select(x => new BuildingItemDto()
                {
                    Id = new BuildingId(x.Id),
                    Location = x.Location,
                    Type = x.Type,
                    Level = x.Level,
                })
                .ToList();

            var queueBuildings = context.QueueBuildings
                .AsNoTracking()
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type != BuildingEnums.Site)
                .GroupBy(x => x.Location)
                .Select(x => new Building()
                {
                    Location = x.Key,
                    Type = x.OrderBy(x => x.Id).Select(x => x.Type).First(),
                    Level = x.OrderBy(x => x.Id).Select(x => x.Level).Max(),
                })
                .AsEnumerable();

            foreach (var queueBuilding in queueBuildings)
            {
                var villageBuilding = buildings.FirstOrDefault(x => x.Location == queueBuilding.Location);
                villageBuilding.QueueLevel = queueBuilding.Level;
            }

            var jobBuildings = context.Jobs
                .AsNoTracking()
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type == JobTypeEnums.NormalBuild)
                .Select(x => x.Content)
                .AsEnumerable()
                .Select(x => JsonSerializer.Deserialize<NormalBuildPlan>(x))
                .GroupBy(x => x.Location)
                .Select(x => new Building()
                {
                    Location = x.Key,
                    Type = x.OrderBy(x => x.Level).Select(x => x.Type).First(),
                    Level = x.OrderByDescending(x => x.Level).Select(x => x.Level).First(),
                });

            foreach (var jobBuilding in jobBuildings)
            {
                var villageBuilding = buildings.FirstOrDefault(x => x.Location == jobBuilding.Location);
                villageBuilding.JobLevel = jobBuilding.Level;
            }

            var resourceJobs = context.Jobs
               .AsNoTracking()
               .Where(x => x.VillageId == villageId.Value)
               .Where(x => x.Type == JobTypeEnums.ResourceBuild)
               .Select(x => x.Content)
               .AsEnumerable()
               .Select(x => JsonSerializer.Deserialize<ResourceBuildPlan>(x))
               .GroupBy(x => x.Plan)
               .Select(x => new ResourceBuildPlan
               {
                   Plan = x.Key,
                   Level = x.OrderByDescending(x => x.Level).Select(x => x.Level).First(),
               })
               .ToDictionary(x => x.Plan, x => x.Level);
            foreach (var building in buildings)
            {
                if (!building.Type.IsResourceField()) continue;
                foreach (var job in resourceJobs)
                {
                    switch (job.Key)
                    {
                        case ResourcePlanEnums.AllResources:
                            building.JobLevel = building.JobLevel < job.Value ? job.Value : building.JobLevel;
                            break;

                        case ResourcePlanEnums.ExcludeCrop:
                            if (building.Type == BuildingEnums.Cropland) break;
                            building.JobLevel = building.JobLevel < job.Value ? job.Value : building.JobLevel;
                            break;

                        case ResourcePlanEnums.OnlyCrop:
                            if (building.Type != BuildingEnums.Cropland) break;
                            building.JobLevel = building.JobLevel < job.Value ? job.Value : building.JobLevel;
                            break;

                        default:
                            break;
                    }
                }
            }

            var items = buildings
                .Select(x => ToListBoxItem(x))
                .ToList();
            return items;
        }

        private static ListBoxItem ToListBoxItem(BuildingItemDto building)
        {
            const string arrow = " -> ";
            var sb = new StringBuilder();
            sb.Append(building.Level);
            if (building.QueueLevel != 0)
            {
                var content = $"{arrow}({building.QueueLevel})";
                sb.Append(content);
            }
            if (building.JobLevel != 0)
            {
                var content = $"{arrow}[{building.JobLevel}]";
                sb.Append(content);
            }

            var item = new ListBoxItem()
            {
                Id = building.Id.Value,
                Content = $"[{building.Location}] {building.Type.Humanize()} | lvl {sb}",
                Color = building.Type.GetColor(),
            };
            return item;
        }

        public List<BuildingItem> GetLevelBuildings(VillageId villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var villageBuildings = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .Select(x => new BuildingItem()
                {
                    Location = x.Location,
                    Type = x.Type,
                    Level = x.Level,
                })
                .AsEnumerable();

            var queueBuildings = context.QueueBuildings
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type != BuildingEnums.Site)
                .GroupBy(x => x.Location)
                .Select(x => new BuildingItem()
                {
                    Location = x.Key,
                    Type = x.OrderBy(x => x.Id).Select(x => x.Type).First(),
                    Level = x.OrderByDescending(x => x.Location).Select(x => x.Level).First(),
                })
                .AsEnumerable();

            var jobBuildings = context.Jobs
                .Where(x => x.VillageId == villageId.Value)
                .Where(x => x.Type == JobTypeEnums.NormalBuild)
                .Select(x => x.Content)
                .AsEnumerable()
                .Select(x => JsonSerializer.Deserialize<NormalBuildPlan>(x))
                .GroupBy(x => x.Location)
                .Select(x => new BuildingItem()
                {
                    Location = x.Key,
                    Type = x.OrderBy(x => x.Location).Select(x => x.Type).First(),
                    Level = x.OrderByDescending(x => x.Location).Select(x => x.Level).First(),
                });

            var buildings = new[] { jobBuildings, queueBuildings, villageBuildings }
                .SelectMany(x => x)
                .GroupBy(x => x.Location)
                .Select(x => new BuildingItem()
                {
                    Location = x.Key,
                    Type = x.OrderBy(x => x.Location).Select(x => x.Type).First(),
                    Level = x.OrderByDescending(x => x.Location).Select(x => x.Level).First(),
                })
                .OrderBy(x => x.Location)
                .ToList();

            var resourceJobs = context.Jobs
               .Where(x => x.VillageId == villageId.Value)
               .Where(x => x.Type == JobTypeEnums.ResourceBuild)
               .Select(x => x.Content)
               .AsEnumerable()
               .Select(x => JsonSerializer.Deserialize<ResourceBuildPlan>(x))
               .GroupBy(x => x.Plan)
               .Select(x => new ResourceBuildPlan
               {
                   Plan = x.Key,
                   Level = x.OrderByDescending(x => x.Level).Select(x => x.Level).First(),
               })
               .ToDictionary(x => x.Plan, x => x.Level);
            foreach (var building in buildings)
            {
                if (!building.Type.IsResourceField()) continue;
                foreach (var job in resourceJobs)
                {
                    if (job.Key == ResourcePlanEnums.AllResources)
                    {
                        building.Level = building.Level < job.Value ? job.Value : building.Level;
                        continue;
                    }
                    if (job.Key == ResourcePlanEnums.ExcludeCrop)
                    {
                        if (building.Type == BuildingEnums.Cropland) continue;
                        building.Level = building.Level < job.Value ? job.Value : building.Level;
                        continue;
                    }
                    if (job.Key == ResourcePlanEnums.OnlyCrop)
                    {
                        if (building.Type != BuildingEnums.Cropland) continue;
                        building.Level = building.Level < job.Value ? job.Value : building.Level;
                        continue;
                    }
                }
            }
            return buildings;
        }

        public void Update(VillageId villageId, List<BuildingDto> dtos)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbBuildings = context.Buildings
                .Where(x => x.VillageId == villageId.Value)
                .ToList();

            foreach (var dto in dtos)
            {
                var dbBuilding = dbBuildings
                    .FirstOrDefault(x => x.Location == dto.Location);
                if (dbBuilding is null)
                {
                    var building = dto.ToEntity(villageId);
                    context.Add(building);
                }
                else
                {
                    dto.To(dbBuilding);
                    context.Update(dbBuilding);
                }
            }
            context.SaveChanges();
        }
    }
}
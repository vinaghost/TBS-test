using Humanizer;
using MainCore.Common.Enums;
using MainCore.Common.Extensions;
using MainCore.Common.Models;
using MainCore.DTO;
using ReactiveUI;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace MainCore.UI.Models.Output
{
    public class ListBoxItem : ReactiveObject
    {
        public ListBoxItem(int id)
        {
            Id = id;
        }

        public ListBoxItem(AccountDto account) : this(account.Id)
        {
            Content = Get(account);
            Color = Color.Black;
        }

        public void Set(AccountDto account)
        {
            Content = Get(account);
        }

        private static string Get(AccountDto account)
        {
            var serverUrl = new Uri(account.Server);
            return $"{account.Username}{Environment.NewLine}({serverUrl.Host})";
        }

        public ListBoxItem(VillageDto village) : this(village.Id)
        {
            Content = Get(village);
            Color = Color.Black;
        }

        public void Set(VillageDto village)
        {
            Content = Get(village);
        }

        private static string Get(VillageDto village)
        {
            return $"{village.Name}{Environment.NewLine}({village.X}|{village.Y})";
        }

        public ListBoxItem(BuildingItemDto building) : this(building.Id)
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
            Content = $"[{building.Location}] {building.Type.Humanize()} | lvl {sb} ";
            Color = building.Type.GetColor();
        }

        public ListBoxItem(JobDto job) : this(job.Id)
        {
            Content = Get(job);
            Color = Color.Black;
        }

        public void Set(JobDto job)
        {
            Content = Get(job);
        }

        private static string Get(JobDto job)
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

        public ListBoxItem(FarmListDto farmList) : this(farmList.Id)
        {
            Content = farmList.Name;
            if (farmList.IsActive)
            {
                Color = Color.Green;
            }
            else
            {
                Color = Color.Red;
            }
        }

        public int Id { get; set; }
        private string _content;

        public string Content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        private Color _color;

        public Color Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }
    }
}
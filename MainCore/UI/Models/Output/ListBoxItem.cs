using Humanizer;
using MainCore.Common.Enums;
using MainCore.Common.Extensions;
using MainCore.Common.Models;
using MainCore.DTO;
using MainCore.Entities;
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
            var serverUrl = new Uri(account.Server);
            Content = $"{account.Username}{Environment.NewLine}({serverUrl.Host})";
            Color = Color.Black;
        }

        public ListBoxItem(Village village) : this(village.Id)
        {
            Content = $"{village.Name}{Environment.NewLine}({village.X}|{village.Y})";
            Color = Color.Black;
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

        public ListBoxItem(Job job) : this(job.Id)
        {
            switch (job.Type)
            {
                case JobTypeEnums.NormalBuild:
                    {
                        var plan = JsonSerializer.Deserialize<NormalBuildPlan>(job.Content);
                        Content = $"Build {plan.Type.Humanize()} to level {plan.Level} at location {plan.Location}";
                        break;
                    }
                case JobTypeEnums.ResourceBuild:
                    {
                        var plan = JsonSerializer.Deserialize<ResourceBuildPlan>(job.Content);
                        Content = $"Build {plan.Plan.Humanize()} to level {plan.Level}";
                        break;
                    }
                default:
                    Content = job.Content;
                    break;
            }

            Color = Color.Black;
        }

        public ListBoxItem(FarmList farmList) : this(farmList.Id)
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
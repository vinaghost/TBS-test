using Humanizer;
using MainCore.Enums;
using MainCore.Models;
using MainCore.Models.Plans;
using ReactiveUI;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Media;

namespace WPFUI.Models.Output
{
    public class ListBoxItem : ReactiveObject
    {
        public ListBoxItem(int id)
        {
            Id = id;
        }

        public ListBoxItem(Account account) : this(account.Id)
        {
            var serverUrl = new Uri(account.Server);
            Content = $"{account.Username}{Environment.NewLine}({serverUrl.Host})";
            Color = Color.FromRgb(0, 0, 0);
        }

        public ListBoxItem(Village village) : this(village.Id)
        {
            Content = $"{village.Name}{Environment.NewLine}({village.X}|{village.Y})";
            Color = Color.FromRgb(0, 0, 0);
        }

        public ListBoxItem(Building building) : this(building.Id)
        {
            Content = $"[{building.Location}] {building.Type.Humanize()} | lvl {building.Level}";
            Color = building.Type.GetColor();
        }

        public ListBoxItem(Job job) : this(job.Id)
        {
            switch (job.Type)
            {
                case JobTypeEnums.NormalBuild:
                    {
                        var plan = JsonSerializer.Deserialize<NormalBuildPlan>(job.Content);
                        Content = $"Build {plan.Building.Humanize()} to level {plan.Level} at location {plan.Location}";
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

            Color = Color.FromRgb(0, 0, 0);
        }

        public void CopyFrom(ListBoxItem source)
        {
            Id = source.Id;
            Content = source.Content;
            Color = source.Color;
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

    public static class ColorExtension
    {
        /// <summary>
        /// Convert Media Color (WPF) to Drawing Color (WinForm)
        /// </summary>
        /// <param name="mediaColor"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Drawing.Color ToDrawingColor(this System.Windows.Media.Color mediaColor)
        {
            return System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
        }

        /// <summary>
        /// Convert Drawing Color (WinForm) to Media Color (WPF)
        /// </summary>
        /// <param name="drawingColor"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Windows.Media.Color ToMediaColor(this System.Drawing.Color drawingColor)
        {
            return System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
        }

        public static Color GetColor(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.Site => System.Drawing.Color.White.ToMediaColor(),
                BuildingEnums.Woodcutter => System.Drawing.Color.Lime.ToMediaColor(),
                BuildingEnums.ClayPit => System.Drawing.Color.Orange.ToMediaColor(),
                BuildingEnums.IronMine => System.Drawing.Color.LightGray.ToMediaColor(),
                BuildingEnums.Cropland => System.Drawing.Color.Yellow.ToMediaColor(),
                _ => System.Drawing.Color.LightCyan.ToMediaColor(),
            };
        }
    }
}
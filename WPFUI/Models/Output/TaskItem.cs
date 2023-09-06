using MainCore.Enums;
using MainCore.Tasks;
using System;

namespace WPFUI.Models.Output
{
    public class TaskItem
    {
        public TaskItem(TaskBase task)
        {
            Task = task.GetName();
            ExecuteAt = task.ExecuteAt;
            Stage = task.Stage;
        }

        public string Task { get; set; }
        public DateTime ExecuteAt { get; set; }
        public StageEnums Stage { get; set; }
    }
}
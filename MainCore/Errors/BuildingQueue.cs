﻿using FluentResults;

namespace MainCore.Errors
{
    public class BuildingQueue : Error
    {
        private BuildingQueue(string message) : base(message)
        {
        }

        public static BuildingQueue NotTaskInqueue => new($"There is no suitable task available in job queue");

        public static BuildingQueue Full => new("Amount of currently building is equal with maximum building can build in same time");
    }
}
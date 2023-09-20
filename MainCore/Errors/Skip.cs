﻿using FluentResults;

namespace MainCore.Errors
{
    public class Skip : Error
    {
        public Skip(string message) : base(message)
        {
        }

        public static Skip VillageNotFound => new("Village not found");
        public static Skip BuildingJobQueueEmpty => new("Building job queue is empty");
        public static Skip BuildingQueueFull => new("Building queue is full");
    }
}
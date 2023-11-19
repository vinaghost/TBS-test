﻿using MainCore.Entities;
using MediatR;

namespace MainCore.Notification.Message
{
    internal class QueueBuildingUpdated : INotification
    {
        public VillageId VillageId { get; }

        public QueueBuildingUpdated(VillageId villageId)
        {
            VillageId = villageId;
        }
    }
}
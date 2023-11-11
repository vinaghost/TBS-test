﻿using MainCore.Notification;
using MainCore.UI.ViewModels.Tabs.Villages;
using MediatR;

namespace MainCore.Notification.Handler
{
    public class VillageSettingRefresh : INotificationHandler<VillageSettingUpdated>
    {
        private readonly VillageSettingViewModel _viewModel;

        public VillageSettingRefresh(VillageSettingViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public async Task Handle(VillageSettingUpdated notification, CancellationToken cancellationToken)
        {
            await _viewModel.SettingRefresh(notification.VillageId);
        }
    }
}
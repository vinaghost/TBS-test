﻿using MainCore.Common.Notification;
using MainCore.UI.ViewModels.UserControls;
using MediatR;

namespace MainCore.UI.Handler
{
    public class AccountListRefresh : INotificationHandler<AccountUpdated>
    {
        private readonly MainLayoutViewModel _mainlayoutViewModel;

        public AccountListRefresh(MainLayoutViewModel mainlayoutViewModel)
        {
            _mainlayoutViewModel = mainlayoutViewModel;
        }

        public async Task Handle(AccountUpdated notification, CancellationToken cancellationToken)
        {
            await _mainlayoutViewModel.LoadAccountList();
        }
    }
}
﻿using System.Windows;

namespace WPFUI.Services
{
    public sealed class MessageService : IMessageService
    {
        public bool Show(string title, string message)
        {
            return MessageBox.Show(message, title,
                        MessageBoxButton.OK) == MessageBoxResult.OK;
        }

        public bool ShowYesNo(string title, string message)
        {
            return MessageBox.Show(message, title,
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }
    }
}
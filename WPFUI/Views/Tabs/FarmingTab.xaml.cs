﻿using MainCore.UI.ViewModels.Tabs;
using ReactiveUI;
using System.Reactive.Disposables;

namespace WPFUI.Views.Tabs
{
    public class FarmingTabBase : ReactiveUserControl<FarmingViewModel>
    {
    }

    /// <summary>
    /// Interaction logic for FarmingTab.xaml
    /// </summary>
    public partial class FarmingTab : FarmingTabBase
    {
        public FarmingTab()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.StartCommand, v => v.StartButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.StopCommand, v => v.StopButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.ActiveFarmListCommand, v => v.ActiveButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LoadFarmListCommand, v => v.Load).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.FarmLists, v => v.FarmlistGrid.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedFarmList, v => v.FarmlistGrid.SelectedItem).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.FarmListSettingInput.UseStartAllButton, v => v.UseStartAllCheckbox.IsChecked).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.FarmListSettingInput.FarmInterval, v => v.FarmInterval.ViewModel).DisposeWith(d);
            });
        }
    }
}
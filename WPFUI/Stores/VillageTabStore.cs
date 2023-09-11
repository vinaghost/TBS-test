using ReactiveUI;
using WPFUI.Enums;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Stores
{
    public class VillageTabStore : ViewModelBase
    {
        private readonly bool[] _tabVisibility = new bool[2];
        private VillageTabType _currentTabType;

        private bool _isNoVillageTabVisible = true;
        private bool _isNormalTabVisible;

        private readonly NoVillageViewModel _noVillageViewModel;
        private readonly InfoViewModel _infoViewModel;

        public VillageTabStore(NoVillageViewModel noVillageViewModel, InfoViewModel infoViewModel)
        {
            _noVillageViewModel = noVillageViewModel;
            _infoViewModel = infoViewModel;
        }

        public void SetTabType(VillageTabType tabType)
        {
            if (tabType == _currentTabType) return;
            _currentTabType = tabType;

            for (int i = 0; i < _tabVisibility.Length; i++)
            {
                _tabVisibility[i] = false;
            }
            _tabVisibility[(int)tabType] = true;

            IsNoVillageTabVisible = _tabVisibility[(int)VillageTabType.NoVillage];
            IsNormalTabVisible = _tabVisibility[(int)VillageTabType.Normal];

            switch (tabType)
            {
                case VillageTabType.NoVillage:
                    _noVillageViewModel.IsActive = true;
                    break;

                case VillageTabType.Normal:
                    _infoViewModel.IsActive = true;
                    break;

                default:
                    break;
            }
        }

        public bool IsNoVillageTabVisible
        {
            get => _isNoVillageTabVisible;
            set => this.RaiseAndSetIfChanged(ref _isNoVillageTabVisible, value);
        }

        public bool IsNormalTabVisible
        {
            get => _isNormalTabVisible;
            set => this.RaiseAndSetIfChanged(ref _isNormalTabVisible, value);
        }

        public NoVillageViewModel NoVillageViewModel => _noVillageViewModel;
        public InfoViewModel InfoViewModel => _infoViewModel;
    }
}
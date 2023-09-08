using ReactiveUI;
using WPFUI.Enums;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.Tabs;

namespace WPFUI.Stores
{
    public class AccountTabStore : ViewModelBase
    {
        private readonly bool[] _tabVisibility = new bool[4];
        private TabType _currentTabType;

        private bool _isNoAccountTabVisible = true;
        private bool _isAddAccountTabVisible;
        private bool _isAddAccountsTabVisible;
        private bool _isNormalTabVisible;

        private readonly NoAccountViewModel _noAccountViewModel;
        private readonly AddAccountViewModel _addAccountViewModel;
        private readonly AddAccountsViewModel _addAccountsViewModel;
        private readonly AccountSettingViewModel _accountSettingViewModel;
        private readonly EditAccountViewModel _editAccountViewModel;
        private readonly DebugViewModel _debugViewModel;

        public AccountTabStore(NoAccountViewModel noAccountViewModel, AddAccountViewModel addAccountViewModel, AddAccountsViewModel addAccountsViewModel, EditAccountViewModel editAccountViewModel, DebugViewModel debugViewModel, AccountSettingViewModel accountSettingViewModel)
        {
            _noAccountViewModel = noAccountViewModel;
            _addAccountViewModel = addAccountViewModel;
            _addAccountsViewModel = addAccountsViewModel;
            _accountSettingViewModel = accountSettingViewModel;
            _editAccountViewModel = editAccountViewModel;
            _debugViewModel = debugViewModel;
        }

        public void SetTabType(TabType tabType)
        {
            if (tabType == _currentTabType) return;
            _currentTabType = tabType;

            for (int i = 0; i < _tabVisibility.Length; i++)
            {
                _tabVisibility[i] = false;
            }
            _tabVisibility[(int)tabType] = true;

            IsNoAccountTabVisible = _tabVisibility[(int)TabType.NoAccount];
            IsAddAccountTabVisible = _tabVisibility[(int)TabType.AddAccount];
            IsAddAccountsTabVisible = _tabVisibility[(int)TabType.AddAccounts];
            IsNormalTabVisible = _tabVisibility[(int)TabType.Normal];

            switch (tabType)
            {
                case TabType.NoAccount:
                    _noAccountViewModel.IsActive = true;
                    break;

                case TabType.Normal:
                    _accountSettingViewModel.IsActive = true;
                    break;

                case TabType.AddAccount:
                    _addAccountViewModel.IsActive = true;
                    break;

                case TabType.AddAccounts:
                    _addAccountsViewModel.IsActive = true;
                    break;

                default:
                    break;
            }
        }

        public bool IsNoAccountTabVisible
        {
            get => _isNoAccountTabVisible;
            set => this.RaiseAndSetIfChanged(ref _isNoAccountTabVisible, value);
        }

        public bool IsAddAccountTabVisible
        {
            get => _isAddAccountTabVisible;
            set => this.RaiseAndSetIfChanged(ref _isAddAccountTabVisible, value);
        }

        public bool IsAddAccountsTabVisible
        {
            get => _isAddAccountsTabVisible;
            set => this.RaiseAndSetIfChanged(ref _isAddAccountsTabVisible, value);
        }

        public bool IsNormalTabVisible
        {
            get => _isNormalTabVisible;
            set => this.RaiseAndSetIfChanged(ref _isNormalTabVisible, value);
        }

        public NoAccountViewModel NoAccountViewModel => _noAccountViewModel;
        public AddAccountViewModel AddAccountViewModel => _addAccountViewModel;
        public AddAccountsViewModel AddAccountsViewModel => _addAccountsViewModel;
        public AccountSettingViewModel AccountSettingViewModel => _accountSettingViewModel;
        public EditAccountViewModel EditAccountViewModel => _editAccountViewModel;
        public DebugViewModel DebugViewModel => _debugViewModel;
    }
}
﻿using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WPFUI.Models.Input;
using WPFUI.Repositories;
using WPFUI.ViewModels.Abstract;
using WPFUI.ViewModels.UserControls;

namespace WPFUI.ViewModels.Tabs
{
    public class EditAccountViewModel : AccountTabBaseViewModel
    {
        public AccessInput AccessInput { get; } = new();
        public AccountInput AccountInput { get; } = new();

        private AccessInput _selectedAcess;
        public ReactiveCommand<Unit, Unit> AddAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAccessCommand { get; }
        public ReactiveCommand<Unit, Unit> EditAccountCommand { get; }

        private readonly IAccountRepository _accountRepository;
        private readonly WaitingOverlayViewModel _waitingOverlayViewModel;

        public EditAccountViewModel(IAccountRepository accountRepository, WaitingOverlayViewModel waitingOverlayViewModel)
        {
            _accountRepository = accountRepository;
            _waitingOverlayViewModel = waitingOverlayViewModel;

            AddAccessCommand = ReactiveCommand.CreateFromTask(AddAccessTask);
            EditAccessCommand = ReactiveCommand.CreateFromTask(EditAccessTask);
            DeleteAccessCommand = ReactiveCommand.CreateFromTask(DeleteAccessTask);
            EditAccountCommand = ReactiveCommand.CreateFromTask(EditAccountTask);

            this.WhenAnyValue(vm => vm.SelectedAccess)
                .WhereNotNull()
                .Subscribe(x => x.CopyTo(AccessInput));
        }

        protected override async Task Load(int accountId)
        {
            var account = await _accountRepository.Get(accountId);
            AccountInput.CopyFrom(account);
        }

        private Task AddAccessTask()
        {
            AccountInput.Accesses.Add(AccessInput.Clone());
            return Task.CompletedTask;
        }

        private Task EditAccessTask()
        {
            AccessInput.CopyTo(SelectedAccess);
            return Task.CompletedTask;
        }

        private Task DeleteAccessTask()
        {
            AccountInput.Accesses.Remove(SelectedAccess);
            SelectedAccess = null;
            return Task.CompletedTask;
        }

        private async Task EditAccountTask()
        {
            _waitingOverlayViewModel.Show("editting account ...");
            await _accountRepository.Edit(AccountInput);
            _waitingOverlayViewModel.Close();
        }

        public AccessInput SelectedAccess
        {
            get => _selectedAcess;
            set => this.RaiseAndSetIfChanged(ref _selectedAcess, value);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.interfaces;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace Cybers.Infrustructure.controls
{
    public class AlertDialogViewModel : BindableBase, IInteractionRequestAware
    {
        private string _title;
        private string _content;
        private AlertDialogNotification _notification;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public DelegateCommand PositiveCommand { get; }
        public DelegateCommand NegativeCommand { get; }

        public AlertDialogViewModel()
        {
            PositiveCommand = new DelegateCommand(OnPositiveCommandExecuted);
            NegativeCommand = new DelegateCommand(OnNegativeCommandExecuted);
        }

        private void OnNegativeCommandExecuted()
        {
            if (_notification != null)
            {
                _notification.Confirmed = false;
            }

            FinishInteraction();
        }

        private void OnPositiveCommandExecuted()
        {
            if (_notification != null)
            {
                _notification.Confirmed = true;
            }

            FinishInteraction();
        }

        public INotification Notification
        {
            get => _notification;
            set
            {
                if (value is AlertDialogNotification)
                {
                    _notification = value as AlertDialogNotification;
                    Title = _notification.Title;
                    Content = (string) _notification.Content;
                    RaisePropertyChanged();
                }
            }
        }
        public Action FinishInteraction { get; set; }
    }
}

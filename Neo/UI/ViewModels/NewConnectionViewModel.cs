using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace Neo.UI.ViewModels
{
    class NewConnectionViewModel : BindableBase, IInteractionRequestAware
    {
        private SaveAsNotification mNotification;

        public string SaveAs { get; set; }

        public ICommand FinishCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public Action FinishInteraction { get; set; }

        public NewConnectionViewModel()
        {
            FinishCommand = new DelegateCommand(AcceptInteraction);
            CancelCommand = new DelegateCommand(CancelInteraction);
        }

        public INotification Notification
        {
            get
            {
                return mNotification;
            }
            set
            {
                if (!(value is SaveAsNotification)) return;
                mNotification = (SaveAsNotification) value;
                OnPropertyChanged(() => Notification);
            }
        }

        public void AcceptInteraction()
        {
            if (mNotification != null)
            {
                mNotification.SaveAs = SaveAs;
                mNotification.Confirmed = true;
            }

            FinishInteraction();
        }

        public void CancelInteraction()
        {
            if (mNotification != null)
            {
                mNotification.SaveAs = null;
                mNotification.Confirmed = false;
            }

            FinishInteraction();
        }
    }

    public class SaveAsNotification : Confirmation
    {
        public string SaveAs { get; set; }

        public SaveAsNotification()
        {
            SaveAs = null;
        }
    }
}

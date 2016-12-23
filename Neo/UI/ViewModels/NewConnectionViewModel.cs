using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace Neo.UI.ViewModels
{
	internal class NewConnectionViewModel : BindableBase, IInteractionRequestAware
    {
        private SaveAsNotification mNotification;

        public string SaveAs { get; set; }

        public ICommand FinishCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public Action FinishInteraction { get; set; }

        public NewConnectionViewModel()
        {
	        this.FinishCommand = new DelegateCommand(AcceptInteraction);
	        this.CancelCommand = new DelegateCommand(CancelInteraction);
        }

        public INotification Notification
        {
            get
            {
                return this.mNotification;
            }
            set
            {
                if (!(value is SaveAsNotification))
                {
	                return;
                }
	            this.mNotification = (SaveAsNotification) value;
                OnPropertyChanged(() => this.Notification);
            }
        }

        public void AcceptInteraction()
        {
            if (this.mNotification != null)
            {
	            this.mNotification.SaveAs = this.SaveAs;
	            this.mNotification.Confirmed = true;
            }

	        this.FinishInteraction();
        }

        public void CancelInteraction()
        {
            if (this.mNotification != null)
            {
	            this.mNotification.SaveAs = null;
	            this.mNotification.Confirmed = false;
            }

	        this.FinishInteraction();
        }
    }

    public class SaveAsNotification : Confirmation
    {
        public string SaveAs { get; set; }

        public SaveAsNotification()
        {
	        this.SaveAs = null;
        }
    }
}

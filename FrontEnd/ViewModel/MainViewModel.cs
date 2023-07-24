using System;
using System.Windows;
using FrontEnd;
using FrontEnd.Model;
using FrontEnd.View;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace FrontEnd.ViewModel
{
    public class MainViewModel : NotifableObject
    {
        public BackendController controller;
        private UserModel user;
        private string massageText;
        private string emailTextbox;
        private ViewManager manager;

        public string EmailTextbox
        {
            get => emailTextbox;
            set
            {
                emailTextbox = value;
                RaisePropertyChanged(nameof(emailTextbox));
            }
        }

        public string MassageText
        {
            get => massageText;
            set { massageText = value; RaisePropertyChanged(nameof(MassageText)); }
        }

        public MainViewModel(BackendController controller, ViewManager manager)
        {
            
            this.controller = controller;
            this.manager = manager;
        }


        public void Login(string password)
        {

            try
            {
                this.user = this.controller.Login(EmailTextbox, password);
                manager.Login(user);
            }
            catch (Exception error)
            {

                MassageText = error.Message;
                RaisePropertyChanged(MassageText);
            }

        }

        public void Register( string password)
        {
            try
            {
                this.user = this.controller.Register(EmailTextbox, password);
                manager.Login(user);
            }
            catch (Exception error)
            {
                MassageText = error.Message;
                RaisePropertyChanged(MassageText);
            }

        }

    }
}

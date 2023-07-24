using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using FrontEnd.Model;
using FrontEnd.View;
using FrontEnd.ViewModel;
using IntroSE.Kanban.Backend.BusinessLayer;
using MaterialDesignThemes.Wpf;

namespace FrontEnd
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private MainViewModel vm;
        private ViewManager manager;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow(ViewManager manager , BackendController controller)
        {
            InitializeComponent();
            
            vm = new MainViewModel(controller, manager);
            DataContext = vm;
            this.manager = manager;
        }


        private void login_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;
            vm.Login(password);
        }

        public void Reset()
        {
            EmailBox.Text = "";
            PasswordBox.Password = "";
            vm.MassageText = "";
        }

        private void register_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;
            vm.Register( password);

        }

        protected void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
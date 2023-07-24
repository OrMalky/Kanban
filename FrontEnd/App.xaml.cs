using System;
using System.Windows;
using FrontEnd.Model;
using FrontEnd.View;

namespace FrontEnd
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public BackendController controller;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            controller = new BackendController();
            ViewManager vm = new ViewManager(controller);
        }
    }
}
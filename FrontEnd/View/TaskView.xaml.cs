using FrontEnd.Model;
using FrontEnd.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FrontEnd.View
{
    /// <summary>
    /// Interaction logic for TaskView.xaml
    /// </summary>
    public partial class TaskView : Window
    {
        ViewManager manager;
        TaskViewModel vm;

        public TaskView(TaskModel task, ViewManager manager, BackendController controller)
        {
            InitializeComponent();
            vm = new TaskViewModel(manager.currentBoard, task, controller, manager.currentUser);
            DataContext = vm;
            this.manager = manager;
        }

        private void Set_Clicked(object sender, RoutedEventArgs e)
        {
            if(vm.setClicked())
            {
                manager.SetTask();
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            manager.ExitTaskView();
        }
    }
}

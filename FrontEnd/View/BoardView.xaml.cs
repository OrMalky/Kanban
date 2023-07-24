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
    /// Interaction logic for BoardView.xaml
    /// </summary>
    public partial class BoardView : Window
    {
        private ViewManager manager;
        private BoardViewModel vm;


        public BoardView(BoardModel board, ViewManager manager,BackendController controller)
        {
            InitializeComponent();
            this.manager = manager;
            vm = new BoardViewModel(board, controller,manager.currentUser);
            DataContext = vm;
        }

        private void Advance_Task_Click(object sender, RoutedEventArgs e)
        {
            vm.advanceTask(vm.SelectedTask);
        }

        private void Add_Task_Click(object sender, RoutedEventArgs e)
        {
            manager.AddTaskOpened();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid dataGrid && dataGrid.SelectedItem is TaskModel selectedTask)
            {
                manager.TaskOpened(selectedTask);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            manager.Back_BoardView();
        }
    }
}

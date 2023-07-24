using FrontEnd.Model;
using System.Windows;
using FrontEnd.ViewModel;

namespace FrontEnd.View
{
    public partial class AddTask : Window
    {
        private ViewManager mangager;
        private BoardModel tasksTable;
        private UserModel user;
        private BoardModel boardModel;
        private AddTaskViewModel vm;

        public AddTask(ViewManager viewM, BackendController controller, UserModel u, BoardModel board)
        {
            InitializeComponent();
            this.user = u;
            this.mangager = viewM;
            this.boardModel = board;
            vm = new AddTaskViewModel(controller, boardModel);
            DataContext = vm;
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if(vm.AddTask(user.Email, boardModel.Id))
            {
                mangager.TaskAdded();
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            mangager.ExitAddTask();
        }
    }
}

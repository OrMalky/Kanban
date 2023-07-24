using FrontEnd.Model;
using IntroSE.Kanban.Backend.BusinessLayer;
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
    /// Interaction logic for ViewManager.xaml
    /// </summary>
    public partial class ViewManager : Window
    {
        private MainWindow loginView;
        private AddTask addTaskView;
        private BoardView boardView;
        private BoardListView boardListView;
        public UserModel currentUser;
        public BoardModel currentBoard;
        private BackendController controller;
        private TaskView taskView;

        public ViewManager(BackendController b)
        {
            InitializeComponent();
            this.Hide();
            this.controller = b;
            if (this.controller == null)
            {
                throw new Exception("Controller is null.");
            }
            this.loginView = new MainWindow(this, controller);
            loginView.Show();
        }

        public void Login(UserModel user)
        {
            loginView.Hide();
            loginView.Reset();
            this.currentUser = user;
            this.boardListView = new BoardListView(user, this, controller);
            boardListView.Show();
        }

        public void Logout()
        {
            boardListView.Close();
            loginView.Show();
        }

        public void BoardSelected(BoardModel board)
        {
            boardListView.Hide();
            this.currentBoard = board;
            this.boardView = new BoardView(board, this,controller);
            boardView.Show();
        }

        public void AddTaskOpened()
        {
            this.addTaskView = new AddTask(this,this.controller,currentUser, currentBoard);
            addTaskView.Show();
        }

        public void TaskAdded()
        {
            addTaskView.Close();
        }

        public void TaskOpened(TaskModel task)
        {
            taskView=new TaskView(task,this,controller); 
            taskView.Show();
        }

        public void SetTask()
        {
            taskView.Close();
        }

        public void Back_BoardView()
        {
            boardView.Close();
            boardListView.Show();
        }

        public void ExitAddTask()
        {
            addTaskView.Close();
        }

        public void ExitTaskView()
        {
            taskView.Close();
        }

    }
}


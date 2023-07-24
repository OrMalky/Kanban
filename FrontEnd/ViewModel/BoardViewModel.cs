using FrontEnd.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontEnd.ViewModel
{
    public class BoardViewModel:NotifableObject
    {
        public BoardModel board;
        public BackendController controller;
        private UserModel user;
        private string massageText;
        private bool isDialogOpen;
        private TaskModel selectedTask;

        public TaskModel SelectedTask
        {
            get => selectedTask;
            set
            {
                selectedTask = value;
                RaisePropertyChanged(nameof(SelectedTask));
            }
        }

        public string MassageText
        {
            get => massageText;
            set { massageText = value; RaisePropertyChanged(nameof(MassageText)); }
        }


        public BoardModel Board
        {
            get => board;
            set
            {
                board = value;
                RaisePropertyChanged(nameof(Board));
            }
        }

        public bool IsDialogOpen { get => isDialogOpen; set { isDialogOpen = value; RaisePropertyChanged(nameof(IsDialogOpen)); } }

        public BoardViewModel(BoardModel board, BackendController controller, UserModel user)
        {
            this.board = board;
            this.user = user;
            this.controller = controller;
            board.loadBoardTasks();
        }

        public void advanceTask(TaskModel task)
        {
            try
            {
                controller.AdvanceTask(user.Email, board.BoardName, task.Id);
                board.advanceTask(task);
                RaisePropertyChanged(nameof(Board));
            }
            catch (Exception e)
            {
                MassageText = e.Message;
                IsDialogOpen = true;
            }
        }
    }
}

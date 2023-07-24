using FrontEnd.Model;
using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontEnd.ViewModel
{
    public class AddTaskViewModel : NotifableObject
    {
        private BackendController controller;
        private string title;
        private string desc;
        private DateTime creationTime = DateTime.Now;
        private DateTime dueDate = DateTime.Now;
        private string assignee;
        private string errorMsg;
        private BoardModel board;
        private bool isDialogOpen = false;

        public string Title { get => title; set => title = value; }
        public string Desc { get => desc; set => desc = value; }
        public DateTime CreationTime { get => creationTime; }
        public DateTime DueDate { get => dueDate; set => dueDate = value; }
        public string Assignee { get => assignee; set => assignee = value; }
        public string ErrorMsg { get => errorMsg; set { errorMsg = value; RaisePropertyChanged(nameof(ErrorMsg)); } }
        public bool IsDialogOpen { get => isDialogOpen; set { isDialogOpen = value; RaisePropertyChanged(nameof(IsDialogOpen)); } }

        public AddTaskViewModel(BackendController controller,BoardModel board)
        {
            this.controller = controller;
            this.board = board;
        }

        public bool AddTask(string email, int boardId)
        {
            try
            {
                string t = Title == null? "" : Title;
                string d = Desc == null ? "" : Desc;
                bool toAssign = Assignee != null && Assignee != "";

                if (toAssign && !controller.IsMember(Assignee, boardId))
                    throw new Exception("Assignee is not a member of the board");

                controller.AddTask(email, boardId, t, d, dueDate);
                TaskModel task = new TaskModel(controller, boardId, dueDate, title, CreationTime, desc);
                board.addTask(task);

                if (toAssign)
                {
                    controller.setTaskAssignee(task.Title, board.BoardName, task.Id, assignee);
                    task.Assignee = assignee;
                }

                RaisePropertyChanged(nameof(board));
                return true;
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message;
                IsDialogOpen = true;
                return false;
            }
        }
    }
}

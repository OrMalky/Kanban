using FrontEnd.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontEnd.ViewModel
{
    internal class TaskViewModel : NotifableObject
    {
        public TaskModel taskModel;
        public BackendController controller;
        UserModel user;
        BoardModel board;

        private int id;
        private DateTime creationTime;
        private DateTime dueDate;
        private string title;
        private string description;
        private string assignee;
        private string massageText;
        private bool isDialogOpen;

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                RaisePropertyChanged("id");
            }
        }

        public DateTime CreationTime
        {
            get { return creationTime; }
            /*set
            {
                creationTime = value;
                RaisePropertyChanged("creationTime");
            }*/
        }

        public DateTime DueDate
        {
            get { return dueDate; }
            set
            {
                dueDate = value;

                RaisePropertyChanged("dueDate");
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged("title");
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                RaisePropertyChanged("decription");
            }
        }

        public string Assignee
        {
            get { return assignee; }
            set
            {
                assignee = value;
                RaisePropertyChanged("assignee");
            }
        }

        public string MassageText
        {
            get => massageText;
            set { massageText = value; RaisePropertyChanged(nameof(MassageText)); }
        }

        public bool IsDialogOpen { get => isDialogOpen; set { isDialogOpen = value; RaisePropertyChanged(nameof(IsDialogOpen)); } }

        public TaskViewModel(BoardModel board, TaskModel taskModel, BackendController controller, UserModel user)
        {
            this.user = user;
            this.controller = controller;
            this.taskModel = taskModel;

            this.id = taskModel.Id;
            this.creationTime = taskModel.CreationTime;
            this.dueDate = taskModel.DueDate;
            this.title = taskModel.Title;
            this.description = taskModel.Description;
            Assignee = taskModel.Assignee;
 
            this.board = board;
        }

        public void GetTaskAssignee()
        {
            string a = controller.GetAssignee(id, board.Id);

            this.assignee = a;
            taskModel.Assignee = this.assignee;
        }

        public bool setClicked()
        {
            try
            {
                if (dueDate != taskModel.DueDate)
                {
                    controller.SetTaskDueDate(user.Email, board.BoardName, id, dueDate);
                    taskModel.DueDate = dueDate;
                }
                if (title != taskModel.Title)
                {
                    controller.SetTaskTitle(user.Email,board.BoardName, id, title);
                    taskModel.Title = title;
                }
                if (description != taskModel.Description)
                {
                    controller.setTaskDesc(user.Email, board.BoardName, id, description);
                    taskModel.Description = description;
                }
                if (assignee != taskModel.Assignee)
                {
                    controller.setTaskAssignee(user.Email, board.BoardName, id, assignee);
                    taskModel.Assignee = assignee;
                }
                return true;
            }
            catch (Exception ex)
            {
                MassageText = ex.Message;
                IsDialogOpen = true;
                return false;
            }
        }
    }
}

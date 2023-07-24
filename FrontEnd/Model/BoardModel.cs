    using IntroSE.Kanban.Backend.DataAccessLayer;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;



namespace FrontEnd.Model
{
    public class BoardModel : NotifableModelObject
    {
        private BackendController controller;
        private int id;
        private string boardname;
        private string user;
        private string owner;
        private ObservableCollection<TaskModel> backlog;
        private ObservableCollection<TaskModel> inprogsess;
        private ObservableCollection<TaskModel> done;

        public string Owner
        {
            get => owner;
            set
            {
                owner = value;
                RaisePropertyChanged("owner");
            }
        }
        public int Id
        {
            get => id;
            set
            {
                id = value;
                RaisePropertyChanged("id");
            }
        }

        public string BoardName
        {
            get => boardname;
            set
            {
                boardname = value; 
                RaisePropertyChanged("boardname");
            }
        }

        public string User
        {
            get => user;
            set
            {
                user = value;
                RaisePropertyChanged(nameof(User));
            }
        }

        public ObservableCollection<TaskModel> Backlog
        {
            get => backlog;
            set
            {
                backlog = value;
                RaisePropertyChanged(nameof(Backlog));
            }
        }

        public ObservableCollection<TaskModel> Inprogress
            {
            get => inprogsess;
            set
            {
                inprogsess = value;
                RaisePropertyChanged(nameof(Inprogress));
            }
        }
        public ObservableCollection<TaskModel> Done
        {
            get => done;
            set
            {
                done = value;
                RaisePropertyChanged(nameof(Done));
            }
        }   

        public void advanceTask(TaskModel task)
        {
            if (Inprogress.Contains(task))
            {
                Inprogress.Remove(task);
                Done.Add(task);
            }
            if (Backlog.Contains(task))
            {
                Backlog.Remove(task);
                Inprogress.Add(task);
            }
        }

        public void addTask(TaskModel task)
        {
            Backlog.Add(task);
        }


        public BoardModel(BackendController controller, int id,string user) : base(controller)
        {
            this.controller = controller;
                
            this.user = user;
            Id = id;
            BoardName = controller.getBoardName(id);
            Owner = controller.GetBoardOwner(id);


        }

        public void loadBoardTasks()
        {
            backlog = controller.GetColumnTasks(user, id, 0);
            inprogsess = controller.GetColumnTasks(user, id, 1);
            done = controller.GetColumnTasks(user, id, 2);

            foreach (TaskModel task in backlog)
            {
                task.Assignee = controller.GetAssignee(task.Id, id);
            }
            foreach (TaskModel task in inprogsess)
            {
                task.Assignee = controller.GetAssignee(task.Id, id);
            }
            foreach (TaskModel task in done)
            {
                task.Assignee = controller.GetAssignee(task.Id, id);
            }
        }

    }
}

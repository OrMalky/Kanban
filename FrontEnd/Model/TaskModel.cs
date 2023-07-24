using System;

namespace FrontEnd.Model
{
    public class TaskModel : NotifableModelObject
    {
        private int id;
        private DateTime creationTime;
        private DateTime dueDate;
        private string title;
        private string description;
        private string assignee;

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
            set
            {
                creationTime = value;
                RaisePropertyChanged("creationTime");
            }
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

        public TaskModel(BackendController controller, int id, DateTime dueDate, string title, DateTime creationDate, string description)
            : base(controller)
        {
            Id = id;
            DueDate = dueDate;
            Title = title;
            CreationTime = creationDate;
            Description = description;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class TaskDTO : IDTO
    {
        //Members
        private int id;
        private int boardId;
        private int column;
        private string title;
        private string assignee;
        private DateTime creationTime;
        private DateTime dueDate;
        private string description;
        private bool done;

        //Properties
        public int Id { get => id; }
        public int BoardId { get => boardId; }
        public int Column { get => column; set => column = value;}
        public string Title { get => title; set => title = value; }
        public string Assignee { get => assignee; set => assignee = value; }
        public DateTime CreationTime { get => creationTime;  }
        public DateTime DueDate { get => dueDate; set => dueDate = value; }
        public string Description { get => description; set => description = value; }
        public bool Done { get => done;  set => done = value;}

        public TaskDTO() { }

        public TaskDTO(int taskId, int boardId, int column, string title, string assignee, DateTime creationTime, DateTime dueDate, string desc, bool done)
        {
            this.id = taskId;
            this.column = column;
            this.boardId = boardId;
            this.title = title;
            this.assignee = assignee;
            this.creationTime = creationTime;
            this.dueDate = dueDate;
            this.description = desc;
            this.done = done;
        }
    }
}

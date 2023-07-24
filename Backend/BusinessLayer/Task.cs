using IntroSE.Kanban.Backend.DataAccessLayer;
using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class Task
    {
        // Members
        private readonly int id;
        private readonly int boardId;
        private readonly DateTime creationTime;
        private DateTime dueDate;
        private string title;
        private string description;
        private Columns column;
        private bool done;
        private User assignee;
        private TaskDTO dto;

        //Properties
        public int Id { get => id; }
        public DateTime CreationTime { get => creationTime; }
        public string Title { get => title; }
        public string Description { get => description; }
        public DateTime DueDate { get => dueDate;}
        [JsonIgnore]
        public int BoardId { get => boardId; }
        [JsonIgnore]
        public User Assignee { get => assignee; }
        [JsonIgnore]
        public Columns Column { get => column; }
        [JsonIgnore]
        public TaskDTO DTO { get => dto; }
        [JsonIgnore]
        public bool Done { get => done; }


        //Constants
        const int DESC_LIMIT = 300;
        const int TITLE_LIMIT = 50;


        public Task() { }

        public Task(int id, int boardId, DateTime dueDate, string title, string desc, User assignee)
        {
            if(!IsValidTitle(title) || !IsValidDueDate(dueDate) || !IsValidDesc(desc))
            {
                throw new Exception($"Task: {id} is not Valid");
            }
            this.id = id;
            this.boardId = boardId;
            creationTime = DateTime.Now;
            this.title = title;
            description = desc;
            this.dueDate = dueDate;
            this.assignee = assignee;
            column = Columns.Backlog;
            dto = new TaskDTO(id, boardId, (int)column, title, assignee == null? "" : assignee.Email, creationTime, dueDate, description, false);
        }

        public Task(TaskDTO dto, User assignee)
        {
            this.dto = dto;
            id = dto.Id;
            boardId = dto.BoardId;
            title = dto.Title;
            creationTime = dto.CreationTime;
            dueDate = dto.DueDate;
            description = dto.Description;
            this.assignee = assignee;
            column = (Columns)dto.Column;
            done = dto.Done;
            if(column == Columns.Done)
            {
                done = true;
                dto.Done = true;
            }
        }

        /// <summary>
        /// Gets the column after this task's current column.
        /// </summary>
        /// <returns>The next column's enum.</returns>
        public Columns GetNextColumn()
        {
            switch (column)
            {
                case Columns.Backlog:
                    return Columns.InProgress;
                case Columns.InProgress:
                    return Columns.Done;
                default:
                    throw new Exception("Invalid column");
            }
        }

        /// <summary>
        /// Delete this task.
        /// </summary>
        public void Delete()
        {
            //Currently nothing to do
            // COOL xD
   
        }

        /// <summary>
        /// Assign this task to a user.
        /// </summary>
        /// <param name="user">The user object (of the action performer).</param>
        /// <param name="newAssignee">The new assignee's user object.</param>
        public void AssignTask(User user, User newAssignee)
        {
            if (CanEdit(user))
            {
                if(assignee != null)
                    assignee.UnassignTask(this);
                assignee = newAssignee;
                if(assignee != null)
                    assignee.AssignTask(this);
                if(assignee != null)
                    dto.Assignee = newAssignee.Email;
                else
                    dto.Assignee = "";
            }
        }

        /// <summary>
        /// Unassign this task from a user.
        /// </summary>
        /// <param name="user">The user object (of the action performer).</param>
        /// <returns>The new task's object</returns>
        public void UnassignTask(User user)
        {
            if (CanEdit(user))
            {
                user.UnassignTask(this);
                assignee = null;
            }
        }

        /// <summary>
        /// Set this task's title.
        /// </summary>
        /// <param name="user">The user object (of the action performer).</param>
        /// <param name="title">The task's title (to set).</param>
        public void SetTitle(User user, string title)
        {
            if (CanEdit(user))
            {
                IsValidTitle(title);
                this.title = title;
                dto.Title = title;
            }
        }

        /// <summary>
        /// Advance this task to the next column.
        /// </summary>
        public void AdvanceTask()
        {
            switch(column)
            {
                case Columns.Backlog:
                    column = Columns.InProgress;
                    dto.Column = 1;
                    break;

                case Columns.InProgress:
                    column = Columns.Done;
                    dto.Column = 2;
                    done = true;
                    dto.Done = true;
                    break;

                case Columns.Done:
                    throw new Exception("Cannot advance a done task");
                    
                default:
                    throw new Exception("Invalid task column");

            }
        }

        /// <summary>
        /// Set this task's description.
        /// </summary>
        /// <param name="user">The user object (of the action performer).</param>
        /// <param name="desc">The task's description (to set).</param>
        public void SetDescription(User user, string desc)
        {
            if (CanEdit(user))
            {
                IsValidDesc(desc);
                description = desc;
                dto.Description = description;
            }
        }

        /// <summary>
        /// Set this task's due date.
        /// </summary>
        /// <param name="user">The user object (of the action performer).</param>
        /// <param name="dueDate">The task's due date (to set).</param>
        public void SetDueDate(User user, DateTime dueDate)
        {
            if (CanEdit(user))
            {
                IsValidDueDate(dueDate);
                this.dueDate = dueDate;
                dto.DueDate = dueDate;
            }
        }

        // Checks if a given user can edit this task
        private bool CanEdit(User user)
        {
            if (done)
                throw new Exception("Task is already done");
            if (assignee != null && !user.Equals(assignee))
                throw new Exception("This user is not assigned to this task and cannot edit it");

            return true;
        }

        // Checks if a given string is a valid title
        private bool IsValidTitle(string title) 
        {
            if (title.Length > TITLE_LIMIT || title.Length == 0)
                throw new Exception($"Invalid task title.");
            return true;
        }



        // Checks if a given string is a valid description
        private bool IsValidDesc(string desc)
        {
            if (desc.Length > DESC_LIMIT)
                throw new Exception($"Description is too long (max {DESC_LIMIT}).");
            return true;
        }

        // Checks if a given DateTime is a valid due date
        private bool IsValidDueDate(DateTime dueDate)
        {
            if (DateTime.Now > dueDate)
                throw new Exception("Invalid due date");
            return true;
        }
        // Returns the name of the assignee empty string if no assignee
        public string GetAssigneeName()
        {
            return  Assignee == null?  "" : Assignee.Email; 
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Task other = (Task)obj;
            return id == other.Id && boardId == other.BoardId;
        }
    }
}

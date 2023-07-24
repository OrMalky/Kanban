using IntroSE.Kanban.Backend.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static log4net.Appender.RollingFileAppender;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class Board
    {
        //Members
        private readonly int id;
        private string name;
        private User owner;
        private Dictionary<int, Task> tasks;
        private List<User> members;
        private Dictionary<Columns, int> maxTasks;
        private Dictionary<Columns, int> columnCount;
        private BoardDTO dto;
        private int taskIdCounter;

        //Properties
        public int Id { get => id; }
        public string Name { get => name; }
        public User Owner { get => owner; }
        public Dictionary<int, Task> Tasks { get => tasks;}
        public List<User> Members { get => members; set=> members = value; }
        public Dictionary<Columns, int> MaxTasks {get => maxTasks ;}
        public BoardDTO DTO { get => dto; }

        public Board() { }

        public Board(int boardId, string name, User creator)
        {
            members = new List<User>();
            id = boardId;
            this.name = name;
            AddMember(creator);
            owner = creator;

            taskIdCounter = 0;
            tasks = new Dictionary<int, Task>();

            //Init max tasks
            maxTasks = new Dictionary<Columns, int>();
            foreach (Columns c in Enum.GetValues(typeof(Columns)))
                maxTasks[c] = -1;

            //Init counter
            columnCount = new Dictionary<Columns, int>();
            foreach (Columns c in Enum.GetValues(typeof(Columns)))
                columnCount[c] = 0;

            dto = new BoardDTO(id, name, owner.Email, maxTasks.Values.ToList(), members.Select(user => user.Email).ToList());

        }

        public Board(BoardDTO dto, User owner)
        {
            members = new List<User>();
            tasks = new Dictionary<int, Task>();
            maxTasks = new Dictionary<Columns, int>();
            taskIdCounter = 0;
            this.owner = owner;
            this.dto = dto;
            id = dto.Id;
            name = dto.Name;

            columnCount = new Dictionary<Columns, int>();
            foreach (Columns c in Enum.GetValues(typeof(Columns)))
                columnCount[c] = 0;
            for (int i=0; i<dto.MaxTasks.Count; i++)
                maxTasks[(Columns)i] = Enum.IsDefined(typeof(Columns), i)
                    ? dto.MaxTasks[i]
                    : throw new Exception($"Invalid column index {i}");

            //Gets owner, members and tasks automatically later
        }

        /// <summary>
        /// Adds a new task to the board.
        /// </summary>
        /// <param name="user">The user object (of the action performer).</param>
        /// <param name="title">The task's title.</param>
        /// <param name="desc">The task's description.</param>
        /// <param name="dueDate">The task's due date.</param>
        /// <returns>The new task's object</returns>
        public Task AddTask(User user, string title, string desc, DateTime dueDate)
        {
            if (!members.Contains(user))
                throw new Exception($"You are not a member of this board");

            if (columnCount[Columns.Backlog] >= maxTasks[Columns.Backlog] && maxTasks[Columns.Backlog] != -1)
                throw new Exception($"Backlog column is full");

            int taskId = taskIdCounter++;
            Task task = new Task(taskId, id, dueDate, title, desc, null);

            tasks.Add(taskId, task);
            columnCount[Columns.Backlog]++;
            return task;
        }

        /// <summary>
        /// Adds am existing task to the board. Used when loading from DB.
        /// </summary>
        /// <param name="task">The task object.</param>
        public void AddTask(Task task)
        {
            /*if(!members.Contains(task.Assignee))
                throw new Exception($"{task.Assignee} is not a member of the board");*/

            tasks.Add(task.Id, task);
            columnCount[task.Column]++;
            taskIdCounter = Math.Max(taskIdCounter, task.Id + 1);
        }

        /// <summary>
        /// Transfer board ownership to a another user.
        /// </summary>
        /// <param name="currentOwner">The user object of the current owner.</param>
        /// <param name="newOwner">The user object of the new owner.</param>
        public void TransferOwnership(User currentOwner, User newOwner)
        {
            if (!owner.Equals(currentOwner))
                throw new Exception($"{currentOwner.Email} is not the owner of the board");
            if (!members.Contains(newOwner))
                throw new Exception($"{newOwner.Email} is not a member of the board");
            
            owner = newOwner;
            dto.Owner = owner.Email;
        }

        /// <summary>
        /// Advances a task in the board to the next column.
        /// </summary>
        /// <param name="task">The task object.</param>
        public void AdvanceTask(Task task)
        {
            if(task.Done)
                throw new Exception($"This task is already done");

            Columns col = task.GetNextColumn();
            if (columnCount[col] < maxTasks[col] || maxTasks[col] == -1)
            {
                columnCount[col]++;
                columnCount[task.Column]--;
                task.AdvanceTask();
            }
        }

        /// <summary>
        /// Checks whether a user is a member of this board.
        /// </summary>
        /// <param name="user">The user object to check.</param>
        /// <returns>True if the user is a member of this board and False otherwise.</returns>
        public bool IsMember(User user) => members.Contains(user);

        /// <summary>
        /// Removes a member from the board.
        /// </summary>
        /// <param name="user">The user object to delete.</param>
        public void RemoveMember(User user) {
            if(user.Equals(owner))
                throw new Exception($"{user.Email} is the owner of the board ({name})");
            if (!members.Contains(user))
                throw new Exception($"{user.Email} is not a member of the board ({name})");

            foreach(Task t in tasks.Values)
            {
                t.UnassignTask(user);
            }

            members.Remove(user);
            dto.Members.Remove(user.Email);
            user.LeaveBoard(this);
        }

        /// <summary>
        /// Adds a new member to the board.
        /// </summary>
        /// <param name="user">The user object to add.</param>
        public void AddMember(User user)
        {
            if (members.Contains(user))
                throw new Exception($"{user.Email} is already a member of the board ({name})");

            members.Add(user);
            user.JoinBoard(this);
        }

        /// <summary>
        /// Delets the board.
        /// All tasks will be deleted as well.
        /// </summary>
        public void Delete()
        {
            owner = null;

            //delete tasks
            foreach(Task t in tasks.Values)
            {
                tasks.Remove(t.Id);
                t.Delete();
            }

            //remove members
            foreach(User u in members)
            {
                u.LeaveBoard(this);
            }
        }

        /// <summary>
        /// Gets a task by its ID.
        /// </summary>
        /// <param name="id">The task's id.</param>
        /// <returns>The task's object if it exist</returns>
        public Task GetTask(int id) => tasks.ContainsKey(id) ? tasks[id] : throw new Exception("No task with matching ID in the board");
        

        public List<Task> GetAllTasks => tasks.Values.ToList();

        /// <summary>
        /// Limits the number of tasks in a column.
        /// </summary>
        /// <param name="column">The column's enum.</param>
        /// <param name="max">The maximum number of tasks.</param>
        public void LimitColumn(Columns column, int max)
        {
            if (maxTasks.ContainsKey(column))
            {
                maxTasks[column] = max;
                dto.MaxTasks[columnCount[column]] = max;
            }
            else
                throw new Exception("Column not found in board");
        }

        /// <summary>
        /// Get all tasks in a column.
        /// </summary>
        /// <param name="column">The column's enum.</param>
        /// <returns>A list of tasks, containing all tasks in the given column</returns>
        public List<Task> GetColumn(Columns column)
        {
            if (!columnCount.ContainsKey(column))
                throw new Exception("Invalid column");

            List<Task> result = new List<Task>();
            foreach(Task task in tasks.Values)
            {
                if(task.Column == column)
                {
                    result.Add(task);
                }
            }
            return result;
        }

        /// <summary>
        /// Get all tasks in the "In Progress" column.
        /// </summary>
        /// <returns>A list of tasks, containing all tasks in the "In Progress" column</returns>
        public List<Task> GetInProgressTasks()
        {
            List<Task> res = new List<Task>();
            foreach(Task task in tasks.Values)
                if(task.Column == Columns.InProgress)
                    res.Add(task);
            return res;
        }

        /// <summary>
        /// Get the maximum number of tasks in a column.
        /// </summary>
        /// <param name="col">The column's enum.</param>
        /// <returns>The maximum number of tasks for this column</returns>
        public int GetColumnLimit(Columns col) => maxTasks.ContainsKey(col) ? maxTasks[col] : throw new Exception("Invalid column");
    }
}

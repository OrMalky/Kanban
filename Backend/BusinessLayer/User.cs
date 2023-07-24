using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class User
    {
        //Members
        private string email;
        private string password;
        private bool loggedIn;
        private List<int> boardsIds;
        private List<Task> assignedTasks;
        private UserDTO dto;

        //Properties
        public string Email { get => email; }
        public string Password { get => password; }
        public bool LoggedIn { get => loggedIn; }
        public List<int> BoardsIds { get => boardsIds; }
        public List<Task> AssignedTasks { get => assignedTasks; }
        public UserDTO DTO { get => dto; }

        public User() { }

        public User(string email, string password)
        {
            this.email = email;
            this.password = password;
            loggedIn = false;
            boardsIds = new List<int>();
            assignedTasks = new List<Task>();
            dto = new UserDTO(email, password);
        }

        public User(UserDTO dto)
        {
            email = dto.Email;
            password = dto.Password;
            boardsIds = new List<int>();
            assignedTasks = new List<Task>();
            loggedIn = false;
            this.dto = dto;
            //Tasks are added automatically later
        }

        /// <summary>
        /// Join to a board.
        /// </summary>
        /// <param name="board">The board object to join.</param>
        public void JoinBoard(Board board) => boardsIds.Add(board.Id);

        /// <summary>
        /// Leave a board.
        /// </summary>
        /// <param name="board">The board object to leave.</param>
        public void LeaveBoard(Board board) => boardsIds.Remove(board.Id);

        /// <summary>
        /// Assign a task to this user.
        /// </summary>
        /// <param name="task">The task object to assign to this user.</param>
        public void AssignTask(Task task) => assignedTasks.Add(task);

        /// <summary>
        /// Unassign a task from this user.
        /// </summary>
        /// <param name="task">The task object to unassign from this user.</param>
        public void UnassignTask(Task task) => assignedTasks.Remove(task);

        /// <summary>
        /// Log in this user.
        /// </summary>
        /// <param name="password">The password for this user.</param>
        public void Login(string password) 
        {
            if(loggedIn)
                throw new Exception("User already logged in");
            if(password != this.password)
                throw new Exception("Wrong password");

            loggedIn = true;
        }

        /// <summary>
        /// Log out this user.
        /// </summary>
        public void Logout() => loggedIn = false;

        /// <summary>
        /// Get this user's tasks that are in the "In Progress" column.
        /// </summary>
        /// <returns>A list of tasks, that contains all this users in progress tasks</returns>
        public List<Task> GetInProgressTasks()
        {
            List<Task> res = new List<Task>();
            foreach(Task task in assignedTasks)
            {
                if (task.Column == Columns.InProgress)
                    res.Add(task);
            }
            return res;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            User other = (User)obj;
            return email.Equals(other.email) && password.Equals(other.password);
        }

        public override int GetHashCode() => email.GetHashCode() ^ password.GetHashCode();
    }
}




using System;
using System.Collections.Generic;
using log4net;
using IntroSE.Kanban.Backend.DataAccessLayer;
using System.Linq;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public enum Columns
    {
        Backlog,
        InProgress,
        Done
    }

    public class BoardFacade
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BoardFacade));
        private UserFacade uf;
        private Dictionary<int, Board> boardsById;
        private Dictionary<(User, string), Board> boardsByName;
        private BoardContoller boardController;
        private TaskController taskController;
        private int boardIdCounter;

        public BoardFacade(UserFacade uf)
        {
            this.uf = uf;
            boardController = new BoardContoller();
            taskController = new TaskController();

            boardsById = new Dictionary<int, Board>();
            boardsByName = new Dictionary<(User, string), Board>();
            boardIdCounter = 0;
        }

        /// <summary>
        /// Adds a new board to the system.
        /// </summary>
        /// <param name="user">Email of the user (the action performer).</param>
        /// <param name="boardName">The new board's name.</param>
        public void CreateBoard(string email, string boardName)
        {
            User user = uf.GetUser(email);

            if (!user.LoggedIn)
            {
                log.Error("User is not logged in. Board creation failed.");
                throw new Exception("User is not logged in. Board creation failed.");
            }

            int id = boardIdCounter++;
            Board board = new Board(id, boardName, user);
            IndexNewBoard(board);
            boardController.Insert(board.DTO);
            log.Info($"New board created: {boardName}");
        }

        /// <summary>
        /// Deletes a board from the system.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardId">The board id.</param>
        public void DeleteBoard(string email, int boardId)
        {
            User user = uf.GetUser(email);
            Board board = GetBoard(boardId);
            DeleteBoard(user, board);
        }

        /// <summary>
        /// Deletes a board from the system.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        public void DeleteBoard(string email, string boardName)
        {
            User user = uf.GetUser(email);
            Board board = GetBoard(email, boardName);
            DeleteBoard(user, board);
        }

        private void DeleteBoard(User user, Board board)
        {
                if (!user.LoggedIn)
                {
                    log.Error("User is not logged in. Board deletion failed.");
                    throw new Exception("User is not logged in. Board deletion failed.");
                }

                if (!user.Equals(board.Owner))
                {
                    log.Error("User is not the owner of this board. Board deletion failed.");
                    throw new Exception("User is not the owner of this board. Board deletion failed.");
                }

                //Delete all board's tasks
                foreach (Task t in board.GetAllTasks)
                    taskController.Delete(t.DTO);

                UnindexBoard(board);
                boardController.Delete(board.DTO);
                log.Info($"Board '{board.Name}' deleted by user '{user.Email}'");
        }

        /// <summary>
        /// Joins a user to a board.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardId">The board id.</param>
        public void JoinBoard(string email, int boardId)
        {
            User user;
            Board board;

            user = uf.GetUser(email);
            board = GetBoard(boardId);

            if (!user.LoggedIn)
            {
                log.Error("User is not logged in. Joining board failed.");
                throw new Exception("User is not logged in. Joining board failed.");
            }

            board.AddMember(user);
            boardController.Update(board.DTO);
            log.Info($"User '{user.Email}' joined board '{board.Name}'");
        }

        /// <summary>
        /// Removes a user from a board.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardId">The board id.</param>
        public void LeaveBoard(string email, int boardId)
        {
            User user = uf.GetUser(email);
            Board board = GetBoard(boardId);

            if (!user.LoggedIn)
            {
                log.Error("User is not logged in. Leaving board failed.");
                throw new Exception("User is not logged in. Leaving board failed.");
            }

            board.RemoveMember(user);
            boardController.Update(board.DTO);
            log.Info($"User '{user.Email}' left board '{board.Name}'");
        }

        /// <summary>
        /// Get all boards a given user is a member of.
        /// </summary>
        /// <param name="email">The user's email to get all boards of.</param>
        /// <returns>A list of board ids the user is a member of.</returns>
        public List<int> GetUserBoards(string email)
        {
            User user = uf.GetUser(email);

            if (!user.LoggedIn)
            {
                log.Error("User is not logged in. Retrieving user boards failed.");
                throw new Exception("User is not logged in. Retrieving user boards failed.");
            }

            return user.BoardsIds;
        }

        /// <summary>
        /// Gets a board's owner by its id.
        /// </summary>
        /// <param name="id">The board's id.</param>
        /// <returns>The board's owner user object.</returns>
        public User GetOwner(int id)
        {
            return boardsById.ContainsKey(id)
                ? boardsById[id].Owner
                : throw new Exception("No matching board found.");
        }

        public bool IsMember(string email, int boardId)
        {
            Board board = GetBoard(boardId);
            User user = uf.GetUser(email);
            return board.IsMember(user);
        }

        /// <summary>
        /// Gets a board's object by its id.
        /// </summary>
        /// <param name="id">The board's id.</param>
        /// <returns>The board's object.</returns>
        public Board GetBoard(int id)
        {
            return boardsById.ContainsKey(id)
                ? boardsById[id]
                : throw new Exception("No matching board found.");
        }

        /// <summary>
        /// Get a board's object by its owner and name.
        /// </summary>
        /// <param name="email">The owner's email.</param>
        /// <param name="boardName">The board's name.</param>
        /// <returns>The board's object.</returns>
        public Board GetBoard(string email, string boardName)
        {
            User owner = uf.GetUser(email);
            return boardsByName.ContainsKey((owner, boardName))
                ? boardsByName[(owner, boardName)]
                : throw new Exception("No mathcing board found");
        }

        /// <summary>
        /// Get a board's object by its id.
        /// </summary>
        /// <param name="boardId">The board id.</param>
        /// <returns>The board's object.</returns>
        public Board GetBoardName(int boardId) => boardsById[boardId];

        /// <summary>
        /// Advances a task in the board to the next column.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="taskId">The task id.</param>

        public void AdvanceTask(string email, string boardName, int taskId)
        {
            User user = uf.GetUser(email);
            Board board = GetBoard(email, boardName);
            Task task = board.GetTask(taskId);
            if (!user.LoggedIn)
            {
                log.Error("User is not logged in. Advancing task failed.");
                throw new Exception("You are not logged in.");
            }

            if (!user.Equals(task.Assignee))
            {
                log.Error("User is not assigned to this task. Advancing task failed.");
                throw new Exception("You are not assigned to this task.");
            }

            board.AdvanceTask(task);
            taskController.Update(task.DTO);
            log.Info($"Task '{task.Id}' advanced to '{task.Column}' in board '{board.Name}' by user '{user.Email}'");
        }

        /// <summary>
        /// Limit a column's maximum number of tasks.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="column">The column's enum.</param>
        /// <param name="max">The maximum number of tasks.</param>

        public void LimitColumns(string email, string boardName, int col, int max)
        {
            User user = uf.GetUser(email);
            Board board = GetBoard(email, boardName);
            Columns column = (Columns)col;

            if (!user.LoggedIn)
            {
                log.Error("User is not logged in. Limiting column failed.");
                throw new Exception("You are not logged in.");
            }

            if (max < 0)
            {
                log.Error("Invalid maximum value. Limiting column failed.");
                throw new Exception("Invalid maximum value.");
            }

            board.LimitColumn(column, max);
            boardController.Update(board.DTO);
            log.Info($"Column '{column}' limited to {max} tasks in board '{board.Name}' by user '{user.Email}'");
        }

        /// <summary>
        /// Sets a task's due date.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="taskId">The task id</param>
        /// <param name="date">The task's due date (to set).</param>
        public void SetDueDate(string email, string boardName, int taskId, DateTime date)
        {
            User user = uf.GetUser(email);
            Board board = GetBoard(email,boardName);
            Task task = board.GetTask(taskId);
            if (!user.LoggedIn)
            {
                log.Error("User is not logged in. Setting due date failed.");
                throw new Exception("You are not logged in.");
            }

            task.SetDueDate(user, date);
            taskController.Update(task.DTO);
            log.Info($"Due date of task '{task.Id}' set to {date} by user '{user.Email}'");
        }

        /// <summary>
        /// Sets a task's title.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="taskId">The task id</param>
        /// <param name="title">The task's title (to set).</param>
        public void SetTitle(string email, string boardName, int taskId, string title)
        {
            User user = uf.GetUser(email);
            Board board = GetBoard(email,boardName);
            Task task = board.GetTask(taskId);
            if (!user.LoggedIn)
            {
                log.Error("User is not logged in. Setting task title failed.");
                throw new Exception("You are not logged in.");
            }

            task.SetTitle(user, title);
            taskController.Update(task.DTO);
            log.Info($"Title of task '{task.Id}' set to '{title}' by user '{user.Email}'");
        }

        /// <summary>
        /// Sets a task's description.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="taskId">The task id</param>
        /// <param name="desc">The task's description (to set).</param>
        public void SetDescription(string email, string boardName, int taskId, string desc)
        {
            User user = uf.GetUser(email);
            Board board = GetBoard(email, boardName);
            Task task = board.GetTask(taskId);
            if (!user.LoggedIn)
            {
                log.Error("User is not logged in. Setting task description failed.");
                throw new Exception("You are not logged in.");
            }

            task.SetDescription(user, desc);
            taskController.Update(task.DTO);
            log.Info($"Description of task '{task.Id}' set to '{desc}' by user '{user.Email}'");
        }

        /// <summary>
        /// Adds a new task to the board.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="title">The new task's title.</param>
        /// <param name="description">The new task's description.</param>
        /// <param name="dueDate">The new task's due date (to set).</param>
        public Task AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            User user = uf.GetUser(email);
            Board board = GetBoard(email, boardName);

            if (!user.LoggedIn)
            {
                log.Error("User is not logged in. Adding task failed.");
                throw new Exception("You are not logged in.");
            }

            Task task = board.AddTask(user, title, description, dueDate);
            taskController.Insert(task.DTO);
            log.Info($"New task added to board '{board.Name}' by user '{user.Email}': {task.Id}");
            return task;
        }

        public Task AddTask(string email, int id, string title, string description, DateTime dueDate)
        {
            Board board = GetBoard(id);
            User user = uf.GetUser(email);

            if (!user.LoggedIn)
            {
                log.Error("User is not logged in. Adding task failed.");
                throw new Exception("You are not logged in.");
            }

            Task task = board.AddTask(user, title, description, dueDate);
            taskController.Insert(task.DTO);
            log.Info($"New task added to board '{board.Name}' by user '{user.Email}': {task.Id}");
            return task;
        }

        /// <summary>
        /// Assign a task to a user.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="taskID">The task id</param>
        /// <param name="emailAssignee">The new assignee's email.</param>
        public void AssignTask(string email,string boardName, int taskID, string emailAssignee)
        {
            User user = uf.GetUser(email);
            Board board = GetBoard(user.Email, boardName);
            Task task = board.GetTask(taskID);
            User newAssignee = null;

            if (emailAssignee != "")
                newAssignee = uf.GetUser(emailAssignee);

            if (newAssignee != null && !board.IsMember(newAssignee))
                throw new Exception($"User {email} is not a member of board {boardName}");

            if (!user.LoggedIn)
            {
                log.Error("User is not logged in. Assigning task failed.");
                throw new Exception("You are not logged in.");
            }

            if (task.Assignee != null && task.Assignee.Equals(newAssignee))
            {
                log.Error("User is already assigned to this task. Assigning task failed.");
                throw new Exception("This user is already assigned to this task.");
            }

            task.AssignTask(user, newAssignee);
            taskController.Update(task.DTO);

            if(newAssignee != null)
                log.Info($"Task '{task.Id}' assigned to user '{newAssignee.Email}' by user '{user.Email}'");
            else
                log.Info($"Task '{task.Id}' unassigned by user '{user.Email}'");
        }

        /// <summary>
        /// Gets a column's name.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="index">The column's index.</param>
        public string GetColumnName(string email, string boardName, int index)
        {
            User user = uf.GetUser(email);
            Board board = GetBoard(email, boardName);

            if (!user.LoggedIn)
            {
                log.Error("User is not logged in. Retrieving column name failed.");
                throw new Exception("You are not logged in.");
            }

            if (!Enum.IsDefined(typeof(Columns), index))
            {
                log.Error("Invalid column index. Retrieving column name failed.");
                throw new Exception("Invalid column index.");
            }

            string columnName = (Columns)index == Columns.InProgress ? "In Progress" : ((Columns)index).ToString();
            log.Info($"Retrieved column name '{columnName}' from board '{board.Name}' by user '{user.Email}'");
            return columnName;
        }

        /// <summary>
        /// Transfers ownership of a board to another user.
        /// </summary>
        /// <param name="currentOwner">The user's email (of the action performer).</param>
        /// <param name="newOwner">The email of the new owner</param>
        /// <param name="boardName">The board's name</param>
        public void TransferOwnership(string currentOwner, string newOwner, string boardName)
        {
            User currentUser, newUser;
            Board board;

            currentUser = uf.GetUser(currentOwner);
            newUser = uf.GetUser(newOwner);
            board = GetBoard(currentOwner, boardName);

            if (!currentUser.LoggedIn)
            {
                log.Error("User is not logged in. Transferring ownership failed.");
                throw new Exception("You are not logged in.");
            }

            foreach (int id in newUser.BoardsIds)
            {
                Board b = boardsById[id];
                if (b.Name == board.Name && b.Owner.Equals(newOwner))
                {
                    log.Error("New owner is already an owner of a board with that name. Transferring ownership failed.");
                    throw new Exception("New owner is already an owner of a board with that name.");
                }
            }

            board.TransferOwnership(currentUser, newUser);

            // Reset the keys for the dictionaries
            UnindexBoard(board);
            IndexNewBoard(board);

            boardController.Update(board.DTO);
            log.Info($"Ownership of board '{board.Name}' transferred from user '{currentOwner}' to user '{newOwner}'");
        }


        /// <summary>
        /// Gets a column's tasks from a board.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="col">The column's index.</param>
        /// <returns>A list of tasks, containing all tasks in the given column of this board.</returns>
        public List<Task> GetColumn(string email, string boardName, int col)
        {
            User user = uf.GetUser(email);
            Board board = GetBoard(email, boardName);
            Columns column = (Columns)col;
            if (!user.LoggedIn)
                throw new Exception("You are not logged in.");

            return board.GetColumn(column);
        }

        public List<Task> GetColumn(string email, int id, int col)
        {
            User user = uf.GetUser(email);
            Board board = GetBoard(id);
            Columns column = (Columns)col;
            if (!user.LoggedIn)
                throw new Exception("You are not logged in.");

            return board.GetColumn(column);
        }

        /// <summary>
        /// Gets a column's maximum number of tasks in a given board.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="col">The column's index.</param>
        /// <returns>The maximum number of tasks</returns>
        public int GetColumnLimit(string email, string boardName, int col)
        {
            User user = uf.GetUser(email);
            Board board = GetBoard(email, boardName);
            Columns column = (Columns)col;

            if (!user.LoggedIn)
            {
                log.Error("User is not logged in. Retrieving column limit failed.");
                throw new Exception("You are not logged in.");
            }

            int columnLimit = board.GetColumnLimit(column);
            log.Info($"Retrieved column limit '{columnLimit}' from column '{column}' in board '{board.Name}' by user '{user.Email}'");
            return columnLimit;
        }

        //Adds a new board to the boards dictionaries
        private void IndexNewBoard(Board board)
        {
            boardsById.Add(board.Id, board);
            boardsByName.Add((board.Owner, board.Name), board);
            log.Info($"Added new board '{board.Name}' to the boards dictionaries");
        }

        //Removes a board from the boards dictionaries
        private void UnindexBoard(Board board)
        {
            int id = board.Id;
            User owner = board.Owner;
            string name = board.Name;

            boardsById.Remove(id);
            boardsByName.Remove((owner, name));
            log.Info($"Removed board '{board.Name}' from the boards dictionaries");
        }

        /// <summary>
        /// Loads all boards from the database.
        /// </summary>
        public void LoadBoardsFromDB()
        {
            foreach (BoardDTO dto in boardController.GetAllFromDB())
            {
                User owner = uf.GetUser(dto.Owner);
                Board board = new Board(dto, owner);
                
                if (boardsById.ContainsKey(board.Id))
                    throw new Exception("Board with this id already exists");
                if (boardsByName.ContainsKey((board.Owner, board.Name)))
                    throw new Exception("Board with this name by this owner already exists");

                IndexNewBoard(board);

                //Add members to the board
                foreach (string member in dto.Members)
                {
                    User user = uf.GetUser(member);
                    board.AddMember(user);
                   
                }
            }
        }

        /// <summary>
        /// Loads all tasks from the database.
        /// </summary>
        public void LoadTasksFromDB()
        {
            log.Info("Loading tasks from database...");
            foreach (TaskDTO dto in taskController.GetAllFromDB())
            {
                Task task;
                if (dto.Assignee != "")
                {
                    User user = uf.GetUser(dto.Assignee);
                    task = new Task(dto, user);
                    user.AssignTask(task);
                }
                else
                {
                    task = new Task(dto, null);
                }
                Board board = GetBoard(task.BoardId);
                board.AddTask(task);
            }
            log.Info("Tasks loaded from database successfully");
        }

        /// <summary>
        /// Clears all boards from the database.
        /// </summary>
        public void ClearAllBoards() => boardController.ClearData();

        /// <summary>
        /// Clears all tasks from the database.
        /// </summary>
        public void ClearAllTasks() => taskController.ClearData();

        internal object GetTaskAssignee(int taskId, int boarId)
        {
            return boardsById[boarId].GetTask(taskId).Assignee;
        }
    } 
}



using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class TaskService
    {
        private static readonly ILog log = Logger.GetLogger(typeof(TaskService));
        private readonly BoardFacade bf;
        //private readonly UserFacade uf;

        public TaskService(BoardFacade bf, UserFacade uf)
        {
            this.bf = bf;
            //this.uf = uf;
        }

        /// <summary>
        /// Set a task's due date.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="taskId">The task id in the board.</param>
        /// <param name="dueDate">The task's due date (to set).</param>
        public string SetTaskDueDate(string email, string boardName, int taskId, DateTime dueDate)
        {
            Response r;
            try
            {
                bf.SetDueDate(email,boardName, taskId, dueDate);
                r = new Response();
                log.Info($"Task {taskId} in board {boardName} due date changed to {dueDate} by {email}");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error(e.Message);
            }
            return r.ToJson();
        }

        /// <summary>
        /// Sets a task's title.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="taskId">The task id in the board.</param>
        /// <param name="title">The task's title (to set).</param>
        public string SetTaskTitle(string email, string boardName, int taskId, string title)
        {
            Response r;
            try
            {
                bf.SetTitle(email,boardName, taskId, title);
                r = new Response();
                log.Info($"Task {taskId} in board {boardName} title changed to {title} by {email}");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error(e.Message);
            }
            return r.ToJson();
        }

        /// <summary>
        /// Sets a task's description.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="taskId">The task id in the board.</param>
        /// <param name="description">The task's description (to set).</param>
        public string SetTaskDescription(string email, string boardName, int taskId, string description)
        {
            Response r;
            try
            {
                bf.SetDescription(email,boardName, taskId, description);
                r = new Response();
                log.Info($"Task {taskId} in board {boardName} description changed to {description} by {email}");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error(e.Message);
            }
            return r.ToJson();
        }

        public string GetTaskAssignee(int taskId, int boarId)
        {
            Response r;
            try
            {
                r = new Response(null, bf.GetTaskAssignee(taskId, boarId));
                log.Info($"Assignee of task {taskId} has been sent");
            }
            catch (Exception e) 
            {
                r = new Response(e.Message);
               log.Error(e.Message);
            }
            return r.ToJson();
        }

        /// <summary>
        /// Assign a task to a user.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="taskID">The task's id.</param>
        /// <param name="emailAssignee">The new assignee's email.</param>
        public string AssignTask(string email, string boardName, int taskID, string emailAssignee)
        {
            Response r;
            try 
            { 
                bf.AssignTask(email,boardName, taskID, emailAssignee);
                r = new Response();
                log.Info($"Task '{taskID}' was assigned to the user {emailAssignee} by user '{email}'");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error($"An error occurred while trying to assign task '{taskID}' to user {emailAssignee} by {email} in board {boardName}:\n{e.Message}");
            }

            return r.ToJson();
        }

        /// <summary>
        /// Adds a new task to a board.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="title">The new task's title.</param>
        /// <param name="description">The new task's description.</param>
        /// <param name="dueDate">The new task's due date (to set).</param>
        public string AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            Response r;
            try
            {

                bf.AddTask(email, boardName, title, description, dueDate);
                r = new Response();
                log.Info($"New task added to the board {boardName} by {email}\n{title}, {description}, {dueDate}");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error(e.Message);
            }
            return r.ToJson();
        }

        public string AddTask(string email, int id, string title, string description, DateTime dueDate)
        {
            Response r;
            try
            {
                bf.AddTask(email, id, title, description, dueDate);
                r = new Response();
                log.Info($"New task added to the board {id}S by {email}\n{title}, {description}, {dueDate}");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error(e.Message);
            }
            return r.ToJson();
        }

        /// <summary>
        /// Advances a task in a board to the next column.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="taskId">The task's id.</param>
        public string AdvanceTask(string email, string boardName, int taskId)
        {
            Response r;

            try
            {
                bf.AdvanceTask(email, boardName, taskId);
                r = new Response();
                log.Info($"Task {taskId} advanced in board {boardName} by {email}");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error(e.Message);
            }
            return r.ToJson();
        }

        /// <summary>
        /// Loads all tasks from the database.
        /// </summary>
        /// <param name="response">A response object to output response to.</param>
        /// <returns>A JSON string representing the response object.</returns>
        public string LoadTasks(ref Response response)
        {
            if (response.ErrorMessage != null)
                return response.ToJson();

            try
            {
                bf.LoadTasksFromDB();
                response = new Response();
                log.Info("Tasks loaded from database successfuly");
            }
            catch (Exception e)
            {
                response = new Response(e.Message);
                log.Error("Error while trying to load tasks from database:\n" + e.Message);
            }
            return response.ToJson();
        }

        /// <summary>
        /// Clears all tasks from the database.
        /// </summary>
        /// <param name="response">A response object to pass</param>
        public string ClearAllTasks(ref Response response)
        {
            if (response.ErrorMessage != null)
                return response.ToJson();

            try
            {
                bf.ClearAllTasks();
                response = new Response();
                log.Info("Tasks database has been cleared");
            }
            catch (Exception e)
            {
                response = new Response(e.Message);
                log.Error("Error while trying to clear tasks database");
            }
            return response.ToJson();
        }
    }
}

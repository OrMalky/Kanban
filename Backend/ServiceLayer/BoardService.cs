using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Task = IntroSE.Kanban.Backend.BusinessLayer.Task;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class BoardService
    {
        private BoardFacade bf;
        //private UserFacade uf;

        private static readonly ILog log = Logger.GetLogger(typeof(BoardService));

        public BoardService(BoardFacade bf, UserFacade uf)
        {
            this.bf = bf;
            //this.uf = uf;
        }

        /// <summary>
        /// Transfers ownership of a board to another user.
        /// </summary>
        /// <param name="currentOwner">The current owner's email.</param>
        /// <param name="newOwner">The new owner's email.</param>
        /// <param name="boardName">The board's name.</param>
        public string TransferOwnership(string currentOwner, string newOwner, string boardName)
        {
            Response r;
            try
            { 
                bf.TransferOwnership(currentOwner, newOwner, boardName);
                r = new Response();
                log.Info($"Board '{boardName}' has been transfered from '{currentOwner}' to '{newOwner}'.");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error($"An error occurred while transfering ownership of board '{boardName}' from user '{currentOwner}' to user '{newOwner}':\n{e.Message}");
            }
            return r.ToJson();
        }


        public string GetOwner(int boardId)
        {
            Response r;
            try
            {
                
                r = new Response(null, bf.GetOwner(boardId).Email);
                log.Info($"Boards ('{boardId}') owner has been found'.");
                
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error($"could not find the boards owner");
            }
            return r.ToJson();
        }

        /// <summary>
        /// Joins a user to a board.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="boardId">The board's id.</param>
        public string JoinBoard(string email, int boardId)
        {
            Response r;
            try
            {
                bf.JoinBoard(email, boardId);
                log.Info($"'{email}' has joined to the board '{boardId}");
                r = new Response();
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error($"An error occurred while trying to join the user: '{email}' to board: '{boardId}'\n{e.Message}");
            }
            return r.ToJson();
        }

        /// <summary>
        /// Removes a user from a board.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="boardId">The board's id.</param>
        public string LeaveBoard(string email, int boardId)
        {
            Response r; 
            try
            {
                bf.LeaveBoard(email, boardId);
                r = new Response();
                log.Info($"'{email}' has left board : {boardId}");
            }
            catch (Exception e) 
            {
                r = new Response(e.Message);
                log.Error($"An error occurred while trying to remove user '{email}' from the board: '{boardId}'\n{e.Message}");
            }

            return r.ToJson();
        }

        /// <summary>
        /// Gets the board's name.
        /// </summary>
        /// <param name="boardId">The board's id.</param>
        /// <returns>The board's name.</returns>
        public string GetBoardName(int boardId)
        {
            Response r;
            try
            {
                string name = bf.GetBoardName(boardId).Name;
                r = new Response(null, name);
                log.Info($"Board '{boardId}' name was retrieved: {name}");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error($"An error occurred while trying to retrieve name of board '{boardId}':\n{e.Message}");
            }

            return r.ToJson();
        }

        /// <summary>
        /// Adds a new board to the system.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardName">The new board's name.</param>
        public string CreateBoard(string email, string boardName)
        {
            Response r;
            try
            {
                bf.CreateBoard(email, boardName);
                r = new Response();
                log.Info($"Board '{boardName}' was created by user '{email}'");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error($"An error occurred while creating board '{boardName}' by user '{email}':\n{e.Message}");
            }
            return r.ToJson();
        }

        /// <summary>
        /// Checks if a user is a member of a given board.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardId">The new board's id.</param>
        /// <returns>True if the user is a member of the board, false otherwise.</returns>
        public string IsMember(string email, int boardId)
        {
            Response r;
            try
            {
                bool result = bf.IsMember(email, boardId);
                r = new Response(null, result);
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
            }
            return r.ToJson();
        }

        /// <summary>
        /// Deletes a board from the system.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardId">The board's id.</param>
        public string DeleteBoard(string email, int boardId)
        {
            Response r;
            try
            {
                bf.DeleteBoard(email, boardId);
                r = new Response();
                log.Info($"Board '{boardId}' was deleted by user '{email}'.");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error($"An error occurred while trying to delete board '{boardId}' by user '{email}':\n{e.Message}");
            }
            return r.ToJson();
        }

        /// <summary>
        /// Deletes a board from the system.
        /// </summary>
        /// <param name="email">The user's email (of the action performer).</param>
        /// <param name="boardId">The board's name.</param>
        public string DeleteBoard(string email, string boardName)
        {
            Response r;
            try
            { 
                bf.DeleteBoard(email, boardName);
                r = new Response();
                log.Info($"Board '{boardName}' was deleted by user '{email}'.");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error($"An error occurred while trying to delete board '{boardName}' by user '{email}':\n{e.Message}");
            }

            return r.ToJson();
        }

        /// <summary>
        /// Gets all the baords of a given user.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <returns>A list of the user's board ids.</returns>
        public string GetUserBoards(string email)
        {
            Response r;
            List<int> result;
            try
            {
                result = bf.GetUserBoards(email);
                r = new Response(null, result);
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error($"An error occurred while trying to get boards of user '{email}':\n{e.Message}");
            }
            return r.ToJson();
        }

        /// <summary>
        /// Gets a column's maximum number of tasks in a given board.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="col">The column's index.</param>
        /// <returns>The maximum number of tasks</returns>
        public string GetColumnLimit(string email, string boardName, int col)
        {
            Response r;
            int limit;
            try
            {
                limit = bf.GetColumnLimit(email, boardName, col);
                r = new Response(null, limit);
                log.Info($"Column limit of column {col} in board '{boardName}' was retrieved by user '{email}'.");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error($"An error occurred while retrieving column limit of column {col} in board '{boardName}' by user '{email}':\n{e.Message}");
            }
            return r.ToJson();
        }

        /// <summary>
        /// Limit a column's maximum number of tasks.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="column">The column's index.</param>
        /// <param name="max">The maximum number of tasks.</param>
        public string LimitColumn(string email, string boardName, int column, int max)
        {
            Response r;
            try
            {
                bf.LimitColumns(email, boardName, column, max);
                r = new Response();
                log.Info($"User {email} has set the limit of column {column} " +
                         $"in board {boardName} to {max}");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error($"Error occurred while setting the limit of column {column} " +
                          $"in board {boardName} to {max} by user {email}:\n{e.Message}");
            }
            return r.ToJson();
        }

        /// <summary>
        /// Gets a column's tasks from a board.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="columnOrdinal">The column's index.</param>
        /// <returns>A list of tasks in the given column of this board.</returns>
        public string GetColumn(string email, string boardName, int columnOrdinal)
        {
            Response r;
            List<Task> result;
            try
            {
                result = bf.GetColumn(email, boardName, columnOrdinal);
                r = new Response(null, result);
                log.Info($"User {email} retrieved the column {columnOrdinal} of board {boardName}");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error(e.Message);
            }
            return r.ToJson();
        }

        /// <summary>
        /// Gets a column's tasks from a board.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="id">The board's id.</param>
        /// <param name="columnOrdinal">The column's index.</param>
        /// <returns>A list of tasks in the given column of this board.</returns>
        public string GetColumn(string email, int id, int columnOrdinal)
        {
            Response r;
            List<Task> result;
            try
            {
                result = bf.GetColumn(email, id, columnOrdinal);
                r = new Response(null, result);
                log.Info($"User {email} retrieved the column {columnOrdinal} of board {id}");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error(e.Message);
            }
            return r.ToJson();
        }

        /// <summary>
        /// Gets a column's name.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="boardName">The board's name.</param>
        /// <param name="columnOrdinal">The column's index.</param>
        /// <returns>The name of the given column.</returns>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            Response r;
            try
            {
                string name = bf.GetColumnName(email, boardName, columnOrdinal);
                r = new Response(null, name);
                log.Info($"User {email} got the name of column {columnOrdinal} in board {boardName}");
            }
            catch (Exception e)
            {
                r = new Response(e.Message);
                log.Error(e.Message);
            }
            return r.ToJson();
        }

        /// <summary>
        /// Loads all boards from the database.
        /// </summary>
        /// <param name="allUsers">A dictionary with all users loaded from the database.</param>
        /// <param name="allTasks">A dictionary with all tasks loaded from the database.</param>
        /// <param name="response">A response object to output response to.</param>
        /// <returns>A JSON string representing the response object.</returns>
        public string LoadBoards(ref Response response)
        {
            if (response.ErrorMessage != null)
                return response.ToJson();

            try
            {
                bf.LoadBoardsFromDB();
                response = new Response();
                log.Info("Loaded boards from database successfuly");
            }
            catch (Exception e)
            {
                response = new Response(e.Message);
                log.Error("Error while trying to load boards from database:\n" + e.Message);
            }
            return response.ToJson();
        }

        /// <summary>
        /// Clears all boards from the database.
        /// <b>You must clear all tasks before clearing boards!</b>
        /// </summary>
        /// <param name="response">A response object to pass</param>
        public string ClearAllBroards(ref Response response)
        {
            if (response.ErrorMessage != null)
                return response.ToJson();

            try
            {
                bf.ClearAllBoards();
                response = new Response();
                log.Info("Boards database has been cleared");
            }
            catch (Exception e)
            {
                response = new Response(e.Message);
                log.Error("Error while trying to clear boards database");
            }
            return response.ToJson();
        }
    }
}
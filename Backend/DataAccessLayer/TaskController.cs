
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.SqlClient;
using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;
using System.Reflection;
using System.IO;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class TaskController : Controller<TaskDTO>
    {
        public TaskController() : base() => tableName = "Tasks";

        //Converts current reader object to a DTO object
        protected override TaskDTO ReaderToObject(SQLiteDataReader r)
        {
            int taskId = r.GetInt32(0);
            int boardId = r.GetInt32(1);
            int column = r.GetInt32(2);
            DateTime due = r.GetDateTime(3);
            string title = r.GetString(4);
            string assignee = r.GetString(5);
            string desc = r.GetString(6);
            bool done = r.GetBoolean(7);
            DateTime creation = r.GetDateTime(8);
            return new TaskDTO(taskId, boardId, column, title, assignee, creation, due, desc, done);
        }

        // Insert command for the users table
        protected override void InsertCommand(TaskDTO dto)
        {
            log.Info($"Inserting task to database: [{dto.Id}:{dto.BoardId}]");
            command.CommandText = "INSERT INTO Tasks (taskId,boardId,column,due,title,assignee,description,done,creation) " +
               $"VALUES (@idVal,@boardIdVal,@colVal,@dueVal,@titleVal,@assigneeVal,@desctriptionVal,@doneVal,@creationVal);";

            command.Parameters.AddWithValue("@idVal", dto.Id);
            command.Parameters.AddWithValue("@boardIdVal", dto.BoardId);
            command.Parameters.AddWithValue("@colVal", dto.Column);
            command.Parameters.AddWithValue("@dueVal", dto.DueDate);
            command.Parameters.AddWithValue("@titleVal", dto.Title);
            command.Parameters.AddWithValue("@assigneeVal", dto.Assignee);
            command.Parameters.AddWithValue("@desctriptionVal", dto.Description);
            command.Parameters.AddWithValue("@doneVal", dto.Done);
            command.Parameters.AddWithValue("@creationVal", dto.CreationTime);
        }

        // Delete command for the users table
        protected override void DeleteCommand(TaskDTO dto)
        {
            log.Info($"Deleting task from database: [{dto.Id}:{dto.BoardId}]");
            command.CommandText = "DELETE FROM Tasks where id = @idVal " +
                                    $"VALUES (@idVal);";

            command.Parameters.AddWithValue("@idVal", dto.Id);
        }

        // Update command for the users table
        protected override void UpdateCommand(TaskDTO dto)
        {
            log.Info($"Updating task in database: [{dto.Id}:{dto.BoardId}], " +
                     $"with values: {dto.DueDate}, {dto.Title}, {dto.Description}, {dto.Assignee}, {dto.Column}, {dto.Done}");
            command.CommandText = "UPDATE Tasks SET due = @dueDateVal , title = @titleVal ," +
                                  "description = @descriptionVal , assignee = @assigneeVal, column = @columnVal, done = @doneVal " +
                                  "WHERE taskId = @idVal and boardId = @boardIdVal ";

            command.Parameters.AddWithValue("@idVal", dto.Id);
            command.Parameters.AddWithValue("@boardIdVal", dto.BoardId);
            command.Parameters.AddWithValue("@dueDateVal", dto.DueDate);
            command.Parameters.AddWithValue("@titleVal", dto.Title);
            command.Parameters.AddWithValue("@descriptionVal", dto.Description);
            command.Parameters.AddWithValue("@assigneeVal", dto.Assignee);
            command.Parameters.AddWithValue("@columnVal", dto.Column);
            command.Parameters.AddWithValue("@doneVal", dto.Done);
        }

        /*
        public override bool Delete(TaskDTO dto)
        {
            using SQLiteConnection connection = new SQLiteConnection(connectionString);
            int res;
            SQLiteCommand command = new SQLiteCommand(null, connection);
            try
            {
                connection.Open();
                command.CommandText = "DELETE FROM Tasks where id = @idVal " +
                                        $"VALUES (@idVal);";

                command.Parameters.AddWithValue("@idVal", dto.Id);

                command.Prepare();
                res = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                command.Dispose();
                connection.Close();
            }
            return res > 0;
        }

        public override bool Update(TaskDTO task)
        {
            using SQLiteConnection connection = new SQLiteConnection(connectionString);
            int res;
            SQLiteCommand command = new SQLiteCommand(null, connection);
            try
            {
                connection.Open();
                command.CommandText = "UPDATE Tasks SET dueDate = @dueDateVal , title = @titleVal , description = @descriptionVal , assignee = @assigneeVal, column = @coulumnVal WHERE id = @idVal and boardId = @boardIdVal " +
                                      $"VALUES (@idVal,@boardIdVal,@dueDateVal,@titleVal,@desctriptionVal,@assigneeVal,@colNumVal);";

                command.Parameters.AddWithValue("@idVal", task.Id);
                command.Parameters.AddWithValue("@boarIdVal", task.BoardId);
                command.Parameters.AddWithValue("@dueDateVal", task.DueDate);
                command.Parameters.AddWithValue("@titleVal", task.Title);
                command.Parameters.AddWithValue("@descriptionVal", task.Description);
                command.Parameters.AddWithValue("@assigneeVal", task.Assignee);
                command.Parameters.AddWithValue("@colNumVal", task.Column);


                command.Prepare();

                res = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                command.Dispose();
                connection.Close();
            }
            return res > 0;
        }

        public override bool Insert(TaskDTO task)
        {
            using SQLiteConnection connection = new SQLiteConnection(connectionString);
            int res;
            SQLiteCommand command = new SQLiteCommand(null, connection);
            try
            {
                connection.Open();
                command.CommandText = "INSERT INTO Task (id,boardIdVal,dueDate,title,description,assignee,colNum,creationTime) " +
                $"VALUES (@idVal,@boardIdVal,@dueDateVal,@titleVal,@desctriptionVal,@assigneeVal,@colNumVal,@creationTimeVal);";

                command.Parameters.AddWithValue("idVal", task.Id);
                command.Parameters.AddWithValue("boarIdVal", task.BoardId);
                command.Parameters.AddWithValue("dueDateVal", task.DueDate);
                command.Parameters.AddWithValue("titleVal", task.Title);
                command.Parameters.AddWithValue("descriptionVal", task.Description);
                command.Parameters.AddWithValue("assigneeVal", task.Assignee);
                command.Parameters.AddWithValue("colNumVal", task.Column);
                command.Parameters.AddWithValue("creationTimeVal", task.CreationTime);

                command.Prepare();

                res = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                command.Dispose();
                connection.Close();
            }
            return res > 0;
        }
        */
    }
}

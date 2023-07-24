
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using IntroSE.Kanban.Backend;
using IntroSE.Kanban.Backend.BusinessLayer;


namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class UserController : Controller<UserDTO>
    {
        public UserController() : base() => tableName = "Users";

        //Converts current reader object to a DTO object
        protected override UserDTO ReaderToObject(SQLiteDataReader r)
        {
            string email = r.GetString(0);
            string password = r.GetString(1);
            
            return new UserDTO(email, password);
        }

        // Insert command for the users table
        protected override void InsertCommand(UserDTO user)
        {
            log.Info("Inserting user to database: " + user.Email);
            command.CommandText = "INSERT INTO Users (email,password) " +
                                    $"VALUES (@emailVal,@passwordVal);";

            command.Parameters.AddWithValue("@emailVal", user.Email);
            command.Parameters.AddWithValue("@passwordVal", user.Password);
        }

        // Delete command for the users table
        protected override void DeleteCommand(UserDTO user)
        {
            log.Info("Deleting user from database: " + user.Email);
            command.CommandText = "DELETE FROM Users WHERE email = @emailVal";
            command.Parameters.AddWithValue("@emailVal", user.Email);
        }

        // Update command for the users table
        protected override void UpdateCommand(UserDTO user)
        {
            log.Info($"Updating user in database: {user.Email}");
            command.CommandText = "UPDATE Users SET password = @passwordVal WHERE email = @emailVal";
            command.Parameters.AddWithValue("@emailVal", user.Email);
            command.Parameters.AddWithValue("@passwordVal", user.Password);
        }

        /*
        public override bool Delete(UserDTO user)
        {
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    int res;
                    SQLiteCommand command = new SQLiteCommand(null, connection);
                    try
                    {
                        connection.Open();
                        command.CommandText = "DELETE FROM Users WHERE email = @emailVal";
                        command.Parameters.AddWithValue("@emailVal", user.Email);


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
            }
        }

        public override bool Update(UserDTO user)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                int res;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = "UPDATE Users SET tasks = @tasksVal, boardsId = @boardsIdVal WHERE email = @emailVal";
                    command.Parameters.AddWithValue("@emailVal", user.Email);
                    command.Parameters.AddWithValue("@passwordVal", user.Password);
                    command.Parameters.AddWithValue("@tasksVal", string.Join(",", user.AssignedTasks.Select(pair => $"{pair.Item1}:{pair.Item2}")));
                    command.Parameters.AddWithValue("@boardsIdVal", string.Join(',', user.BoardsId));

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
        }

        public override bool Insert(UserDTO user)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                int res;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = "INSERT INTO Users (email,password,tasks) " +
                                          $"VALUES (@emailVal,@passwordVal,@tasksVal);";
                    command.Parameters.AddWithValue("emailVal", user.Email);
                    command.Parameters.AddWithValue("passwordVal", user.Password);
                    command.Parameters.AddWithValue("tasksVal", string.Join(",", user.AssignedTasks.Select(pair =>
                        $"{pair.Item1}:{pair.Item2}")));

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
        }
        */
    }
}

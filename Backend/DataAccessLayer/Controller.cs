using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

using IntroSE.Kanban.Backend.BusinessLayer;
using static log4net.Appender.RollingFileAppender;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public abstract class Controller<T> where T : IDTO
    {
        protected static readonly ILog log = Logger.GetLogger(typeof(Controller<T>));
        protected string connectionString;
        protected SQLiteConnection connection;
        protected SQLiteCommand command;
        protected SQLiteDataReader reader;
        protected string query;
        protected string tableName;

        protected Controller()
        {
            string dir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            connectionString = $"Data Source={dir};Version=3;";
            connection = new SQLiteConnection(connectionString);
            //Console.WriteLine(dir);
        }

        /// <summary>
        /// Gets all records from the database.
        /// </summary>
        /// <returns>A list of all <typeparamref name="T"/>s.</returns>
        public List<T> GetAllFromDB()
        {
            List<T> result = new List<T>();
            try
            {
                connection.Open();
                query = "SELECT * FROM " + tableName;
                command = new SQLiteCommand(query, connection);

                reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    result.Add(ReaderToObject(reader));
                }
                log.Info("Database table retrieved successfuly: " + tableName);
            }
            catch (Exception ex)
            {
                log.Error("Error while trying to retrieve database table: " + tableName + "\n" + ex.Message);
            }
            finally
            {
                reader.Close();
                command.Dispose();
                connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Clears all records from the database.
        /// </summary>
        public bool ClearData()
        {
            bool isSuccessful = false;
            try
            {
                connection.Open();
                query = $"DELETE FROM {tableName}";
                command = new SQLiteCommand(query, connection);
                command.ExecuteNonQuery();
                isSuccessful = true;
                log.Info("Database table cleared successfuly: " + tableName);
            }
            catch (Exception ex)
            {
                log.Error("Error while trying to clear database table: " + tableName + "\n" + ex.Message);
            }
            finally
            {
                command.Dispose();
                connection.Close();
            }
            return isSuccessful;
        }

        /// <summary>
        /// Inserts a new record to the database.
        /// </summary>
        /// <param name="dto">The relevant DTO object.</param>
        /// <returns><see langword="true"/> if the action was successful, <see langword="false"/> otherwise.</returns>
        public bool Insert(T dto) => ExecuteCommand(InsertCommand, dto);

        /// <summary>
        /// Deletes a record from the database.
        /// </summary>
        /// <param name="dto">The relevant DTO object.</param>
        /// <returns><see langword="true"/> if the action was successful, <see langword="false"/> otherwise.</returns>
        public bool Delete(T dto) => ExecuteCommand(DeleteCommand, dto);

        /// <summary>
        /// Updates a record in the database.
        /// </summary>
        /// <param name="dto">The relevant DTO object.</param>
        /// <returns><see langword="true"/> if the action was successful, <see langword="false"/> otherwise.</returns>
        public bool Update(T dto) => ExecuteCommand(UpdateCommand, dto); 

        protected abstract void InsertCommand(T dto);

        protected abstract void DeleteCommand(T dto);

        protected abstract void UpdateCommand(T dto);

        /// <summary>
        /// Executes a command on the database.
        /// </summary>
        /// <param name="c">Function of command to execute.</param>
        /// <param name="dto">The relevant DTO object.</param>
        /// <returns><see langword="true"/> if the action was successful, <see langword="false"/> otherwise.</returns>
        public bool ExecuteCommand(Action<T> c,T dto)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                int res = -1;
                this.command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    c(dto);
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                    log.Info("Database updated successfuly: " + tableName);
                }
                catch (Exception ex)
                {
                    log.Error("Error while trying to update database table: " + tableName + "\n" + ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                return res > 0;
            }
        }

        //Converts current reader object to a DTO object
        protected abstract T ReaderToObject(SQLiteDataReader r);
    }
}

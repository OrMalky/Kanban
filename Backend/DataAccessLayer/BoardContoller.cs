using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using IntroSE.Kanban.Backend.BusinessLayer;
using System.IO;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class BoardContoller : Controller<BoardDTO>
    {
        public BoardContoller() : base() => tableName = "Boards";

        //Converts current reader object to a DTO object
        protected override BoardDTO ReaderToObject(SQLiteDataReader r)
        {
            int id = r.GetInt32(0);
            string name = r.GetString(1);
            string owner = r.GetString(2);
            List<int> maxTasks = r.GetString(3).Split(',').Select(int.Parse).ToList();
            List<string> members = r.GetString(4).Split(',').ToList();
            return new BoardDTO(id, name, owner, maxTasks, members);
        }

        // Insert command for the boards table
        protected override void InsertCommand(BoardDTO dto)
        {
            log.Info("Inserting board to database: " + dto.Id);
            command.CommandText = "INSERT INTO Boards (id,name,owner,maxTasks,members) " +
                                         $"VALUES (@id,@name,@owner,@maxTasks,@members)";

            command.Parameters.AddWithValue("id", dto.Id);
            command.Parameters.AddWithValue("name", dto.Name);
            command.Parameters.AddWithValue("owner", dto.Owner);
            command.Parameters.AddWithValue("maxTasks", string.Join(',', dto.MaxTasks));
            command.Parameters.AddWithValue("members", string.Join(',', dto.Members));
        }

        // Delete command for the boards table
        protected override void DeleteCommand(BoardDTO dto)
        {
            log.Info("Deleting board from database: " + dto.Id);
            command.CommandText = "DELETE FROM Boards WHERE id = @boardId";
            command.Parameters.AddWithValue("boardId", dto.Id);
        }

        // Update command for the boards table
        protected override void UpdateCommand(BoardDTO dto)
        {
            log.Info($"Updating board in database: {dto.Id}, with values: {dto.Owner}, {dto.MaxTasks}, {dto.Members}");
            command.CommandText = "UPDATE Boards SET " +
                                  "owner = @ownerVal , maxTasks = @maxTasksVal, members = @membersVal " +
                                  "WHERE id = @idVal";
                
            command.Parameters.AddWithValue("@idVal", dto.Id);
            command.Parameters.AddWithValue("@ownerVal", dto.Owner);
            command.Parameters.AddWithValue("@maxTasksVal", string.Join(',', dto.MaxTasks));
            command.Parameters.AddWithValue("@membersVal", string.Join(',', dto.Members));
        }

        /*

        public override bool Insert(BoardDTO board)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                int res;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = "INSET INTO Boards (id,name,owner,maxTasks,members) " +
                                          $"VALUES (@id,@name,@owner,@maxTasks,@members)";
                    command.Parameters.AddWithValue("id", board.Id);
                    command.Parameters.AddWithValue("owner", board.Name);
                    command.Parameters.AddWithValue("creator", board.Owner);
                    command.Parameters.AddWithValue("maxTasks", string.Join(',', board.MaxTasks));
                    command.Parameters.AddWithValue("members", board.Members);

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

        public override bool Delete(BoardDTO dto)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                int res;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = "DELETE FROM Boards WHERE id = @boardId";
                    command.Parameters.AddWithValue("boardId", dto.Id);

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

        public override bool Update(BoardDTO dto)
        {
            throw new NotImplementedException();
        }
        */
    }
}


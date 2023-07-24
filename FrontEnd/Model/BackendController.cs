using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace FrontEnd.Model
{
    public class BackendController
    {
        private Services services;

        public BackendController(Services services)
        {
            this.services = services;
        }

        public BackendController()
        {
            this.services = ServiceFactory.Create();
        }

        public UserModel Login(string email, string password)
        {
            ExecuteServiceMethod(() => services.us.Login(email, password));
            return new UserModel(this, email);
        }

        public UserModel Register(string email, string password)
        {
            ExecuteServiceMethod(() => services.us.Register(email, password));
            return new UserModel(this, email);
        }

        public void Logout(string email)
        {
            ExecuteServiceMethod(() => services.us.Logout(email));
        }

        public void AddTask(string email, int boardId, string title, string description, DateTime dueDate)
        {
            ExecuteServiceMethod(() => services.ts.AddTask(email, boardId, title, description, dueDate));
        }

        public void AdvanceTask(string email, string boardName, int taskId)
        {
            ExecuteServiceMethod(() => services.ts.AdvanceTask(email, boardName, taskId));
        }

        public void SetTaskDueDate(string email, string boardName, int taskId, DateTime dueDate)
        {
            ExecuteServiceMethod(() => services.ts.SetTaskDueDate(email, boardName, taskId, dueDate));
        }

        public void SetTaskTitle(string email, string boardName, int taskId, string title)
        {
            ExecuteServiceMethod(() => services.ts.SetTaskTitle(email, boardName, taskId, title));
        }

        public void setTaskDesc(string email, string boardName, int taskId, string desc)
        {
            ExecuteServiceMethod(() => services.ts.SetTaskDescription(email, boardName, taskId, desc)); ;
        }

        public void setTaskAssignee(string email, string boardName, int taskId, string newAssignee)
        {
            ExecuteServiceMethod(() => services.ts.AssignTask(email, boardName, taskId, newAssignee));
        }

        public string GetAssignee(int taskId, int boardId)
        {

            
                Response r = ExecuteServiceMethod(() => services.ts.GetTaskAssignee(taskId,boardId));
                if (r.ReturnValue == null)
                    return "";
                else 
                    return ((JsonElement)r.ReturnValue).GetProperty("Email").GetString();
            

        }

        public bool IsMember(string email, int boardId)
        {
            Response r = ExecuteServiceMethod(() => services.bs.IsMember(email, boardId));
            return r.ErrorMessage == null;
        }

        public BoardModel CreateBoard(string email, string boardName)
        {
            Response response = ExecuteServiceMethod(() => services.bs.CreateBoard(email, boardName));
            return new BoardModel(this, (int)response.ReturnValue,email);
        }

        private ObservableCollection<int> ParseJsonArray(JsonElement json)
        {
            List<int> list = new List<int>();

            if (json.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement element in json.EnumerateArray())
                {
                    if (element.ValueKind == JsonValueKind.Number)
                    {
                        int value = element.GetInt32();
                        list.Add(value);
                    }
                }
            }

            return new ObservableCollection<int>(list);
        }



        public ObservableCollection<BoardModel> GetUserBoards(string email)
        {
            Response response = ExecuteServiceMethod(() => services.bs.GetUserBoards(email));

            if (response.ReturnValue is JsonElement jsonElement)
            {
                response.ReturnValue = ParseJsonArray(jsonElement);
            }
            ObservableCollection<int> boardIds = (ObservableCollection<int>)response.ReturnValue;
            ObservableCollection<BoardModel> userBoards = new ObservableCollection<BoardModel>();

            foreach (int id in boardIds)
            {
                BoardModel board = new BoardModel(this, id, email);
                userBoards.Add(board);
            }

            return userBoards;
        }

       

        public ObservableCollection<TaskModel> GetColumnTasks(string email, int id, int col)
        {
            Response response = ExecuteServiceMethod(() => services.bs.GetColumn(email, id, col));
            var jsonDocument = JsonDocument.Parse(response.ReturnValue.ToString());
            JsonElement root = jsonDocument.RootElement;

            ObservableCollection<TaskModel> columnTasks = new ObservableCollection<TaskModel>();

            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement taskElement in root.EnumerateArray())
                {
                    TaskModel task = ExtractTaskFromJson(taskElement);
                    columnTasks.Add(task);
                }
            }

            return columnTasks;
        }


        private TaskModel ExtractTaskFromJson(JsonElement taskElement)
        {
            TaskModel task = new TaskModel(this,
                taskElement.GetProperty("Id").GetInt32(),
                DateTime.Parse(taskElement.GetProperty("DueDate").GetString()),
                taskElement.GetProperty("Title").GetString(),
                DateTime.Parse(taskElement.GetProperty("CreationTime").GetString()),
                taskElement.GetProperty("Description").GetString());

            return task;
        }

        public string GetBoardOwner(int boardId)
        {
            Response r = ExecuteServiceMethod(() => services.bs.GetOwner(boardId));
            return r.ReturnValue.ToString();
        }
        public string getBoardName( int boardId)
        {
            Response response = ExecuteServiceMethod(() => services.bs.GetBoardName( boardId));
            return response.ReturnValue.ToString();
        }

        private Response ExecuteServiceMethod(Func<string> serviceFunction)
        {
            string jsonResult = serviceFunction.Invoke();
            Response r = JsonSerializer.Deserialize<Response>(jsonResult);

            if (!string.IsNullOrEmpty(r.ErrorMessage))
            {
                throw new Exception(r.ErrorMessage);
            }

            return r;
        }

        
    }
}

using System;
using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;
using System.Windows.Markup;
using System.Runtime.ExceptionServices;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;

namespace BackendTests
{
    public class TaskServiceTests
    {
        TaskService ts;
        BoardService bs;
        UserService us;
        Response? response;

        public TaskServiceTests(UserService us, TaskService ts, BoardService bs)
        {
            this.us = us;
            this.ts = ts;
            this.bs = bs;
            response = new Response();
        }

        public void addTaskTests()
        {
            //add a full implemented task
            string resJson1 = ts.AddTask("tasktests1@gmail.com", "boardName", "taskName", "taskDescripiton", DateTime.Parse("1/1/2025"));//0
            response = JsonSerializer.Deserialize<Response>(resJson1);
            if (response != null && response.ErrorMessage != null)
            {
                Console.WriteLine("Error: " + response.ErrorMessage);
            }
            else
            {
                Console.WriteLine("Test passed: added new task");
            }

            // Test that a user that's not in the board cannot add a task to it
            string res = ts.AddTask("tasktests2@gmail.com", "boardName", "taskName", "taskDescripiton", DateTime.Parse("1/1/2025"));//1
            response = JsonSerializer.Deserialize<Response>(res);
            if(response == null)
                Console.WriteLine("ERROR: RESPONSE WAS NULL!");
            else if (response.ErrorMessage == null)
            {
                Console.WriteLine("Error: User that's not in the board added a task");
            }
            else
            {
                Console.WriteLine("Test passed: " + response.ErrorMessage);
            }

            //add a task to a non existing board
            string resJson2 = ts.AddTask("tasktests1@gmail.com", "nonExsitingBoard", "taskName", "taskDescripiton", new DateTime(2050, 1, 13));//2
            response = JsonSerializer.Deserialize<Response>(resJson2);
            if (response != null && response.ErrorMessage != null)
            {
                Console.WriteLine("Test passed: Error that should be thrown: " + response.ErrorMessage);
            }
            else
            {
                Console.WriteLine("Error: Task is created in a board that doasnt exist");
            }
        }

        public void settersTests()
        {
            //const string FMT = "yyyy-MM-dd";
            DateTime date = new DateTime(2050, 1, 1);

            //create task to set its properties
            ts.AddTask("tasktests1@gmail.com", "boardName", "taskName", "taskDescripiton", date);//3

            //Test that a user that's not the asignee cannot edit the task
            string r, result;

            // Title
            r = ts.SetTaskTitle("tasktests2@gmail.com", "boardName", 0, "newName");
            response = JsonSerializer.Deserialize<Response>(r);
            result = (response.ErrorMessage == null) ? "Error: a user who is not the asignee changed the task's title" : "Test passed: " + response.ErrorMessage;
            Console.WriteLine(result);

            // Desc
            r = ts.SetTaskDescription("tasktests2@gmail.com", "boardName", 0, "new description");
            response = JsonSerializer.Deserialize<Response>(r);
            result = (response.ErrorMessage == null) ? "Error: a user who is not the asignee changed the task's description" : "Test passed: " + response.ErrorMessage;
            Console.WriteLine(result);

            // Due Date
            r = ts.SetTaskDueDate("tasktests2@gmail.com", "boardName", 0, DateTime.Parse("2/2/2222"));
            response = JsonSerializer.Deserialize<Response>(r);
            result = (response.ErrorMessage == null) ? "Error: a user who is not the asignee changed the task's due date" : "Test passed: " + response.ErrorMessage;
            Console.WriteLine(result);

            // Advance Task
            r = ts.AdvanceTask("tasktests2@gmail.com", "boardName", 0);
            response = JsonSerializer.Deserialize<Response>(r);
            result = (response.ErrorMessage == null) ? "Error: a user who is not the asignee advanced the task" : "Test passed: " + response.ErrorMessage;
            Console.WriteLine(result);

            //Test that proper use of set works
            ts.AssignTask("tasktests1@gmail.com", "boardName", 0, "tasktests1@gmail.com");

            //Set new title
            string resJson1 = ts.SetTaskTitle("tasktests1@gmail.com", "boardName", 0, "newName");
            Response? response1 = JsonSerializer.Deserialize<Response>(resJson1);
            if (response1 != null && response1.ErrorMessage != null)
            {
                Console.WriteLine("Error: " + response1.ErrorMessage);
            }

            else
            {
                Console.WriteLine("Test Passed: name has been changed");
            }
            //set new description
            string resJson2 = ts.SetTaskDescription("tasktests1@gmail.com", "boardName", 0, "newDescription");
            Response? response2 = JsonSerializer.Deserialize<Response>(resJson2);
            if (response2 != null && response2.ErrorMessage != null)
            {
                Console.WriteLine("Error: " + response2.ErrorMessage);
            }

            else
            {
                Console.WriteLine("Test Passed: Description has been changed");
            }

            //set dueDate
            date = new DateTime(2050, 1, 13);
            string resJson3 = ts.SetTaskDueDate("tasktests1@gmail.com", "boardName", 0, date);
            Response? response3 = JsonSerializer.Deserialize<Response>(resJson3);
            if (response3 != null && response3.ErrorMessage != null)
            {
                Console.WriteLine("Error: " + response3.ErrorMessage);
            }

            else
            {
                Console.WriteLine("Test Passed: DueDate has been changed");
            }
        }

        public void assignTest()
        {
            string r, result;

            // Generate task for testing
            ts.AddTask("tasktests1@gmail.com", "boardName", "testTask", "desc", new DateTime(2050, 1, 13));//0

            //test assignment by non existing user
            r = ts.AssignTask("nonExisting@gmail.com", "boardName", 0, "tasktests1@gmail.com");
            response = JsonSerializer.Deserialize<Response>(r);
            result = (response.ErrorMessage == null) ? "Error: non existing was assigned a task" : "Test passed: " + response.ErrorMessage;
            Console.WriteLine(result);

            //test assignment by non board member user
            r = ts.AssignTask("tasktests2@gmail.com", "boardName", 0, "tasktests1@gmail.com");
            response = JsonSerializer.Deserialize<Response>(r);
            result = (response.ErrorMessage == null) ? "Error: task was assigned by non board member" : "Test passed: " + response.ErrorMessage;
            Console.WriteLine(result);

            //test assignment of non existing user
            r = ts.AssignTask("tasktests1@gmail.com", "boardName", 0, "nonExisting@gmail.com");
            response = JsonSerializer.Deserialize<Response>(r);
            result = (response.ErrorMessage == null) ? "Error: non existing user was assigned a task" : "Test passed: " + response.ErrorMessage;
            Console.WriteLine(result);

            //test assignment of non board member user
            r = ts.AssignTask("tasktests1@gmail.com", "boardName", 0, "tasktests2@gmail.com");
            response = JsonSerializer.Deserialize<Response>(r);
            result = (response.ErrorMessage == null) ? "Error: non board member user was assigned a task" : "Test passed: " + response.ErrorMessage;
            Console.WriteLine(result);

            //Join another user to the board
            bs.JoinBoard("tasktests2@gmail.com", 0);
            
            //test assignment of non existing task
            r = ts.AssignTask("tasktests1@gmail.com", "boardName", -15, "tasktests2@gmail.com");
            response = JsonSerializer.Deserialize<Response>(r);
            result = (response.ErrorMessage == null) ? "Error: user was assigned a non existing task" : "Test passed: " + response.ErrorMessage;
            Console.WriteLine(result);

            //test proper assignment of unassigned task
            r = ts.AssignTask("tasktests1@gmail.com", "boardName", 0, "tasktests1@gmail.com");
            response = JsonSerializer.Deserialize<Response>(r);
            result = (response != null && response.ErrorMessage != null) ? "Error: assignment of unassigned task failed. " + response.ErrorMessage : "Test passed: unassigned task successfuly assigned";
            Console.WriteLine(result);

            //test reassignment of assigned task not by asignee
            r = ts.AssignTask("tasktests1@gmail.com", "boardName", 0, "tasktests1@gmail.com");
            response = JsonSerializer.Deserialize<Response>(r);
            result = (response.ErrorMessage == null) ? "Error: assigned task was reassigned not by asignee" : "Test passed: " + response.ErrorMessage;
            Console.WriteLine(result);

            //test proper reassignment of assigned task
            r = ts.AssignTask("tasktests1@gmail.com", "boardName", 0, "tasktests2@gmail.com");
            response = JsonSerializer.Deserialize<Response>(r);
            result = (response != null && response.ErrorMessage != null) ? "Error: reassignment of assigned task failed. " + response.ErrorMessage : "Test passed: assigned task successfuly reassigned";
            Console.WriteLine(result);
        } 

        public void runTaskServiceTest()
        {
            Console.WriteLine("\n\n@@@@@@@@@@@@@@@@@@@@@@@@@\n Starting Task Service Tests \n@@@@@@@@@@@@@@@@@@@@@@@@@");
            us.Register("tasktests1@gmail.com", "P1assword");
            us.Register("tasktests2@gmail.com", "P1assword");
            bs.CreateBoard("tasktests1@gmail.com", "boardName");

            //Console.WriteLine("*** Adding Task Tests ***");
            //addTaskTests();
            //Console.WriteLine("*** Editing Task Tests ***");
            //settersTests();
            Console.WriteLine("*** Assigning Task Tests ***");
            assignTest();
        }
    }
}

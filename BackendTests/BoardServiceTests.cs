using System;
using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;
using System.Windows.Markup;
using System.Runtime.ExceptionServices;
using System.Text.Json.Serialization;

namespace BackendTests
{
    public class BoardServiceTests
    {
        BoardService bs;
        UserService uf;
        Response? response;

        public BoardServiceTests(BoardService bs , UserService uf) 
        { 
            this.bs = bs;
            this.uf = uf;
            response = new Response();
        }


        
        public void createBoardTests(){

            string resJson11 = uf.Register("username@gmail.com", "P1assword");
            string resJson1 = bs.CreateBoard("username@gmail.com", "boardname");
            
            response = JsonSerializer.Deserialize<Response>(resJson1);
            if (response != null && response.ErrorMessage != null)
            {
                Console.WriteLine("Error:" + response.ErrorMessage);
            }
            else
            {
                Console.WriteLine("Test Passed: Board created");
            }

            string resJson2 = bs.CreateBoard("", "boardName");
            response = JsonSerializer.Deserialize<Response>(resJson2);
            if(response != null && response.ErrorMessage != null)
            {
                Console.WriteLine("Test Passed: Error that should be thrown: " + response.ErrorMessage);
            }
            else
            {
                Console.WriteLine("Error: Malformed board has been created, without a username");
            }

            string resJson3 = bs.CreateBoard("username", "");
            response = JsonSerializer.Deserialize<Response>(resJson3);
            if (response != null && response.ErrorMessage != null)
            {
                Console.WriteLine("Test Passed: Error that should be thrown: " + response.ErrorMessage);
            }
            else
            {
                Console.WriteLine("Error: Malformed board has been created, without a boardname");
            }

            string resJson4 = bs.CreateBoard("username", "boardname");
            response = JsonSerializer.Deserialize<Response>(resJson4);
            if (response != null && response.ErrorMessage != null)
            {
                Console.WriteLine("Test Passed: Error that should be thrown:  " + response.ErrorMessage);
            }
            else if (response != null)
            {
                Console.WriteLine("Error: Malformed board has been created, key already exist");
            }


        }
        

        public void deleteBoardTests()
        {
            // first, create a board to delete
            string resultJson11 = uf.Register("newname@gmail.com", "P1assword");
            string resultJson1 = bs.CreateBoard("newname@gmail.com", "newboard");


            //try to delete it
            string resJson1 = bs.DeleteBoard("newname@gmail.com", "newboard");
            response = JsonSerializer.Deserialize<Response>(resJson1);
            if (response != null && response.ErrorMessage != null)
            {
                Console.WriteLine("Error:" + response.ErrorMessage);
            }
            else
            {
                Console.WriteLine("Test Passed: Board deleted");
            }

            //try to delete it again, should be error
            string resJson11 = bs.DeleteBoard("newname@gmail.com", "newboard");
            response = JsonSerializer.Deserialize<Response>(resJson11);
            if (response != null && response.ErrorMessage != null)    
            {
                Console.WriteLine("Test Passed: Error that should be thrown:  " + response.ErrorMessage);
            }
            else
            {
                Console.WriteLine("Error: an error should be thorown, no such board");
            }

            //try to delete a board that never been in the boardfacade.
            string resJson2 = bs.DeleteBoard("noSuchName", "noSuchBoard");
            response = JsonSerializer.Deserialize<Response>(resJson2);
            if (response != null && response.ErrorMessage != null)
            {
                Console.WriteLine("Test Passed: Error that should be thrown:  " + response.ErrorMessage);
            }
            else
            {
                Console.WriteLine("Error: an error should be thorown, no such board");
            }

            // try to delete unowned board
            // Create 2 users and one board
            string email1 = "test1@test.com";
            string email2 = "test2@test.com";
            string boardName = "testBoard";

            uf.Register(email1, "Password1");
            uf.Register(email2, "Password1");
            bs.CreateBoard(email1, boardName);

            //join user2 to board 1
            bs.JoinBoard(email1, 0);

            //User2 trying to delete a board that hes not owning (Expecting error)
            string r = bs.DeleteBoard(email2, boardName);
            response = JsonSerializer.Deserialize<Response>(r);
            if (response != null && response.ErrorMessage != null)
            {
                Console.WriteLine("Test passed: should have been thrown: " + response.ErrorMessage);
            }
            else
            {
                Console.WriteLine("Error: Board deleted not by the owner");
            }

        }

        public void leaveBoardTests()
        {
            //create 2 users and one board
            string email1 = "test3@test.com";//user1
            string email2 = "test4@test.com";//user2
            string boardName = "testBoard";

            uf.Register(email1, "Password1");
            uf.Register(email2, "Password1");
            bs.CreateBoard(email1, boardName);

            //user1 trying to leave a board that he owns (expect error)
            string r = bs.LeaveBoard(email1, 0);
            response = JsonSerializer.Deserialize<Response>(r);
            if (response != null && response.ErrorMessage != null)
            {
                Console.WriteLine("Test passed: should have been thrown: " + response.ErrorMessage);
            }
            else
            {
                Console.WriteLine("Error: An owner has left its board");
            }

            //user2 joins the board
            bs.JoinBoard(email2 , 0);

            //user2 trying to leave a board that he's not owning
            r = bs.LeaveBoard(email2, 0);
            response = JsonSerializer.Deserialize<Response>(r);
            if (response != null && response.ErrorMessage == null)
            {
                Console.WriteLine("Test passed: A guest user left some board");
            }
            else
            {
                Console.WriteLine("Error: Board deleted not by the owner");
            }
        }

        public void passOwnershipTest()
        {
            string email1 = "test1@test.com";   //user1
            string email2 = "test2@test.com";   //user2
            string boardName = "testBoard";
            string r;

            //Register a user and create a board
            uf.Register(email1, "Password1");
            bs.CreateBoard("test1@test.com", boardName);

            //Test that ownership of a board cannot be passed to an unexisting user
            r = bs.TransferOwnership(email1, boardName, email2);
            response = JsonSerializer.Deserialize<Response>(r);
            if (response != null && response.ErrorMessage == null)
            {
                Console.WriteLine("Error: Board ownership passed to unexisting user");
            }
            else
            {
                Console.WriteLine("Test passed: " + response.ErrorMessage);
            }

            //Test that ownership of a board cannot be passed to an unexisting user
            r = bs.TransferOwnership(email2, boardName, email1);
            response = JsonSerializer.Deserialize<Response>(r);
            if (response != null && response.ErrorMessage == null)
            {
                Console.WriteLine("Error: Board ownership passed from unexisting user");
            }
            else
            {
                Console.WriteLine("Test passed: " + response.ErrorMessage);
            }

            //Register another user
            uf.Register(email2 , "Password2");

            //Test that ownership of an unexisting board cannot be passed
            r = bs.TransferOwnership(email2, "unexistingBoard", email1);
            response = JsonSerializer.Deserialize<Response>(r);
            if (response != null && response.ErrorMessage == null)
            {
                Console.WriteLine("Error: Unexisting board ownership passed");
            }
            else
            {
                Console.WriteLine("Test passed: " + response.ErrorMessage);
            }

            //join user2 to the board
            bs.JoinBoard(email2, 0);

            //Test board ownership cannot be passed by someone who is not the owner
            r = bs.TransferOwnership(email2, boardName, email1);
            response = JsonSerializer.Deserialize<Response>(r);
            if (response != null && response.ErrorMessage == null)
            {
                Console.WriteLine("Error: board ownership passed by not-owner");
            }
            else
            {
                Console.WriteLine("Test passed: " + response.ErrorMessage);
            }

            //Test legal board ownership pass
            r = bs.TransferOwnership(email1, boardName, email2);
            response = JsonSerializer.Deserialize<Response>(r);
            if (response != null && response.ErrorMessage != null)
            {
                Console.WriteLine("Error: board ownership pass failed " + response.ErrorMessage);
            }
            else
            {
                Console.WriteLine("Test passed: board ownership passed successfuly");
            }
        }



        public void runBoardTests()
        {
            Console.WriteLine("\n\n@@@@@@@@@@@@@@@@@@@@@@@@@\n Starting Board Service Tests \n@@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine("*** Create Tests ***");
            createBoardTests();
            Console.WriteLine("*** Delete Tests ***");
            deleteBoardTests();
            Console.WriteLine("*** Leave Tests ***");
            leaveBoardTests();
        }


    }
    

}


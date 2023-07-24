using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace BackendTests
{
    public class UserServiceTests
    {
        UserService us;
        public UserServiceTests(UserService us) { this.us = us; }

        public void registerTest(string mail, string pass)
        {
            string resJson1 = us.Register(mail, pass);
            Response? response1 = JsonSerializer.Deserialize<Response>(resJson1);
            if (response1 != null && response1.ErrorMessage != null)
            {
                Console.WriteLine("Error: "+ response1.ErrorMessage);
            }
            else 
            {
                Console.WriteLine("registered succesfully!");
            }
        }

        public void loginTest(string mail,string pass)
        {
            string resJson1 = us.Login(mail, pass);
            Response? response1 = JsonSerializer.Deserialize<Response>(resJson1);
            if (response1 != null && response1.ErrorMessage != null)
            {
                Console.WriteLine("Error: " + response1.ErrorMessage);
            }
            else 
            {
                Console.WriteLine("logged in succesfully!");
            }
        }

        public void logoutTest(string mail) 
        { 
            string resJson1 = us.Logout(mail);
            Response? response1 = JsonSerializer.Deserialize<Response>(resJson1);
            if (response1 != null && response1.ErrorMessage != null)
            {
                Console.WriteLine("Error: " + response1.ErrorMessage);
            }
            else {
                Console.WriteLine("logged out succesfully!");
            }
        }

        public void runUserTests()
        {
            Console.WriteLine("\n\n@@@@@@@@@@@@@@@@@@@@@@@@@\n Starting User Service Tests\n@@@@@@@@@@@@@@@@@@@@@@@@@");
            string email = "talmalma6@gmail.com";
            string password = "P1assword";

            registerTest(email, password);
            logoutTest(email);
            loginTest(email, password);
            Console.WriteLine("***************************Test that should fail:*******************************");
            loginTest("", password);
            logoutTest(email);
            Console.WriteLine("*******************************Test that should fail::*******************************");
            logoutTest(email);

        }
    }
}

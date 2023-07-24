using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class UserService
	{
        private static readonly ILog log = Logger.GetLogger(typeof(UserService));
        private UserFacade uf;

		public UserService(UserFacade uf)
		{
            this.uf = uf;
		}

        /// <summary>
        /// Register a new user to the system.
        /// </summary>
        /// <param name="email">The new user's email.</param>
        /// <param name="password">The new user's password.</param>
        public string Register(string email, string password)
        {
            Response response;
            try
            {
                uf.Register(email.ToLower(), password);
                log.Info($"User registered successfully: {email}");
                Login(email, password);
                response = new Response();
            }
            catch (Exception e)
            {
                response = new Response(e.Message);
                log.Error("Error while trying to register user with email: " + email + "\n" + e.Message);
            }
            return response.ToJson();
        }

        /// <summary>
        /// Log in a user to the system.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="password">The user's password.</param>
        public string Login(string email, string password)
        {
            Response response;
            try
            {
                log.Info("Loggin in user: " + email);
                uf.Login(email.ToLower(), password);
                response = new Response(null, email);

                
                
                log.Info("User with email: " + email + " logged in successfully");
            }
            catch (Exception e)
            {
                response = new Response(e.Message);
                string str = response.ToJson();
                Response response2 = JsonSerializer.Deserialize<Response>(str);
                log.Error("Error on login for user with email: " + email + "\n" + e.Message);
            }
            return response.ToJson();
        }

        /// <summary>
        /// Log out a user from the system.
        /// </summary>
        /// <param name="email">Email of the user.</param>
        public string Logout(string email)
        {
            Response response;
            try
            {
                log.Info("User logging out: " + email);
                uf.Logout(email.ToLower());
                response = new Response();
                log.Info("User with email: " + email + " logged out successfully");
            }
            catch (Exception e)
            {
                response = new Response(e.Message);
                log.Error("Error on logout for user with email: " + email + "\n" + e.Message);
            }
            return response.ToJson();
        }

        /// <summary>
        /// Get a user object by its email.
        /// </summary>
        /// <param name="email">The user's email.</param>
        public string GetInProgressTasks(string email)
        {
            Response response;
            List<Task> tasks;
            try
            {
                tasks = uf.GetInProgressTasks(email);
                response = new Response(null, tasks);
                log.Info("Retrieved all boards of user " + email);
            }
            catch (Exception e)
            {
                response = new Response(e.Message);
                log.Error("Error on retrieving boards of user " + email + "\n" + e.Message);
            }
            return response.ToJson();
        }

        /// <summary>
        /// Loads all users from the database.
        /// </summary>
        /// <param name="response">A response object to output response to.</param>
        /// <returns>A JSON string representing the response object.</returns>
        public string LoadUsers(ref Response response)
        {
            if (response.ErrorMessage != null)
                return response.ToJson();

            try
            {
                uf.LoadUsersFromDB();
                response = new Response();
                log.Info("Users loaded from database successfuly");
            }
            catch (Exception e)
            {
                response = new Response(e.Message);
                log.Error("Error while trying to load users from database:\n" + e.Message);
            }
            return response.ToJson();
        }

        /// <summary>
        /// Clears all users from the database.
        /// </summary>
        /// <param name="response">A response object to output response to</param>
        public string ClearAllUsers(ref Response response)
        {
            if(response.ErrorMessage != null)
                return response.ToJson();

            try
            {
                uf.ClearAllUsers();
                response = new Response();
                log.Info("Users database has been cleared");
            }
            catch (Exception e)
            {
                response = new Response(e.Message);
                log.Error("Error while trying to clear users database");
            }
            return response.ToJson();
        }
    }
}

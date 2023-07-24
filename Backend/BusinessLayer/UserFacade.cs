using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class UserFacade
    {
        private Dictionary<string, User> users;
        private UserController userController;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UserFacade()
        {
            users = new Dictionary<string, User>();
            userController = new UserController();
        }

        /// <summary>
        /// Register a new user to the system.
        /// </summary>
        /// <param name="email">The new user's email.</param>
        /// <param name="password">The new user's password.</param>
        /// <returns>The new user's object.</returns>
        public User Register(string email, string password)
        {
            if (!IsValidEmail(email))
                throw new Exception("Invalid email");
            if (!IsValidPassword(password))
                throw new Exception("Illegal password");
            if (users.ContainsKey(email))
                throw new Exception("This email is already registered");

            User user = new User(email, password);
            users.Add(email, user);
            userController.Insert(user.DTO);

            return user;
        }

        /// <summary>
        /// Log in a user to the system.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="password">The user's password.</param>
        public void Login(string email, string password)
        {
            if (!users.ContainsKey(email))
                throw new Exception("Email not registered");

            User user = users[email];
            user.Login(password);

            log.Info($"User logged in successfully: {email}");
        }

        /// <summary>
        /// Log out a user from the system.
        /// </summary>
        /// <param name="user">The user object to log out.</param>
        public void Logout(string email)
        {
            User user = GetUser(email);

            if (!user.LoggedIn)
                throw new Exception("User is not logged in");

            user.Logout();

            log.Info($"User logged out successfully: {user.Email}");
        }

        /// <summary>
        /// Get a user object by its email.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <returns>The user object if it exists.</returns>
        public User GetUser(string email)
        {
            if (users.ContainsKey(email))
            {
                User user = users[email];
                log.Info($"User retrieved successfully: {email}");
                return user;
            }
            else
            {
                throw new Exception($"Cannot find user: {email}");
            }
        }

        /// <summary>
        /// Gets a user's 'in progress' tasks.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <returns>A list of tasks, containing all this user's 'in progress' tasks.</returns>
        public List<Task> GetInProgressTasks(string email)
        {
            User user = GetUser(email);

            if (user.LoggedIn)
            {
                List<Task> tasks = user.GetInProgressTasks();

                log.Info($"'In Progress' tasks retrieved successfully for user: {email}");
                return tasks;
            }
            else
            {
                throw new Exception("User is not logged in");
            }
        }

        // Checks if a given string is a valid password.
        internal bool IsValidPassword(string password) =>
            password.Length >= 6 && password.Length <= 20 && password.Any(char.IsDigit)
            && password.Any(char.IsUpper) && password.Any(char.IsLower);

        // Checks if a given string is a valid email.
        internal bool IsValidEmail(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }

        /// <summary>
        /// Loads all users from the database.
        /// </summary>
        /// <returns>Dictionary of all users.</returns>
        public Dictionary<string, User> LoadUsersFromDB()
        {
            log.Info("Loading users from the database...");

            foreach (UserDTO dto in userController.GetAllFromDB())
            {
                users.Add(dto.Email, new User(dto));
            }

            log.Info("Users loaded from the database successfully");
            return users;
        }

        /// <summary>
        /// Clears all users from the database.
        /// </summary>
        public void ClearAllUsers()
        {
            log.Info("Clearing all users from the database...");

            userController.ClearData();

            log.Info("All users cleared from the database successfully");
        }
    }
}

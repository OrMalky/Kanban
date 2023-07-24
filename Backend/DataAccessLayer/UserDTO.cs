using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntroSE.Kanban.Backend.BusinessLayer;


namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class UserDTO : IDTO
    {
        //Members
        private string email;
        private string password;

        //Properties
        public string Email { get => email; }
        public string Password { get => password; }

        public UserDTO() { }

        public UserDTO(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
    }
}

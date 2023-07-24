using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FrontEnd.Model
{
    public class UserModel : NotifableModelObject
    {
        private string email;
        private ObservableCollection<BoardModel> boardsOfUser;
        

        public string Email
        {
            get => email; 
        }

        public ObservableCollection<BoardModel> BoardsOfUser
        {
            get => boardsOfUser;
        }



        public UserModel(BackendController controller, string email) : base(controller)
        {
            this.email = email;
            boardsOfUser = controller.GetUserBoards(email);


        }

        public override string ToString()
        {
            return Email; // Change this to whatever property you want to display
        }
    }
}

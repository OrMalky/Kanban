using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrontEnd;
using FrontEnd.Model;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace FrontEnd.ViewModel
{
    public class BoardListViewModel :NotifableObject
    {
        private ObservableCollection<BoardModel> boards;
        private UserModel user;

        public BackendController controller;
        private string massageText;
        public string MassageText
        {
            get => massageText;
            set
            {
                massageText = value; RaisePropertyChanged(nameof(MassageText)); }
            }

        public BoardListViewModel(UserModel user, BackendController controller)
        {
            this.controller = controller;
            this.boards = user.BoardsOfUser;
            this.user = user;
        }
        public ObservableCollection<BoardModel> Boards
        {
            get => boards;
            set
            {
                boards = value;
                RaisePropertyChanged(nameof(boards));
            }
        }

        public UserModel User
        {
            get => user;
            set
            {
                user = value;
                RaisePropertyChanged(nameof(User));
            }
        }

        public void LogOut()
        {
            controller.Logout(User.Email);
        }
        public BoardListViewModel(UserModel u)
        {
            this.user = u;
            Boards = user.BoardsOfUser;
            //should happen in BoardListModel
        }

        public BoardModel selectBoard(int id)
        {
            return null;
        }



    }
}

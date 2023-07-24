using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class Services
    {
        public BoardService bs;
        public TaskService ts;
        public UserService us;
        public Services(UserFacade uf, BoardFacade bf)  
        {
            bs = new BoardService(bf, uf);
            ts = new TaskService(bf, uf);
            us = new UserService(uf);
            Response response = new Response();

            us.LoadUsers(ref response);
            bs.LoadBoards(ref response);
            ts.LoadTasks(ref response);
        }


    }
}

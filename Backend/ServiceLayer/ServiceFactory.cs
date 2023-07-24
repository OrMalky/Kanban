using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class ServiceFactory
    {
        public static Services Create()
        {
            UserFacade uf = new UserFacade();
            BoardFacade bf = new BoardFacade(uf);
            return (new Services(uf, bf));
        }
    }
}

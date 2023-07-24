using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontEnd.Model
{
    public class NotifableModelObject : NotifableObject
    {
        public BackendController Controller { get; private set; }

        protected NotifableModelObject(BackendController controller)
        {
            this.Controller = controller;
        }
    }
}

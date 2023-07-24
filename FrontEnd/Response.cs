using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Diagnostics.Metrics;
using System.Linq;

namespace FrontEnd
{
    public class Response
    {
        public string ErrorMessage { get; set; }
        public object ReturnValue { get; set; }

        public Response()
        {
            ErrorMessage = null;
            ReturnValue = null;
        }

        public Response(string ErrorMsg)
        {
            ErrorMessage = ErrorMsg;
            ReturnValue = null;
        }

        public Response(string ErrorMsg, object obj)
        {
            ErrorMessage = ErrorMsg;
            ReturnValue = obj;
        }
    }
}

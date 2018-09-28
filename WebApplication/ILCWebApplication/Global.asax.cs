using System;
using System.Web;
using ILCWebApplication.HandleException;

namespace ILCWebApplication
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            ExceptionLogger logger = new ExceptionLogger();
            Exception ex = Server.GetLastError().GetBaseException();
            logger.HandleException(ex);

            if (ex is HttpRequestValidationException)
            {
                string dir = VirtualPathUtility.GetDirectory(Request.AppRelativeCurrentExecutionFilePath);
                string errorPage = "~/CustomErrors/DefFrameError.aspx";
                if (dir == "~/")
                {
                    errorPage = "~/CustomErrors/DefError.aspx";
                }
                
                Server.Transfer(errorPage, false);              
            }
        }
    }
}

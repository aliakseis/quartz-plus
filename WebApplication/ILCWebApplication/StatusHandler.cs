using System;
using System.ServiceProcess;
using System.Web;
using ILCWebApplication.ILCSettings;

namespace ILCWebApplication
{
    public class StatusHandler : IHttpHandler
    {
        #region Implementation of IHttpHandler

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"></see> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests. </param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Write(GetServiceInfo());
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"></see> instance.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Web.IHttpHandler"></see> instance is reusable; otherwise, false.
        /// </returns>
        public bool IsReusable
        {
            get { return true; }
        }

        #endregion

        private static string GetServiceInfo()
        {
            IlcStatusData data = IlcWebDao.GetStatusData();

            string status = GetServiceStatus();
            if (status != ConstExpressions.SERVICE_STATUS_STARTED)
                data.currServiceState = ConstExpressions.WORK_STATUS_IDLE;

            return string.Format("{0},{1},{2},{3}",
                                 status, data.lastCheckDate, data.currServiceState, DateTime.Now);
        }

        /// <summary>
        /// Gets service status
        /// </summary>
        /// <returns>service status string</returns>
        public static string GetServiceStatus()
        {
            try
            {
                ServiceController serviceController = new ServiceController(WebSettings.GetServiceName(),
                                                                            WebSettings.GetServiceLocation());
                switch (serviceController.Status)
                {
                    case ServiceControllerStatus.Running:
                        return ConstExpressions.SERVICE_STATUS_STARTED;
                    case ServiceControllerStatus.Stopped:
                        return ConstExpressions.SERVICE_STATUS_STOPPED;
                    default:
                        return ConstExpressions.SERVICE_STATUS_PAUSED;
                }
            }
            catch (Exception ex)
            {
                return ((ex.InnerException != null) ? ex.InnerException.Message + ". " : "") + ex.Message;
            }
        }
    }
}
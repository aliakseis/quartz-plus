using System;
using System.ServiceProcess;
using System.Web;
using ILCWebApplication.HandleException;
using ILCWebApplication.ILCSettings;
using Utils;

namespace ILCWebApplication
{
    public class ServiceControlHandler : IHttpHandler
    {
        #region Implementation of IHttpHandler

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"></see> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests. </param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            int btnId = Convert.ToInt32(context.Request.Params["btn"]);
            context.Response.Write(ProcessButtonClick(btnId));
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

        private static string ProcessButtonClick(int btnId)
        {
            switch (btnId)
            {
                case 1: 
                    StartService();
                    break;
                case 2:
                    StopService();
                    break;
                case 3:
                    QuickCheck();
                    break;
            }
            return "";
        }

        private static void StartService()
        {
            try
            {
                ServiceController serviceController = new ServiceController(WebSettings.GetServiceName(),
                                                                            WebSettings.GetServiceLocation());

                if (serviceController.Status == ServiceControllerStatus.Stopped)
                {
                    serviceController.Start();
                }
            }
            catch (Exception ex)
            {
                new ExceptionLogger().HandleException(ex);
            }
        }

        private static void StopService()
        {
            try
            {
                ServiceController serviceController = new ServiceController(WebSettings.GetServiceName(),
                                                                            WebSettings.GetServiceLocation());

                if (serviceController.Status == ServiceControllerStatus.Running)
                {
                    if (serviceController.CanStop)
                    {
                        serviceController.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                new ExceptionLogger().HandleException(ex);
            }
        }

        private static void QuickCheck()
        {
            try
            {
                ServiceController serviceController = new ServiceController(WebSettings.GetServiceName(),
                                                            WebSettings.GetServiceLocation());

                serviceController.ExecuteCommand((int)ServiceCustomCommand.StartCheck);
            }
            catch (Exception ex)
            {
                new ExceptionLogger().HandleException(ex);
            }
        }
        
        /// <summary>
        /// returns initial text for the start/stop button
        /// </summary>
        /// <returns>initial text</returns>
        public static string GetInitialStartStopText()
        {
            string status = StatusHandler.GetServiceStatus();
            return status == ConstExpressions.SERVICE_STATUS_STARTED ? ConstExpressions.STOP_BUTTON_TEXT : ConstExpressions.START_BUTTON_TEXT;
        }

        /// <summary>
        /// returns initial Start/Stop button disabled status
        /// </summary>
        /// <returns>disabled or not</returns>
        public static string GetInitialStartStopStatus()
        {
            string status = StatusHandler.GetServiceStatus();

            if (status == ConstExpressions.SERVICE_STATUS_STARTED || status == ConstExpressions.SERVICE_STATUS_STOPPED)
                return "";
            return "Disabled";
        }

        /// <summary>
        /// returns initial QuickCheck button disabled status
        /// </summary>
        /// <returns>disabled or not</returns>
        public static string GetInitialQuickCheckStatus()
        {
            string status = StatusHandler.GetServiceStatus();
            if (status != ConstExpressions.SERVICE_STATUS_STARTED)
                return "Disabled";

            IlcStatusData data = IlcWebDao.GetStatusData();
            return data.currServiceState == ConstExpressions.WORK_STATUS_IDLE ? "" : "Disabled";
        }
    }
}

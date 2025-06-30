using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WorkMate
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            try
            {
                AreaRegistration.RegisterAllAreas();
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                //BundleConfig.RegisterBundles(BundleTable.Bundles);
            }
            catch (Exception ex)
            {
                LogError("Application_Start Error: " + ex.ToString());
                throw ex;
            }

        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if (exception != null)
            {
                // Log the error
                LogError("Application_Error: " + exception.ToString());

                // Get HTTP status code
                int httpCode = 500;
                if (exception is HttpException httpEx)
                {
                    httpCode = httpEx.GetHttpCode();
                }

                // Clear the error
                Server.ClearError();

                // Redirect based on error type
                Response.Clear();

                switch (httpCode)
                {
                    case 404:
                        Response.Redirect("~/Error/NotFound");
                        break;
                    case 403:
                        Response.Redirect("~/Error/Forbidden");
                        break;
                    default:
                        Response.Redirect("~/Error/Index");
                        break;
                }
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                LogError("Session_Start Error: " + ex.ToString());
            }


        }

        protected void Session_End(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                LogError("Session_End Error: " + ex.ToString());
            }
        }

        private void LogError(string message)
        {
            try
            {
                string logPath = Server.MapPath("~/App_Data/Logs/");

               
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }

                string fileName = "ErrorLog_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                string fullPath = Path.Combine(logPath, fileName);

                string logEntry = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + message + Environment.NewLine;

                File.AppendAllText(fullPath, logEntry);
            }
            catch
            {
                
                try
                {
                    System.Diagnostics.EventLog.WriteEntry("WorkMate Application", message,
                        System.Diagnostics.EventLogEntryType.Error);
                }
                catch(Exception ex)
                {
                    throw ex;
                    
                }
            }
        }
    }
}

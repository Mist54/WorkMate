using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Serilog;

namespace WorkMate
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Server.MapPath("~/App_Data/Logs/log-.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                AreaRegistration.RegisterAllAreas();
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Application_Start Error"); // Use Serilog for logging
                throw ex;
            }

        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if (exception != null)
            {
                // Log the error
                Log.Error(exception, "Application_Error"); // Use Serilog for logging

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
                        Response.Redirect("~/Error/AccessDenied");
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
                Log.Error(ex, "Session_Start Error"); // Use Serilog for logging
            }


        }

        protected void Session_End(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Session_End Error"); // Use Serilog for logging
            }
        }

        protected void Application_End()
        {
            Log.CloseAndFlush(); // Close and flush Serilog logger
        }
    }
}

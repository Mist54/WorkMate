using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WorkMate.Models;
using WorkMate.ViewModels;

namespace WorkMate.Controllers
{
    [Authorize]
    public class TimeTrackingController : Controller
    {
        // GET: TimeTracking
        public ActionResult Index()
        {
           return View();
        }
        #region Toaster Methods

        private void SetSuccessToast(string message, string title)
        {
            TempData["ToastMessage"] = message;
            TempData["ToastTitle"] = title;
            TempData["ToastType"] = "success";
        }

        private void SetErrorToast(string message, string title)
        {
            TempData["ToastMessage"] = message;
            TempData["ToastTitle"] = title;
            TempData["ToastType"] = "error";
        }

        private void SetWarningToast(string message, string title)
        {
            TempData["ToastMessage"] = message;
            TempData["ToastTitle"] = title;
            TempData["ToastType"] = "warning";
        }

        private void SetInfoToast(string message, string title)
        {
            TempData["ToastMessage"] = message;
            TempData["ToastTitle"] = title;
            TempData["ToastType"] = "info";
        }

        #endregion
    }
}

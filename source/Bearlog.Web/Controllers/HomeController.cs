﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bearlog.Web.Services;

namespace Bearlog.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var users = new DbService().GetUsers();
            ViewData["users"] = users;
            return View();
        }
    }
}
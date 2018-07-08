﻿using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Bearlog.Web.Services;
using GenTrees;

namespace Bearlog.Web.Controllers
{
    public class Tools
    {
        public static string GetConnectionString(string name)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[name]
                .ConnectionString;
            connectionString += "User ID=u0351346_striderajr;Password=ps^9wL31";
            return connectionString;
        }
    }

    /// <summary>
    /// Контроллер для другого моего проекта GenTrees
    /// Из-за сертификата SSL, который не распостраняется на поддомены,
    /// сервис будт жить здесь. Плак-плак
    /// </summary>
    public class GenTreesController : Controller
    {
        public JsonResult Index()
        {
            return Json(new {Message = "GenTrees service v0.0.2"}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TestMessage()
        {
            return Json(new {Message = "Greetings from ASP.NET RESTful service ^_^"}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TestDb()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["GenTreesDb"].ConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(Tools.GetConnectionString("GenTreesDb")))
                {
                    conn.Open();
                    return Json(new { Message = connectionString + "      =>     " + "Ok!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new {Message = connectionString + "      =>     " + "Fail :("}, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Register(string userName, string password, string email)
        {
            try
            {
                using (var db = new GenTreesContext(Tools.GetConnectionString("GenTreesDb")))
                {
                    if (db.Users.Any(x => x.UserName == userName))
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            message = $"UserName {userName} is already taken"
                        }, JsonRequestBehavior.AllowGet);
                    }

                    // TODO validate correct email

                    db.Users.Add(new User
                    {
                        UserName = userName,
                        Email = email,
                        Password = Hash.GetHashCode(password),
                        RegistrationDate = DateTime.Now,
                        LasActivityDate = null
                    });
                    db.SaveChanges();

                    return Json(new {isSuccess = true}, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Error in registration procedure",
                    details = e.Message,
                    stackTrace = e.StackTrace
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Authenticate(string userName, string password)
        {
            try
            {
                using (var db = new GenTreesContext(Tools.GetConnectionString("GenTreesDb")))
                {
                    if (db.Users.Any(x => x.UserName == userName))
                    {
                        var user = db.Users.First(x => x.UserName == userName);
                        if (user.Password == Hash.GetHashCode(password))
                        {
                            return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
                        }

                        return Json(new
                        {
                            isSuccess = false,
                            message = "Wrong password"
                        }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new
                    {
                        isSuccess = false,
                        message = $"User {userName} not found"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Error in authentication procedure",
                    details = e.Message,
                    stackTrace = e.StackTrace
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
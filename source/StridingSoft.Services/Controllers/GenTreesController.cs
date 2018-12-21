﻿using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Mvc;
using System.Linq;
using System.Resources;
using StridingSoft.Services.Services;

namespace StridingSoft.Services.Controllers
{
    public class Tools
    {
        public static string GetConnectionString(string name)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[name].ConnectionString;

            string dbUser = Resources.Secret.ResourceManager.GetString($"{name}_User");
            string dbPassword = Resources.Secret.ResourceManager.GetString($"{name}_Password");

            connectionString += $"User ID={dbUser};Password={dbPassword}";
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
                    if (db.Users.Any(x => x.user_name == userName))
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
                        user_name = userName,
                        email = email,
                        password = Hash.GetHashCode(password),
                        registration_date = DateTime.Now,
                        last_activity_date = null
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

        public bool UserExists(string userName)
        {
            try
            {
                using (var db = new GenTreesContext(Tools.GetConnectionString("GenTreesDb"))) {
                    var users = db.Users.ToList();
                    return db.Users.Any(x => x.user_name == userName);
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public JsonResult Authenticate(string userName, string password)
        {
            try
            {
                using (var db = new GenTreesContext(Tools.GetConnectionString("GenTreesDb")))
                {
                    if (db.Users.Any(x => x.user_name == userName))
                    {
                        var user = db.Users.First(x => x.user_name == userName);
                        if (user.password == Hash.GetHashCode(password))
                        {
                            return Json(new
                            {
                                isSuccess = true,
                                // this token will be required for all actions
                                token = Hash.GetHashCode(userName+Hash.GetHashCode(password)) 
                            }, JsonRequestBehavior.AllowGet);
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
using ELearning.Models;
using StuFeeWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace ELearning.Controllers
{
    public class LoginSignupController : Controller
    {
        // GET: LoginSignup
        DBAccess db = new DBAccess();
        public ActionResult Login()
        {
            ViewBag.HideHeader = true;
            return View();
        }
        [HttpPost]
        public ActionResult Login(Login user)
        {
            if(ModelState.IsValid)
            {
                db.OpenCon();
                if (user.UserRole == "Student")
                {             
                    string q = "Select * from student where s_email='" + user.Username + "' and s_password= '" + user.Password + "'";
                    db.cmd = new SqlCommand(q, db.con);
                    SqlDataReader sdr = db.cmd.ExecuteReader();
                    sdr.Read();
                    if (sdr.HasRows)
                    {
                        Session["sid"] = sdr[0].ToString();
                        Session["sname"] = sdr[1].ToString();
                        Session["semail"] = sdr[2].ToString();
                        Session["upassword"] = sdr[3].ToString();
                        Session["sinterest"] = sdr[4].ToString();
                        return RedirectToAction("Dashboard", "Student");
                    }
                    else
                    {
                        Response.Write("<script>alert('Invalid Student!');</script>");
                    }
                    sdr.Close();
                   

                }
                else if (user.UserRole == "Teacher")
                {
                    string q = "Select * from teacher where t_email='" + user.Username + "' and t_password= '" + user.Password + "'";
                    db.cmd = new SqlCommand(q, db.con);
                    SqlDataReader sdr = db.cmd.ExecuteReader();
                    sdr.Read();
                    if (sdr.HasRows)
                    {
                        Session["tid"] = sdr[0].ToString();
                        Session["tname"] = sdr[1].ToString();
                        Session["temail"] = sdr[3].ToString();
                        Session["tpassword"] = sdr[2].ToString();
                        Session["tq"] = sdr[4].ToString();
                        return RedirectToAction("Dashboard", "Teacher");
                    }
                    else
                    {
                        Response.Write("<script>alert('Invalid Teacher!');</script>");
                    }
                    sdr.Close();
                   
                }
                else if (user.UserRole == "Admin")
                {
                    string q = "Select * from admin where email='" + user.Username + "' and password= '" + user.Password + "'";
                    db.cmd = new SqlCommand(q, db.con);
                    SqlDataReader sdr = db.cmd.ExecuteReader();
                    sdr.Read();
                    if (sdr.HasRows)
                    {
                        Session["aid"] = sdr[0].ToString();
                        Session["aname"] = sdr[1].ToString();
                        Session["apassword"] = sdr[2].ToString();
                        Session["aemial"] = sdr[3].ToString();
                        return RedirectToAction("Dashboard", "Admin");

                    }
                    else
                    {
                        Response.Write("<script>alert('Invalid Admin!');</script>");
                    }
                    sdr.Close();
                  
                }
                db.CloseCon();
            }
            return View();
        }
       
        public ActionResult SignUp()
        {
            ViewBag.HideHeader = true;
            return View();
        }
        [HttpPost]
     
        public ActionResult SignUp(SignUpViewModel model)
        {
            
                db.OpenCon();
                if (model.UserRole == "Student")
                {
                    string q = "insert into Student values('" + model.sname + "','" + model.semail + "','" + model.spassword + "','" + model.sInterset + "','"+" "+"') ";
                    db.InsertUpdateDelete(q);
                    Response.Write("<script>alert('" + model.sname + " Registered!');</script>");
                }
                else if (model.UserRole == "Teacher")
                {

                    string q = "insert into teacher values('" + model.tname + "','" + model.tpassword + "','" + model.temail + "','" + model.tqualification + "') ";
                    db.InsertUpdateDelete(q);
                    Response.Write("<script>alert('" + model.tname + " Registered!');</script>");
                }
                db.CloseCon();

                return RedirectToAction("Login"); 
           
        }

    }
}
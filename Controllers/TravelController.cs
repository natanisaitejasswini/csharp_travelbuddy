using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravelApp.Factory;
using travelbuddy.Models;
using CryptoHelper;

namespace travelbuddy.Controllers
{
    public class TravelController : Controller
    {
         private readonly TravelRepository travelFactory;
          private readonly TripsRepository tripsFactory;
         public TravelController()
        {
            travelFactory = new TravelRepository();
            tripsFactory = new TripsRepository();
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            if(TempData["errors"] != null)
            {
               ViewBag.errors = TempData["errors"];
            }
            return View("Login");
        }
// Post Methods:: Login, Registration
        [HttpPost]
        [Route("registration")]
        public IActionResult Create(User newuser)
        {   
            Console.WriteLine("birthday is" + newuser.birthday);
            List<string> temp_errors = new List<string>();
            if(ModelState.IsValid)
            {
                 if(travelFactory.FindEmail(newuser.email) == null){ // Checking email is registered previously
                    if(newuser.birthday < DateTime.Today){ //checking date is in past or not
                    travelFactory.Add(newuser);
                    ViewBag.User_Extracting = travelFactory.FindByID();
                    int current_other_id = ViewBag.User_Extracting.id;
                    HttpContext.Session.SetInt32("current_id", (int) current_other_id);
                    return RedirectToAction("Dashboard");
                    }
                    else{
                        temp_errors.Add("Birthday date must be in past");
                        TempData["errors"] = temp_errors;
                        return RedirectToAction("Index");
                    }
                 }else{
                    temp_errors.Add("Email is already in use");
                    TempData["errors"] = temp_errors;
                    return RedirectToAction("Index");
                }
            }
            foreach(var error in ModelState.Values)
            {
                if(error.Errors.Count > 0)
                {
                    temp_errors.Add(error.Errors[0].ErrorMessage);
                }  
            }
            TempData["errors"] = temp_errors;
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login(string email, string password)
        {
            List<string> temp_errors = new List<string>();
            if(email == null || password == null)
            {
                temp_errors.Add("Enter Email and Password Fields to Login");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Index");
            }
//Login User Id Extracting query
          User check_user = travelFactory.FindEmail(email);
            if(check_user == null)
            {
                temp_errors.Add("Email is not registered");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Login");
            }
            bool correct = Crypto.VerifyHashedPassword((string) check_user.password, password);
            if(correct)
            {
                HttpContext.Session.SetInt32("current_id", check_user.id);
                return RedirectToAction("Dashboard");
            }
            else{
                temp_errors.Add("Password is not matching");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Login");
            }
        }
 //Dashboard start
        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            //on refresh once after logout
            if(HttpContext.Session.GetInt32("current_id") == null)
            {
                return RedirectToAction("Index");
            }
            //Dashboard begins
            ViewBag.User_one = travelFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            ViewBag.User_one_trips = tripsFactory.CurrentUserTrips((int)HttpContext.Session.GetInt32("current_id"));
            ViewBag.User_Except_trips = tripsFactory.ExceptCurrentUserTrips((int)HttpContext.Session.GetInt32("current_id"));
            return View("Dashboard");
        }
//New Trip adding page
        [HttpGet]
        [Route("new")]
        public IActionResult New()
        {
            //on refresh once after logout
            if(HttpContext.Session.GetInt32("current_id") == null)
            {
                return RedirectToAction("Index");
            }
            if(TempData["errors"] != null)
            {
               ViewBag.errors = TempData["errors"];
            }

            ViewBag.User_one = travelFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            return View("AddTravel");
        }
        [HttpPost]
        [Route("addtrip")]
         public IActionResult AddTrip(Trip newtrip)
        {
            List<string> temp_errors = new List<string>();
            if(ModelState.IsValid)
            {
                if(newtrip.travelstart > DateTime.Now && newtrip.travelend > newtrip.travelstart)
                {
                 tripsFactory.AddTrip(newtrip);
                 ViewBag.User_Extracting = tripsFactory.Trip_Last_ID();
                 tripsFactory.Add_Traveller(ViewBag.User_Extracting.id, (int)HttpContext.Session.GetInt32("current_id"));
                 Console.WriteLine("Trip is Successfully added");
                 return RedirectToAction("Dashboard");
                }
                else{
                    temp_errors.Add("Select Travel dates correctly");
                    TempData["errors"] = temp_errors;
                    return RedirectToAction("New");
                }
            }
            foreach(var error in ModelState.Values)
            {
                if(error.Errors.Count > 0)
                {
                    temp_errors.Add(error.Errors[0].ErrorMessage);
                }  
            }
            TempData["errors"] = temp_errors;
            return RedirectToAction("New");
        }
//Join Trip
        [HttpGet]
        [Route("join/{id}")]
        public IActionResult Admin_Join(string id = "")
        {
            ViewBag.User_one = travelFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            tripsFactory.Join_Trip(id,(int)HttpContext.Session.GetInt32("current_id"));
            return RedirectToAction("Dashboard");
        }
//Trip Show page
        [HttpGet]
        [Route("destination/{id}")]
        public IActionResult Show(string id = "")
        {
            ViewBag.Trip_Info = tripsFactory.destination_Info(id);
            ViewBag.Other_Users = tripsFactory.others(id);
            ViewBag.User_one = travelFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            return View("Show");
        }
 // Logout
        [HttpGet]
        [Route("logout")]
         public IActionResult Logout()
         {
             HttpContext.Session.Clear();
             return RedirectToAction("Index");

         }
    }
}

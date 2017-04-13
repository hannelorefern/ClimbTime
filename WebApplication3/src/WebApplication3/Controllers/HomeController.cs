using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using WebApplication3.App_Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        static List<Student> signedInStudents = new List<Student>();
        static int countForTesting = 1;
        // SqlConnection conn = new SqlConnection("Data Source=SQL5019.SmarterASP.NET;Initial Catalog=DB_A16A06_climb;User Id=DB_A16A06_climb_admin;Password=climbdev1;");
        DataAccesser db = new DataAccesser("Data Source=SQL5019.SmarterASP.NET;Initial Catalog=DB_A16A06_climb;User Id=DB_A16A06_climb_admin;Password=climbdev1;");

        public IActionResult Index()
        {
            //Here, fill signed in students from the signed in table in the database
            


            return View(signedInStudents);
        }

        public IActionResult Reports()
        {
            ViewData["Message"] = "Page for Reports";

            return View();
        }

        public IActionResult Settings()
        {
            ViewData["Message"] = "Page for Settings";

            return View();
        }

        public IActionResult Users()
        {
            ViewData["Message"] = "Page for Users";

            return View();
        }



        public IActionResult Error()
        {
            return View();
        }
        public IActionResult AddClimber(string lastNameToSearch, string firstNameToSearch)
        {
            Student toAdd = db.findUser(firstNameToSearch, lastNameToSearch);
            toAdd.time = DateTime.Now.ToString("MMM d, yyyy H:mm:ss");
            db.addVisit(toAdd);

            signedInStudents.Add(toAdd);


            return View("Index",signedInStudents);
        }

        public IActionResult RemoveClimbers()
        {
             
            string temp = this.Request.Form["signOutCheckBox"];

            string[] toRemoveIndex = temp.Split(','); 
            for (int i = toRemoveIndex.Length - 1; i>=0; i--)
            {
                int index = Int16.Parse(toRemoveIndex[i]);
                string firstNameToRemove = signedInStudents[index].firstName;
                string lastNameToRemove = signedInStudents[index].lastName;
                db.finishVisit(firstNameToRemove, lastNameToRemove);
                signedInStudents.Remove(signedInStudents[index]);

            }

            

            
            return View("Index", signedInStudents);
        }

        public IActionResult getCheckoutPage() {
            return View("unknownWireframe");
        }

        public void CheckoutShoes() {
            //not implemented
        }

        public void CheckoutHarness()
        {
            //not implemented
        }
    }

}

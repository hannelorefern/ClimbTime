using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        static List<Student> signedInStudents = new List<Student>();
        static int countForTesting = 1;

        public IActionResult Index()
        {
            //Here, fill signed in students from the signed in table in the database
            


            return View(signedInStudents);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
        public IActionResult AddClimber(string NameToSearch)
        {
            //replace this line with adding to the the signed-in Database
            signedInStudents.Add(new Student(countForTesting++, NameToSearch));
            
            

            return View("Index",signedInStudents);
        }

        public IActionResult RemoveClimbers(string signOutCheckBox)
        {
             
            string temp = this.Request.Form["signOutCheckBox"];

            string[] toRemoveIndex = temp.Split(','); 
            for (int i = toRemoveIndex.Length - 1; i>=0; i--)
            {
                int index = Int16.Parse(toRemoveIndex[i]);
                signedInStudents.Remove(signedInStudents[index]);

            }

            

            
            return View("Index", signedInStudents);
        }


    }

}

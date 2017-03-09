using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

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
        public IActionResult AddClimber(string lastNametoSearch, string firstNameToSearch)
        {
            //replace this line with adding to the the signed-in Database
            int visitAdded;
            SqlConnection conn = new SqlConnection("Data Source=SQL5019.SmarterASP.NET;Initial Catalog=DB_A16A06_climb;User Id=DB_A16A06_climb_admin;Password=climbdev1;");
            SqlCommand cmd = new SqlCommand("createVisit", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@userFirst", SqlDbType.VarChar).Value = firstNameToSearch;
            cmd.Parameters.Add("@userLast", SqlDbType.VarChar).Value = lastNametoSearch;
            cmd.Parameters.Add("@visitType", SqlDbType.VarChar).Value = "test type";

 //           cmd.Parameters.AddWithValue("userFirst", firstNameToSearch);
   //         cmd.Parameters.AddWithValue("userLast", lastNametoSearch);
     //       cmd.Parameters.AddWithValue("visitType", "test type");
            try
            {
                conn.Open();
                visitAdded = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Execption creating visit. " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
            signedInStudents.Add(new Student(countForTesting++, lastNametoSearch, firstNameToSearch));


            Console.WriteLine(visitAdded);
            return View("Index",signedInStudents);
        }

        public IActionResult RemoveClimbers()
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

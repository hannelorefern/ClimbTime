using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        static List<Student> signedInStudents = new List<Student>();
        static int countForTesting = 1;
        SqlConnection conn = new SqlConnection("Data Source=SQL5019.SmarterASP.NET;Initial Catalog=DB_A16A06_climb;User Id=DB_A16A06_climb_admin;Password=climbdev1;");

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
            //This will eventually correspond to the button for homepage-10, but
            //for now, it is tied to something else for our tests

            //replace this line with adding to the the signed-in Database
            int visitAdded;
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
            signedInStudents.Add(new Student(countForTesting++, lastNametoSearch, firstNameToSearch, DateTime.Now.ToString("MMM d, yyyy H:mm:ss")));


            return View("Index",signedInStudents);
        }

        public IActionResult RemoveClimbers()
        {//This corresponds to Homepage-7
             
            string temp = this.Request.Form["signOutCheckBox"];

            string[] toRemoveIndex = temp.Split(','); 
            for (int i = toRemoveIndex.Length - 1; i>=0; i--)
            {
                int index = Int16.Parse(toRemoveIndex[i]);
                Debug.WriteLine("signed in students: " + signedInStudents.Count);
                Debug.WriteLine(index);
                SqlCommand cmd = new SqlCommand("checkoutUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@firstName", SqlDbType.VarChar).Value = signedInStudents[index].firstName;
                cmd.Parameters.Add("@lastName", SqlDbType.VarChar).Value = signedInStudents[index].lastName;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Exeception checkout out user. " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                signedInStudents.Remove(signedInStudents[index]);

            }

            

            
            return View("Index", signedInStudents);
        }

        public IActionResult getCheckoutPage() {
            return View("SignInDetails");
        }
        //start of method stubs

        public void SignInClimber() {
            //This corresponds to item Homepage-1
            // this method should take the card swipe and direct to the sign in page for the appropriate user
            //Student toSignIn;

            //return View("SignInDetails", toSignIn);
        }

        public void UserSearch()
        {   //This corresponds to item Homepage-2

            // this method is for implementing the search by name
        }

        public IActionResult AddUser() {
            //This corresponds to item Homepage-3

            //this method just passes to batch add user with a count of 1;
            return BatchAddUser(1);
        }

        public IActionResult BatchAddUser(int count) {
            //this corresponds to item Homepage-4

            //this method should take us to the add users page, configured for the desired count of users

            

            return null;
        }

        public void CheckoutShoes() {
            //This corresponds to item Homepage-9

            //this should log in the data base that the shoes were used, and any assorted data
        }

        public void CheckoutHarness()
        {   //This corresponds to item Homepage-9

            //this should log in the data base that the harness was used, and any assorted data
        }

        public IActionResult AddCertificationToUser() {
            //This corresponds to item Homepage-13

            //I assume, but may be wrong that
            //this should redirect to a page for adding certifications to users, 
            //configured for the user and certification as chosen 
            return null;
        }


        //Stubbed out methods for Add Users Page
        public void TemporaryUserStore() {
            //This corresponds to item BatchAddUsers-2, when there are more users to add in the current batch
            //this should hold on to the data to be comitted to the database until it's ready, and
            //this should reset the page for the next user to be added
        }

        public IActionResult GroupInfoFinished() {
            //this corresponds to item BatchAddUsers-2, when on the final user for the current batch
            //this should hold on to the data to be comitted to the database until it's ready, and
            //this should redirect to (I think) the page with the appropriate training video

            return null;


        }

        public IActionResult InstructionalVideoFinished()
        {   //this corresponds to item BatchAddUsers-5, the call should be restricted on the JS/HTML side of things (I think)
            //this should retrieve the data for the current group watching, and
            //this should return the waiver signing page for this group
            return null;
        }

        public void RegisterUser() {
            //this corresponds to item BatchAddUsers-7, when there are more users to add in the current batch
            //this should commit user data and the waiver to the database, and
            //this should prepare the page for the next user
        }

        public void RegisterFinalUser(){
            //this corresponds to item BatchAddUsers-7, when on the final user
            //this should commit user data and the waiver to the database and
            //return to which page? this is currently unknown by Parker.

        }


    }

}

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
        static List<User> signedInUsers = new List<User>();
        static int countForTesting = 1;
        // SqlConnection conn = new SqlConnection("Data Source=SQL5019.SmarterASP.NET;Initial Catalog=DB_A16A06_climb;User Id=DB_A16A06_climb_admin;Password=climbdev1;");
        DataAccesser db = new DataAccesser("Data Source=SQL5019.SmarterASP.NET;Initial Catalog=DB_A16A06_climb;User Id=DB_A16A06_climb_admin;Password=climbdev1;");

        public IActionResult Index()
        {

            signedInUsers = db.getSignedIn();
            signedInUsers.Sort();
            

            return View(signedInUsers);
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
            User toAdd = db.findUser(firstNameToSearch, lastNameToSearch);
            toAdd.time = DateTime.Now.ToString("MMM d, yyyy H:mm:ss");
            db.addVisit(toAdd);

            signedInUsers.Add(toAdd);


            return View("Index",signedInUsers);
        }

        public IActionResult RemoveClimbers()
        {//This corresponds to Homepage-7
             
            string temp = this.Request.Form["signOutCheckBox"];
            if (temp==null) { }
            else { 
            string[] toRemoveIndex = temp.Split(','); 
            for (int i = toRemoveIndex.Length - 1; i>=0; i--)
                {
                int index = Int16.Parse(toRemoveIndex[i]);
                string firstNameToRemove = signedInUsers[index].firstName;
                string lastNameToRemove = signedInUsers[index].lastName;
                db.finishVisit(firstNameToRemove, lastNameToRemove);
                signedInUsers.Remove(signedInUsers[index]);
    
                }

            }


            
            return View("Index", signedInUsers);
        }


        public async Task<ActionResult> GetMatchesForSignIn(string searchTerm)
        {
            
            string[] names = searchTerm.Split(' ');
            if (names.Length == 1)
            {
                names = new string[]{names[0], ""};
            }
            List<User> searchResults = db.searchForUsers(names[0], names[1]);

            return PartialView("SearchResults", searchResults);
        }

        public async Task<ActionResult> GetMatchesForUserPage(string searchTerm)
        {
            string[] names = searchTerm.Split(' ');
            if (names.Length == 1)
            {
                names = new string[] { names[0], "" };
            }
            List<User> searchResults = db.searchForUsers(names[0], names[1]);

            return PartialView("UserPageSearch", searchResults);

        }

        
        //start of method stubs

        public IActionResult SignInClimber(string CardSwipe) {
            //This corresponds to item Homepage-1
            // this method should take the card swipe and direct to the sign in page for the appropriate user
            System.Diagnostics.Debug.WriteLine("*************" + CardSwipe.First());
            User toSignIn;
            if (CardSwipe != null)
            {
                toSignIn = db.findUser(CardSwipe);

                //return View("SignInDetails", toSignIn);
                return AddClimber(toSignIn.lastName, toSignIn.firstName);
            }
        
            return View("Index", signedInUsers);
        }

        public void UserSearch(string SearchTerm)
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

            List<User> group = new List<User>();
            for (int i = 0; i<count; i++)
            {
                group[i] = new WebApplication3.User();
            }

            return View("AddUserStep1", group);
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

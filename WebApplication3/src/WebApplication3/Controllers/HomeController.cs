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
using System.IO;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        static List<User> signedInUsers = new List<User>();
        // SqlConnection conn = new SqlConnection("Data Source=SQL5019.SmarterASP.NET;Initial Catalog=DB_A16A06_climb;User Id=DB_A16A06_climb_admin;Password=climbdev1;");
        DataAccessor db = new DataAccessor("Data Source=SQL5019.SmarterASP.NET;Initial Catalog=DB_A16A06_climb;User Id=DB_A16A06_climb_admin;Password=climbdev1;", false);
        string path = "./";

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
            return View();
        }



        public IActionResult Error()
        {
            return View();
        }
        public IActionResult AddClimber(string lastNameToSearch, string firstNameToSearch)
        {
            User toAdd = db.getUser(firstNameToSearch, lastNameToSearch);
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
                db.finishVisit(signedInUsers[index]);
                signedInUsers.Remove(signedInUsers[index]);
    
                }

            }


            
            return View("Index", signedInUsers);
        }


        public async Task<ActionResult> GetMatchesForSignIn(string searchTerm)
        {
            string[] names = new string[] { "" };
            if (searchTerm != null)
            {
                names = searchTerm.Split(' ');
            }
           
            if (names.Length == 1)
            {
                names = new string[]{names[0], names[0]};
            }
            List<User> searchResults = db.searchForUsers(names[0], names[1]);

            return PartialView("SearchResults", searchResults);
        }

        public async Task<ActionResult> GetMatchesForUserPage(string searchTerm)
        {
            string[] names = new string[] { "" };
            if (searchTerm != null)
            {
                names = searchTerm.Split(' ');
            }
            if (names.Length <= 1)
            {
                names = new string[] { names[0], names[0] };
            }
            List<User> searchResults = db.searchForUsers(names[0], names[1]);

            return PartialView("UserSearchResults", searchResults);

        }

        public IActionResult TestMethod(List<User> userList)
        {
            //TO DO: assign a default user type to new users

            string temp = this.Request.Form["firstNameField"];
            string[] firstNames = temp.Split(',');
            temp = this.Request.Form["lastNameField"];
            string[] lastNames = temp.Split(',');
            temp = this.Request.Form["phoneField"];
            string[] phones = temp.Split(',');
            temp = this.Request.Form["addressField"];
            string[] addresses = temp.Split(',');
            temp = this.Request.Form["cardswipeField"];
            string[] cardswipes = temp.Split(',');
            List<User> users = new List<User>();
            for (int i = 0; i < firstNames.Length; i++)
            {
                User toAdd = new WebApplication3.User();
                toAdd.firstName = firstNames[i];
                toAdd.lastName = lastNames[i];
                toAdd.phoneNumber = phones[i];
                toAdd.email = addresses[i];
                toAdd.studentID = cardswipes[i];
                //toAdd.userType = **default VALUE**


                users.Add(toAdd);
            }

            return View("EmbeddedVideo", users);
        }



        public IActionResult SignInClimber(string CardSwipe) {

            //To Do: re route this method to the sign in details page, instead of the temp sign in action
            if (CardSwipe == null) {
                return View("Index", signedInUsers);
            }
            System.Diagnostics.Debug.WriteLine("*************" + CardSwipe.First());
            User toSignIn;
            if (CardSwipe != null)
            {
                toSignIn = db.getUser(CardSwipe);

                //return View("SignInDetails", toSignIn);
                return AddClimber(toSignIn.lastName, toSignIn.firstName);
            }
        
            return View("Index", signedInUsers);
        }

        public IActionResult ShowUserDetails(string IDToShow) {
            User toShow = db.getUser(IDToShow);

            return View("Users", toShow);
        }


        public IActionResult AddUser() {
            //This corresponds to item Homepage-3

            //this method just passes to batch add user with a count of 1;
            return BatchAddUser("1");
        }

        public IActionResult BatchAddUser(string batchAddField) {
            //this corresponds to item Homepage-4
            if (batchAddField != null)
            {
                int count = int.Parse(batchAddField);
                //this method should take us to the add users page, configured for the desired count of users

                List<User> group = new List<User>();
                for (int i = 0; i < count; i++)
                {
                    group.Add(new WebApplication3.User());
                }

                return View("AddUserStep1", group);
            }
            else
            { return View("Index", signedInUsers); }
        }

        public IActionResult MoveGroupToVideo()
        {

            string temp = this.Request.Form["nameField"];
            string[] names = temp.Split(',');
            temp = this.Request.Form["phoneField"];
            string[] phones = temp.Split(',');
            temp = this.Request.Form["addressField"];
            string[] addresses = temp.Split(',');
            temp = this.Request.Form["cardswipeField"];
            string[] cardswipes = temp.Split(',');
            List<User> users = new List<User>();
            for(int i = 0; i < names.Length; i++)
            {
                string[] firstLast = names[i].Split(' ');
                User toAdd = new WebApplication3.User();
                toAdd.firstName = firstLast[0];
                toAdd.lastName = firstLast[firstLast.Length - 1];
                toAdd.phoneNumber = phones[i];
                toAdd.email = addresses[i];
                toAdd.studentID = cardswipes[i];
                //toAdd.userType = **default VALUE**


                users.Add(toAdd);

            }


            return View("EmbeddedVideo",users);
        }


        public IActionResult SaveData(string NameField, string SystemIDField,
                                      string SIDField, string ShoeField,
                                      string HarnessField, string PhoneField,
                                      string EmailField, string UserTypeField)
        {
            int systemID = Convert.ToInt32(SystemIDField);
            if (NameField != null)
            {
                string[] names = NameField.Split(' ');
                db.updateName(names[0], names[names.Length - 1], systemID);
                //db.updateName
            }
            if (SIDField != null)
            {
                db.updateStudentID(SIDField, systemID);
            }
            if (ShoeField != null)
            {
                db.updateShoeSize(ShoeField, systemID);
           
            }
            if(HarnessField!= null)
            {
                db.updateHarnessSize(HarnessField, systemID);
            }
            if(PhoneField!=null)
            {
                db.updatePhone(PhoneField, systemID);
            }
            if(EmailField!=null)
            {
                db.updateEmail(EmailField, systemID);
            }
            if(UserTypeField!=null)
            {
                db.updateUserType(UserTypeField, systemID);
            }
            
            Debug.WriteLine(NameField + " " + SystemIDField);
            return View("Users");
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


        //These methods are for the settings page
        public string getHarnessCount(string harnessSize)
        {
            string count = ""+0;
            //count = databaseRead
            return count;
        }
        public string getShoeCount(string shoeSize)
        {
            string count = "" + 0;
            //count = databaseRead
            return count;
        }
        public IActionResult saveInventoryEdits(string shoebox, string shoeboxsize, string harnessbox, string harnessboxsize) {
            if (shoebox != null)
            {
                //databaseWrite
            }
            if (harnessbox!= null)
            {
                //databaseWrite
            }
            return View("Settings");
        }

        public string[] getClasses()
        {
            //Course courses = db.getCourses();
            
            //put all the course names in a string []

            //return the array


            return null;
        }

        public IActionResult AddClass()
        {
            return View("ClassCreation");
        }

        public IActionResult EditClass(string className)
        {
            //Course toEdit = db.getCourse(className); //or something like it

            return View("ClassCreation");
        }

        public IActionResult RemoveClass(string className)
        {
            //db.removeCourse();

            return View("Settings");
        }

        public string[] GetCertificationNames()
        {
            List<Certification> certifications = db.getCerts();
            string[] result = new string[certifications.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = certifications[i].title;
            }


            return result;
        }

        public IActionResult AddCertification()
        {
            return View("CertificationCreation");
        }

        public IActionResult EditCertification(string certificationName)
        {
            //Course toEdit = db.getCourse(className); //or something like it

            return View("CertificationCreation");
        }

        public IActionResult RemoveCertification(string certificationName)
        {
            //db.removeCourse();

            return View("Settings");
        }

        public IActionResult SaveCertification(string nameField, string yearsField, string descriptionField) {

            //database Writeout

            return View("Settings");
        }


        public string[] GetStaffNames()
        {
            //read in Staff names, return them as a string

            return null;
        }

        public IActionResult AddStaff()
        {
            return View("StaffCreation");
        }

        public IActionResult EditStaff(string staffName)
        {
            //Course toEdit = db.getCourse(className); //or something like it

            return View("StaffCreation");
        }

        public IActionResult RemoveStaff(string staffName)
        {
            //db.removeCourse();

            return View("Settings");
        }

       public void generateCourseReport(string filename)
         {
             FileStream file = new FileStream(Path.Combine(path, filename), FileMode.Create);
             using (StreamWriter fout = new StreamWriter(file)) {
                 List<string[]> records = db.allCourseReport();
                 foreach (string[] record in records)
                 {
                     foreach (string field in record)
                     {
                         fout.Write(field + ",");
                     }
                     fout.Write("\n");
                 }
             }
         }
 
         public void generateVisitReport(string filename)
         {
             FileStream file = new FileStream(Path.Combine(path, filename), FileMode.Create);
             using (StreamWriter fout = new StreamWriter(file))
             {
                 List<string[]> records = db.allVisitReport();
                 foreach (string[] record in records)
                 {
                     foreach (string field in record)
                     {
                         fout.Write(field + ",");
                     }
                     fout.Write("\n");
                 }
             }
         }

        public FileResult certificationReport()
        {
            string fileName = "CertificationReport" + DateTime.Now + ".csv";
            generateCertificationReport(fileName);
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(Path.Combine(path, fileName)), "application/csv")
            {
                FileDownloadName = fileName
            };
            return result;
        }
 
         public void generateCertificationReport(string filename)
         {
             FileStream file = new FileStream(Path.Combine(path, filename), FileMode.Create);
             using (StreamWriter fout = new StreamWriter(file))
             {
                 List<string[]> records = db.allCertificationReport();
                 foreach (string[] record in records)
                 {
                     foreach (string field in record)
                     {
                         fout.Write(field + ",");
                     }
                     fout.Write("\n");
                 }
             }
         }


    }

}

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

        public IActionResult HomePage()
        {

            signedInUsers = db.getSignedIn();
            signedInUsers.Sort();
            

            return View("HomePage",signedInUsers);
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


            return View("HomePage", signedInUsers);
        }
        public IActionResult AddClimberBySystemID(string systemID, string courseID)
        {
            

            int sysID = int.Parse(systemID);
            User toAdd = db.getUser(sysID);
            toAdd.time = DateTime.Now.ToString("MMM d, yyyy H:mm:ss");
            if (courseID.Equals("undefined"))
            {
                //no course type
            }
            else
            {
                //courseID is an int hiding in a string, and is the course they're signing in for
            }
            db.addVisit(toAdd);

            signedInUsers.Add(toAdd);


            return View("HomePage", signedInUsers);
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


            
            return View("HomePage", signedInUsers);
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

        



        public IActionResult SignInClimber(string CardSwipe) {

            //To Do: re route this method to the sign in details page, instead of the temp sign in action
            if (CardSwipe == null) {
                return View("HomePage", signedInUsers);
            }
            System.Diagnostics.Debug.WriteLine("*************" + CardSwipe.First());
            User toSignIn;
            if (CardSwipe != null)
            {
                toSignIn = db.getUser(CardSwipe);

                return View("SignInDetails", toSignIn);
            }
        
            return View("HomePage", signedInUsers);
        }

        public IActionResult ShowUserDetails(string IDToShow) {
            User toShow = db.getUser(IDToShow);
            User display = new User();
            display.systemID = toShow.systemID;
            display.studentID = toShow.studentID;
            display.userType = toShow.userType;
            display.firstName = toShow.firstName;
            display.lastName = toShow.lastName;
            if (toShow.ShoeSize != null){
                display.ShoeSize = toShow.ShoeSize;
            } else{
                display.ShoeSize = "Information not found";
            }
            if (toShow.HarnessSize != null)
            { display.HarnessSize = toShow.HarnessSize;
            } else{
                display.HarnessSize = "Information not found";
            }
            if (toShow.phoneNumber != null)
            { display.phoneNumber = toShow.phoneNumber;
            } else{
                display.phoneNumber = "Information not found";
            }
            if (toShow.email != null)
            { display.email = toShow.email;
            }else{
                display.email = "Information not found";
            }
            return View("Users", display);
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
            { return View("HomePage", signedInUsers); }
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

        public IActionResult MoveGroupToWaiver()
        {

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


                users.Add(toAdd);
            }

            return View("Waiver", users);
        }

        public IActionResult FinalizeWaiver()
        {

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
                char firstChar = cardswipes[i][0];
                if (char.IsLetter(firstChar)) {
                    toAdd.netID = cardswipes[i];
                }
                else
                {
                    toAdd.studentID = cardswipes[i];
                }
                toAdd.userType = "G";


                users.Add(toAdd);
            }
            foreach (User user in users)
            {
                //addUser to Database;
                string[] tempArray = user.convertToStringArray();
                db.addUser(tempArray);
                
            }
            return View("HomePage", signedInUsers);
        }

        public IActionResult takeToCertification(string userID, string certID)
        {
            //this needs to take the user to a certification page, ready for the user and the cert IDs passed

            return null;
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
            
            //this should log in the data base that the shoes were used, and any assorted data
        }

        public void CheckoutHarness()
        {   

            //this should log in the data base that the harness was used, and any assorted data
        }

        public IActionResult AddCertificationToUser() {
            //This corresponds to item Homepage-13

            //I assume, but may be wrong that
            //this should redirect to a page for adding certifications to users, 
            //configured for the user and certification as chosen 
            return null;
        }

        public IActionResult AddDistance(string userID, string distance)
        {
            int toAdd = int.Parse(distance);
            int sysID = int.Parse(userID);

            db.updateDistance(sysID, toAdd);

            User ret = db.getUser(sysID);
            return View("Users", ret);
        }


        //These methods are for the settings page
        public string getHarnessCount(string harnessSize)
        {
            string[] data = db.getInventoryData("Harness", harnessSize);
            string count = "" + data[3];
            return count;
        }
        public string getShoeCount(string shoeSize)
        {
            string[] data = db.getInventoryData("Shoes", shoeSize);
            string count = "" + data[3];
            return count;
        }
        public IActionResult saveInventoryEdits(string shoebox, string shoeboxsize, string harnessbox, string harnessboxsize) {
            if (shoebox != null)
            {
                string shoes = "Shoes";
                int newCount = int.Parse(shoebox);
                db.setEquipCount(shoes, shoeboxsize, newCount);
            }
            if (harnessbox!= null)
            {
                string equipName = "Harness";
                int newCount = int.Parse(harnessbox);
                db.setEquipCount(equipName, harnessboxsize, newCount);
            }
            return View("Settings");
        }

        public string[][] getCourseData()
        {
            List<Course> courses = db.getCourses();
            string[] name = new string[courses.Count];
            string[] sysID = new string[courses.Count];
            for(int i = 0; i < courses.Count; i++)
            {
                name[i] = courses[i].title;
                sysID[i] = ""+courses[i].ID;
            }
            //put all the course names in a string []

            //return the array
            string[][] ret = { name, sysID };
            
            return ret;
        }

        public IActionResult AddClass()
        {
            return View("ClassCreation");
        }

        public IActionResult EditClass(string courseId)
        {
            int cID = int.Parse(courseId);
            Course toEdit = db.getCourse(cID); 
            
            return View("ClassCreation",toEdit);
        }

        public IActionResult RemoveClass(string courseId)
        {
            int cID = int.Parse(courseId);
            db.removeCourse(cID);
            return View("Settings");
        }

        public IActionResult SaveCourse(string TitleField, string CourseCodeField, string timeStartField, string timeEndField, string termField, string systemIDField)
        {
            Course toSave = new Course();
            toSave.code = CourseCodeField;
            string dayString = this.Request.Form["dayField"];
            string[] days = dayString.Split(',');
            dayString = "";
            for (int i = 0; i < days.Count(); i++)
            {
                dayString += days[i]; 
            }
            toSave.days = dayString;
            //start time and end time currently do not get recorded
            //Finish this method;
            //toSave.term also needs a look;
            int sysID = int.Parse(systemIDField);
            if (sysID == -1)
            {

                //db.addCourse(toSave);
            }
            else
            {
                //db.updateCourse(toSave);
            }



            return View("Settings");
        }

        public string[][] GetCertificationData()
        {
            List<Certification> certifications = db.getCerts();
            string[] certIds = new string[certifications.Count];
            string[] certNames = new string[certifications.Count];
            for (int i = 0; i < certNames.Length; i++)
            {
                certIds[i] =""+ certifications[i].ID;
                certNames[i] = certifications[i].title;
            }

            string[][] ret = { certNames, certIds };
            return ret;
        }

        public IActionResult AddCertification()
        {
            return View("CertificationCreation");
        }

        public IActionResult EditCertification(string certificationID)
        {
            int id = int.Parse(certificationID);
            Certification toEdit = db.getCertification(id);
            return View("CertificationCreation", toEdit);
        }

        public IActionResult RemoveCertification(string certificationID)
        {
            int id = int.Parse(certificationID);
            Certification toEdit = db.getCertification(id);
            db.removeCertification(toEdit);
            return View("Settings");
        }

        public IActionResult SaveCertification(string nameField, string yearsField, string IDField) {
            int sysID= int.Parse(IDField);
            int yearsValid = int.Parse(yearsField);
            if (sysID == -1)
            {
                db.addCertification(nameField, yearsValid);
            }
            else
            {
                db.updateCertification(nameField, yearsValid, sysID);
            }

            return View("Settings");
        }


        public string[][] GetStaffData()
        {
            //read in Staff names, return them as a string
            List<User> staff = db.getStaffUsers();
            string[] staffNames = new string[staff.Count];
            string[] staffID = new string[staff.Count];
            
            for (int i = 0; i<staff.Count; i++)
            {
                staffNames[i] = staff[i].Name();
                staffID[i] = ""+staff[i].systemID;
            }
            string[][] ret = { staffNames, staffID };
            return ret;
        }

        public IActionResult AddStaff()
        {
            return View("AddStaff");
        }

        public IActionResult EditStaff(string staffID)
        {
            User toEdit = db.getUser(int.Parse(staffID)); //or something like it

            return View("AddStaff", toEdit);
        }

        public IActionResult RemoveStaff(string staffID)
        {
            //db.removeCourse();
            db.updateUserType("G", int.Parse(staffID));

            return View("Settings");
        }

        public IActionResult SaveStaff(string nameField, string passwordField)
        {
            string[] names = nameField.Split(' ');
            User user = db.getUser(names[0], names[names.Count() - 1]);

            db.updateUserType("S", user.systemID);
            db.addSignIn(nameField, passwordField);

            return View("Settings");
        }
       

        public FileResult CertificationReport()
        {
            string fileName = "CertificationReport" + DateTime.Today.ToString("dd-MM-yyyy") + ".csv";
            generateCertificationReport(fileName);
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(Path.Combine(path, fileName)), "application/csv")
            {
                FileDownloadName = fileName
            };
            return result;
        }

        public FileResult CourseReport()
        {
            string fileName = "CourseReport" + DateTime.Today.ToString("dd-MM-yyyy") + ".csv";
            generateCourseReport(fileName);
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(Path.Combine(path, fileName)), "application/csv")
            {
                FileDownloadName = fileName
            };
            return result;
        }

        public FileResult VisitReport()
        {
            string fileName = "VisitReport" + DateTime.Today.ToString("dd-MM-yyyy") + ".csv";
            generateVisitReport(fileName);
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(Path.Combine(path, fileName)), "application/csv")
            {
                FileDownloadName = fileName
            };
            return result;
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

        public IActionResult SignInAdmin()
        {
            string uName = this.Request.Form["usernameField"];
            string pWord = this.Request.Form["passwordField"];
            if (db.getSignIn(uName, pWord))
                return View("HomePage", signedInUsers);
            else
                return View("Index");
        }
        public IActionResult Index()
        {
            return View("Index");
        }

    }

}

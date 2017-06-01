using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.App_Data;
using System.Diagnostics;
using WebApplication3.Models;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication3.Controllers
{
    public class TestController : Controller
    {
        // GET: /<controller>/
        DataAccessor db = new DataAccessor("Data Source=SQL5019.SmarterASP.NET;Initial Catalog=DB_A16A06_climb;User Id=DB_A16A06_climb_admin;Password=climbdev1;", true);

        public IActionResult Index()
        {
            User testUser = new User();
            testUser.systemID = 9;
            testUser.firstName = "Test";
            testUser.lastName = "User";
            Course testCourse = new Course();
            testCourse.ID = 9;

            db.testPrint("BEGIN TESTING");

            //user table
            db.testPrint("FIND USER BY NAME");
            db.getUser("John", "Sample");

            db.testPrint("FIND USER BY CARDSWIPE");
            db.getUser("33333333");

            db.testPrint("ADD USER");
            string[] args = { "G", "Test", "User", "11111111", "testNetID", null, null, "07.0", "S", "0", null };
            db.addUser(args);

            db.testPrint("SEARCH FOR USERS");
            db.searchForUsers("j", "j");

            //contacts table

            //course table
            db.testPrint("ADD COURSE");
            string[] args2 = { "TestCourse", "TEST101", "MTWRF", "9:00", "10:00", "0", "0", null, null };
            db.addCourse(args2);

            db.testPrint("REMOVE COURSE");
            db.removeCourse(100);

            //term table
            db.testPrint("ADD TERM");
            db.addTerm("Winter", 2017, new DateTime(2017, 1, 1), new DateTime(2017, 3, 1));

            db.testPrint("REMOVE TERM");
            db.removeTerm("Winter", 2017);

            //enrolled table
            db.testPrint("ENROLL USER");
            db.enrollUser(testUser, testCourse);

            db.testPrint("UNENROLL USER");
            db.unenrollUser(testUser, testCourse);

            //certification table
            db.testPrint("ADD CERTIFICATION");
            db.addCertification("TestCertification", 1);

            db.testPrint("REMOVE CERTIFICATION");
            Certification testCert = new Certification();
            testCert.ID = 9;
            db.removeCertification(testCert);

            db.testPrint("GET CERTIFICATIONS");
            db.getCerts();

            //usercertification table
            db.testPrint("CERTIFY USER");
            db.certifyUser(testUser, testCert);

            db.testPrint("CLEAN CERTIFICATIONS");
            db.cleanUserCert();

            //equipment table
            db.testPrint("ADD EQUIPMENT TYPE");
            db.addEquipType("TestType", "XL");

            db.testPrint("REMOVE EQUIPMENT TYPE");
            db.removeEquipType("TestType", "XL");

            db.testPrint("SET EQUIPMENT COUNT");
            db.setEquipCount("TestType", "XL", 100);

            db.testPrint("GET INVENTORY");
            db.equipInventory();

            //equipmentuse table
            db.testPrint("CHECKOUT EQUIPMENT");
            db.equipCheckout(5, testUser, 2);

            //visit table
            db.testPrint("ADD VISIT");
            db.addVisit(testUser,"testtype");

            db.testPrint("FINSIH VISIT");
            db.finishVisit(testUser);

            db.testPrint("GET SIGNED IN USERS");
            db.getSignedIn();

            //visittype table
            db.testPrint("ADD VISIT TYPE");
            db.addVisitType("TestOfAddVisitType", 0, 0);

            db.testPrint("REMOVE VISIT TYPE");
            db.removeVisitType("TestOfAddVisitType");
           
            return View();
        }
    }
}

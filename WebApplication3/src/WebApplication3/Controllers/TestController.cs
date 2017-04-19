using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.App_Data;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication3.Controllers
{
    public class TestController : Controller
    {
        // GET: /<controller>/
        DataAccesser db = new DataAccesser("Data Source=SQL5019.SmarterASP.NET;Initial Catalog=DB_A16A06_climb;User Id=DB_A16A06_climb_admin;Password=climbdev1;");

        public IActionResult Index()
        {
            db.addVisitType("TestOfAddVisitType", 0, 0);

            db.removeVisitType("TestOfAddVisitType");
           
            return View();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Models
{
    public class Certification
    {
        public int ID { get; set; }
        public string title { get; set; }
        public int yearsBeforeExp { get; set; }
        public string description { get; set; }

        public Certification()
        {
            ID = 0;
            title = null;
            yearsBeforeExp = 0;
        }
    }
}

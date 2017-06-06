using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Models
{
    public class Course
    {
        public int ID { get; set; }
        public string title { get; set; }
        public string code { get; set; }
        public string days { get; set; }
        public TimeSpan start { get; set; }
        public TimeSpan end { get; set; }
        public int termID { get; set; } 
        public int certID { get; set; }
   
        public Course(int id, string title, string code, string days, TimeSpan start, TimeSpan end, int term, Certification cert)
        {
            ID = id;
            this.title = title;
            this.code = code;
            this.days = days;
            this.start = start;
            this.end = end;
            this.termID = term;
            this.certID = cert.ID;
        }

        public Course()
        {
            ID = -1;
            title = null;
            code = null;
            days = null;
            start = TimeSpan.Zero;
            end = TimeSpan.Zero;
            termID = -1;
            certID = -1;
        }
    }
}

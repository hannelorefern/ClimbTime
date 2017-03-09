using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3
{
    public class Student
    {
        public int ID { get; set; }
        public Boolean Selected { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }

        public Student(int ID, string lastName, string firstName) {
            this.ID = ID;
            this.lastName = lastName;
            this.firstName = firstName;
            this.Selected = true;
        }
    }
}

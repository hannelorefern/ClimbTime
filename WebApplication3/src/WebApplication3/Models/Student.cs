using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3
{
    public class Student
    {
        public int ID { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string time { get; set; }

        public Student()
        {
            ID = 0;
            lastName = null;
            firstName = null;
            time = null;
        }

        public Student(int ID, string lastName, string firstName, string time) {
            this.ID = ID;
            this.lastName = lastName;
            this.firstName = firstName;
            this.time = time; 
        }
    }
}

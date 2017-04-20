using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3
{
    public class User
    {
        public int ID { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string time { get; set; }

        public User()
        {
            ID = 0;
            lastName = null;
            firstName = null;
            time = null;
        }

        public User(int ID, string lastName, string firstName, string time) {
            this.ID = ID;
            this.lastName = lastName;
            this.firstName = firstName;
            this.time = time; 
        }

        public String Name()
        {
            return String.Format("%s, %s", this.lastName, this.firstName);
        }
    }
}

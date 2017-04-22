using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3
{
    public class User : IComparable
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
        {//note to Hannelore: Parker changed this method because I guess String.Format doesn't work like formatting in C.
            return String.Format("{0}, {1}", this.lastName, this.firstName);
        }

        public int CompareTo(object obj)
        {
            int ret = 0;
            if (obj != null)
            {
                User input = (User)obj;
                if (input != null)
                {
                    ret = this.lastName.CompareTo(input.lastName);
                    if (ret == 0)
                    {
                        ret = this.firstName.CompareTo(input.firstName);
                    }
                 
                }
            }

            return ret;
        }
        
    }
}

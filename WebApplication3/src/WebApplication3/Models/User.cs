using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3
{
    public class User : IComparable
    {
        public string studentID { get; set; }
        public int systemID { get; set; }
        public string ShoeSize { get; set; }
        public string HarnessSize { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string userType { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string time { get; set; }


        public User()
        {
            studentID =""+ 0;
            lastName = null;
            firstName = null;
            time = null;
        }

        public User(string ID, string lastName, string firstName, string time) {
            this.studentID = ID;
            this.lastName = lastName;
            this.firstName = firstName;
            this.time = time; 
        }

        public User(string ID, string ShoeSize, string HarnessSize, string phoneNumber, string email, string userType, string lastName, string firstName, string time, int systemID)
        {
            this.studentID = ID;
            this.ShoeSize = ShoeSize;
            this.HarnessSize = HarnessSize;

            this.phoneNumber = phoneNumber;
            this.email = email;
            this.userType = userType;
            this.lastName = lastName;
            this.firstName = firstName;
            this.time = time;

            this.systemID = systemID;
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

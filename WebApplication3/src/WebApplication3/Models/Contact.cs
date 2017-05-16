using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Models
{
    public class Contact
    {
        public int systemID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public int userID { get; set; }

        public Contact()
        {
            systemID = -1;
            userID = -1;
            firstName = null;
            lastName = null;
            phone = null;
        }
    }
}

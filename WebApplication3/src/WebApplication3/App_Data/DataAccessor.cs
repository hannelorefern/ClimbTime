using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Models;

namespace WebApplication3.App_Data
{
    public class DataAccessor
    {
        private SqlConnection conn { get; set; }
        private SqlCommandInterface cmd;

        public DataAccessor(string connString, bool testMode)
        {
            conn = new SqlConnection(connString);
            conn.Open();
            if (testMode)
                cmd = new TestCommand();
            else
                cmd = new SqlCommandWrapper();
        }

        ~DataAccessor()
        {
            conn.Close();
        }

        public void testPrint(string s)
        {
            Debug.WriteLine("\n##### " + s);
        }

        // users
        public User getUser(string firstName, string lastName)
        {
            User ret = new User();
            cmd.reinitialize("SELECT * FROM dbo.users WHERE firstName = @firstName AND lastName = @lastName", conn);
            cmd.addParameter("@firstName", firstName);
            cmd.addParameter("@lastName", lastName);
            using (SqlDataReader reader = cmd.executeReader())
            {
                if (reader.Read())
                {
                    ret.systemID = (int)reader["userID"];
                    ret.studentID = (string)reader["SID"];
                    ret.firstName = (string)reader["firstName"];
                    ret.lastName = (string)reader["lastName"];
                }
            }
            return ret;
        }

        public User getUser(int systemID)
        { User ret = new User();
            
                cmd.reinitialize("SELECT * FROM dbo.users WHERE userID = @systemID", conn);
                cmd.addParameter("@systemID", systemID);

            using (SqlDataReader reader = cmd.executeReader())
            {
                if (reader.Read())
                {
                    ret.systemID = (int)reader["userID"];
                    ret.studentID = (string)reader["SID"];
                    ret.userType = (string)reader["userType"];
                    ret.firstName = (string)reader["firstName"];
                    ret.lastName = (string)reader["lastName"];


                    if (!DBNull.Value.Equals(reader["shoeSize"]))
                    { ret.ShoeSize = (string)reader["shoeSize"]; }
                    else
                    {
                        ret.ShoeSize = "Information not found";
                    }
                    if (!DBNull.Value.Equals(reader["harnessSize"]))
                    { ret.HarnessSize = (string)reader["harnessSize"]; }
                    else
                    {
                        ret.HarnessSize = "Information not found";
                    }
                    if (!DBNull.Value.Equals(reader["phone"]))
                    { ret.phoneNumber = (string)reader["phone"]; }
                    else
                    {
                        ret.phoneNumber = "Information not found";
                    }
                    if (!DBNull.Value.Equals(reader["email"]))
                    { ret.email = (string)reader["email"]; }
                    else
                    {
                        ret.email = "Information not found";
                    }
                }
            }
            return ret;
        }

        public User getUser(string CardSwipe) {
            User ret = new User();
            if (char.IsLetter(CardSwipe.First()))
            {
                cmd.reinitialize("SELECT * FROM dbo.users WHERE netID = @netID", conn);
                cmd.addParameter("@netID", CardSwipe);
            }
            else
            {
                cmd.reinitialize("SELECT * FROM dbo.users WHERE SID = @sid", conn);
                cmd.addParameter("@sid", CardSwipe);
            }
            using (SqlDataReader reader = cmd.executeReader())
            {
                if (reader.Read())
                {
                    ret.systemID = (int)reader["userID"];
                    ret.studentID = (string)reader["SID"];
                    ret.userType = (string)reader["userType"];
                    ret.firstName = (string)reader["firstName"];
                    ret.lastName = (string)reader["lastName"];


                    if (!DBNull.Value.Equals(reader["shoeSize"]))
                    { ret.ShoeSize = (string)reader["shoeSize"]; }
                    else
                    {
                        ret.ShoeSize = "Information not found";
                    }
                    if (!DBNull.Value.Equals(reader["harnessSize"]))
                    { ret.HarnessSize = (string)reader["harnessSize"]; }
                    else
                    {
                        ret.HarnessSize = "Information not found";
                    }
                    if (! DBNull.Value.Equals(reader["phone"]))
                    { ret.phoneNumber = (string)reader["phone"]; }
                    else
                    {
                        ret.phoneNumber = "Information not found";
                    }
                    if (! DBNull.Value.Equals(reader["email"]))
                    { ret.email = (string)reader["email"]; }
                    else
                    {
                        ret.email = "Information not found";
                    }
                    


                }
            }
            return ret;
        }

        public int addUser(string[] args)
        {
            //returns ID of added user, -1 if not successful

            //utype, first, last, sid, phone, email, shoe, harness, miles
            //each contains VALUE or NULL

            char utype = args[0][0];
            string firstName = args[1];
            string lastName = args[2];
            int sid = Convert.ToInt32(args[3]);
            string netID = args[4];
            string phone = args[5];
            string email = args[6];
            string shoeSize = args[7];
            string harnessSize = args[8];
            int num;
            int miles;
            bool flag = Int32.TryParse(args[9], out num);
            if (flag) { miles = num; } else { miles = -1; }
            int contact;
            flag = Int32.TryParse(args[10], out num);
            if (flag) { contact = num; } else { contact = -1; }
            int ret = -1;

            cmd.reinitialize("INSERT INTO dbo.users (userType, firstName, lastName, SID, phone, email, shoeSize, harnessSize, mile) " +
                "output INSERTED.ID VALUES(@userType, @firstName, @lastName, @sid, @phoneNumber, @email, @shoeSize, @harnessSize, @miles)", conn);
            cmd.addParameter("@userType", utype);
            cmd.addParameter("@firstName", firstName);
            cmd.addParameter("@lastName", lastName);
            cmd.addParameter("@sid", sid);
            cmd.addParameter("@netID", netID);
            cmd.addParameter("@phone", phone);
            cmd.addParameter("@email", email);
            cmd.addParameter("@shoeSize", shoeSize);
            cmd.addParameter("@harnessSize", harnessSize);
            if (miles >= 0)
                cmd.addParameter("@miles", miles);
            if (contact >= 0)
                cmd.addParameter("@contact", contact);

            try
            {
                ret = (int)cmd.executeScalar();
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception creating user. " + ex.Message);
            }
            return ret;
        }

        public List<User> searchForUsers(string firstName, string lastName)
        {
            //non case sensitive
            //note: wildcard characters added with addParameter
            List<User> Users = new List<User>();
            string commandText = "SELECT * FROM dbo.users WHERE ";
            if (firstName != "")
            {
                commandText += "firstName LIKE @firstName";
            }

            if (lastName.Equals(firstName)) {
                commandText += " OR ";
            }
            else
            {
                commandText += " AND ";
            }

            if (lastName != "")
            {
                commandText += "lastName like @lastName";
            }
            cmd.reinitialize(commandText, conn);
            if (firstName != "")
            { cmd.addParameter("@firstName", firstName + '%'); }
            if (lastName != "")
            { cmd.addParameter("@lastName", lastName + '%'); }

            using (SqlDataReader reader = cmd.executeReader())
            {
                while (reader.Read())
                {
                    User temp = new User();
                    temp.firstName = (string)reader["firstName"];
                    temp.lastName = (string)reader["lastName"];
                    temp.studentID = (string)reader["SID"];
                    
                    Users.Add(temp);
                }
            }


            return Users;
        }

        //visits
        public bool addVisit(User climber)
        {
            //TO DO: IMPLEMENT SEPARATE VISIT TYPES
            bool retFlag = false;
            cmd.reinitialize("createVisit", conn);
            cmd.isStoredProcedure();
            cmd.addParameter("@userID", climber.systemID);
            cmd.addParameter("@visitType", "test type");

            try
            {
                cmd.executeScalar();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Execption creating visit. " + ex.Message);
            }
            return retFlag;
        }

        public bool finishVisit(User climber)
        {
            bool retFlag = false;
            cmd.reinitialize("finishVisit", conn);
            cmd.isStoredProcedure();
            cmd.addParameter("@firstName", climber.firstName);
            cmd.addParameter("@lastName",  climber.lastName);

            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception completing visit. " + ex.Message);
            }
            return retFlag;
        }

        public List<User> getSignedIn()
        {
            List<User> ret = new List<User>();
            cmd.reinitialize("SELECT * FROM dbo.visits JOIN dbo.users ON dbo.visits.userID = dbo.users.userID WHERE endDateTime IS NULL", conn);
            // this SqlCommand will need to be edited so that it only cares about tracked visit types.

            using (SqlDataReader reader = cmd.executeReader())
            {
                while (reader.Read())
                {
                    User temp = new User();
                    temp.firstName = (string)reader["firstName"];
                    temp.lastName = (string)reader["lastName"];
                    temp.studentID = (string)reader["SID"];
                    temp.systemID = (int)reader["userID"];

                    DateTime tempTime = (DateTime)reader["startDateTime"];

                    temp.time = tempTime.ToString("MMM d, yyyy H:mm:ss");
                    ret.Add(temp);
                }
            }
            return ret;
        }

        //certifications
        public int addCertification(string title, int yearsBeforeExp)
        {
            int ret = -1;
            cmd.reinitialize("INSERT INTO dbo.certifications (title, yearsBeforeExp) output INSERTED.ID VALUES (@title, @years)", conn);
            cmd.addParameter("@title", title);
            cmd.addParameter("@years", yearsBeforeExp);
            try
            {
                ret = (int)cmd.executeScalar();
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception adding new certification. " + ex.Message);
            }
            return ret;
        }

        public Certification getCertification(int id)
        {
            cmd.reinitialize("SELECT * WHERE certID = @id", conn);
            cmd.addParameter("@id", id);
            Certification ret = new Certification();
            using (SqlDataReader reader = cmd.executeReader())
            {
                if (reader.Read())
                {
                    ret.ID = (int)reader["certID"];
                    ret.title = (string)reader["title"];
                    ret.yearsBeforeExp = (int)reader["yearsBeforeExp"];
                }
            }
            return ret;
        }

        public bool removeCertification(Certification cert)
        {
            bool retFlag = false;
            cmd.reinitialize("DELETE FROM dbo.certifications WHERE certID = @id", conn);
            cmd.addParameter("@id", cert.ID);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception deleting certification. " + ex.Message);
            }
            return retFlag;
        }

        public List<Certification> getCerts()
        {
            List<Certification> ret = new List<Certification>();
            cmd.reinitialize("SELECT * FROM dbo.certification", conn);
            using (SqlDataReader reader = cmd.executeReader())
            {
                while (reader.Read())
                {
                    Certification temp = new Certification();
                    temp.ID = (int)reader["certID"];
                    temp.title = (string)reader["title"];
                    temp.yearsBeforeExp = (int)reader["yearsBeforeExp"];
                    ret.Add(temp);
                }
            }
            return ret;
        }

        //usercertifications
        public bool certifyUser(User user, Certification cert)
        {
            //TO DO: FIGURE OUT HOW TO GET ID OF CURRENTLY LOGGED IN USER
            bool retFlag = false;
            DateTime today = DateTime.Today;
            cmd.reinitialize("INSERT INTO dbo.usercertifications (userID, certID, datePosted, postedBy, expDate) VALUES (@uID, @cID, @now, 0, @dateExp", conn);
            cmd.addParameter("@uID", user.systemID);
            cmd.addParameter("@cID", cert.ID);
            cmd.addParameter("@now", today);
            cmd.addParameter("@dateExp", today.AddYears(cert.yearsBeforeExp));
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception certifying user. " + ex.Message);
            }
            return retFlag;
        }

        public bool cleanUserCert()
        {
            bool retFlag = false;
            //TO DO: FIGURE THIS SHIT OUT
            cmd.reinitialize("DELETE FROM dbo.usercertifications WHERE year(@today) - year(expDate) >= 6", conn);
            cmd.addParameter("@today", DateTime.Today);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception clearing outdated certifications. " + ex.Message);
            }
            return retFlag;
        }

        //terms
        public int addTerm(string quarter, int year, DateTime startDate, DateTime endDate)
        {
            int ret = -1;
            cmd.reinitialize("INSERT INTO dbo.term (quarter, year, startDate, endDate) output INSERTED.ID VALUES (@q, @y, @s, @e", conn);
            cmd.addParameter("@q", quarter);
            cmd.addParameter("@y", year);
            cmd.addParameter("@s", startDate);
            cmd.addParameter("@e", endDate);
            try
            {
                ret = (int)cmd.executeScalar();
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception adding new term. " + ex.Message);
            }
            return ret;
        }

        public bool removeTerm(string quarter, int year)
        {
            bool retFlag = false;
            cmd.reinitialize("DELETE FROM dbo.term WHERE quarter=@q AND year=@y", conn);
            cmd.addParameter("@q", quarter);
            cmd.addParameter("@y", year);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception removing term. " + ex.Message);
            }
            return retFlag;
        }

        //courses
        public int addCourse(string[] args)
        {
            string title = args[0];
            string code = args[1];
            string days = args[2]; //a <= 5 character string where each character stands for a day
            TimeSpan start = TimeSpan.Parse(args[3]);
            TimeSpan end = TimeSpan.Parse(args[4]);
            int equip;
            int cert;
            int num;
            int term = Convert.ToInt32(args[5]);
            bool result = Int32.TryParse(args[6], out num);
            if (result) { equip = num; } else { equip = -1; }
            result = Int32.TryParse(args[7], out num);
            if (result) { cert = num; } else { cert = -1; }
            
            int ret = -1;
            cmd.reinitialize("INSERT INTO dbo.course (title, code, daysOfWeek, startTime, endTime, term, checkout, certification) " +
                "output INSERTED.ID VALUES (@title, @code, @days, @start, @end, @term, @equip, @cert)", conn);
            cmd.addParameter("@title", title);
            cmd.addParameter("@code", code);
            cmd.addParameter("@days", days);
            cmd.addParameter("@start", start);
            cmd.addParameter("@end", end);
            cmd.addParameter("@term", term);
            if(equip >= 0)
                cmd.addParameter("@equip", equip);
            if(cert >= 0)
                cmd.addParameter("@cert", cert);

            try
            {
                ret = (int)cmd.executeScalar();
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception adding new course. " + ex.Message);
            }
            return ret;
        }

        public bool removeCourse(int courseID)
        {
            bool retFlag = false;
            cmd.reinitialize("DELETE FROM dbo.course WHERE courseID = @id", conn);
            cmd.addParameter("@id", courseID);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception removing course. " + ex.Message);
            }
            return retFlag;
        }

        //enrolled
        public int enrollUser(User u, Course c)
        {
            int ret = -1;
            cmd.reinitialize("INSERT INTO dbo.enrolled (userID, courseID, dateTimeEnrolled) output INSERTED.ID VALUES (@uid, @cid, @now)", conn);
            cmd.addParameter("@uid", u.systemID);
            cmd.addParameter("@cid", c.ID);
            cmd.addParameter("@now", DateTime.Now);
            try
            {
                ret = (int)cmd.executeScalar();
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception enrolling user in course. " + ex.Message);
            }
            return ret;
        }

        public bool unenrollUser(User u, Course c)
        {
            bool retFlag = false;
            cmd.reinitialize("DELETE FROM dbo.enrolled WHERE userID = @uid AND courseID = @cid", conn);
            cmd.addParameter("@uid", u.systemID);
            cmd.addParameter("@cid", c.ID);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception unenrolling user from course. " + ex.Message);
            }
            return retFlag;
        }

        //equipment
        public bool addEquipType(string name, string size)
        {
            bool retFlag = false;
            cmd.reinitialize("INSERT INTO dbo.equipment (name, size) VALUES (@name, @size)", conn);
            cmd.addParameter("@name", name);
            cmd.addParameter("@size", size);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception adding equipment type. " + ex.Message);
            }
            return retFlag;

        }

        public int setEquipCount(string name, string size, int count)
        {
            int ret = -1;
            cmd.reinitialize("UPDATE dbo.equipment SET count = @newCount WHERE name = @name AND size = @size", conn);
            cmd.addParameter("@name", name);
            cmd.addParameter("@size", size);
            cmd.addParameter("@newCount", count);
            try
            {
                ret = (int)cmd.executeScalar();
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception adding equipment. " + ex.Message);
            }
            return ret;
        }

        public bool removeEquipType(string name, string size)
        {
            bool retFlag = false;
            cmd.reinitialize("DELETE FROM dbo.equipment (name, size) WHERE name = @name AND size = @size", conn);
            cmd.addParameter("@name", name);
            cmd.addParameter("@size", size);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception removing equipment type. " + ex.Message);
            }
            return retFlag;
        }

        public Dictionary<string[], int> equipInventory()
        {
            Dictionary<string[], int> ret = new Dictionary<string[], int>();
            string[] temp = new string[2]; 

            cmd.reinitialize("SELECT * FROM dbo.equipment", conn);
            using (SqlDataReader reader = cmd.executeReader())
            {
                while (reader.Read())
                {
                    temp[0] = (string)reader["name"];
                    temp[1] = (string)reader["size"];
                    ret.Add(temp, (int)reader["equipID"]);
                }
            }
            return ret;
        }

        public string[] getInventoryData(string name, string size)
        {
            string[] ret = new string[4];
            cmd.reinitialize("SELECT * FROM dbo.equipment WHERE name = @name AND size = @size", conn);
            cmd.addParameter("@name", name);
            cmd.addParameter("@size", size);
            using (SqlDataReader reader = cmd.executeReader())
            {
                while (reader.Read())
                {
                    ret[0] = (string)reader["equipID"];
                    ret[1] = (string)reader["name"];
                    ret[2] = (string)reader["size"];
                    ret[3] = (string)reader["count"];
                }
            }
            return ret;
        }
        //equipmentuse
        public bool equipCheckout(int visitID, User climber, int equipID)
        {
            bool retFlag = false;
            cmd.reinitialize("INSERT INTO dbo.equipmentuse (visitID, userID, equipID, checkoutDateTime) VALUES (@v, @u, @e, @c)", conn);
            cmd.addParameter("@v", visitID);
            cmd.addParameter("@u", climber.systemID);
            cmd.addParameter("@e", equipID);
            cmd.addParameter("@c", DateTime.Now);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception checking out equipment. " + ex.Message);
            }
            return retFlag;
        }

        //visittype
        public bool addVisitType(string visitTypeName, int certID, int courseID) {
            bool retFlag = false;            
            cmd.reinitialize("INSERT INTO dbo.visittype (title, certID, courseID) VALUES(@title, @certID, @courseID)", conn);
            cmd.addParameter("@title", visitTypeName);
            cmd.addParameter("@certID", certID);
            cmd.addParameter("@courseID", courseID);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception adding visit type. " + ex.Message);
            }

            return retFlag;
        }

        public bool removeVisitType(string titleToRemove)
        {
            bool retFlag = false;
            cmd.reinitialize("DELETE FROM dbo.visittype WHERE title = @title", conn);
            cmd.addParameter("@title", titleToRemove);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception removing visit type. " + ex.Message);
            }




            return retFlag;
        }

        //contacts
        public bool addContact(string firstName, string lastName, string phone, User climber)
        {
            bool retFlag = false;
            cmd.reinitialize("INSERT INTO dbo.contacts (firstName, lastName, phone, userID) VALUES (@first, @last, @phone, @user)", conn);
            cmd.addParameter("@first", firstName);
            cmd.addParameter("@last", lastName);
            cmd.addParameter("@phone", phone);
            cmd.addParameter("@user", climber.systemID);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception adding contact. " + ex.Message);
            }
            return retFlag;
        }

        public Contact getContact(User climber)
        {
            Contact ret = new Contact();
            cmd.reinitialize("SELECT * FROM dbo.contacts WHERE userID = @uID", conn);
            cmd.addParameter("@uID", climber.systemID);
            try
            {
                using (SqlDataReader reader = cmd.executeReader())
                {
                    if (reader.Read())
                    {
                        ret.systemID = (int)reader["contactID"];
                        ret.firstName = (string)reader["firstName"];
                        ret.lastName = (string)reader["lastName"];
                        ret.phone = (string)reader["phone"];
                        ret.userID = (int)reader["userID"];
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception getting user contact. " + ex.Message);
            }
        
            return ret;
        }

        public void updateName(string firstName, string lastName, int userID)
        {
            cmd.reinitialize("UPDATE dbo.users SET firstName = @firstName, lastName = @lastName WHERE userID = @userID", conn);
            cmd.addParameter("@firstName", firstName);
            cmd.addParameter("@lastName", lastName);
            cmd.addParameter("@userID", userID);
            try { cmd.execute(); }
            catch (Exception ex)
            {
                throw new Exception("Exception updating user. " + ex.Message);
            }
        }
        public void updateStudentID(string studentID, int userID)
        {
            cmd.reinitialize("UPDATE dbo.users SET SID = @studentID WHERE userID = @userID", conn);
            cmd.addParameter("@studentID", studentID);
            cmd.addParameter("@userID", userID);
            try { cmd.execute(); }
            catch (Exception ex)
            {
                throw new Exception("Exception updating user. " + ex.Message);
            }
        }
        public void updateShoeSize(string shoeSize, int userID)
        {
            cmd.reinitialize("UPDATE dbo.users SET shoeSize = @shoeSize WHERE userID = @userID", conn);
            cmd.addParameter("@shoeSize", shoeSize);
            cmd.addParameter("@userID", userID);
            try { cmd.execute(); }
            catch (Exception ex)
            {
                throw new Exception("Exception updating user. " + ex.Message);
            }
        }
        public void updateHarnessSize(string harnessSize, int userID)
        {
            cmd.reinitialize("UPDATE dbo.users SET harnessSize = @harnessSize WHERE userID = @userID", conn);
            cmd.addParameter("@harnessSize", harnessSize);
            cmd.addParameter("@userID", userID);
            try { cmd.execute(); }
            catch (Exception ex)
            {
                throw new Exception("Exception updating user. " + ex.Message);
            }

        }
        public void updatePhone(string phoneNum, int userID)
        {
            cmd.reinitialize("UPDATE dbo.users SET phone = @phoneNum WHERE userID = @userID", conn);
            cmd.addParameter("@phoneNum", phoneNum);
            cmd.addParameter("@userID", userID);
            try { cmd.execute(); }
            catch (Exception ex)
            {
                throw new Exception("Exception updating user. " + ex.Message);
            }

        }
        public void updateEmail(string email, int userID)
        {
            cmd.reinitialize("UPDATE dbo.users SET email = @email WHERE userID = @userID", conn);
            cmd.addParameter("@email", email);
            cmd.addParameter("@userID", userID);
            try { cmd.execute(); }
            catch (Exception ex)
            {
                throw new Exception("Exception updating user. " + ex.Message);
            }

        }
        public void updateUserType(string userType, int userID)
        {
            cmd.reinitialize("UPDATE dbo.users SET userType = @userType WHERE userID = @userID", conn);
            cmd.addParameter("@userType", userType);
            cmd.addParameter("@userID", userID);
            try { cmd.execute(); }
            catch (Exception ex)
            {
                throw new Exception("Exception updating user. " + ex.Message);
            }

        }

    }

}

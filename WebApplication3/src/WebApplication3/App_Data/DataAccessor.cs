using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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
            if (testMode)
                cmd = new TestCommand();
            else
                cmd = new SqlCommandWrapper();
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
            conn.Open();
            using (SqlDataReader reader = cmd.executeReader())
            {
                if (reader.Read())
                {
                    ret.systemID = (int)reader["userID"];
                    ret.studentID = (string)reader["SID"];
                    ret.userType = (string)reader["userType"];
                    ret.firstName = (string)reader["firstName"];
                    ret.lastName = (string)reader["lastName"];
                    ret.ShoeSize = (string)reader["shoeSize"];
                    ret.HarnessSize = (string)reader["harnessSize"];
                    ret.phoneNumber = (string)reader["phone"];
                    ret.email = (string)reader["email"];
                }
            }
            conn.Close();
            return ret;
        }

        public User getUser(int systemID)
        { User ret = new User();
            
                cmd.reinitialize("SELECT * FROM dbo.users WHERE userID = @systemID", conn);
                cmd.addParameter("@systemID", systemID);
            conn.Open();
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
                    if (!DBNull.Value.Equals(reader["email"]))
                    { ret.email = (string)reader["email"]; }
                    else
                    {
                        ret.email = null;
                    }
                    if (!DBNull.Value.Equals(reader["miles"]))
                    {
                        ret.meters = (int)reader["miles"];
                    }
                    else
                    {
                        ret.meters = 0;
                    }
                }
            }
            conn.Close();
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
            conn.Open();
            using (SqlDataReader reader = cmd.executeReader())
            {
                if (reader.Read())
                {
                    ret.systemID = (int)reader["userID"];
                    ret.userType = (string)reader["userType"];
                    ret.firstName = (string)reader["firstName"];
                    ret.lastName = (string)reader["lastName"];

                    if (!DBNull.Value.Equals(reader["SID"]))
                    { ret.studentID = (string)reader["SID"]; }
                    else
                    {
                        ret.studentID = null;
                    }
                    if (!DBNull.Value.Equals(reader["shoeSize"]))
                    { ret.ShoeSize = (string)reader["shoeSize"]; }
                    else
                    {
                        ret.ShoeSize = null;
                    }
                    if (!DBNull.Value.Equals(reader["harnessSize"]))
                    { ret.HarnessSize = (string)reader["harnessSize"]; }
                    else
                    {
                        ret.HarnessSize = null;
                    }
                    if (!DBNull.Value.Equals(reader["phone"]))
                    { ret.phoneNumber = (string)reader["phone"]; }
                    else
                    {
                        ret.phoneNumber = null;
                    }
                    if (!DBNull.Value.Equals(reader["email"]))
                    { ret.email = (string)reader["email"]; }
                    else
                    {
                        ret.email = null;
                    }
                    if (!DBNull.Value.Equals(reader["miles"]))
                    {
                        ret.meters = (int)reader["miles"];
                    }
                    else
                    {
                        ret.meters = 0;
                    }

                }
            }
            conn.Close();
            return ret;
        }

     public List<User> getUsersByType(string type)
{
    List<User> ret = new List<User>();
    cmd.reinitialize("SELECT * FROM dbo.users WHERE userType = @ut", conn);
    cmd.addParameter("@ut", type);
            conn.Open();
    using (SqlDataReader reader = cmd.executeReader())
    {
        while (reader.Read())
        {
            User temp = new User();
            temp.firstName = (string)reader["firstName"];
            temp.lastName = (string)reader["lastName"];
            temp.studentID = (string)reader["SID"];
            temp.systemID = (int)reader["userID"];
            temp.netID = (string)reader["netID"];
            temp.phoneNumber = (string)reader["phone"];
            temp.email = (string)reader["email"];
            temp.HarnessSize = (string)reader["harnessSize"];
            temp.ShoeSize = (string)reader["shoeSize"];
            ret.Add(temp);
        }
    }
            conn.Close();
    return ret;
}

public List<User> getStaffUsers()
{
    List<User> ret = new List<User>();
    ret.AddRange(getUsersByType("A")); //admin
    ret.AddRange(getUsersByType("S")); //staff
    return ret;
}

        public void updateDistance(int userID, int distance)
        {
            int oldDistance = 0;
            cmd.reinitialize("SELECT * FROM dbo.users WHERE userID = @userID", conn);
            cmd.addParameter("@userID", userID);
            using (SqlDataReader reader = cmd.executeReader())
            {
                if (reader.Read())
                {
                    if (DBNull.Value.Equals(reader["miles"]))
                    {

                    }
                    else
                    {
                        oldDistance = (int)reader["miles"];
                        distance += oldDistance;
                    }
                }
            }
            cmd.reinitialize("UPDATE dbo.users set miles = @distance Where userID = @userID", conn);
            cmd.addParameter("@distance", distance);
            cmd.addParameter("@userID", userID);
            cmd.execute();
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
            //
            cmd.reinitialize("INSERT INTO dbo.users (userType, firstName, lastName, SID, netID, phone, email, shoeSize, harnessSize, miles) " +
                "output INSERTED.userID VALUES(@userType, @firstName, @lastName, @sid, @netID, @phoneNumber, @email, @shoeSize, @harnessSize, @miles)", conn);
            cmd.addParameter("@userType", utype);
            cmd.addParameter("@firstName", firstName);
            cmd.addParameter("@lastName", lastName);

            cmd.addParameter("@sid", sid);
            if (netID == null) {
                cmd.addParameter("@netID", DBNull.Value);
            }
            else {
                cmd.addParameter("@netID", netID);
            }

            if (phone == null) { cmd.addParameter("@phoneNumber", DBNull.Value); }
            else { cmd.addParameter("@phoneNumber", phone); }

            if (email == null) { cmd.addParameter("@email", DBNull.Value); }
            else { cmd.addParameter("@email", email); }

            if (shoeSize == null) { cmd.addParameter("@shoeSize", DBNull.Value); }
            else { cmd.addParameter("@shoeSize", shoeSize); }

            if (harnessSize == null) { cmd.addParameter("@harnessSize", DBNull.Value); }
            else { cmd.addParameter("@harnessSize", harnessSize); }
            
                cmd.addParameter("@miles", miles);
            
                cmd.addParameter("@contact", contact);

            try
            {
                ret = (int)cmd.executeScalar();
            }
            catch (Exception ex)
            {
                this.AppendToLog("Exeception creating user. " + ex.Message);
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
            conn.Open();
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
            conn.Close();


            return Users;
        }

        //visits
        public bool addVisit(User climber, string visitTypeName)
        {
            //TO DO: IMPLEMENT SEPARATE VISIT TYPES
            bool retFlag = false;
            DateTime now = DateTime.Now;
            cmd.reinitialize("INSERT INTO dbo.visits(userID, visitTypeID, startDateTime) VALUES(@userID, (SELECT visitTypeID FROM dbo.visittype WHERE title = @visitType), @start)", conn);
            cmd.addParameter("@userID", climber.systemID);
            cmd.addParameter("@visitType", visitTypeName);
            cmd.addParameter("@start", now);
            try
            {
                cmd.executeScalar();
                retFlag = true;
            }
            catch (Exception ex)
            {
                this.AppendToLog("Execption creating visit. " + ex.Message);
            }
            return retFlag;
        }

        public bool finishVisit(User climber)
        {
            bool retFlag = false;
            cmd.reinitialize("UPDATE dbo.visits SET endDateTime = @out, duration = DATEDIFF(n, startDateTime, @out) "+
                "WHERE userID IN(SELECT userID FROM users WHERE firstName = @firstName AND lastName = @lastName) AND visitTypeID = 1 AND endDateTime IS NULL", conn);
            cmd.addParameter("@firstName", climber.firstName);
            cmd.addParameter("@lastName",  climber.lastName);
            cmd.addParameter("@out", DateTime.Now);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                this.AppendToLog("Exeception completing visit. " + ex.Message);
            }
            return retFlag;
        }

        public List<User> getSignedIn()
        {
            List<User> ret = new List<User>();
            cmd.reinitialize("SELECT * FROM dbo.visits JOIN dbo.users ON dbo.visits.userID = dbo.users.userID WHERE endDateTime IS NULL", conn);
            // this SqlCommand will need to be edited so that it only cares about tracked visit types.
            conn.Open();
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
            conn.Close();
            return ret;
        }

        //certifications
        public int addCertification(string title, int yearsBeforeExp)
        {
            int ret = -1;
            cmd.reinitialize("INSERT INTO dbo.certification (title, yearsBeforeExp) output INSERTED.certID VALUES (@title, @years)", conn);
            cmd.addParameter("@title", title);
            cmd.addParameter("@years", yearsBeforeExp);
            try
            {
                ret = (int)cmd.executeScalar();
            }
            catch (Exception ex)
            {
                this.AppendToLog("Exeception adding new certification. " + ex.Message);
            }
            return ret;
        }

        public Certification getCertification(int id)
        {
            cmd.reinitialize("SELECT * FROM dbo.certification WHERE certID = @id", conn);
            cmd.addParameter("@id", id);
            Certification ret = new Certification();
            conn.Open();
            using (SqlDataReader reader = cmd.executeReader())
            {
                if (reader.Read())
                {
                    ret.ID = (int)reader["certID"];
                    ret.title = (string)reader["title"];
                    ret.yearsBeforeExp = (int)reader["yearsBeforeExp"];
                }
            }
            conn.Close();
            return ret;
        }

        public bool removeCertification(Certification cert)
        {
            bool retFlag = false;
            cmd.reinitialize("DELETE FROM dbo.certification WHERE certID = @id", conn);
            cmd.addParameter("@id", cert.ID);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                this.AppendToLog("Exeception deleting certification. " + ex.Message);
            }
            return retFlag;
        }

        public List<Certification> getCerts()
        {
            List<Certification> ret = new List<Certification>();
            cmd.reinitialize("SELECT * FROM dbo.certification", conn);
            conn.Open();
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
            conn.Close();
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
                this.AppendToLog("Exeception certifying user. " + ex.Message);
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
                this.AppendToLog("Exeception clearing outdated certifications. " + ex.Message);
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
                this.AppendToLog("Exeception adding new term. " + ex.Message);
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
                this.AppendToLog("Exeception removing term. " + ex.Message);
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
                this.AppendToLog("Exeception adding new course. " + ex.Message);
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
                this.AppendToLog("Exeception removing course. " + ex.Message);
            }
            return retFlag;
        }

        public Course getCourse(int courseID)
        {
            cmd.reinitialize("SELECT * FROM dbo.course where courseID=@id", conn);
            cmd.addParameter("@id", courseID);
            Course temp = new Course();
            conn.Open();
            using (SqlDataReader reader = cmd.executeReader())
            {
                while (reader.Read())
                {
                    temp.ID = (int)reader["courseID"];
                    temp.title = (string)reader["title"];
                    temp.code = (string)reader["code"];
                    temp.days = (string)reader["daysOfWeek"];
                    temp.start = (TimeSpan)reader["startTime"];
                    temp.end = (TimeSpan)reader["endTime"];
                    temp.termID = (int)reader["term"];
                    if (!DBNull.Value.Equals(reader["certification"]))
                    {
                        temp.certID = (int)reader["certification"];
                    }
                    if (!DBNull.Value.Equals(reader["checkout"]))
                    { temp.equipID = (int)reader["checkout"]; }
                    
                }
            }
            conn.Close();
            return temp;

        }

        public List<Course> getCourses()
        {
            List <Course> ret = new List<Course>();
            cmd.reinitialize("SELECT * FROM dbo.course", conn);
            conn.Open();
            using (SqlDataReader reader = cmd.executeReader())
            {
                while (reader.Read())
                {
                    Course temp = new Course();
                    temp.ID = (int)reader["courseID"];
                    temp.title = (string)reader["title"];
                    temp.code = (string)reader["code"];
                    temp.days = (string)reader["daysOfWeek"];
                    temp.start = (TimeSpan)reader["startTime"];
                    temp.end = (TimeSpan)reader["endTime"];
                    temp.termID = (int)reader["term"];
                    if (!DBNull.Value.Equals(reader["certification"]))
                    {
                        temp.certID = (int)reader["certification"];
                    }
                    if (!DBNull.Value.Equals(reader["checkout"]))
                    { temp.equipID = (int)reader["checkout"]; }
                    ret.Add(temp);
                }
            }
            conn.Close();
            return ret;
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
                this.AppendToLog("Exeception enrolling user in course. " + ex.Message);
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
                this.AppendToLog("Exeception unenrolling user from course. " + ex.Message);
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
                this.AppendToLog("Exeception adding equipment type. " + ex.Message);
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
                this.AppendToLog("Exeception adding equipment. " + ex.Message);
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
                this.AppendToLog("Exception removing equipment type. " + ex.Message);
            }
            return retFlag;
        }

        public Dictionary<string[], int> equipInventory()
        {
            Dictionary<string[], int> ret = new Dictionary<string[], int>();
            string[] temp = new string[2]; 

            cmd.reinitialize("SELECT * FROM dbo.equipment", conn);
            conn.Open();
            using (SqlDataReader reader = cmd.executeReader())
            {
                while (reader.Read())
                {
                    temp[0] = (string)reader["name"];
                    temp[1] = (string)reader["size"];
                    ret.Add(temp, (int)reader["equipID"]);
                }
            }
            conn.Close();
            return ret;
        }

        public string[] getInventoryData(string name, string size)
        {
            string[] ret = new string[4];
            cmd.reinitialize("SELECT * FROM dbo.equipment WHERE name = @name AND size = @size", conn);
            cmd.addParameter("@name", name);
            cmd.addParameter("@size", size);
            conn.Open();
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
            conn.Close();
            return ret;
        }
        //equipmentuse
        public bool equipCheckout(User climber, int equipID)
        {
            bool retFlag = false;
            cmd.reinitialize("INSERT INTO dbo.equipmentuse (userID, equipID, checkoutDateTime) VALUES ( @u, @e, @c)", conn);
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
                this.AppendToLog("Exeception checking out equipment. " + ex.Message);
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
                this.AppendToLog("Exception adding visit type. " + ex.Message);
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
                this.AppendToLog("Exception removing visit type. " + ex.Message);
            }
            return retFlag;
        }

        public List<string> getVisitTypes()
        {
            List<string> ret = new List<string>();
            cmd.reinitialize("SELECT * FROM dbo.visittype", conn);
            conn.Open();
            using(SqlDataReader reader = cmd.executeReader())
            {
                while (reader.Read())
                {
                    ret.Add((string)reader["title"]);
                }
            }
            conn.Close();
            return ret;
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
                this.AppendToLog("Exception adding contact. " + ex.Message);
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
                conn.Open();
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
                conn.Close();
            }
            catch (Exception ex)
            {
                this.AppendToLog("Exception getting user contact. " + ex.Message);
            }
        
            return ret;
        }

        //sign in
        public bool addSignIn(string userName, string password, User newStaff)
        {
            bool retFlag = false;
            cmd.reinitialize("INSERT INTO dbo.signin (userName, password, userID) VALUES (@u, @p, @id)", conn);
            cmd.addParameter("@u", userName);
            cmd.addParameter("@p", password);
            cmd.addParameter("@id", newStaff.systemID);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                this.AppendToLog("Exception adding staff member. " + ex.Message);
            }
            return retFlag;
        }

        public bool removeSignIn(User climber)
        {
            bool retFlag = false;
            cmd.reinitialize("DELETE FROM dbo.signin WHERE userID = @u)", conn);
            cmd.addParameter("@u", climber.systemID);
            try
            {
                cmd.execute();
                retFlag = true;
            }
            catch (Exception ex)
            {
                this.AppendToLog("Exception removing staff member. " + ex.Message);
            }
            return retFlag;
        }

        public bool isValidSignIn(string userName, string password)
        {
            bool retFlag = false;
            cmd.reinitialize("SELECT * FROM dbo.signin WHERE userName=@u AND password=@p", conn);
            cmd.addParameter("@u", userName);
            cmd.addParameter("@p", password);
            try
            {
                conn.Open();
                using(SqlDataReader reader = cmd.executeReader())
                {
                    if (reader.Read())
                        retFlag = true;
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                this.AppendToLog("Exception looking up staff member. " + ex.Message);
            }
            return retFlag;
        }
        
        //update
        public void updateName(string firstName, string lastName, int userID)
        {
            cmd.reinitialize("UPDATE dbo.users SET firstName = @firstName, lastName = @lastName WHERE userID = @userID", conn);
            cmd.addParameter("@firstName", firstName);
            cmd.addParameter("@lastName", lastName);
            cmd.addParameter("@userID", userID);
            try { cmd.execute(); }
            catch (Exception ex)
            {
                this.AppendToLog("Exception updating user. " + ex.Message);
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
                this.AppendToLog("Exception updating user. " + ex.Message);
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
                this.AppendToLog("Exception updating user. " + ex.Message);
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
                this.AppendToLog("Exception updating user. " + ex.Message);
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
                this.AppendToLog("Exception updating user. " + ex.Message);
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
                this.AppendToLog("Exception updating user. " + ex.Message);
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
                this.AppendToLog("Exception updating user. " + ex.Message);
            }

        }

        public void updateCertification(string title, int yearsValid, int sysID)
        {
            cmd.reinitialize("UPDATE dbo.certification SET title = @t, yearsBeforeExp = @y WHERE certID = @c", conn);
            cmd.addParameter("@t", title);
            cmd.addParameter("@y", yearsValid);
            cmd.addParameter("@c", sysID);
            try { cmd.execute(); }
            catch (Exception ex) { this.AppendToLog("Exception updating certification. " + ex.Message); }
        }

        //reports
        public List<string[]> courseReport(Course c)
        {
            List <string[]> ret = new List<string[]>();
            string[] temp = { "User Type", "First Name", "Last Name" };
            ret.Add(temp);
            cmd.reinitialize("SELECT firstName, lastName, userType FROM dbo.enrolled INNER JOIN dbo.users ON dbo.users.userID = dbo.enrolled.userID WHERE courseID = @cID", conn);
            cmd.addParameter("@cID", c.ID);
            try
            {
                conn.Open();
                using (SqlDataReader reader = cmd.executeReader())
                {
                    while (reader.Read())
                    {
                        temp[0] = (string)reader["userType"];
                        temp[1] = (string)reader["firstName"];
                        temp[2] = (string)reader["lastName"];
                        ret.Add(temp);
                    }
                }
                conn.Close();
            } catch(Exception ex)
            {
                this.AppendToLog("Exception generating course report." + ex.Message);
            }
            return ret;
        }

        public List<string[]> allCourseReport()
        {
            List<string[]> ret = new List<string[]>();
            
            ret.Add(new string[]{ "User Type", "First Name", "Last Name", "Course" });
            cmd.reinitialize("SELECT firstName, lastName, userType, code FROM dbo.enrolled AS e INNER JOIN dbo.users AS u ON e.userID = u.userID INNER JOIN dbo.course AS c ON c.courseID = e.courseID ORDER BY c.code", conn);
            try
            {
                conn.Open();
                using (SqlDataReader reader = cmd.executeReader())
                {
                    while (reader.Read())
                    {
                        string[] temp = new string[4];
                        temp[0] = (string)reader["userType"];
                        temp[1] = (string)reader["firstName"];
                        temp[2] = (string)reader["lastName"];
                        temp[3] = (string)reader["code"];
                        ret.Add(temp);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                this.AppendToLog("Exception generating course report." + ex.Message);
            }
            return ret;
        }

        public List<string[]> visitReport(DateTime start, DateTime end)
        {
            List<string[]> ret = new List<string[]>();
            string[] temp = { "SID", "Last Name", "First Name", "Visit Type", "Duration" };
            ret.Add(temp);
            cmd.reinitialize("SELECT SID, lastName, First Name, title, duration FROM dbo.visits AS v INNER JOIN dbo.users AS u ON v.userID = u.userID INNER JOIN dbo.visittype AS t ON v.visitTypeID = t.visitTypeID WHERE startDateTime >= @s AND startDateTime <= @e", conn);
            cmd.addParameter("@s", start);
            cmd.addParameter("@e", end);
            try
            {
                conn.Open();
                using (SqlDataReader reader = cmd.executeReader())
                {
                    while (reader.Read())
                    {
                        temp[0] = (string)reader["SID"];
                        temp[1] = (string)reader["lastName"];
                        temp[2] = (string)reader["firstName"];
                        temp[3] = (string)reader["title"];
                        temp[4] = (string)reader["duration"];
                        ret.Add(temp);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                this.AppendToLog("Exception generating visit report." + ex.Message);
            }
            return ret;
        }

        public List<string[]> allVisitReport()
        {
            List<string[]> ret = new List<string[]>();
            ret.Add(new string[] { "SID", "Last Name", "First Name", "Visit Type", "Duration" });
            cmd.reinitialize("SELECT SID, lastName, firstName, title, duration FROM dbo.visits AS v INNER JOIN dbo.users AS u ON v.userID = u.userID INNER JOIN dbo.visittype AS t ON v.visitTypeID = t.visitTypeID", conn);
            try
            {
                conn.Open();
                using (SqlDataReader reader = cmd.executeReader())
                {
                    while (reader.Read())
                    {
                        string[] temp = new string[5];
                        temp[0] = (string)reader["SID"];
                        temp[1] = (string)reader["lastName"];
                        temp[2] = (string)reader["firstName"];
                        temp[3] = (string)reader["title"];
                        int t = (int)reader["duration"];
                        temp[4] = t.ToString();
                        ret.Add(temp);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                this.AppendToLog("Exception generating visit report." + ex.Message);
            }
            return ret;
        }

        public List<string[]> certificationReport(Certification cert)
        {
            List<string[]> ret = new List<string[]>();
            string[] temp = { "Certification", "Last Name", "First Name", "Expiration Date" };
            ret.Add(temp);
            cmd.reinitialize("SELECT title, lastName, firstName, expDate FROM dbo.usercertifications AS uc INNER JOIN dbo.users AS u ON uc.userID = u.userID INNER JOIN dbo.certification AS c ON c.certID = uc.certID WHERE certID = @cID ORDER BY lastName", conn);
            cmd.addParameter("@cID", cert.ID);
            try
            {
                conn.Open();
                using (SqlDataReader reader = cmd.executeReader())
                {
                    while (reader.Read())
                    {
                        temp[0] = (string)reader["title"];
                        temp[1] = (string)reader["lastName"];
                        temp[2] = (string)reader["firstName"];
                        temp[3] = (string)reader["expDate"];
                        ret.Add(temp);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                this.AppendToLog("Exception generating certification report." + ex.Message);
            }
            return ret;
        }

        public List<string[]> allCertificationReport()
        {
            List<string[]> ret = new List<string[]>();
            ret.Add(new string[] { "Certification", "Last Name", "First Name", "Expiration Date" });
            cmd.reinitialize("SELECT title, lastName, firstName, expDate FROM dbo.usercertifications AS uc INNER JOIN dbo.users AS u ON uc.userID = u.userID INNER JOIN dbo.certification AS c ON c.certID = uc.certID ORDER BY title", conn);
            try
            {
                conn.Open();
                using (SqlDataReader reader = cmd.executeReader())
                {
                    while (reader.Read())
                    {
                        string[] temp = new string[4];
                        temp[0] = (string)reader["title"];
                        temp[1] = (string)reader["lastName"];
                        temp[2] = (string)reader["firstName"];
                        DateTime t = (DateTime)reader["expDate"];
                        temp[3] = t.ToString("MM-dd-yyyy");
                        ret.Add(temp);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                this.AppendToLog("Exception generating certification report." + ex.Message);
            }
            return ret;
        }

        private void AppendToLog(string error)
        {
            string path = "./Error_Log.txt";
            FileStream file = new FileStream(path, FileMode.Append);
            using (StreamWriter fout = new StreamWriter(file))
            {
                fout.Write(DateTime.Now.ToString() + ": " + error + "\n");
            }
        }

    }

}

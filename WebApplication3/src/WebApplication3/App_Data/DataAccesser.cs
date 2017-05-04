using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Models;

namespace WebApplication3.App_Data
{
    public class DataAccesser
    {
        private SqlConnection conn { get; set; }
        private SqlCommandInterface cmd;

        public DataAccesser(string connString, bool testMode)
        {
            conn = new SqlConnection(connString);
            conn.Open();
            if (testMode)
                cmd = new TestCommand();
            else
                cmd = new SqlCommandWrapper();
        }

        ~DataAccesser()
        {
            conn.Close();
        }

        // users
        public User findUser(string firstName, string lastName)
        {
            User ret = new User();
            cmd.reinitialize("SELECT * FROM dbo.users WHERE firstName = @firstName AND lastName = @lastName", conn);
            cmd.addParameter("@firstName", firstName);
            cmd.addParameter("@lastName", lastName);
            using (SqlDataReader reader = cmd.executeReader())
            {
                if (reader.Read())
                {
                    ret.ID = (int)reader["userID"];
                    ret.firstName = (string)reader["firstName"];
                    ret.lastName = (string)reader["lastName"];
                }
            }
            return ret;
        }

        public User findUser(string CardSwipe) {
            User ret = new User();
            if (char.IsLetter(CardSwipe.First()))
            {
                //TO DO: FIX THIS SHIT.
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
                    ret.ID = (int)reader["userID"];
                    ret.firstName = (string)reader["firstName"];
                    ret.lastName = (string)reader["lastName"];

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
            string phone = args[4];
            string email = args[5];
            string shoeSize = args[6];
            string harnessSize = args[7];
            int num;
            int miles;
            bool flag = Int32.TryParse(args[8], out num);
            if (flag) { miles = num; } else { miles = -1; }
            int ret = -1;

            cmd.reinitialize("INSERT INTO dbo.users (userType, firstName, lastName, SID, phone, email, shoeSize, harnessSize, mile) " +
                "output INSERTED.ID VALUES(@userType, @firstName, @lastName, @sid, @phoneNumber, @email, @shoeSize, @harnessSize, @miles)", conn);
            cmd.addParameter("@userType", utype);
            cmd.addParameter("@firstName", firstName);
            cmd.addParameter("@lastName", lastName);
            cmd.addParameter("@sid", sid);
            cmd.addParameter("@phone", phone);
            cmd.addParameter("@email", email);
            cmd.addParameter("@shoeSize", shoeSize);
            cmd.addParameter("@harnessSize", harnessSize);
            if (miles >= 0)
                cmd.addParameter("@miles", miles);

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

        //visits
        public bool addVisit(User climber)
        {
            //TO DO: IMPLEMENT SEPARATE VISIT TYPES
            bool retFlag = false;
            cmd.reinitialize("createVisit", conn);
            cmd.isStoredProcedure();
            cmd.addParameter("@userID", climber.ID);
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

        public bool finishVisit(string firstName, string lastName)
        {
            bool retFlag = false;
            cmd.reinitialize("finishVisit", conn);
            cmd.isStoredProcedure();
            cmd.addParameter("@firstName", firstName);
            cmd.addParameter("@lastName",  lastName);

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
            cmd.reinitialize("SELECT * FROM dbo.certifications", conn);
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
        public bool certifyUser(User student, Certification cert)
        {
            //TO DO: FIGURE OUT HOW TO GET ID OF CURRENTLY LOGGED IN USER
            bool retFlag = false;
            DateTime today = DateTime.Today;
            cmd.reinitialize("INSERT INTO dbo.usercertifications (userID, certID, datePosted, postedBy, expDate) VALUES (@uID, @cID, @now, 0, @dateExp", conn);
            cmd.addParameter("@uID", student.ID);
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

        public bool cleanUserCert(User student, Certification cert)
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

        public bool removeTerm(int termID)
        {
            bool retFlag = false;
            cmd.reinitialize("DELETE FROM dbo.term WHERE termID = @id", conn);
            cmd.addParameter("@id", termID);
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
            cmd.addParameter("@uid", u.ID);
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
            cmd.addParameter("@uid", u.ID);
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

        public int addEquip(string name, string size)
        {
            int ret = -1;
            cmd.reinitialize("UPDATE dbo.equipment SET count = count + 1 WHERE name = @name AND size = @size", conn);
            cmd.addParameter("@name", name);
            cmd.addParameter("@size", size);
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

        public int removeEquip(string name, string size)
        {
            int ret = -1;
            cmd.reinitialize("UPDATE dbo.equipment SET count = count - 1 WHERE name = @name AND size = @size", conn);
            cmd.addParameter("@name", name);
            cmd.addParameter("@size", size);
            try
            {
                ret = (int)cmd.executeScalar();
            }
            catch (Exception ex)
            {
                throw new Exception("Exeception removing equipment. " + ex.Message);
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

        //equipmentuse
        //add user/equipment pair, remove outdated records, maybe remove the check-in column? do we need it? who knows?

        //visittype
        public bool addVisitType(string visitTypeName, int certID, int courseID) {
            bool retFlag = false;
            
            System.Diagnostics.Debug.WriteLine("you are inside addVisitType");
            
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

        public void updateName(string firstName, string lastName, int userID)
        {

        }

        public void updateStudentID(string studentID, int userID)
        {

        }
        public void updateShoeSize(string shoeSize, int userID)
        {

        }
        public void updateHarnessSize(string harnessSize, int userID)
        {

        }
        public void updatePhone(string phoneNum, int userID)
        {

        }
        public void updateEmail(string email, int userID)
        {

        }
        public void updateUserType(string userType, int userID)
        {

        }


        //add, remove
    }

   
}

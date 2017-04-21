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
        private SqlCommand cmd;

        public DataAccesser(string connString)
        {
            conn = new SqlConnection(connString);
            conn.Open();
        }

        ~DataAccesser()
        {
            conn.Close();
        }

        // users
        public User findUser(string firstName, string lastName)
        {
            User ret = new User();
            cmd = new SqlCommand("SELECT * FROM dbo.users WHERE firstName = @firstName AND lastName = @lastName", conn);
            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);
            using (SqlDataReader reader = cmd.ExecuteReader())
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
                cmd = new SqlCommand("SELECT * FROM dbo.users WHERE @@INSERTCOLNAME@@ = @netID", conn);
                cmd.Parameters.AddWithValue("@netID", CardSwipe);
            }
            else
            {
                cmd = new SqlCommand("SELECT * FROM dbo.users WHERE SID = @sid", conn);
                cmd.Parameters.AddWithValue("@sid", CardSwipe);
            }
            using (SqlDataReader reader = cmd.ExecuteReader())
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

            cmd = new SqlCommand("INSERT INTO dbo.users (userType, firstName, lastName, SID, phone, email, shoeSize, harnessSize, mile) " +
                "output INSERTED.ID VALUES(@userType, @firstName, @lastName, @sid, @phoneNumber, @email, @shoeSize, @harnessSize, @miles)", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@userType", utype);
            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);
            cmd.Parameters.AddWithValue("@sid", sid);
            cmd.Parameters.AddWithValue("@phone", phone);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@shoeSize", shoeSize);
            cmd.Parameters.AddWithValue("@harnessSize", harnessSize);
            if (miles >= 0)
                cmd.Parameters.AddWithValue("@miles", miles);

            try
            {
                ret = (int)cmd.ExecuteScalar();
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
            cmd = new SqlCommand("createVisit", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@userID", climber.ID);
            cmd.Parameters.AddWithValue("@visitType", "test type");

            try
            {
                cmd.ExecuteScalar();
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
            cmd = new SqlCommand("finishVisit", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName",  lastName);

            try
            {
                cmd.ExecuteNonQuery();
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
            cmd = new SqlCommand("INSERT INTO dbo.certifications (title, yearsBeforeExp) output INSERTED.ID VALUES (@title, @years)");
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@years", yearsBeforeExp);
            try
            {
                ret = (int)cmd.ExecuteScalar();
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
            cmd = new SqlCommand("DELETE FROM dbo.certifications WHERE certID = @id");
            cmd.Parameters.AddWithValue("@id", cert.ID);
            try
            {
                cmd.ExecuteNonQuery();
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
            cmd = new SqlCommand("SELECT * FROM dbo.certifications", conn);
            using (SqlDataReader reader = cmd.ExecuteReader())
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
            cmd = new SqlCommand("INSERT INTO dbo.usercertifications (userID, certID, datePosted, postedBy, expDate) VALUES (@uID, @cID, @now, 0, @dateExp", conn);
            cmd.Parameters.AddWithValue("@uID", student.ID);
            cmd.Parameters.AddWithValue("@cID", cert.ID);
            cmd.Parameters.AddWithValue("@now", today);
            cmd.Parameters.AddWithValue("@dateExp", today.AddYears(cert.yearsBeforeExp));
            try
            {
                cmd.ExecuteNonQuery();
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
            cmd = new SqlCommand("DELETE FROM dbo.usercertifications WHERE year(@today) - year(expDate) >= 6", conn);
            cmd.Parameters.AddWithValue("@today", DateTime.Today);
            try
            {
                cmd.ExecuteNonQuery();
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
            cmd = new SqlCommand("INSERT INTO dbo.term (quarter, year, startDate, endDate) output INSERTED.ID VALUES (@q, @y, @s, @e", conn);
            cmd.Parameters.AddWithValue("@q", quarter);
            cmd.Parameters.AddWithValue("@y", year);
            cmd.Parameters.AddWithValue("@s", startDate);
            cmd.Parameters.AddWithValue("@e", endDate);
            try
            {
                ret = (int)cmd.ExecuteScalar();
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
            cmd = new SqlCommand("DELETE FROM dbo.term WHERE termID = @id", conn);
            cmd.Parameters.AddWithValue("@id", termID);
            try
            {
                cmd.ExecuteNonQuery();
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
            cmd = new SqlCommand("INSERT INTO dbo.course (title, code, daysOfWeek, startTime, endTime, term, checkout, certification) " +
                "output INSERTED.ID VALUES (@title, @code, @days, @start, @end, @term, @equip, @cert)", conn);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@code", code);
            cmd.Parameters.AddWithValue("@days", days);
            cmd.Parameters.AddWithValue("@start", start);
            cmd.Parameters.AddWithValue("@end", end);
            cmd.Parameters.AddWithValue("@term", term);
            if(equip >= 0)
                cmd.Parameters.AddWithValue("@equip", equip);
            if(cert >= 0)
                cmd.Parameters.AddWithValue("@cert", cert);

            try
            {
                ret = (int)cmd.ExecuteScalar();
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
            cmd = new SqlCommand("DELETE FROM dbo.course WHERE courseID = @id", conn);
            cmd.Parameters.AddWithValue("@id", courseID);
            try
            {
                cmd.ExecuteNonQuery();
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
            cmd = new SqlCommand("INSERT INTO dbo.enrolled (userID, courseID, dateTimeEnrolled) output INSERTED.ID VALUES (@uid, @cid, @now)", conn);
            cmd.Parameters.AddWithValue("@uid", u.ID);
            cmd.Parameters.AddWithValue("@cid", c.ID);
            cmd.Parameters.AddWithValue("@now", DateTime.Now);
            try
            {
                ret = (int)cmd.ExecuteScalar();
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
            cmd = new SqlCommand("DELETE FROM dbo.enrolled WHERE userID = @uid AND courseID = @cid", conn);
            cmd.Parameters.AddWithValue("@uid", u.ID);
            cmd.Parameters.AddWithValue("@cid", c.ID);
            try
            {
                cmd.ExecuteNonQuery();
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
            cmd = new SqlCommand("INSERT INTO dbo.equipment (name, size) VALUES (@name, @size)", conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@size", size);
            try
            {
                cmd.ExecuteNonQuery();
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
            cmd = new SqlCommand("UPDATE dbo.equipment SET count = count + 1 WHERE name = @name AND size = @size", conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@size", size);
            try
            {
                ret = (int)cmd.ExecuteScalar();
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
            cmd = new SqlCommand("UPDATE dbo.equipment SET count = @newCount WHERE name = @name AND size = @size", conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@size", size);
            cmd.Parameters.AddWithValue("@newCount", count);
            try
            {
                ret = (int)cmd.ExecuteScalar();
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
            cmd = new SqlCommand("UPDATE dbo.equipment SET count = count - 1 WHERE name = @name AND size = @size", conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@size", size);
            try
            {
                ret = (int)cmd.ExecuteScalar();
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
            cmd = new SqlCommand("DELETE FROM dbo.equipment (name, size) WHERE name = @name AND size = @size", conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@size", size);
            try
            {
                cmd.ExecuteNonQuery();
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
            
            cmd = new SqlCommand("INSERT INTO dbo.visittype (title, certID, courseID) VALUES(@title, @certID, @courseID)", conn);
            cmd.Parameters.AddWithValue("@title", visitTypeName);
            cmd.Parameters.AddWithValue("@certID", certID);
            cmd.Parameters.AddWithValue("@courseID", courseID);
            try
            {
                cmd.ExecuteNonQuery();
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
            cmd = new SqlCommand("DELETE FROM dbo.visittype WHERE title = @title", conn);
            cmd.Parameters.AddWithValue("@title", titleToRemove);
            try
            {
                cmd.ExecuteNonQuery();
                retFlag = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception removing visit type. " + ex.Message);
            }




            return retFlag;
        }
        //add, remove
    }

   
}

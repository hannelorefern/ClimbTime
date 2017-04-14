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

        public Student findUser(string firstName, string lastName)
        {
            Student ret = new Student();
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

        public bool addVisit(Student climber)
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
            int miles = Convert.ToInt32(args[8]);
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

        public int addCertification(string title, int yearsBeforeExp)
        {
            int ret = -1;
            cmd = new SqlCommand("INSERT INTO dbo.certifications output INSERTED.ID (title, yearsBeforeExp) VALUES (@title, @years)");
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

        public bool certifyUser(Student student, Certification cert)
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

        public bool cleanUserCert(Student student, Certification cert)
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

        //add/remove term
        

        //add/remove course

        //enroll/unenroll user

        //add/remove equipment

        //checkout equipment

        //add/remove visittype
    }
}

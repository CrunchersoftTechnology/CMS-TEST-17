using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Services;
using System.Collections;
using System.Net;
using System.IO;
using System.Text;
using System.Net.NetworkInformation;
using System.Web.Script.Serialization;

namespace CMS.Web
{
    public partial class getregDataNew : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string currentDate = System.DateTime.Now.AddDays(1).ToShortDateString();

        }
        public class UserDetails
        {
            public string stud_name { get; set; }
            public string reg_date { get; set; }
            public string exp_date { get; set; }
            public string reg_key { get; set; }
            public string device_id { get; set; }
            public string server_Date { get; set; }
            public string ClassName { get; set; }

        }

        public class AttendanceDetails1
        {
            public string ADate { get; set; }
            public string Astatus { get; set; }
        }

        public class regStudDetails
        {
            public string stud_id { get; set; }
            public string client_id { get; set; }
            public string status { get; set; }
            public string allow_fees { get; set; }
            public string allow_result { get; set; }
            public string allow_attendance { get; set; }
        }

        public static string reg_id1 = "";
        public static string device_id1 = "";
        SqlConnection con1 = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
        DateTime dt = new DateTime();
        string currentDate = System.DateTime.Now.ToShortDateString();

        public static DateTime serverTime = DateTime.Now;
        public static DateTime utcTime = serverTime.ToUniversalTime();
        //convert it to Utc using timezone setting of server computer
        public static TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public static DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
        public static string app_code2 = "";

        [WebMethod]
        public static Array getUserData(string device_id, string app_code, string className)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
            app_code2 = app_code;
            List<UserDetails> details = new List<UserDetails>();

            string str = "select top 1 * from regInfoDetails where device_id='" +
                device_id + "' and Product_Id='" + app_code + "' and classCom_Name='" + className + "' order by Id desc";

            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                int t2 = 0;
                int count = 0;
                UserDetails user = new UserDetails();


                while (dr.Read())
                {
                    count++;
                    user.stud_name = dr["stud_name"].ToString();
                    user.reg_date = dr["reg_date"].ToString();
                    user.exp_date = dr["exp_date"].ToString();
                    user.reg_key = dr["reg_key"].ToString();
                    user.device_id = dr["device_id"].ToString();
                    user.server_Date = localTime.ToString("ddMMyyyy");
                    user.ClassName = dr["classCom_Name"].ToString();
                    device_id1 = user.device_id;
                    // insertData();
                    details.Add(user);
                }



                return details.ToArray();
            }

        }

        public static void insertData()
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");

            using (con)
            {
                con.Open();
                SqlCommand cmd2 = new SqlCommand("insert into RegHistory values('" + reg_id1 + "','" + device_id1 + "','" + app_code2 + "','" + localTime.ToString() + "')", con);
                cmd2.ExecuteNonQuery();
            }
        }

        [WebMethod]
        public static Array getAttendanceDetails(string dtFrom, string dtTo, string stud_id, string client_id)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
            List<AttendanceDetails1> Attendancedetails = new List<AttendanceDetails1>();
            DateTime dtFrom2 = Convert.ToDateTime(dtFrom);
            DateTime dtTo2 = DateTime.Parse(dtTo);


            string str = @"select ADate,Status from tblOMRAttendance where S_Id=@sid and C_Id=@cid and ADate between @df and @dt ";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                cmd.Parameters.AddWithValue("@sid", stud_id);
                cmd.Parameters.AddWithValue("@cid", client_id);
                cmd.Parameters.AddWithValue("@df", DateTime.Parse(dtFrom));
                cmd.Parameters.AddWithValue("@dt", DateTime.Parse(dtTo));
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    AttendanceDetails1 atten_details = new AttendanceDetails1();
                    atten_details.ADate = dr["ADate"].ToString();
                    atten_details.Astatus = dr["Status"].ToString();
                    Attendancedetails.Add(atten_details);
                }

                return Attendancedetails.ToArray();
            }
        }
        [WebMethod]

        public static Array checkOMRStudnet(string username, string password, string playerId)
        {
            string sid = "";
            string cid = "";
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
            string str = "select * from tblOMRRegistration where UserName='" + username + "' and Password='" + password + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            List<regStudDetails> regDetails = new List<regStudDetails>();
            regStudDetails ob = new regStudDetails();
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ob.client_id = dr["C_Id"].ToString();
                    ob.stud_id = dr["S_Id"].ToString();
                    ob.status = dr["Status"].ToString();
                    ob.allow_attendance = dr["AllowAttendance"].ToString();
                    ob.allow_fees = dr["AllowFees"].ToString();
                    ob.allow_result = dr["AllowResult"].ToString();
                    sid = dr["S_Id"].ToString();
                    cid = dr["C_Id"].ToString();
                }
                regDetails.Add(ob);
            }

            if (sid != "" && cid != "")
            {
                string sname = "";
                string className = "";
                string ac = "";
                SqlConnection con1 = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
                using (con1)
                {
                    con1.Open();

                    string strDetails = "select * from tblOMRStudentDetails where S_Id='" + sid + "' and C_Id='" + cid + "'";
                    SqlCommand cmdDetails = new SqlCommand(strDetails, con1);
                    SqlDataReader dr = cmdDetails.ExecuteReader();
                    while (dr.Read())
                    {
                        sname = dr["Student_Name"].ToString();
                        className = dr["Class"].ToString();
                        ac = dr["Ac_Year"].ToString();
                    }

                    dr.Close();

                    string strCount = "select count(*) from tblOMRAppRegistration where S_Id='" + sid + "' and C_Id='" + cid + "'";
                    SqlCommand cmdCount = new SqlCommand(strCount, con1);
                    int count = Convert.ToInt32(cmdCount.ExecuteScalar());
                    if (count == 0)
                    {
                        string strNew = "insert into tblOMRAppRegistration(S_Id, C_Id, Student_Name, Class, Ac_Year, App_Id) values('" + sid + "','" + cid + "', '" + sname + "','" + className + "','" + ac + "','" + playerId + "')";
                        SqlCommand cmd = new SqlCommand(strNew, con1);
                        int updateCount = cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string strUpdate = "update tblOMRAppRegistration set App_Id='" + playerId + "', Class='" + className + "', Ac_Year='" + ac + "', Student_Name='" + sname + "' where S_Id='" + sid + "' and C_Id='" + cid + "'";
                        SqlCommand cmd = new SqlCommand(strUpdate, con1);
                        int updateCount = cmd.ExecuteNonQuery();
                    }
                }
            }
            return regDetails.ToArray();
        }


        public class StudentResultData
        {
            public string test_id { get; set; }
            public string paper_name { get; set; }
            public string c_ans { get; set; }
            public string inc_ans { get; set; }
            public string tot_mark { get; set; }
            public string ob_mark { get; set; }
            public string t_rank { get; set; }
            public string t_dt { get; set; }
            public string t_topper { get; set; }
        }
        [WebMethod]
        public static Array getStudentResultData(string stud_id, string client_id)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
            string str = "select * from tblOMRTestResult where S_Id='" + stud_id + "' and C_Id='" + client_id + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            List<StudentResultData> regDetails = new List<StudentResultData>();

            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    StudentResultData ob = new StudentResultData();
                    ob.test_id = dr["Test_Id"].ToString();
                    ob.ob_mark = dr["Obtained_Marks"].ToString();
                    ob.tot_mark = dr["Total_Marks"].ToString();
                    ob.c_ans = dr["Correct_Answer"].ToString();
                    ob.inc_ans = dr["Incorrect_Answer"].ToString();
                    ob.paper_name = dr["Paper_Name"].ToString();
                    ob.t_rank = dr["Rank"].ToString();
                    ob.t_dt = dr["TDate"].ToString();
                    ob.t_topper = dr["Topper"].ToString();
                    regDetails.Add(ob);
                }

            }
            return regDetails.ToArray();
        }


        public class StudentOverallResultData
        {
            public string s_name { get; set; }
            public string ob_mark { get; set; }
            public string tot_mark { get; set; }
            public string t_perc { get; set; }
            public string t_rank { get; set; }
        }
        [WebMethod]
        public static Array getStudentOverallResultData(string stud_id, string client_id)
        {
            List<StudentOverallResultData> regDetails = new List<StudentOverallResultData>();
            try
            {
                SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
                string str = "select * from tblOMROverallResult where S_Id='" + stud_id + "' and C_Id='" + client_id + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";


                using (con)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(str, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        StudentOverallResultData ob = new StudentOverallResultData();
                        ob.ob_mark = dr["OverallObtained"].ToString();
                        ob.tot_mark = dr["OverallTotal"].ToString();
                        ob.s_name = dr["StudName"].ToString();
                        ob.t_rank = dr["OverallRank"].ToString();
                        ob.t_perc = dr["Percentage"].ToString();
                        regDetails.Add(ob);
                    }

                }
            }
            catch
            {

            }
            return regDetails.ToArray();
        }

        public class StudentOverallResultTopperData
        {
            public string ts_name { get; set; }
            public string tob_mark { get; set; }
            public string ttot_mark { get; set; }
            public string tt_perc { get; set; }
            public string tt_rank { get; set; }
        }
        [WebMethod]
        public static Array getStudentOverallResultTopperData(string client_id, string stud_id)
        {
            List<StudentOverallResultTopperData> regDetails = new List<StudentOverallResultTopperData>();
            try
            {
                SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
                string str = "select * from tblOMROverallResult where OverallRank='1' and C_Id='" + client_id + "' and (Ac_Year IN((select Ac_Year FROM tblOMROverallResult WHERE (S_Id = '" + stud_id + "')))) AND (Class IN ((SELECT Class FROM tblOMROverallResult WHERE (S_Id = '" + stud_id + "'))))";

                using (con)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(str, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        StudentOverallResultTopperData ob = new StudentOverallResultTopperData();
                        ob.tob_mark = dr["OverallObtained"].ToString();
                        ob.ttot_mark = dr["OverallTotal"].ToString();
                        ob.ts_name = dr["StudName"].ToString();
                        ob.tt_rank = dr["OverallRank"].ToString();
                        ob.tt_perc = dr["Percentage"].ToString();
                        regDetails.Add(ob);
                    }

                }
            }
            catch
            {

            }
            return regDetails.ToArray();
        }


        [WebMethod]
        public static void setOMRStudentPassword(string sid, string cid, string pwd)
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");

                using (con)
                {
                    con.Open();
                    SqlCommand cmd2 = new SqlCommand("update tblOMRRegistration set Password='" + pwd + "' where S_Id='" + sid + "' and C_Id='" + cid + "' ", con);
                    cmd2.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

            }
        }


        public class getStudentFees
        {
            public string total_fee { get; set; }
            public string paid_fee { get; set; }
            public string rem_fee { get; set; }
        }

        [WebMethod]
        public static Array getStudentFeesData(string stud_id, string client_id)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
            string str = "select * from tblOMRFeesDetail where S_Id='" + stud_id + "' and C_Id='" + client_id + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            List<getStudentFees> feesDetails = new List<getStudentFees>();
            getStudentFees ob = new getStudentFees();
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    ob.total_fee = dr["Total_Fees"].ToString();
                    ob.paid_fee = dr["Paid_Fees"].ToString();
                    ob.rem_fee = dr["Remaining_Fees"].ToString();
                    feesDetails.Add(ob);
                }

            }
            return feesDetails.ToArray();
        }



        public class aboutClass
        {
            public string class_name { get; set; }
            public string head_name { get; set; }
            public string contact_No { get; set; }
            public string email_id { get; set; }
            public string address { get; set; }
            public string website { get; set; }
            public string about_class { get; set; }
        }
        [WebMethod]
        public static Array getClassData(string client_id)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
            string str = "select * from tblOMRClassDetails where C_Id='" + client_id + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            List<aboutClass> classDetails = new List<aboutClass>();
            aboutClass ob = new aboutClass();
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    ob.class_name = dr["Class_Name"].ToString();
                    ob.head_name = dr["Head_Name"].ToString();
                    ob.contact_No = dr["Contact_No"].ToString();

                    ob.email_id = dr["Email_Id"].ToString(); ;
                    ob.address = dr["Address"].ToString(); ;
                    ob.about_class = dr["About_Class"].ToString(); ;
                    ob.website = dr["Class_URL"].ToString(); ;
                    classDetails.Add(ob);
                }

            }
            return classDetails.ToArray();
        }

        public class getStudentFeesHistory
        {
            public string receipt_no { get; set; }
            public string paid_date { get; set; }
            public string paid_fee { get; set; }

        }
        [WebMethod]
        public static Array getStudentFeesHistoryData(string stud_id, string client_id)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
            string str = "select * from tblOMRFeesHistory where S_Id='" + stud_id + "' and C_Id='" + client_id + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            List<getStudentFeesHistory> feesDetails = new List<getStudentFeesHistory>();

            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    getStudentFeesHistory ob = new getStudentFeesHistory();
                    ob.receipt_no = dr["Receipt_No"].ToString();
                    DateTime dt = Convert.ToDateTime(dr["FDate"]);
                    ob.paid_date = dt.ToShortDateString();
                    ob.paid_fee = dr["Paid_Fees"].ToString();
                    feesDetails.Add(ob);
                }

            }
            return feesDetails.ToArray();
        }


        public class StudentInfo
        {
            public string stud_name { get; set; }
            public string className { get; set; }
            public string ac_year { get; set; }
            public string stud_mobNo { get; set; }
            public string parent_mobNo { get; set; }
        }
        [WebMethod]
        public static Array getStudentInfo(string stud_id, string client_id)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
            string str = "select * from tblOMRStudentDetails where S_Id='" + stud_id + "' and C_Id='" + client_id + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            List<StudentInfo> studDetails = new List<StudentInfo>();
            StudentInfo ob = new StudentInfo();
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    ob.stud_name = dr["Student_Name"].ToString();
                    ob.className = dr["Class"].ToString();
                    ob.ac_year = dr["Ac_Year"].ToString();
                    ob.stud_mobNo = dr["Student_No"].ToString();
                    ob.parent_mobNo = dr["Parent_No"].ToString();
                    studDetails.Add(ob);
                }

            }
            return studDetails.ToArray();
        }

        public class getCMSStudentFees
        {
            public string total_fee { get; set; }
            public string paid_fee { get; set; }
        }


        [WebMethod]
        public static Array getCMSStudentFeesData(string stud_id)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cmstestcruncher;User ID=cmstestcruncher;Password=cmstestcruncher@123");
            List<getCMSStudentFees> feesDetails = new List<getCMSStudentFees>();
            getCMSStudentFees ob = new getCMSStudentFees();


            string str = "SELECT CONVERT(int, ROUND(SUM(i.Payment), 0)) AS Payment, CONVERT(int, ROUND(AVG(s.FinalFees), 0)) AS FinalFees FROM dbo.Installments AS i INNER JOIN dbo.Students AS s ON i.UserId = s.UserId WHERE (i.UserId = '" + stud_id + "')";
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ob.paid_fee = dr["Payment"].ToString();
                    ob.total_fee = dr["FinalFees"].ToString();
                    if (dr["Payment"].ToString() != "")
                        feesDetails.Add(ob);
                }
            }

            if (feesDetails.Count == 0)
            {
                SqlConnection conFinalFee = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cmstestcruncher;User ID=cmstestcruncher;Password=cmstestcruncher@123");
                string strFinalFee = "SELECT CONVERT(int, ROUND(AVG(FinalFees), 0)) AS FinalFees FROM dbo.Students WHERE (UserId = '" + stud_id + "')";
                using (conFinalFee)
                {
                    conFinalFee.Open();
                    SqlCommand cmd = new SqlCommand(strFinalFee, conFinalFee);
                    int finalFee = Convert.ToInt32(cmd.ExecuteScalar());
                    ob.paid_fee = "0";
                    ob.total_fee = finalFee.ToString();
                    feesDetails.Add(ob);
                }
            }

            return feesDetails.ToArray();
        }

        public class getCMSStudentFeesHistory
        {
            public string paid_date { get; set; }
            public string payment { get; set; }

        }

        [WebMethod]
        public static Array getCMSStudentFeesHistoryData(string stud_id)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            string str = "select * from Installments where UserId='" + stud_id + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            List<getCMSStudentFeesHistory> feesDetails = new List<getCMSStudentFeesHistory>();

            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    getCMSStudentFeesHistory ob = new getCMSStudentFeesHistory();
                    DateTime dt = Convert.ToDateTime(dr["CreatedOn"]);
                    ob.paid_date = dt.ToShortDateString();
                    ob.payment = dr["Payment"].ToString();
                    feesDetails.Add(ob);
                }

            }
            return feesDetails.ToArray();
        }

        public class getCMSSendingSMS
        {
            public string resultCode { get; set; }
            public string UserId { get; set; }
            public string ParentContact { get; set; }
            public string StudentName { get; set; }
            public string ClassName { get; set; }
            public string SId { get; set; }
            public string ClassId { get; set; }
            public string SelectedSubjects { get; set; }
            public string StudentContact { get; set; }
            public string IsActive { get; set; }
            public string SchoolName { get; set; }
            public string Email { get; set; }
            public string DOJ { get; set; }
            public string BranchName { get; set; }
            public string PhotoPath { get; set; }
            public string BranchId { get; set; }
            public string BatchId { get; set; }
            public string NotificationId { get; set; }
            public string SubjectName { get; set; }
            public string BatchName { get; set; }
            public string MaxOfflineTestPaperId { get; set; }
            public string MaxOfflineTestStudentMarksId { get; set; }
        }

        [WebMethod]
        public static Array sendingMessage(string message, string username, string password, string parentContact, string sender, string lastName, string dob)
        {
            string sendto = "";
            List<string> subjectsList = new List<string>();
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            //string str = "SELECT s.SId, s.MiddleName, s.UserId, s.LastName, c.Name, c.ClassId, s.FirstName, s.ParentContact, s.StudentContact, s.DOB, s.SelectedBatches,
            //s.IsActive, s.DOJ FROM dbo.Students AS s INNER JOIN dbo.Classes AS c ON s.ClassId = c.ClassId where s.ParentContact='" + parentContact + "' and
            //s.LastName = '" + lastName + "' and Cast(s.DOB as Date) = '" + dob + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";

            string strString = "SELECT c.Name AS ClassName, c.ClassId as ClassId, s.SId, s.FirstName, s.MiddleName, s.LastName, s.DOJ, s.ParentContact, s.StudentContact, s.PhotoPath, s.IsActive, s.SelectedSubject, s.BatchId, sc.Name AS SchoolName, " +
                         " br.Name AS BranchName, br.BranchId, u.Email, b.Name AS BatchName, sbj.Name AS SubjectName, s.UserId, CONVERT(varchar(15), CAST(b.InTime AS TIME), 100) " +
                        " AS BatchInTime, CONVERT(varchar(15), CAST(b.OutTime AS TIME), 100) AS BatchOutTime FROM dbo.StudentSubjects AS sb INNER JOIN dbo.Subjects AS sbj ON sb.SubjectId = sbj.SubjectId INNER JOIN" +
                        " dbo.Schools AS sc INNER JOIN dbo.Students AS s INNER JOIN dbo.Classes AS c ON s.ClassId = c.ClassId ON sc.SchoolId = s.SchoolId INNER JOIN" +
                         " dbo.Branches AS br ON s.BranchId = br.BranchId INNER JOIN dbo.AspNetUsers AS u ON s.UserId = u.Id ON sb.UserId = s.UserId INNER JOIN" +
                         " dbo.Batches AS b ON s.BatchId = b.BatchId where s.ParentContact='" + parentContact + "' and s.LastName = '" + lastName + "' and Cast(s.DOB as Date) = '" + dob + "'";

            List<getCMSSendingSMS> StudentDetails = new List<getCMSSendingSMS>();
            getCMSSendingSMS ob = new getCMSSendingSMS();
            ob.resultCode = "Student is not registered in class.";
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(strString, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ob.UserId = dr["UserId"].ToString();
                    ob.ParentContact = dr["ParentContact"].ToString();
                    ob.StudentContact = dr["StudentContact"].ToString();
                    ob.ClassName = dr["ClassName"].ToString();
                    ob.SId = dr["SId"].ToString();
                    ob.ClassId = dr["ClassId"].ToString();
                    ob.SelectedSubjects = dr["SelectedSubject"].ToString();
                    sendto = dr["ParentContact"].ToString();
                    ob.StudentName = dr["FirstName"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                    ob.IsActive = dr["IsActive"].ToString();
                    ob.BranchName = dr["BranchName"].ToString();
                    //ob.SubjectWithBatch = dr[""].ToString();
                    ob.SchoolName = dr["SchoolName"].ToString();
                    ob.Email = dr["Email"].ToString();
                    ob.DOJ = dr["DOJ"].ToString().Split(' ')[0];
                    ob.BranchId = dr["BranchId"].ToString();
                    ob.BatchId = dr["BatchId"].ToString();
                    ob.BatchName = dr["BatchName"].ToString();
                    ob.PhotoPath = dr["PhotoPath"].ToString().Split(' ')[0];
                    subjectsList.Add(dr["SubjectName"].ToString());
                }
                ob.SubjectName = string.Join(", ", subjectsList);

                dr.Close();
                int maxNotificationId = 0;
                string strMaxId = "select Max(NotificationId) from Notifications";
                string strMaxOfflineTestPaperId = "select Max(OfflineTestPaperId) from OfflineTestPapers";
                string strMaxOfflineTestStudentMarksId = "select Max(OfflineTestStudentMarksId) from OfflineTestStudentMarks";
                //using (con1)
                //{
                // con1.Open();
                SqlCommand cmdMaxId = new SqlCommand(strMaxId, con);
                string dataNew = cmdMaxId.ExecuteScalar().ToString();
                if (dataNew != "")
                    maxNotificationId = Convert.ToInt32(dataNew);

                SqlCommand cmdMaxOfflineTestPaperId = new SqlCommand(strMaxOfflineTestPaperId, con);
                string MaxOfflineTestPaperId = cmdMaxOfflineTestPaperId.ExecuteScalar().ToString();
                ob.MaxOfflineTestPaperId = MaxOfflineTestPaperId != "" ? MaxOfflineTestPaperId : "0";

                SqlCommand cmdMaxOfflineTestStudentMarksId = new SqlCommand(strMaxOfflineTestStudentMarksId, con);
                string MaxOfflineTestStudentMarksId = cmdMaxOfflineTestStudentMarksId.ExecuteScalar().ToString();
                ob.MaxOfflineTestStudentMarksId = MaxOfflineTestStudentMarksId != "" ? MaxOfflineTestStudentMarksId : "0";
                //}


                ob.NotificationId = maxNotificationId.ToString();

                StudentDetails.Add(ob);

                if (sendto != "" && StudentDetails.Select(x => x.ParentContact).FirstOrDefault() != "" && StudentDetails.Select(x => x.IsActive).FirstOrDefault() == "True")
                {
                    String WebResponseString = "";
                    String URL = "";
                    try
                    {
                        //URL = "http://103.16.101.52:8080/bulksms/bulksms?username=" + username + "&password=" + password + "&type=0&dlr=1&destination=" + sendto + "&source=" + sender + "&message=" + message + "";

                        URL = "http://173.45.76.227/send.aspx?username=ctpune&pass=Ctpune@1&route=trans1&senderid=ctpune&numbers=" + sendto + "&message=" + message + "";

                        WebRequest webrequest; WebResponse webresponse;
                        webrequest = HttpWebRequest.Create(URL);//Hit URL Link
                        webrequest.Timeout = 25000;
                        try
                        {
                            webresponse = webrequest.GetResponse();//Get Response
                            StreamReader reader = new StreamReader(webresponse.GetResponseStream()); //Read Response and store in variable
                            WebResponseString = reader.ReadToEnd();
                            ob.resultCode = WebResponseString;
                            webresponse.Close();
                        }
                        catch (Exception ex)
                        {
                            WebResponseString = "Request Timeout";
                            ob.resultCode = WebResponseString;
                            return ex.Message.ToArray();
                            #region sms code
                            //1701: Success, Message Submitted Successfully, In this case you will  receive
                            //the response 1701|<CELL_NO>:<MESSAGE ID>, The message Id can
                            //then be used later to map the delivery reports to this message.
                            //1702: Invalid URL Error, This means that one of the parameters was not
                            //provided or left blank
                            //1703: Invalid value in username or password field
                            //1704: Invalid value in "type" field
                            //1705: Invalid Message
                            //1706: Invalid Destination
                            //1707: Invalid Source (Sender)
                            //1708: Invalid value for "dlr" field
                            //1709: User validation failed
                            //1710: Internal Error
                            //1025 :Insufficient Credit
                            //1032 : Destination in DND
                            //1033 : Sender / Template Mismatch 
                            #endregion
                        }
                    }
                    catch
                    {

                    }

                    string[] ggg = ob.resultCode.Split('|');
                    if (ggg[0] == "6")
                    {
                        ob.resultCode = "Invalid URL Error, This means that one of the parameters was not provided or left blank.";
                        ob.resultCode = "Invalid URL Error, This means that one of the parameters was not provided or left blank.";
                    }
                    else if (ggg[0] == "4")
                    {
                        ob.resultCode = "Message can't sent, Internet is not connected";
                    }
                    else if (ggg[0] == "5")
                    {
                        ob.resultCode = "Invalid Source (Sender Id), Plz Set Valid SenderId";
                        ob.resultCode = "Sorry Failed To send Message, Try Again with Valid Sender Id";
                    }
                    else if (ggg[0] == "2")
                    {
                        ob.resultCode = "Invalid Destination";
                    }
                    else if (ggg[0] == "1")
                    {
                        ob.resultCode = "sent";
                    }
                    else if (ggg[0] == "3")
                    {
                        ob.resultCode = "Insufficient credit";
                    }
                    else if (ggg[0] == "7")
                    {
                        ob.resultCode = "Submission Error.";
                    }
                }

            }
            return StudentDetails.ToArray();
        }

        public class getAttendanceDetailsOMR
        {
            public string ADate { get; set; }
            public string Astatus { get; set; }
        }

        [WebMethod]
        public static Array getStudentAttendance(string stud_id, string client_id)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
            List<getAttendanceDetailsOMR> Attendancedetails = new List<getAttendanceDetailsOMR>();

            string str = @"select ADate,Status from tblOMRAttendance where S_Id=@sid and C_Id=@cid";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                cmd.Parameters.AddWithValue("@sid", stud_id);
                cmd.Parameters.AddWithValue("@cid", client_id);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    getAttendanceDetailsOMR atten_details = new getAttendanceDetailsOMR();
                    atten_details.ADate = dr["ADate"].ToString();
                    atten_details.Astatus = dr["Status"].ToString();
                    Attendancedetails.Add(atten_details);
                }

                return Attendancedetails.ToArray();
            }
        }

        public class getCMSAttendance
        {
            public string ADate { get; set; }
            public string SubjectName { get; set; }
            public string BatchName { get; set; }
            public string InTime { get; set; }
            public string OutTime { get; set; }
            public string Month { get; set; }
            public string Status { get; set; }
        }

        [WebMethod]
        public static Array getCMSAttendanceData(string SId, string ClassId, string selectedBatches, string branchId)
        {
            List<getCMSAttendance> attendanceDetails = new List<getCMSAttendance>();
            int status = 0;
            try
            {
                SqlConnection con = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
                //string str = "select MONTH(a.Date) as MonthOfDate, FORMAT(a.Date, 'dd-MM-yyyy') as ADate, a.BatchId, b.Name as BatchName, FORMAT(b.InTime, 'hh:mm tt') as InTime, FORMAT(b.OutTime, 'hh:mm tt') as OutTime, a.StudentAttendence from Attendances as a INNER JOIN Batches as b ON a.BatchId = b.BatchId where a.ClassId = '" + ClassId + "' and a.BatchId IN (" + selectedBatches + ") order by a.Date";
                string str = "select MONTH(a.Date) as MonthOfDate, FORMAT(a.Date, 'dd-MM-yyyy') as ADate, a.StudentAttendence from Attendances as a where a.ClassId = '" + ClassId + "' and a.BatchId IN (" + selectedBatches + ") and a.BranchId= '" + branchId + "' order by a.Date";
                using (con)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(str, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        status = 0;
                        getCMSAttendance attendance = new getCMSAttendance();
                        attendance.ADate = dr["ADate"].ToString();
                        attendance.Month = dr["MonthOfDate"].ToString();
                        string[] SIdList = dr["StudentAttendence"].ToString().Split(',');
                        if (SIdList.Contains(SId))
                        {
                            status = 1;
                        }
                        if (status == 1)
                            attendance.Status = "Present";
                        else
                            attendance.Status = "Absent";
                        attendanceDetails.Add(attendance);
                    }
                }
            }
            catch
            {
            }
            return attendanceDetails.ToArray();
        }

        public class getCMSAttendanceCount
        {
            public string[] TotalCount { get; set; }
            public string[] PresentCount { get; set; }
        }

        [WebMethod]
        public static Array getCMSAttendanceDataCount(string SId, string ClassId, string selectedBatches, string branchId)
        {
            List<getCMSAttendanceCount> attendanceDetails = new List<getCMSAttendanceCount>();
            List<string> tCount = new List<string>();
            List<string> presentCount = new List<string>();
            getCMSAttendanceCount attendance = new getCMSAttendanceCount();
            int strMonth1 = 0;
            int strMonth2 = 0;
            int strMonth3 = 0;
            int strMonth4 = 0;
            int strMonth5 = 0;
            int strMonth6 = 0;
            int strMonth7 = 0;
            int strMonth8 = 0;
            int strMonth9 = 0;
            int strMonth10 = 0;
            int strMonth11 = 0;
            int strMonth12 = 0;
            try
            {
                SqlConnection con = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
                string str = "select MONTH(Date) as DateMonth, StudentAttendence from Attendances where ClassId = '" + ClassId + "' and BranchId= '" + branchId + "' and BatchId IN (" + selectedBatches + ")";
                using (con)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(str, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string[] SIdList = dr["StudentAttendence"].ToString().Split(',');
                        if (SIdList.Contains(SId))
                        {
                            string strMonth = dr["DateMonth"].ToString();
                            if (strMonth == "1")
                                strMonth1 += 1;
                            else if (strMonth == "2")
                                strMonth2 += 1;
                            else if (strMonth == "3")
                                strMonth3 += 1;
                            else if (strMonth == "4")
                                strMonth4 += 1;
                            else if (strMonth == "5")
                                strMonth5 += 1;
                            else if (strMonth == "6")
                                strMonth6 += 1;
                            else if (strMonth == "7")
                                strMonth7 += 1;
                            else if (strMonth == "8")
                                strMonth8 += 1;
                            else if (strMonth == "9")
                                strMonth9 += 1;
                            else if (strMonth == "10")
                                strMonth10 += 1;
                            else if (strMonth == "11")
                                strMonth11 += 1;
                            else if (strMonth == "12")
                                strMonth12 += 1;
                        }
                    }
                }

                presentCount.Add(strMonth1.ToString());
                presentCount.Add(strMonth2.ToString());
                presentCount.Add(strMonth3.ToString());
                presentCount.Add(strMonth4.ToString());
                presentCount.Add(strMonth5.ToString());
                presentCount.Add(strMonth6.ToString());
                presentCount.Add(strMonth7.ToString());
                presentCount.Add(strMonth8.ToString());
                presentCount.Add(strMonth9.ToString());
                presentCount.Add(strMonth10.ToString());
                presentCount.Add(strMonth11.ToString());
                presentCount.Add(strMonth12.ToString());

                for (int i = 1; i <= 12; i++)
                {
                    SqlConnection con1 = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
                    string strCount = "SELECT count(*) from Attendances where MONTH(Date)='" + i + "' and ClassId = '" + ClassId + "' and BranchId= '" + branchId + "' and BatchId IN (" + selectedBatches + ")";
                    using (con1)
                    {
                        SqlCommand cmd = new SqlCommand(strCount, con1);
                        con1.Open();
                        tCount.Add(cmd.ExecuteScalar().ToString());
                    }
                }
                attendance.PresentCount = presentCount.ToArray();
                attendance.TotalCount = tCount.ToArray();
                attendanceDetails.Add(attendance);
            }
            catch
            {
            }
            return attendanceDetails.ToArray();
        }

        public class setPlayerIdCMSParentApp
        {
            public string result { get; set; }
        }

        [WebMethod]
        public static string Ping()
        {

            return "Pong";
        }

        [WebMethod]
        public static Array setPlayerIdCMSParentAppData(string userId, string playerId)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            string str = "select * from Students where UserId='" + userId + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            List<setPlayerIdCMSParentApp> setPlayerId = new List<setPlayerIdCMSParentApp>();
            string parentAppPlayerId = "";
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    parentAppPlayerId = dr["parentAppPlayerId"].ToString();
                }
            }
            str = "update Students set parentAppPlayerId='" + playerId + "' where UserId='" + userId + "'";
            SqlConnection con1 = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            using (con1)
            {
                con1.Open();
                SqlCommand cmd = new SqlCommand(str, con1);
                int updateCount = cmd.ExecuteNonQuery();
                setPlayerIdCMSParentApp ob = new setPlayerIdCMSParentApp();
                if (updateCount == 0)
                {
                    ob.result = "not found";
                    setPlayerId.Add(ob);
                    return setPlayerId.ToArray();
                }
                else if (updateCount == 1)
                {
                    if (parentAppPlayerId == "")
                    {
                        ob.result = "set";
                        setPlayerId.Add(ob);
                        return setPlayerId.ToArray();
                    }
                    else
                    {
                        ob.result = "already set";
                        setPlayerId.Add(ob);
                        return setPlayerId.ToArray();
                    }
                }
            }
            return setPlayerId.ToArray();
        }

        public class getStudentBatches
        {
            public string SelectedBatches { get; set; }
        }

        [WebMethod]
        public static Array getStudentBatchesData(string userId)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            string str = "select * from Students where UserId='" + userId + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            List<getStudentBatches> getBatches = new List<getStudentBatches>();
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    getStudentBatches ob = new getStudentBatches();
                    ob.SelectedBatches = dr["SelectedBatches"].ToString();
                    getBatches.Add(ob);
                }
            }

            return getBatches.ToArray();
        }

        [WebMethod]
        public static Array GetStudentDetails(string userId)
        {
            string sendto = "";
            List<string> subjectsList = new List<string>();
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            //string str = "SELECT s.SId, s.MiddleName, s.UserId, s.LastName, c.Name, c.ClassId, s.FirstName, s.ParentContact, s.StudentContact, s.DOB, s.SelectedBatches, s.IsActive, s.DOJ FROM dbo.Students AS s INNER JOIN dbo.Classes AS c ON s.ClassId = c.ClassId where s.ParentContact='" + parentContact + "' and s.LastName = '" + lastName + "' and Cast(s.DOB as Date) = '" + dob + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";

            string strString = "SELECT c.Name AS ClassName, c.ClassId as ClassId, s.SId, s.FirstName, s.MiddleName, s.LastName, s.DOJ, s.ParentContact, s.StudentContact, s.PhotoPath, s.IsActive, s.SelectedSubject, s.BatchId, sc.Name AS SchoolName, " +
                         " br.Name AS BranchName, br.BranchId, u.Email, b.Name AS BatchName, sbj.Name AS SubjectName, s.UserId, CONVERT(varchar(15), CAST(b.InTime AS TIME), 100) " +
                        " AS BatchInTime, CONVERT(varchar(15), CAST(b.OutTime AS TIME), 100) AS BatchOutTime FROM dbo.StudentSubjects AS sb INNER JOIN dbo.Subjects AS sbj ON sb.SubjectId = sbj.SubjectId INNER JOIN" +
                        " dbo.Schools AS sc INNER JOIN dbo.Students AS s INNER JOIN dbo.Classes AS c ON s.ClassId = c.ClassId ON sc.SchoolId = s.SchoolId INNER JOIN" +
                         " dbo.Branches AS br ON s.BranchId = br.BranchId INNER JOIN dbo.AspNetUsers AS u ON s.UserId = u.Id ON sb.UserId = s.UserId INNER JOIN" +
                         " dbo.Batches AS b ON s.BatchId = b.BatchId where s.UserId='" + userId + "'";

            List<getCMSSendingSMS> StudentDetails = new List<getCMSSendingSMS>();
            getCMSSendingSMS ob = new getCMSSendingSMS();
            ob.resultCode = "Student is not registered in class.";
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(strString, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ob.UserId = dr["UserId"].ToString();
                    ob.ParentContact = dr["ParentContact"].ToString();
                    ob.StudentContact = dr["StudentContact"].ToString();
                    ob.ClassName = dr["ClassName"].ToString();
                    ob.SId = dr["SId"].ToString();
                    ob.ClassId = dr["ClassId"].ToString();
                    ob.SelectedSubjects = dr["SelectedSubject"].ToString();
                    sendto = dr["ParentContact"].ToString();
                    ob.StudentName = dr["FirstName"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                    ob.IsActive = dr["IsActive"].ToString();
                    ob.BranchName = dr["BranchName"].ToString();
                    //ob.SubjectWithBatch = dr[""].ToString();
                    ob.SchoolName = dr["SchoolName"].ToString();
                    ob.Email = dr["Email"].ToString();
                    ob.DOJ = dr["DOJ"].ToString().Split(' ')[0];
                    ob.BranchId = dr["BranchId"].ToString();
                    ob.BatchId = dr["BatchId"].ToString();
                    ob.BatchName = dr["BatchName"].ToString();
                    ob.PhotoPath = dr["PhotoPath"].ToString().Split(' ')[0];
                    subjectsList.Add(dr["SubjectName"].ToString());
                }
                ob.SubjectName = string.Join(", ", subjectsList);

                int maxNotificationId = 0;
                string strMaxId = "select Max(NotificationId) from Notifications";
                SqlConnection con1 = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
                using (con1)
                {
                    con1.Open();
                    SqlCommand cmdMaxId = new SqlCommand(strMaxId, con1);
                    string dataNew = cmdMaxId.ExecuteScalar().ToString();
                    if (dataNew != "")
                        maxNotificationId = Convert.ToInt32(cmdMaxId.ExecuteScalar());
                }

                ob.NotificationId = maxNotificationId.ToString();
                StudentDetails.Add(ob);
                return StudentDetails.ToArray();
            }
        }

        public class getCMSTimeTable
        {
            public string Description { get; set; }
            public string FileName { get; set; }
            public string Date { get; set; }
            public string Category { get; set; }
            public string AttachmentDescription { get; set; }
        }

        [WebMethod]
        public static Array GetStudentTimeTable(string branchId, string classId, string batchId)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            string str = "select * from StudentTimetables";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            List<getCMSTimeTable> details = new List<getCMSTimeTable>();
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    getCMSTimeTable ob = new getCMSTimeTable();
                    string[] branchList = dr["SelectedBranches"].ToString().Split(',');
                    if (branchList.Contains(branchId))
                    {
                        if (dr["SelectedClasses"].ToString() == "" && dr["SelectedBatches"].ToString() == "")
                        {
                            ob.Category = dr["Category"].ToString();
                            ob.Date = dr["StudentTimetableDate"].ToString();
                            ob.Description = dr["Description"].ToString();
                            ob.FileName = dr["FileName"].ToString();
                            ob.AttachmentDescription = dr["AttachmentDescription"].ToString();
                            details.Add(ob);
                        }

                        else
                        {
                            string[] classList = dr["SelectedClasses"].ToString().Split(',');
                            if (classList.Contains(classId))
                            {
                                if (dr["SelectedBatches"].ToString() != "")
                                {
                                    string[] batchList = dr["SelectedBatches"].ToString().Split(',');
                                    #region rough
                                    //string[] studentbatchList = selectedbatches.Split(',');
                                    //int count = 0;
                                    //foreach (var batch in studentbatchList)
                                    //{
                                    //    if (batchList.Contains(batch))
                                    //    {
                                    //        count++;
                                    //    }
                                    //} 
                                    #endregion
                                    if (batchList.Contains(batchId))
                                    {
                                        ob.Category = dr["Category"].ToString();
                                        ob.Date = dr["StudentTimetableDate"].ToString();
                                        ob.Description = dr["Description"].ToString();
                                        ob.FileName = dr["FileName"].ToString();
                                        ob.AttachmentDescription = dr["AttachmentDescription"].ToString();
                                        details.Add(ob);
                                    }
                                }
                                else
                                {
                                    ob.Category = dr["Category"].ToString();
                                    ob.Date = dr["StudentTimetableDate"].ToString();
                                    ob.Description = dr["Description"].ToString();
                                    ob.FileName = dr["FileName"].ToString();
                                    ob.AttachmentDescription = dr["AttachmentDescription"].ToString();
                                    details.Add(ob);
                                }
                            }
                        }
                    }
                }
            }
            return details.ToArray();
        }

        public class getCMSDailyPracticePaper
        {
            public string Description { get; set; }
            public string FileName { get; set; }
            public string Date { get; set; }
            public string AttachmentDescription { get; set; }
        }

        [WebMethod]
        public static Array GetStudentDailyPracticePaper(string branchId, string classId, string batchId)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            string str = "select * from DailyPracticePapers";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            List<getCMSDailyPracticePaper> details = new List<getCMSDailyPracticePaper>();
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    getCMSDailyPracticePaper ob = new getCMSDailyPracticePaper();
                    string[] branchList = dr["SelectedBranches"].ToString().Split(',');
                    if (branchList.Contains(branchId))
                    {
                        if (dr["SelectedClasses"].ToString() == "" && dr["SelectedBatches"].ToString() == "")
                        {
                            ob.Date = dr["DailyPracticePaperDate"].ToString();
                            ob.Description = dr["Description"].ToString();
                            ob.FileName = dr["FileName"].ToString();
                            ob.AttachmentDescription = dr["AttachmentDescription"].ToString();
                            details.Add(ob);
                        }

                        else
                        {
                            string[] classList = dr["SelectedClasses"].ToString().Split(',');
                            if (classList.Contains(classId))
                            {
                                if (dr["SelectedBatches"].ToString() != "")
                                {
                                    string[] batchList = dr["SelectedBatches"].ToString().Split(',');
                                    #region rough
                                    //string[] studentbatchList = selectedbatches.Split(',');
                                    //int count = 0;
                                    //foreach (var batch in studentbatchList)
                                    //{
                                    //    if (batchList.Contains(batch))
                                    //    {
                                    //        count++;
                                    //    }
                                    //} 
                                    #endregion
                                    if (batchList.Contains(batchId))
                                    {
                                        ob.Date = dr["DailyPracticePaperDate"].ToString();
                                        ob.Description = dr["Description"].ToString();
                                        ob.FileName = dr["FileName"].ToString();
                                        ob.AttachmentDescription = dr["AttachmentDescription"].ToString();
                                        details.Add(ob);
                                    }
                                }
                                else
                                {
                                    ob.Date = dr["DailyPracticePaperDate"].ToString();
                                    ob.Description = dr["Description"].ToString();
                                    ob.FileName = dr["FileName"].ToString();
                                    ob.AttachmentDescription = dr["AttachmentDescription"].ToString();
                                    details.Add(ob);
                                }
                            }
                        }
                    }
                }
            }
            return details.ToArray();
        }

        public class getCMSNotification
        {
            public string NotificationId { get; set; }
            public string Message { get; set; }
            public string Date { get; set; }
            public string Category { get; set; }
        }

        [WebMethod]
        public static Array GetCMSNotification(string branchId, string classId, string selectedbatches, string notificationMaxId,
            string maxOfflineTestPaperId, string maxOfflineTestStudentMarksId, string userId, string selectedSubjects, string studentName)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            string str = "select * from Notifications where NotificationId > '" + notificationMaxId + "' and Media like '%AppNotification%'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            List<getCMSNotification> details = new List<getCMSNotification>();
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    getCMSNotification ob = new getCMSNotification();
                    string allUser = dr["AllUser"].ToString();
                    if (allUser != "True")
                    {
                        string[] branchList = dr["SelectedBranches"].ToString().Split(',');
                        if (branchList.Contains(branchId))
                        {
                            string[] classList = dr["SelectedClasses"].ToString().Split(',');
                            if (classList.Contains(classId))
                            {
                                string[] batchList = dr["SelectedBatches"].ToString().Split(',');
                                string[] studentbatchList = selectedbatches.Split(',');
                                int count = 0;
                                foreach (var batch in studentbatchList)
                                {
                                    if (batchList.Contains(batch))
                                    {
                                        count++;
                                    }
                                }
                                if (count > 0)
                                {
                                    ob.Date = dr["CreatedOn"].ToString();
                                    ob.Message = dr["NotificationMessage"].ToString();
                                    ob.NotificationId = dr["NotificationId"].ToString();
                                    ob.Category = "Notice";
                                    details.Add(ob);
                                }
                            }
                        }
                    }
                    else
                    {
                        ob.Date = dr["CreatedOn"].ToString();
                        ob.Message = dr["NotificationMessage"].ToString();
                        ob.NotificationId = dr["NotificationId"].ToString();
                        ob.Category = "Notice";
                        details.Add(ob);
                    }
                }

                string getOfflineTestQuery = "select * from OfflineTestPapers where OfflineTestPaperId > '" + maxOfflineTestPaperId + "' and Media like '%AppNotification%' and ClassId = '" + classId + "' and SubjectId in (" + selectedSubjects + ")";
                SqlCommand cmdOfflineTest = new SqlCommand(getOfflineTestQuery, con);
                dr.Close();
                SqlDataReader sdr = cmdOfflineTest.ExecuteReader();
                while (sdr.Read())
                {
                    string[] branchList = sdr["SelectedBranches"].ToString().Split(',');
                    if (branchList.Contains(branchId))
                    {
                        string[] batchList = sdr["SelectedBatches"].ToString().Split(',');
                        if (batchList.Contains(selectedbatches))
                        {
                            getCMSNotification ob = new getCMSNotification();
                            ob.Date = sdr["CreatedOn"].ToString();
                            ob.Message = "OfflineTest-<br />" + sdr["Title"].ToString() + " test paper on " + Convert.ToDateTime(sdr["TestDate"]).ToString("dd-MM-yyyy") + ", Total Marks - " + sdr["TotalMarks"].ToString();
                            ob.NotificationId = sdr["OfflineTestPaperId"].ToString();
                            ob.Category = "Test";
                            details.Add(ob);
                        }
                    }
                }

                string getOfflineTestMarksQuery = "select * from OfflineTestStudentMarks inner join OfflineTestPapers on OfflineTestStudentMarks.OfflineTestPaperId = OfflineTestPapers.OfflineTestPaperId where OfflineTestStudentMarksId > '" + maxOfflineTestStudentMarksId + "' and UserId = '" + userId + "'";
                SqlCommand cmdOfflineTestMarks = new SqlCommand(getOfflineTestMarksQuery, con);
                sdr.Close();
                SqlDataReader sdrMarks = cmdOfflineTestMarks.ExecuteReader();
                while (sdrMarks.Read())
                {
                    getCMSNotification ob = new getCMSNotification();
                    ob.Date = sdrMarks["CreatedOn"].ToString();
                    ob.Message = "Marks-<br />Name - " + studentName + "<br />Title - " + sdrMarks["Title"].ToString() + "<br />Marks - " + sdrMarks["ObtainedMarks"].ToString() + "/" +
                        sdrMarks["TotalMarks"].ToString() + "<br />Percentage - " + sdrMarks["Percentage"].ToString();
                    ob.NotificationId = sdrMarks["OfflineTestStudentMarksId"].ToString();
                    ob.Category = "Test";
                    details.Add(ob);
                }
            }
            return details.ToArray();
        }

        [WebMethod]
        public static string RegisterAndroidUserDetails(string device_id, string app_code, string classname, string studentName, string mobileNumber, string key,
            string installationDate, string expireDate)
        {
            string result = "";
            try
            {
                SqlConnection con1 = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
                List<UserDetails> details = new List<UserDetails>();

                using (con1)
                {
                    con1.Close();
                    con1.Open();
                    SqlCommand cmd2 = new SqlCommand("insert into regInfoDetails(reg_date, exp_date, device_id, reg_key, stud_name, mobile_no, classCom_Name, Product_Id) values('" + installationDate + "','" + expireDate + "','" + device_id + "','" + key + "', '" + studentName + "', '" + mobileNumber + "', '" + classname + "','" + app_code + "')", con1);
                    cmd2.ExecuteNonQuery();
                    result = "inserted";
                    con1.Close();
                }
                #region Rough
                //using (con)
                //{
                //string str = "select count(*) from regInfoDetails where device_id='" + device_id + "' and Product_Id='" + app_code + "'";//and exp_date > '" + expireDate + "'"; where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
                //    con.Open();
                //    SqlCommand cmd = new SqlCommand(str, con);
                //    int registrationCount = Convert.ToInt32(cmd.ExecuteScalar());
                //    if (registrationCount == 0)
                //    {
                //        SqlConnection con1 = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
                //    }
                //    else
                //    {
                //        SqlConnection con1 = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");

                //        using (con1)
                //        {
                //            con1.Close();
                //            con1.Open();
                //            SqlCommand cmd2 = new SqlCommand("update regInfoDetails set classCom_Name='" + classname + "', reg_date='" + installationDate + "', exp_date='" + expireDate + "', reg_key='" + key + "', stud_name='" + studentName + "', mobile_no='" + mobileNumber + "' where device_id='" + device_id + "' and classCom_Name='" + classname + "' and Product_Id='" + app_code + "'", con1);
                //            cmd2.ExecuteNonQuery();
                //            result = "updated";
                //            con1.Close();
                //        }
                //    }

                //} 
                #endregion
            }
            catch (Exception)
            {
            }
            return result;
        }

        [WebMethod]
        public static Array getStudentAttendanceOmClass(string stud_id, string client_id)
        {
            string academicYear = "";
            string classname = "";
            string batch = "";
            int rollNo = 0;

            SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cmsautoattendanceR1;User ID=cmsautoattendanceR1;Password=cmsautoattendanceR1@123");
            List<getAttendanceDetailsOMR> Attendancedetails = new List<getAttendanceDetailsOMR>();

            SqlConnection con1 = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cmsautoattendanceR1;User ID=cmsautoattendanceR1;Password=cmsautoattendanceR1@123");
            string getStudent = "SELECT s.RollNo, s.AcademicYear, s.Class, s.Batch, s.SId, s.MachineId, m.ClientClassId FROM dbo.Students AS s INNER JOIN dbo.Machines AS m ON s.MachineId = m.MachineSerial WHERE (s.SId = '" + stud_id + "') AND (m.ClientClassId = '" + client_id + "')";
            using (con1)
            {
                con1.Close();
                con1.Open();
                SqlCommand cmd = new SqlCommand(getStudent, con1);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    academicYear = dr["AcademicYear"].ToString();
                    classname = dr["Class"].ToString();
                    batch = dr["Batch"].ToString();
                    rollNo = Convert.ToInt32(dr["RollNo"].ToString());
                }
            }


            string str = "SELECT a.StudentsRollNo, a.Date, a.AcademicYear, a.Class, a.Batch FROM dbo.AttendanceDetails AS a INNER JOIN dbo.Machines AS m ON a.MachineSerial = m.MachineSerial INNER JOIN dbo.Students AS s ON m.MachineSerial = s.MachineId WHERE (m.ClientClassId = '" + client_id + "') GROUP BY a.StudentsRollNo, a.Date, a.AcademicYear, a.Class, a.Batch";
            //string str = "select StudentsRollNo, Date from Attendance where S_Id=@sid and C_Id=@cid";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                cmd.Parameters.AddWithValue("@sid", stud_id);
                cmd.Parameters.AddWithValue("@cid", client_id);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    if (academicYear == dr["AcademicYear"].ToString() &&
                        classname == dr["Class"].ToString() && batch == dr["Batch"].ToString())
                    {
                        getAttendanceDetailsOMR atten_details = new getAttendanceDetailsOMR();
                        atten_details.ADate = dr["Date"].ToString();
                        List<int> studentRollNo = (dr["StudentsRollNo"].ToString().Split(',')).ToList().Select(x => int.Parse(x)).ToList();

                        if (studentRollNo.Contains(rollNo))
                        {
                            atten_details.Astatus = "Present";
                        }
                        else
                        {
                            atten_details.Astatus = "Absent";
                        }

                        Attendancedetails.Add(atten_details);
                    }
                }

                return Attendancedetails.ToArray();
            }
        }

        public class AndroidOfflineRegistration
        {
            public string StudentName { get; set; }
            public string ClientName { get; set; }
            public string AppCode { get; set; }
            public string RegistrationDate { get; set; }
            public string ExpiryDate { get; set; }
            public string RegistrationKey { get; set; }
            public string DeviceId { get; set; }
            public string ServerDate { get; set; }
            public string Result { get; set; }
        }

        [WebMethod]
        public static AndroidOfflineRegistration RegisterAndroidStudentOnline(string device_id, string app_code, string classname, string studentName, string mobileNumber, string key,
            string installationDate, string expireDate)
        {
            List<AndroidOfflineRegistration> oldRegistrationDetails = new List<AndroidOfflineRegistration>();
            AndroidOfflineRegistration registrationDetails = new AndroidOfflineRegistration();
            try
            {
                SqlConnection con = new SqlConnection("Data Source=184.168.47.13;Initial Catalog=cruncherWeb;User ID=cruncher;Password=cruncher@2014");
                app_code2 = app_code;


                string str = "select count(*) from AndroidRegistrationKeys where RegistrationKey='" +
                    key + "' and ClientName ='" + classname + "' group by Id";
                con.Close();
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                int registrationCount = Convert.ToInt32(cmd.ExecuteScalar());

                if (registrationCount == 1)
                {
                    int installationDay = Convert.ToInt32(installationDate.Substring(0, 2));
                    int installationMonth = Convert.ToInt32(installationDate.Substring(2, 2));
                    int installationYear = Convert.ToInt32(installationDate.Substring(4, 4));

                    int expireDay = Convert.ToInt32(expireDate.Substring(0, 2));
                    int expireMonth = Convert.ToInt32(expireDate.Substring(2, 2));
                    int expireYear = Convert.ToInt32(expireDate.Substring(4, 4));


                    DateTime installationDeviceDate = new DateTime(installationYear, installationMonth, installationDay);
                    DateTime expireDeviceDate = new DateTime(expireYear, expireMonth, expireDay);

                    var serverDate = localTime.ToString("ddMMyyyy");

                    if (serverDate == installationDate)
                    {

                        string strRegistrationQuery = "select * from AndroidOfflineRegistrationDetails where RegistrationKey='" +
                        key + "' and ClientName ='" + classname + "'";

                        con.Close();
                        con.Open();
                        SqlCommand cmdRegistrationKey = new SqlCommand(strRegistrationQuery, con);
                        SqlDataReader dr = cmdRegistrationKey.ExecuteReader();

                        while (dr.Read())
                        {
                            registrationDetails.StudentName = dr["StudentName"].ToString();
                            registrationDetails.RegistrationDate = dr["RegistrationDate"].ToString();
                            registrationDetails.ExpiryDate = dr["ExpiryDate"].ToString();
                            registrationDetails.RegistrationKey = dr["DeviceId"].ToString();
                            registrationDetails.ServerDate = dr["ServerDate"].ToString();
                            registrationDetails.DeviceId = dr["DeviceId"].ToString();
                            registrationDetails.AppCode = dr["AppCode"].ToString();
                        }

                        dr.Close();

                        if (registrationDetails.DeviceId == null || registrationDetails.DeviceId == "")
                        {
                            string strOldRegistrationQuery = "select * from AndroidOfflineRegistrationDetails where DeviceId='" +
                            device_id + "' and ClientName ='" + classname + "' and AppCode = '" + app_code + "'";

                            con.Close();
                            con.Open();
                            SqlCommand cmdOldRegistrationKey = new SqlCommand(strOldRegistrationQuery, con);
                            SqlDataReader drOld = cmdOldRegistrationKey.ExecuteReader();

                            while (drOld.Read())
                            {
                                AndroidOfflineRegistration androidOfflineRegistration = new AndroidOfflineRegistration();

                                androidOfflineRegistration.StudentName = drOld["StudentName"].ToString();
                                androidOfflineRegistration.RegistrationDate = drOld["RegistrationDate"].ToString();
                                androidOfflineRegistration.ExpiryDate = drOld["ExpiryDate"].ToString();
                                androidOfflineRegistration.RegistrationKey = drOld["DeviceId"].ToString();
                                androidOfflineRegistration.ServerDate = drOld["ServerDate"].ToString();
                                androidOfflineRegistration.DeviceId = drOld["DeviceId"].ToString();

                                oldRegistrationDetails.Add(androidOfflineRegistration);
                            }

                            drOld.Close();

                            var getDetails = oldRegistrationDetails.Where(x => Convert.ToDateTime(x.ExpiryDate) > localTime.Date).ToList();

                            if (getDetails.Count == 0)
                            {
                                SqlCommand cmd2 = new SqlCommand(
                                    "insert into AndroidOfflineRegistrationDetails(StudentName, ClientName, AppCode, RegistrationDate, ExpiryDate, RegistrationKey, DeviceId, ServerDate) values('" +
                                    studentName + "','" + classname + "','" + app_code + "','" + installationDeviceDate.ToString("yyyy-MM-dd") + "', '" + expireDeviceDate.ToString("yyyy-MM-dd") + "', '" + key +
                                    "', '" + device_id + "','" + localTime.Date.ToString("yyyy-MM-dd") + "')", con);
                                cmd2.ExecuteNonQuery();
                                registrationDetails.Result = "Inserted";
                            }
                            else
                            {
                                registrationDetails.Result = "This device is already registered with another key.";
                                return registrationDetails;
                            }
                        }
                        else
                        {
                            var expireDateFromDb = Convert.ToDateTime(registrationDetails.ExpiryDate).Date;

                            if (registrationDetails.DeviceId == device_id && registrationDetails.AppCode == app_code)
                            {
                                if (localTime.Date <= expireDateFromDb)
                                {
                                    registrationDetails.RegistrationDate = Convert.ToDateTime(registrationDetails.RegistrationDate).ToString("ddMMyyyy");
                                    registrationDetails.ExpiryDate = Convert.ToDateTime(registrationDetails.ExpiryDate).ToString("ddMMyyyy");
                                    registrationDetails.ServerDate = serverDate;
                                    registrationDetails.Result = "Correct";
                                    return registrationDetails;
                                }
                                else
                                {
                                    registrationDetails.Result = "App is expired.";
                                    return registrationDetails;
                                }
                            }
                            else
                            {
                                registrationDetails.Result = "This registration key is already registered on another device.";
                                return registrationDetails;
                            }

                        }
                    }
                    else
                    {
                        registrationDetails.Result = "Please check device date!";
                        return registrationDetails;
                    }
                }
                else
                {
                    registrationDetails.Result = "Invalid registration key!";
                    return registrationDetails;
                }

            }
            catch (Exception)
            {
                registrationDetails.Result = "Internet";
            }
            return registrationDetails;
        }

        public class getCMSStudentSendingSMS
        {
            public string resultCode { get; set; }
            public string UserId { get; set; }
            public string ParentContact { get; set; }
            public string StudentName { get; set; }
            public string ClassName { get; set; }
            public string SId { get; set; }
            public string ClassId { get; set; }
            public string SelectedSubjects { get; set; }
            public string StudentContact { get; set; }
            public string IsActive { get; set; }
            public string SchoolName { get; set; }
            public string Email { get; set; }
            public string DOJ { get; set; }
            public string BranchName { get; set; }
            public string PhotoPath { get; set; }
            public string BranchId { get; set; }
            public string BatchId { get; set; }
            public string NotificationId { get; set; }
            public string OnlineTestId { get; set; }
            public string SubjectName { get; set; }
            public string BatchName { get; set; }
            public string PdfUploadId { get; set; }
            public string MaxOfflineTestPaperId { get; set; }
            public string MaxOfflineTestStudentMarksId { get; set; }
        }

        [WebMethod]
        public static Array sendingMessageStudentApp(string message, string username, string password, string studentContact, string sender, string lastName, string dob, string firstName, string emailId)
        {
            List<getCMSStudentSendingSMS> StudentDetails = new List<getCMSStudentSendingSMS>();
            getCMSStudentSendingSMS ob = new getCMSStudentSendingSMS();
            try
            {
                string sendto = "";
                List<string> subjectsList = new List<string>();
                SqlConnection con = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
                //string str = "SELECT s.SId, s.MiddleName, s.UserId, s.LastName, c.Name, c.ClassId, s.FirstName, s.ParentContact, s.StudentContact, s.DOB, s.SelectedBatches,
                //s.IsActive, s.DOJ FROM dbo.Students AS s INNER JOIN dbo.Classes AS c ON s.ClassId = c.ClassId where s.ParentContact='" + parentContact + "' and
                //s.LastName = '" + lastName + "' and Cast(s.DOB as Date) = '" + dob + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";

                string strString = "SELECT c.Name AS ClassName, c.ClassId as ClassId, s.SId, s.FirstName, s.MiddleName, s.LastName, s.DOJ, s.ParentContact, s.StudentContact, s.PhotoPath, s.IsActive, s.SelectedSubject, s.BatchId, sc.Name AS SchoolName, " +
                             " br.Name AS BranchName, br.BranchId, u.Email, b.Name AS BatchName, sbj.Name AS SubjectName, s.UserId, CONVERT(varchar(15), CAST(b.InTime AS TIME), 100) " +
                            " AS BatchInTime, CONVERT(varchar(15), CAST(b.OutTime AS TIME), 100) AS BatchOutTime FROM dbo.StudentSubjects AS sb INNER JOIN dbo.Subjects AS sbj ON sb.SubjectId = sbj.SubjectId INNER JOIN" +
                            " dbo.Schools AS sc INNER JOIN dbo.Students AS s INNER JOIN dbo.Classes AS c ON s.ClassId = c.ClassId ON sc.SchoolId = s.SchoolId INNER JOIN" +
                             " dbo.Branches AS br ON s.BranchId = br.BranchId INNER JOIN dbo.AspNetUsers AS u ON s.UserId = u.Id ON sb.UserId = s.UserId INNER JOIN" +
                             " dbo.Batches AS b ON s.BatchId = b.BatchId where s.StudentContact='" + studentContact + "' and s.LastName = '" + lastName + "' and Cast(s.DOB as Date) = '" + dob + "' and s.FirstName = '" + firstName + "' and u.Email = '" + emailId + "'";

                ob.resultCode = "Student is not registered in class.";
                using (con)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(strString, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        ob.UserId = dr["UserId"].ToString();
                        ob.ParentContact = dr["ParentContact"].ToString();
                        ob.StudentContact = dr["StudentContact"].ToString();
                        ob.ClassName = dr["ClassName"].ToString();
                        ob.SId = dr["SId"].ToString();
                        ob.ClassId = dr["ClassId"].ToString();
                        ob.SelectedSubjects = dr["SelectedSubject"].ToString();
                        sendto = dr["StudentContact"].ToString();
                        ob.StudentName = dr["FirstName"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                        ob.IsActive = dr["IsActive"].ToString();
                        ob.BranchName = dr["BranchName"].ToString();
                        //ob.SubjectWithBatch = dr[""].ToString();
                        ob.SchoolName = dr["SchoolName"].ToString();
                        ob.Email = dr["Email"].ToString();
                        ob.DOJ = dr["DOJ"].ToString().Split(' ')[0];
                        ob.BranchId = dr["BranchId"].ToString();
                        ob.BatchId = dr["BatchId"].ToString();
                        ob.BatchName = dr["BatchName"].ToString();
                        ob.PdfUploadId = "0";
                        ob.PhotoPath = dr["PhotoPath"].ToString().Split(' ')[0];
                        subjectsList.Add(dr["SubjectName"].ToString());
                    }
                    ob.SubjectName = string.Join(", ", subjectsList);

                    int maxNotificationId = 0;
                    int maxOnlineTestId = 0;
                    string strMaxPDFId = "select Max(PDFUploadId) from PDFUploads";
                    string strMaxId = "select Max(NotificationId) from Notifications";
                    string strMaxOnlineTestId = "select Max(ArrengeTestId) from ArrengeTests";
                    string strMaxOfflineTestPaperId = "select Max(OfflineTestPaperId) from OfflineTestPapers";
                    string strMaxOfflineTestStudentMarksId = "select Max(OfflineTestStudentMarksId) from OfflineTestStudentMarks";
                    SqlConnection con1 = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
                    using (con1)
                    {
                        dr.Close();
                        con1.Open();
                        SqlCommand cmdMaxId = new SqlCommand(strMaxId, con1);
                        string dataNew = cmdMaxId.ExecuteScalar().ToString();
                        if (dataNew != "")
                            maxNotificationId = Convert.ToInt32(cmdMaxId.ExecuteScalar());
                        else
                            maxNotificationId = 0;

                        SqlCommand cmdPDF = new SqlCommand(strMaxPDFId, con1);
                        string pdfId = cmdPDF.ExecuteScalar().ToString();
                        ob.PdfUploadId = pdfId;

                        SqlCommand cmdMaxOnlineTestId = new SqlCommand(strMaxOnlineTestId, con1);
                        string dataOnlineTEstId = cmdMaxOnlineTestId.ExecuteScalar().ToString();
                        if (dataOnlineTEstId != "")
                            maxOnlineTestId = Convert.ToInt32(cmdMaxOnlineTestId.ExecuteScalar());
                        else
                            maxOnlineTestId = 0;

                        SqlCommand cmdMaxOfflineTestPaperId = new SqlCommand(strMaxOfflineTestPaperId, con);
                        string MaxOfflineTestPaperId = cmdMaxOfflineTestPaperId.ExecuteScalar().ToString();
                        ob.MaxOfflineTestPaperId = MaxOfflineTestPaperId != "" ? MaxOfflineTestPaperId : "0";

                        SqlCommand cmdMaxOfflineTestStudentMarksId = new SqlCommand(strMaxOfflineTestStudentMarksId, con);
                        string MaxOfflineTestStudentMarksId = cmdMaxOfflineTestStudentMarksId.ExecuteScalar().ToString();
                        ob.MaxOfflineTestStudentMarksId = MaxOfflineTestStudentMarksId != "" ? MaxOfflineTestStudentMarksId : "0";
                    }
                    ob.NotificationId = maxNotificationId.ToString();

                    ob.OnlineTestId = maxOnlineTestId.ToString();

                    StudentDetails.Add(ob);

                    if (sendto != "" && StudentDetails.Select(x => x.StudentContact).FirstOrDefault() != "" && StudentDetails.Select(x => x.IsActive).FirstOrDefault() == "True")
                    {
                        String WebResponseString = "";
                        String URL = "";
                        try
                        {
                            //URL = "http://103.16.101.52:8080/bulksms/bulksms?username=" + username + "&password=" + password + "&type=0&dlr=1&destination=" + sendto + "&source=" + sender + "&message=" + message + "";

                            URL = "http://173.45.76.227/send.aspx?username=ctpune&pass=Ctpune@1&route=trans1&senderid=ctpune&numbers=" + sendto + "&message=" + message + "";

                            WebRequest webrequest; WebResponse webresponse;
                            webrequest = HttpWebRequest.Create(URL);//Hit URL Link
                            webrequest.Timeout = 25000;
                            try
                            {
                                webresponse = webrequest.GetResponse();//Get Response
                                StreamReader reader = new StreamReader(webresponse.GetResponseStream()); //Read Response and store in variable
                                WebResponseString = reader.ReadToEnd();
                                ob.resultCode = WebResponseString;
                                webresponse.Close();
                            }
                            catch (Exception ex)
                            {
                                WebResponseString = "Request Timeout";
                                ob.resultCode = WebResponseString;
                                return ex.Message.ToArray();
                                #region sms code
                                //1701: Success, Message Submitted Successfully, In this case you will  receive
                                //the response 1701|<CELL_NO>:<MESSAGE ID>, The message Id can
                                //then be used later to map the delivery reports to this message.
                                //1702: Invalid URL Error, This means that one of the parameters was not
                                //provided or left blank
                                //1703: Invalid value in username or password field
                                //1704: Invalid value in "type" field
                                //1705: Invalid Message
                                //1706: Invalid Destination
                                //1707: Invalid Source (Sender)
                                //1708: Invalid value for "dlr" field
                                //1709: User validation failed
                                //1710: Internal Error
                                //1025 :Insufficient Credit
                                //1032 : Destination in DND
                                //1033 : Sender / Template Mismatch 
                                #endregion
                            }
                        }
                        catch
                        {

                        }

                        string[] ggg = ob.resultCode.Split('|');
                        if (ggg[0] == "6")
                        {
                            ob.resultCode = "Invalid URL Error, This means that one of the parameters was not provided or left blank.";
                            ob.resultCode = "Invalid URL Error, This means that one of the parameters was not provided or left blank.";
                        }
                        else if (ggg[0] == "4")
                        {
                            ob.resultCode = "Message can't sent, Internet is not connected";
                        }
                        else if (ggg[0] == "5")
                        {
                            ob.resultCode = "Invalid Source (Sender Id), Plz Set Valid SenderId";
                            ob.resultCode = "Sorry Failed To send Message, Try Again with Valid Sender Id";
                        }
                        else if (ggg[0] == "2")
                        {
                            ob.resultCode = "Invalid Destination";
                        }
                        else if (ggg[0] == "1")
                        {
                            ob.resultCode = "sent";
                        }
                        else if (ggg[0] == "3")
                        {
                            ob.resultCode = "Insufficient credit";
                        }
                        else if (ggg[0] == "7")
                        {
                            ob.resultCode = "Submission Error.";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ob.resultCode = ex.Message;
                return StudentDetails.ToArray();
            }
            return StudentDetails.ToArray();
        }

        [WebMethod]
        public static Array setPlayerIdCMSStudentAppData(string userId, string playerId)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            string str = "select * from Students where UserId='" + userId + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            List<setPlayerIdCMSParentApp> setPlayerId = new List<setPlayerIdCMSParentApp>();
            string studentAppPlayerId = "";
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    studentAppPlayerId = dr["studentAppPlayerId"].ToString();
                }
            }
            str = "update Students set studentAppPlayerId='" + playerId + "' where UserId='" + userId + "'";
            SqlConnection con1 = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            using (con1)
            {
                con1.Open();
                SqlCommand cmd = new SqlCommand(str, con1);
                int updateCount = cmd.ExecuteNonQuery();
                setPlayerIdCMSParentApp ob = new setPlayerIdCMSParentApp();
                if (updateCount == 0)
                {
                    ob.result = "not found";
                    setPlayerId.Add(ob);
                    return setPlayerId.ToArray();
                }
                else if (updateCount == 1)
                {
                    if (studentAppPlayerId == "")
                    {
                        ob.result = "set";
                        setPlayerId.Add(ob);
                    }
                    else
                    {
                        ob.result = "already set";
                        setPlayerId.Add(ob);
                    }
                }
            }

            str = "update Students set studentAppPlayerId='' where studentAppPlayerId = '" + playerId + "' and UserId!='" + userId + "'";
            SqlConnection con2 = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            using (con2)
            {
                con2.Open();
                SqlCommand cmd = new SqlCommand(str, con2);
                int updateCount = cmd.ExecuteNonQuery();

            }

            return setPlayerId.ToArray();
        }

        public class getCMSStudentNotification
        {
            public string NotificationId { get; set; }
            public string Message { get; set; }
            public string Date { get; set; }
            public string Category { get; set; }
        }

        [WebMethod]
        public static Array GetCMSStudentNotification(string branchId, string classId, string selectedbatches, string notificationMaxId, string pdfUploadId, string arrengeTestId,
            string maxOfflineTestPaperId, string maxOfflineTestStudentMarksId, string userId, string selectedSubjects, string studentName)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            SqlConnection con1 = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            SqlConnection con2 = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            string str = "select * from Notifications where NotificationId > '" + notificationMaxId + "'";// where City='" + txtCity.Text + "'";//  and ClassName='" + txtClassName.Text + "'";
            List<getCMSStudentNotification> details = new List<getCMSStudentNotification>();

            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    getCMSStudentNotification ob = new getCMSStudentNotification();
                    string allUser = dr["AllUser"].ToString();
                    if (dr["NotificationId"].ToString() != notificationMaxId && dr["Media"].ToString().Contains("AppNotification"))
                    {
                        if (allUser != "True")
                        {
                            string[] branchList = dr["SelectedBranches"].ToString().Split(',');
                            if (branchList.Contains(branchId))
                            {
                                string[] classList = dr["SelectedClasses"].ToString().Split(',');
                                if (classList.Contains(classId))
                                {
                                    string[] batchList = dr["SelectedBatches"].ToString().Split(',');
                                    string[] studentbatchList = selectedbatches.Split(',');
                                    int count = 0;
                                    foreach (var batch in studentbatchList)
                                    {
                                        if (batchList.Contains(batch))
                                        {
                                            count++;
                                        }
                                    }
                                    if (count > 0)
                                    {
                                        ob.Date = dr["CreatedOn"].ToString();
                                        ob.Message = dr["NotificationMessage"].ToString();
                                        ob.NotificationId = dr["NotificationId"].ToString();
                                        ob.Category = "Notice";
                                        details.Add(ob);
                                    }
                                }
                            }
                        }
                        else
                        {
                            ob.Date = dr["CreatedOn"].ToString();
                            ob.Message = dr["NotificationMessage"].ToString();
                            ob.NotificationId = dr["NotificationId"].ToString();
                            ob.Category = "Notice";
                            details.Add(ob);
                        }
                    }
                }

                string getOfflineTestQuery = "select * from OfflineTestPapers where OfflineTestPaperId > '" + maxOfflineTestPaperId + "' and Media like '%AppNotification%' and ClassId = '" + classId + "' and SubjectId in (" + selectedSubjects + ")";
                SqlCommand cmdOfflineTest = new SqlCommand(getOfflineTestQuery, con);
                dr.Close();
                SqlDataReader sdr = cmdOfflineTest.ExecuteReader();
                while (sdr.Read())
                {
                    string[] branchList = sdr["SelectedBranches"].ToString().Split(',');
                    if (branchList.Contains(branchId))
                    {
                        string[] batchList = sdr["SelectedBatches"].ToString().Split(',');
                        if (batchList.Contains(selectedbatches))
                        {
                            getCMSStudentNotification ob = new getCMSStudentNotification();
                            ob.Date = sdr["CreatedOn"].ToString();
                            ob.Message = "OfflineTest-<br />" + sdr["Title"].ToString() + " test paper on " + Convert.ToDateTime(sdr["TestDate"]).ToString("dd-MM-yyyy") + ", Total Marks - " + sdr["TotalMarks"].ToString();
                            ob.NotificationId = sdr["OfflineTestPaperId"].ToString();
                            ob.Category = "Offline";
                            details.Add(ob);
                        }
                    }
                }

                string getOfflineTestMarksQuery = "select * from OfflineTestStudentMarks inner join OfflineTestPapers on OfflineTestStudentMarks.OfflineTestPaperId = OfflineTestPapers.OfflineTestPaperId where OfflineTestStudentMarksId > '" + maxOfflineTestStudentMarksId + "' and UserId = '" + userId + "'";
                SqlCommand cmdOfflineTestMarks = new SqlCommand(getOfflineTestMarksQuery, con);
                sdr.Close();
                SqlDataReader sdrMarks = cmdOfflineTestMarks.ExecuteReader();
                while (sdrMarks.Read())
                {
                    getCMSStudentNotification ob = new getCMSStudentNotification();
                    ob.Date = sdrMarks["CreatedOn"].ToString();
                    ob.Message = "Marks-<br />Name - " + studentName + "<br />Title - " + sdrMarks["Title"].ToString() + "<br />Marks - " + sdrMarks["ObtainedMarks"].ToString() + "/" +
                        sdrMarks["TotalMarks"].ToString() + "<br />Percentage - " + sdrMarks["Percentage"].ToString();
                    ob.NotificationId = sdrMarks["OfflineTestStudentMarksId"].ToString();
                    ob.Category = "Offline";
                    details.Add(ob);
                }
            }

            if (pdfUploadId != "")
            {
                string strPDFUpload = "select pdfu.PdfUploadId, pdfu.Title ,pdfu.FileName ,pdfu.CreatedOn , pdfc.Name from PDFUploads as pdfu Inner Join PDFCategories as pdfc on pdfu.PDFCategoryId = pdfc.PDFCategoryId where pdfu.PdfUploadId > '" + pdfUploadId + "' and IsSend = 1 and ClassId = '" + classId + "'";
                using (con1)
                {
                    con1.Open();
                    SqlCommand cmd = new SqlCommand(strPDFUpload, con1);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        getCMSStudentNotification ob = new getCMSStudentNotification();
                        ob.Message = dr["Title"].ToString() + "$^$ File - " + dr["FileName"].ToString() + "$^$ Category - " + dr["Name"].ToString();
                        ob.NotificationId = dr["PdfUploadId"].ToString();
                        ob.Date = dr["CreatedOn"].ToString();
                        ob.Category = "PDF";
                        details.Add(ob);
                    }
                }
            }

            if (arrengeTestId != "")
            {
                string strOnlineTest = "select atest.CreatedOn, tpaper.Title, atest.Date, atest.StartTime, atest.TimeDuration, atest.TestPaperId, atest.ArrengeTestId, atest.SelectedBatches, atest.SelectedBranches from ArrengeTests atest inner join TestPapers as tpaper on atest.TestPaperId = tpaper.TestPaperId where atest.ArrengeTestId > '" + arrengeTestId + "' and tpaper.ClassId = '" + classId + "'";
                using (con2)
                {
                    con2.Open();
                    SqlCommand cmd = new SqlCommand(strOnlineTest, con2);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string[] batchList = dr["SelectedBatches"].ToString().Split(',');
                        string[] branchList = dr["SelectedBranches"].ToString().Split(',');
                        if (batchList.Contains(selectedbatches) && branchList.Contains(branchId))
                        {
                            var date = Convert.ToDateTime(dr["Date"].ToString()).ToString("dd-MM-yyyy");
                            var startTime = Convert.ToDateTime(dr["StartTime"].ToString()).ToString("hh:mm tt");
                            getCMSStudentNotification ob = new getCMSStudentNotification();
                            ob.Message = dr["Title"].ToString() + "$^$Date:" + date + "$^$Start Time:" + startTime + "$^$Duration:" + dr["TimeDuration"].ToString() + "$^$TestPaperId:" + dr["TestPaperId"].ToString() + "$^$" + 1;
                            ob.NotificationId = dr["ArrengeTestId"].ToString();
                            ob.Date = dr["CreatedOn"].ToString();
                            ob.Category = "Test";
                            details.Add(ob);
                        }
                    }
                }
            }

            return details.ToArray();
        }

        [WebMethod]
        public static string gettt(string testpaperid)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("Accept:application/json");
                    ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                    //var questionResult = client.DownloadString("https://cmsTest.crunchersoft.com/Api/PaperAPi/" + testpaperid);

                    var questionResult = client.DownloadString("https://cmsTest.crunchersoft.com/Api/PaperAPi/" + testpaperid);
                    //var questionResult = client.DownloadString("https://localhost:44300/Api/PaperAPi/" + testpaperid);
                    //var branch = JsonConvert.DeserializeObject<List<GetQuestionDetails>>(questionResult);
                    return questionResult;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [WebMethod]
        public static string getpdf(string categorypdf)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("Accept:application/json");
                    ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                    //var questionResult = client.DownloadString("https://cmsTest.crunchersoft.com/Api/PdfAPi/" + testpaperid);

                    //var pdfesult = client.DownloadString("https://cmsTest.crunchersoft.com/Api/PDFCategoryApi/" + categorypdf);
                    var pdfesult = client.DownloadString("https://localhost:49932/Api/PDFCategoryApi/" + categorypdf);
                    //var branch = JsonConvert.DeserializeObject<List<GetQuestionDetails>>(questionResult);
                    return pdfesult;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [WebMethod]
        public static string pdf(string pdftype)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("Accept:application/json");
                    ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };

                    var pdfResult = client.DownloadString("http://cmstest.crunchersoft.com/Api/NotesApi/Get/" + pdftype);
                   
                    return pdfResult;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        [WebMethod]
        public static string PostResultOfOnlineTest(string testDetails)
        {
            //testDetails = "{\"SId\":\"18\",\"TestPaperId\":\"8\",\"TestDate\":\"05-15-2018\",\"TimeDuration\":\"60\",\"StartTime\":\"3:00 PM\",\"Questions\":[{\"QuestionId\":\"3\",\"CorrectAnswer\":\"B\",\"StudentAnswer\":\"C\",\"Status\":\"Incorrect\"},{\"QuestionId\":\"4\",\"CorrectAnswer\":\"B\",\"StudentAnswer\":\"D\",\"Status\":\"Incorrect\"},{\"QuestionId\":\"5\",\"CorrectAnswer\":\"B\",\"StudentAnswer\":\"Unsolved\",\"Status\":\"Unsolved\"}]}";
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("Content-Type:application/json");
                    client.Headers.Add("Accept:application/json");
                    ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                    //JavaScriptSerializer js = new JavaScriptSerializer();
                    //var testDetails1 = js.Deserialize<TestResult>(testDetails);
                    // var url = "https://cms.dev.crunchersoft.com/Api/GetAPi";
                    var url = "https://cmsTest.crunchersoft.com/Api/GetAPi";
                    var result = client.UploadString(url, "POST", testDetails);
                    var response = client.ResponseHeaders;
                    return result;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public class getCMSResults
        {
            public string ObtainedMarks { get; set; }
            public string TotalMarks { get; set; }
            public string Timing { get; set; }
            public string Duration { get; set; }
            public string Title { get; set; }
            public string Percentage { get; set; }
            public string TestDate { get; set; }
            public string Category { get; set; }
            public string Status { get; set; }
        }

        [WebMethod]
        public static Array GetStudentResults(string userId)
        {
            SqlConnection con = new SqlConnection("Data Source=184.168.47.13; Initial Catalog=cmstestcruncher; User ID=cmstestcruncher; Password=cmstestcruncher@123");
            string str = "select * from OfflineTestStudentMarks inner join OfflineTestPapers on OfflineTestStudentMarks.OfflineTestPaperId = OfflineTestPapers.OfflineTestPaperId where UserId='" + userId + "'";
            List<getCMSResults> details = new List<getCMSResults>();
            using (con)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(str, con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    getCMSResults ob = new getCMSResults();
                    ob.Title = dr["Title"].ToString();
                    ob.TestDate = dr["TestDate"].ToString().Split(' ')[0];
                    ob.Timing = Convert.ToDateTime(dr["TestInTime"]).ToString("hh:mm tt");
                    ob.Duration = "";
                    ob.ObtainedMarks = dr["ObtainedMarks"].ToString();
                    ob.TotalMarks = dr["TotalMarks"].ToString();
                    ob.Percentage = dr["Percentage"].ToString();
                    ob.Category = "Offline";
                    ob.Status = dr["IsPresent"].ToString();
                    details.Add(ob);
                }
                dr.Close();

                string strOnline = "select UserId, Title, StartTime, TimeDuration, ObtainedMarks, OutOfMarks, TestDate from ArrangeTestResults inner join TestPapers on ArrangeTestResults.TestPaperId = TestPapers.TestPaperId where UserId='" + userId + "'";
                var cmdOnline = new SqlCommand(strOnline, con);
                SqlDataReader sdr = cmdOnline.ExecuteReader();
                while (sdr.Read())
                {
                    getCMSResults ob = new getCMSResults();
                    ob.Title = sdr["Title"].ToString();
                    ob.TestDate = sdr["TestDate"].ToString().Split(' ')[0];
                    ob.Timing = Convert.ToDateTime(sdr["StartTime"]).ToString("hh:mm tt");
                    ob.Duration = sdr["TimeDuration"].ToString();
                    ob.ObtainedMarks = sdr["ObtainedMarks"].ToString();
                    ob.TotalMarks = sdr["OutOfMarks"].ToString();
                    var percentage = ((Convert.ToDouble(sdr["ObtainedMarks"]) / Convert.ToDouble(sdr["OutOfMarks"])) * 100).ToString();
                    ob.Percentage = percentage;
                    ob.Category = "Online";
                    ob.Status = "Present";
                    details.Add(ob);
                }
                sdr.Close();
            }
            return details.ToArray();
        }
    }
}
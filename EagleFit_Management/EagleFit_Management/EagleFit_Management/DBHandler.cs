/*
 EagleFit_Management: DBHandler
  
 Fall 2013 - Winter 2014
 
 Description: This class contains all of the database controls for the program. 
               All methods that write to or query from the database are housed in this class.
 
 Linked .cs/xaml Files: MainNavigation.xaml, AddMultipleUsers.xaml, AddVisit.xaml, ClassInformation.xaml, Reports.xaml,
                        UserManager.xaml
 
 Created By: Sarah Henderson
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using MySql.Data.MySqlClient;

//testing stuff
//using System.Data.SqlClient;
//using System.Data.Common.DbParameterCollection;


using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography;

using System.Globalization;

namespace EagleFit_Management
{
    
    public class DBHandler
    {

        private string dbIP;
        private string dbName;
        private string dbUserName;
        private string dbPassword;
        private string connectionString;
        
       private MySqlConnection conn;

       //private DateTime sdate;
       //private DateTime edate;


       #region Constructors
       //--------------------------- Constructors -----------------------------------------

        public DBHandler()
        {
            this.dbIP = "146.187.135.45";
            this.dbName = "EagleFit";
            this.dbUserName = "root";
            this.dbPassword = "#32-jDs7e*Q";

            connectToMySql();
        }


    
        public DBHandler(string dbIP, string dbName, string dbUserName, string dbPassword)
        {
            this.dbIP = dbIP;
            this.dbName = dbName;
            this.dbUserName = dbUserName;
            this.dbPassword = dbPassword;
        }

       #endregion




        #region Database Connections & Disconnect
        //--------------------------------------- Database Connection/Disconnect ---------------------------------


        /*Establishes a connection to the MySQL database
         Exception: if a connection can't be established then the exception is caught and a message box appears.
         */
        public void connectToMySql()
        {
            String connectS = "server=" + this.dbIP + ";uid=" + this.dbUserName + ";pwd=" + this.dbPassword + ";database=" + this.dbName + ";";
            //this.connectionString = string.Format("server={0}; user id={1}; password={2}; database={3}; pooling=false", this.dbIP, this.dbUserName, this.dbPassword, this.dbName);

            try
            {
                conn = new MySqlConnection(connectS);
                
                conn.Open();
                //MessageBox.Show("inside connection try");
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Error connecting to the server:" + e.Message + "\r\n\r\nMake sure your settings are correct.");
            }
        }//end connectToMySql



        //close the MySQL Connection
        public void disconnectMySql()
        {
            conn.Close();
        }//end disconnectMySql


        #endregion



        #region Main Window
        //------------------------------------- Main Window ------------------------------------------

        /*
         This method allows user to login to the program.
         It is called from the Main Window form.
         */
        public bool loginQuery(String username, String password)
        {
            //String q1 = "SELECT FROM Users";

            return true;
        }


        #endregion


        #region Hashing/UnHashing Passwords
        //--------------------------------------------- Password Hashing/UnHashing code ----------------------------------
         
         /* The following 2 methods, ComputeHash and VerifyHash deal with encrypting
         * passwords.  The code was lifted from http://www.obviex.com/samples/hash.aspx
         * with very little modification.  The saltBytes passed into the 
         * ComputeHash method will be null as to let the method figure the 
         * size on the fly */
        public static string computeHash(string plainText, byte[] saltBytes)
        {
            if (saltBytes == null)
            {
                // Define min and max salt sizes.
                int minSaltSize = 4;
                int maxSaltSize = 8;

                // Generate a random number for the size of the salt.
                Random random = new Random();
                int saltSize = random.Next(minSaltSize, maxSaltSize);

                // Allocate a byte array, which will hold the salt.
                saltBytes = new byte[saltSize];

                // Initialize a random number generator.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                // Fill the salt with cryptographically strong byte values.
                rng.GetNonZeroBytes(saltBytes);
            }
            // Convert plain text into a byte array.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Allocate array, which will hold plain text and salt.
            byte[] plainTextWithSaltBytes =
                    new byte[plainTextBytes.Length + saltBytes.Length];

            // Copy plain text bytes into resulting array.
            for (int i = 0; i < plainTextBytes.Length; i++)
                plainTextWithSaltBytes[i] = plainTextBytes[i];

            // Append salt bytes to the resulting array.
            for (int i = 0; i < saltBytes.Length; i++)
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

            // Because we support multiple hashing algorithms, we must define
            // hash object as a common (abstract) base class. We will specify the
            // actual hashing algorithm class later during object creation.
            HashAlgorithm hash;
            hash = new MD5CryptoServiceProvider();
            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            // Create array which will hold hash and original salt bytes.
            byte[] hashWithSaltBytes = new byte[hashBytes.Length +
                                                saltBytes.Length];

            // Copy hash bytes into resulting array.
            for (int i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            // Append salt bytes to the result.
            for (int i = 0; i < saltBytes.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

            // Convert result into a base64-encoded string.
            string hashValue = Convert.ToBase64String(hashWithSaltBytes);

            // Return the result.
            return hashValue;
        }



        public static bool verifyHash(string plainText, string hashValue)
        {
            // Convert base64-encoded hash value into a byte array.
            byte[] hashWithSaltBytes = Convert.FromBase64String(hashValue);

            // We must know size of hash (without salt).
            int hashSizeInBits, hashSizeInBytes;

            hashSizeInBits = 128;
            // Convert size of hash from bits to bytes.
            hashSizeInBytes = hashSizeInBits / 8;

            // Make sure that the specified hash value is long enough.
            if (hashWithSaltBytes.Length < hashSizeInBytes)
                return false;
            // Allocate array to hold original salt bytes retrieved from hash.
            byte[] saltBytes = new byte[hashWithSaltBytes.Length -
                                        hashSizeInBytes];

            // Copy salt from the end of the hash to the new array.
            for (int i = 0; i < saltBytes.Length; i++)
                saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];

            // Compute a new hash string.
            string expectedHashString = computeHash(plainText, saltBytes);

            // If the computed hash matches the specified hash,
            // the plain text value must be correct.
            return (hashValue == expectedHashString);
        }


        #endregion



        #region UserManager Form
        //------------------------------------- UserManager form ---------------------------------------------


        /*
         This method adds a new Manager/Instructor/Supervisor to the database
         It is called from the UserManager form.
         */
        public int addNewLogin(String name, String role, String password)
        {
            password = computeHash(password, null);
            //connectToMySql();
            int success = -1;
            try
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO Users VALUES('@name', '@role', '@password');");
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@role", role);
                command.Parameters.AddWithValue("@password", password);
                command.ExecuteNonQuery();
                success = 1;
            }
            catch (MySqlException)
            {

            }
            //finally
            //{
              //  disconnectMySql();
            //}
            return success;
        } //end addNewLogin



        /*
         This method changes the password of a current user
         It is called from the UserManager form.
         */
        public bool changePassword(String curUserName, String curPW, String newPass1, String newPass2)
        {
            //query database
            String curDBUser = "";
            String curDBPassword = "";

            try
            {
                //get current password for username
                MySqlCommand command = new MySqlCommand("Select user_name, password FROM Users WHERE user_name = '@DBUser';");
                command.Parameters.AddWithValue("@DBUser", curUserName);

                //check to make sure current password matches

                //submit new password to Database


                return true;
            }
            catch(Exception c)
            {
                String c1 = c.Message;
                return false;
            }
        }//end method change password


   #endregion


        #region AddMultipleUsers
        //-------------------------------------- Add Multiple users---------------------------------------------------


#endregion



        #region Reports
                //----------------------------------------- Reports ---------------------------------------------------------



        #endregion



        #region Add/EditVisits
        //--------------------------------------- Add/Edit Visit ----------------------------------------------------

        #endregion



        #region Add/Edit Additional Activities
        //--------------------------------------- Add/Edit Additional Activities -------------------------------------






        #endregion



        #region Class Information
        //---------------------------------------- Class Information -------------------------------------------

        /*Add new class listing to the database.
         * This method is called from the ClassInformation form.
         */
        public bool addClassToDatabase(int courseID, String className, int section, String quarterEndDate, int credits, String startTime, String endTime, int year, String quarter)
        {
            connectToMySql();
            try
            {

                /* Courses(course_ID INT(3) NOT NULL, course_Name VARCHAR(25) NOT NULL, Start_Time TIME, End_Time TIME, 
                Section_Num INT(1) NOT NULL, Quarter_End_Date DATE NOT NULL, Credits INT(1), Quarter VARCHAR(6) NOT NULL,
	            Year INT(4) NOT NULL, PRIMARY KEY (course_ID, Section_Num, Quarter_End_Date)
                */

               // DataFormat df = quarterEndDat

                //DateTime dtTimeObj = Convert.ToDateTime(quarterEndDate);
                //dtTimeObj.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                //DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
                //dtfi.ShortDatePattern = "yyyy-mm-dd";
                //dtfi.DateSeparator = "-";
                //DateTime objDate = Convert.ToDateTime(quarterEndDate);

               
                TimeSpan sTime = convertToTimeSpan(startTime);                
                TimeSpan eTime = convertToTimeSpan(endTime);
                
                MessageBox.Show("Start Time: " + sTime.ToString("HH:mm:ss") + "end Time: " + eTime.ToString("HH:mm:ss"));              
                //MessageBox.Show(objDate.ToString());

                MySqlCommand command = new MySqlCommand("INSERT INTO Courses(@course_ID, @course_Name, @Time_In, @Time_Out, @Section_Num, @QuarterED, @Credits, @Quarter, @Year);", conn);
                command.Parameters.AddWithValue("@course_ID", courseID);
                command.Parameters.AddWithValue("@course_Name", className);
                command.Parameters.AddWithValue("@Time_In", MySqlDbType.Time).Value = sTime.ToString("HH:mm:ss");
                command.Parameters.AddWithValue("@Time_Out", MySqlDbType.Time).Value = eTime.ToString("HH:mm:ss");
                command.Parameters.AddWithValue("@Section_Num", section);
                command.Parameters.AddWithValue("@QuarterED", MySqlDbType.Date).Value = quarterEndDate;
                command.Parameters.AddWithValue("@Credits", credits);
                command.Parameters.AddWithValue("@Quarter", quarter); 
                command.Parameters.AddWithValue("@Year", year);
                
                
                command.ExecuteNonQuery();

                return true;

             }//end try

            catch (MySqlException noAdd)
            {
                MessageBox.Show("No Add in Query: " + noAdd.Message);
                return false;
            }


        }//end method addClassToDatabase


        /*String is passed in as hh:mm:ss for Group Exercise. N/A is passed in for a fast fitness course
         */
        private TimeSpan convertToTimeSpan(String t)
        {
            if (!t.Equals("N/A"))
            {                
                TimeSpan time = TimeSpan.Parse(t);
               MessageBox.Show("convertToTimeSpan: " + time);
                return time;
            }
            else
                return new TimeSpan(11, 59, 0);
        }




        #endregion

        //------------------------------------------------------------------------------------------------------




    }//end class
}//end namespace

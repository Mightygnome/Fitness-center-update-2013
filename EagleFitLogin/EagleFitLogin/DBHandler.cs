using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
     using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows;

using MySql.Data.MySqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography;

namespace EagleFitLogin
{
    
    public class DBHandler
    {
        private String dbIP;
        private String dbName;
        private String dbUserName;
        private String dbPassword;
        private String connectionString;
        private MySqlConnection conn;
        private DateTime sdate;
        private DateTime edate;


        //
        //default contstructor
        //
        public DBHandler()
        {
            this.dbIP = "localhost";
            this.dbName = "bodyshop";
            this.dbUserName = "root";
            this.dbPassword = "redalert";
        }

        //
        //constructor
        public DBHandler(String dbIP, String dbName, String dbUserName, String dbPassword)
        {
            this.dbIP = dbIP;
            this.dbName = dbName;
            this.dbUserName = dbUserName;
            this.dbPassword = dbPassword;
        }

        //
        //gets and sets
        //
        public String iP
        {
            get
            {
                return this.dbIP;
            }
            set
            {
                this.dbIP = value;
            }
        }

        public String nameOfDb
        {
            get
            {
                return this.dbName;
            }
            set
            {
                this.dbName = value;
            }
        }

        public String userName
        {
            get
            {
                return this.dbUserName;
            }
            set
            {
                this.dbUserName = value;
            }
        }

        public String pwd
        {
            get
            {
                return this.dbPassword;
            }
            set
            {
                this.dbPassword = value;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return this.sdate;
            }
            set
            {
                this.sdate = value;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return this.edate;
            }
            set
            {
                this.edate = value;
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: connectToMySql
         * Parameters: none
         * Output: void
         * Exception: MySqlException
         * Description: This method makes a connection string based on the 
         * object's values and attempts to make a connection to the database
         */
        private void connectToMySql()
        {
            this.connectionString = string.Format("server={0}; user id={1}; password={2}; database={3}; pooling=false", this.dbIP, this.dbUserName, this.dbPassword, this.dbName);

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Error connecting to the server:" + e.Message + "\r\n\r\nMake sure your settings are correct.");
            }
        }//end connectToMySql

        /*
         * Author: Russ Utt
         * Method Name: disconnectMySql
         * Parameters: none
         * Output: void
         * Exception: MySqlException
         * Description: disconnects the connection to the database
         */
        private void disconnectMySql()
        {
            conn.Close();
        }//end disconnectMySql



        /*
         * Author: Russ Utt
         * Method Name: isSettingsCorrect
         * Parameters: a connection string
         * Output: bool (true if it is valid, false if not)
         * Exception: MySqlException
         * Description: This method test a possible connection string and 
         * verifys it is valid but trying to connect to the database
         */
        public bool isSettingsConnect(String connectionString)
        {
            bool connect = false;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();
                connect = true;
            }
            catch (MySqlException)
            {
                connect = false;
            }
            finally
            {
                disconnectMySql();
            }
            return connect;
        }

        /*
         * Author: Russ Utt
         * Method Name: addNewUser
         * Parameters: User's name, Role of user, User's password
         * Output: int (1 if a new user was successfully added, -1 if not)
         * Exception: MySqlException
         * Description: This method will create a new user in the database 
         * (not a student), this is used to create members who can login and  
         * access the system, role is either going to be admin or employee
         */
        public int addNewUser(String name, String role, String pass)
        {
            pass = computeHash(pass, null);
            //MessageBox.Show(pass);
            connectToMySql();
            int success = -1;
            try
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO users VALUES('" + name.Replace("'", "''") + "', '" + role.Replace("'", "''") + "', '" + pass.Replace("'", "''") + "')", conn);
                command.ExecuteNonQuery();
                success = 1;
            }
            catch (MySqlException)
            {
            }
            finally
            {
                disconnectMySql();
            }
            return success;
        }
        /*
         * Author: Russ Utt
         * Method Name: changeUserPassword
         * Parameters: User's name, User's new password, Admin's password
         * Output: int (-1 if connection was lost and nothing happened, 1 if the admin
         * password was correct and the user password got succesfully changed, 0 if
         * the admin password was not correct)
         * Exception: MySqlException
         * Description: This method will attempt to change a user's password.  It
         * first needs to verify that the admin's password is correct first.
         */
        public int changeUserPassword(String name, String changedPassword, String adminPassword)
        {
            changedPassword = computeHash(changedPassword, null);
            bool ok = isAdminPasswordCorrect(adminPassword);
            int success = -1;
            if (ok)
            {
                connectToMySql();

                try
                {
                    MySqlCommand command = new MySqlCommand("UPDATE users SET password = '" + changedPassword.Replace("'", "''") + "' WHERE user_name = '" + name.Replace("'", "''") + "'", conn);
                    command.ExecuteNonQuery();
                    success = 1;
                }
                catch (MySqlException)
                {
                }
                finally
                {
                    disconnectMySql();
                }
            }
            else
            {
                success = 0;
            }
            return success;
        }//end changeUserPasswor

        /*
         * Author: Russ Utt
         * Method Name: removeUser
         * Parameters: User's name
         * Output: int (1 if successfully removed, -1 if not)
         * Exception: MySqlException
         * Description: This method will attempt to remove a user from the 
         * database.
         */
        public int removeUser(String user)
        {
            int success = -1;
            bool userExist = doesUserExist(user);
            if (userExist == true)
            {
                connectToMySql();

                try
                {
                    MySqlCommand command = new MySqlCommand("DELETE FROM users WHERE user_name = '" + user.Replace("'", "''") + "'", conn);
                    command.ExecuteNonQuery();
                    success = 1;
                }
                catch (MySqlException)
                {
                }
                finally
                {
                    disconnectMySql();
                }
            }
            else
            {
                success = -1;
            }

            return success;
        }//end removeUser


        /*
         * Author: Russ Utt
         * Method Name: addMemberCategory
         * Parameters: description of the category (for example: "credit", "non-credit")
         * Output: int
         * Exception: MySqlException
         * Description: This method allows the user to add a category type for it's members
         */
        public int addMemberCategory(String description)
        {
            String desc = description.ToUpper();
            int success = -1;
            bool exists = doesCategoryExist(description);
            bool existButNotVisible = doesCategoryExistButNotVisible(description);
            if (exists == false && existButNotVisible == false)
            {
                connectToMySql();
                try
                {
                    MySqlCommand command = new MySqlCommand("INSERT INTO categories VALUES('" + description.Replace("'", "''") + "', 1)", conn);
                    command.ExecuteNonQuery();
                    success = 1;
                }
                catch (MySqlException)
                {

                }
                finally
                {
                    disconnectMySql();
                }
            }
            else if (exists == false && existButNotVisible == true)
            {
                connectToMySql();
                try
                {
                    MySqlCommand command = new MySqlCommand("UPDATE categories SET is_visible = 1 WHERE UPPER(cat_name) = '" + desc.Replace("'", "''") + "'", conn);
                    command.ExecuteNonQuery();
                    success = 1;
                }
                catch (MySqlException)
                {

                }
                finally
                {
                    disconnectMySql();
                }
            }
            else
            {
                success = -1;
            }
            return success;
        }//end addMemberCategory

        /*
         * Author: Russ Utt
         * Method Name: doesCategoryExist
         * Parameters: description of the category (for example: "credit", "non-credit")
         * Output: bool
         * Exception: MySqlException
         * Description: This method checks to see if a member category already exists in the database, this 
         * is being used to avoid problems of adding another category with the same name
         */
        public bool doesCategoryExist(String description)
        {
            String desc = description.ToUpper();
            int count = -1;
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT cat_name FROM categories WHERE UPPER(cat_name) = '" + desc.Replace("'", "''") + "' AND is_visible = 1", conn);
                da.Fill(dt);
                count = dt.Rows.Count;
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (MySqlException)
            {
                return true;
            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: doesCategoryExistButNotVisible
         * Parameters: description of the category (for example: "credit", "non-credit")
         * Output: int
         * Exception: MySqlException
         * Description: This method checks to see if a member category exists but is set to is_visible = 0,
         * meaning that the program no longer sees this category but it exists in the database.  Because
         * of data integrity categories cannot be truly deleted from database so we set them as being
         * visible or not
         */
        public bool doesCategoryExistButNotVisible(String description)
        {
            String desc = description.ToUpper();
            int count = -1;
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT cat_name FROM categories WHERE UPPER(cat_name) = '" + desc.Replace("'", "''") + "' AND is_visible = 0", conn);
                da.Fill(dt);
                count = dt.Rows.Count;
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (MySqlException)
            {
                //MessageBox.Show("getting here");
                return true;
            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: removeMemberCategory
         * Parameters: description of the category (for example: "credit", "non-credit")
         * Output: int
         * Exception: MySqlException
         * Description: This method will remove the member category from the program but not from
         * the database (because of data integrity).  is_visible = 0 means it is not visible to the
         * program
         */
        public int removeMemberCategory(String description)
        {
            connectToMySql();
            int success = -1;
            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE categories SET is_visible = 0 WHERE cat_name = '" + description.Replace("'", "''") + "'", conn);
                command.ExecuteNonQuery();
                success = 1;
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return success;
        }

        /*
         * Author: Russ Utt
         * Method Name: addSection
         * Parameters: section number
         * Output: int
         * Exception: MySqlException
         * Description: This method will allow a user to add a section to the program
         */
        public int addSection(int sectionNumber)
        {
            bool secExists = doesSectionExist(sectionNumber);
            bool secExistsButNotVisilbe = doesSectionExistButNotVisible(sectionNumber);
            int success = -1;
            connectToMySql();
            if (secExists == false && secExistsButNotVisilbe == false) //need to see if the section number exists at all
            {
                try
                {
                    MySqlCommand command = new MySqlCommand("INSERT INTO section VALUES(" + sectionNumber + ", 1)", conn);
                    command.ExecuteNonQuery();
                    success = 1;
                }
                catch (MySqlException)
                {

                }
                finally
                {
                    disconnectMySql();
                }
                return success;
            }
            else if (secExists == false && secExistsButNotVisilbe == true) //if it exists but not visible it needs to be set as visible
            {
                try
                {
                    MySqlCommand command = new MySqlCommand("UPDATE section SET is_visible = 1 WHERE section_number = " + sectionNumber, conn);
                    command.ExecuteNonQuery();
                    success = 1;
                }
                catch (MySqlException)
                {

                }
                finally
                {
                    disconnectMySql();
                }
                return success;
            }
            else
            {
                return success;
            }
        }//end addSection

        /*
         * Author: Russ Utt
         * Method Name: removeSection
         * Parameters: section number
         * Output: int
         * Exception: MySqlException
         * Description: This method will remove the section from the program (not the db), by setting the 
         * is_visible flag to 0
         */
        public int removeSection(int sectionNumber)
        {
            int success = -1;
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE section SET is_visible = 0 WHERE section_number = " + sectionNumber, conn);
                command.ExecuteNonQuery();
                success = 1;
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return success;
        }//end removeSection

        /*
         * Author: Russ Utt
         * Method Name: addAdditionalActivity
         * Parameters: description of the activity and the value to be associated with it
         * Output: int
         * Exception: MySqlException
         * Description: This method allows the user to set "default" additional activities along with
         * "default" value associated with them.  These activities can be chosen from a list when adding
         * activities to a member
         */
        public int addAdditionalActivity(String description, int value)
        {
            int success = -1;
            bool activityExists = doesActivityExist(description);
            bool activityNotVisible = doesActivityExistButNotVisible(description);
            if (activityExists == false && activityNotVisible == false) //make sure the activities does not exist at all
            {
                connectToMySql();

                try
                {
                    MySqlCommand command = new MySqlCommand("INSERT INTO activity_categories VALUES(null, '" + description.Replace("'", "''") + "', " + value + ", 1)", conn);
                    command.ExecuteNonQuery();
                    success = 1;
                }
                catch (MySqlException)
                {

                }
                finally
                {
                    disconnectMySql();
                }
                return success;
            }
            else if (activityExists == false && activityNotVisible == true) //if it exists but not visible set is_visible = 1
            {
                int actID = getActivityID(description);
                connectToMySql();

                try
                {
                    MySqlCommand command = new MySqlCommand("UPDATE activity_categories SET is_visible = 1, activity_value = " + value + " WHERE activity_id = " + actID, conn);
                    command.ExecuteNonQuery();
                    success = 1;
                }
                catch (MySqlException)
                {

                }
                finally
                {
                    disconnectMySql();
                }
                return success;
            }
            else
            {
                return success;
            }
        }//end addAdditionalActivity

        /*
         * Author: Russ Utt
         * Method Name: removeAdditionalActivity
         * Parameters: description of the additional activity
         * Output: int
         * Exception: MySqlException
         * Description: This method will remove the additional activity from the program by settings
         * is_visible = 0
         */
        public int removeAdditionalActivity(String description)
        {
            int actID = getActivityID(description);
            if (actID != -1)
            {
                connectToMySql();
                int success = -1;
                try
                {
                    MySqlCommand command = new MySqlCommand("UPDATE activity_categories SET is_visible = 0 WHERE activity_id = " + actID, conn);
                    command.ExecuteNonQuery();
                    success = 1;
                }
                catch (MySqlException)
                {

                }
                finally
                {
                    disconnectMySql();
                }
                return success;
            }
            else
            {
                return -1;
            }
        }//end removeAdditionalActivity

        /*
         * Author: Russ Utt
         * Method Name: editAdditionalActivity
         * Parameters: original additional activity description, new activity description, new value
         * Output: int
         * Exception: MySqlException
         * Description: This method will edit an existing additional activity
         */
        public int editAdditionalActivity(String description, String newDescription, int value)
        {
            connectToMySql();
            int success = -1;
            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE activity_categories SET activity_value = " + value + ",activity_description = '" + newDescription.Replace("'", "''") + "'" + " WHERE activity_description = '" + description.Replace("'", "''") + "'", conn);
                command.ExecuteNonQuery();
                success = 1;
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return success;
        }//end editAdditionalActivity

        /*
         * Author: Russ Utt
         * Method Name: insertSettings
         * Parameters: name of settings, value
         * Output: int
         * Exception: MySqlException
         * Description: This method is adding the default values for the settings.  Settings are the
         * maximum number of visits a credit member can earn in one day and the minimum amount of time
         * a credit member must work out to earn a visit
         */
        public void insertSettings(String name, int value)
        {
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO settings VALUES('" + name.Replace("'", "''") + "', " + value + ")", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: changeSettings
         * Parameters: name of settings, value
         * Output: int
         * Exception: MySqlException
         * Description: This method will allows the user to change the default number of visits a 
         * credit member can earn in a day and the minimum number of minutes they need to earn a visit
         */
        public void changeSettings(String name, int value)
        {
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE settings SET settings_value = " + value + " WHERE settings_name = '" + name.Replace("'", "''") + "'", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: lookupMemberById
         * Parameters: member id
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method grabs all the information from the members table about a particular member.  The member
         * must not be archived (is_archived = 0)
         */
        public DataTable lookupMemberById(String memID)
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM members WHERE member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {
            }
            finally
            {
                disconnectMySql();
            }
            return dt;

        }//end lookupMemberById

        /*
         * Author: Russ Utt
         * Method Name: lookupMemberByLastName
         * Parameters: members first and last name
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method grabs all the information from the members table about a particular member.  The member
         * must not be archived (is_archived = 0)
         */
        public DataTable lookupMemberByLastName(String lastName, String firstName)
        {
            String last = lastName.ToUpper();
            String first = firstName.ToUpper();
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM members WHERE UPPER(last_name) = '" + last.Replace("'", "''") + "' AND UPPER(first_name) = '" + first.Replace("'", "''") + "' AND is_archived = 0", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;

        }//end lookupMemberByLastName

        /*
         * Author: Russ Utt
         * Method Name: getVisitsForMember
         * Parameters: member id
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method gets the time_in, time_out, and value from all the unarchived visits in the
         * visits table for a particular member
         */
        public DataTable getVisitsForMember(String memID)
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT time_in, time_out, value FROM visits WHERE member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;

        }//end getVisistsForMember

        /*
         * Author: Russ Utt
         * Method Name: getAdditionalActivities
         * Parameters: member id
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method gets the date_assigned, value, activity_descriptions from the member_activities
         * table (non-archived) for a particul member.  This method can also be used for report generation, if the member_id 
         * is "All" then it will grab all non-archived member_activity information
         */
        public DataTable getAdditionalActivities(String memID)
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                if (memID == "All")
                {
                    MySqlDataAdapter da = new MySqlDataAdapter("SELECT member_id, date_assigned, value, activity_description FROM member_activities NATURAL JOIN activity_categories ORDER BY member_id, date_assigned DESC", conn);
                }
                else
                {
                    MySqlDataAdapter da = new MySqlDataAdapter("SELECT date_assigned, value, activity_description FROM member_activities NATURAL JOIN activity_categories WHERE member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0", conn);
                    da.Fill(dt);
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;

        }//end getAdditionalActivities

        /*
         * Author: Russ Utt
         * Method Name: setVisits
         * Parameters: value, time_in, time_out, member id
         * Output: void
         * Exception: MySqlException
         * Description: This method allows the user to edit a member's visit information for a paritcular day.
         */
        public void setVisits(int value, DateTime time_in, DateTime time_out, String memID)
        {
            String timeIn, timeOut;
            timeIn = time_in.Year + "-" + time_in.Month + "-" + time_in.Day + " " + time_in.Hour + ":" + time_in.Minute + ":" + time_in.Second;
            timeOut = time_out.Year + "-" + time_out.Month + "-" + time_out.Day + " " + time_out.Hour + ":" + time_out.Minute + ":" + time_out.Second;
            int currentValue = getVisitValue(memID, timeIn);
            int difference = 0;
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE visits SET value = " + value + ", time_in = '" + timeIn.Replace("'", "''") + "' WHERE time_in = '" + timeIn.Replace("'", "''") + "' AND member_id = '" + memID.Replace("'", "''") + "'", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            difference = value - currentValue;
            updateVisitTotal(memID, difference); //needs to update the member's total_current_visits
        }

        /*
         * Author: Russ Utt
         * Method Name: getVisitValue
         * Parameters: member id, time-in
         * Output: int
         * Exception: MySqlException
         * Description: This method gets the visit value from a certain visit.
         */
        private int getVisitValue(string memID, string timeIn)
        {
            DataTable dt = new DataTable();
            int value = 0;
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT value FROM visits WHERE member_id = '" + memID.Replace("'", "''") + "' AND time_in = '" + timeIn.Replace("'", "''") + "'", conn);
                da.Fill(dt);
                DataRow dr = dt.Rows[0];
                value = Int32.Parse(dr[0].ToString());
            }
            catch (MySqlException)
            {
            }
            finally
            {
                disconnectMySql();
            }
            return value;
        }

        /*
         * Author: Russ Utt
         * Method Name: setAdditionalActivitiesValue
         * Parameters: value, member's id, description of additional activity, date the activity was assinged
         * Output: void
         * Exception: MySqlException
         * Description: This method allows the user to change the value associated with an additional activity
         * for a particular member
         */
        public void setAdditionalActivitiesValue(int value, String memID, String description, DateTime assigned)
        {
            int actID = getActivityID(description);
            String dateAssigned = assigned.Year + "-" + assigned.Month + "-" + assigned.Day + " " + assigned.Hour + ":" + assigned.Minute + ":" + assigned.Second;
            int currentValue = getAdditionalActivityValue(memID, actID, dateAssigned);
            int difference = 0;
            connectToMySql();
            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE member_activities SET value = " + value + " WHERE member_id = '" + memID.Replace("'", "''") + "' AND activity_id = " + actID + " AND is_archived = 0 AND date_assigned = '" + dateAssigned.Replace("'", "''") + "'", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            difference = value - currentValue;
            updateVisitTotal(memID, difference);  //needs to update the member's total_current_visits
        }

        /*
         * Author: Russ Utt
         * Method Name: setAdditionalActivity
         * Parameters: date/time assigned, value, description of activity, member's id
         * Output: void
         * Exception: MySqlException
         * Description: This method allows the user to give an additional activity to a particular user
         */
        public void setAdditionalActivity(DateTime assigned, int value, String description, String memID)
        {
            String dateAssigned = assigned.Year + "-" + assigned.Month + "-" + assigned.Day + " " + assigned.Hour + ":" + assigned.Minute + ":" + assigned.Second;
            int actID = getActivityID(description);
            if (actID == 0)
            {
                actID = insertAdditionalActivity(description, value);
            }
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO member_activities VALUES(null, '" + memID.Replace("'", "''") + "', '" + dateAssigned.Replace("'", "''") + "', " + actID + ", " + value + ", 0)", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            updateVisitTotal(memID, value); //needs to update the member's total_current_visits
        }

        /*
         * Author: Russ Utt
         * Method Name: getActivityID
         * Parameters: description of the activity
         * Output: void
         * Exception: MySqlException
         * Description: This method was written as a "helper" method to grab the primary key of the activity_categories
         * to be used in other methods
         */
        public int getActivityID(String description)
        {
            int actID = 0;
            String desc = description.ToUpper();
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter("SELECT activity_id FROM activity_categories WHERE UPPER(activity_description) = '" + desc.Replace("'", "''") + "'", conn);
                dataAdapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow myRow = dt.Rows[0];
                    actID = Int32.Parse(myRow[0].ToString());
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return actID;
        }//end getActivityID

        /*
         * Author: Russ Utt
         * Method Name: insertAdditionalActivity
         * Parameters: description of the activity, value
         * Output: void
         * Exception: MySqlException
         * Description: This method inserts a new additional activity into the activity_categories table
         */
        public int insertAdditionalActivity(String description, int value)
        {
            int actID = 0;
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO activity_categories VALUES(null, '" + description.Replace("'", "''") + "', " + value + ", 1)", conn);
                command.ExecuteNonQuery();
                actID = getActivityID(description);

            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return actID;
        }

        /*
         * Author: Russ Utt
         * Method Name: createNewMember
         * Parameters: member's id, first name, last name, category type, section number, time member will expire
         * Output: void
         * Exception: MySqlException
         * Description: This method inserts a new member into the members table
         * to be used in other methods
         */
        public void createNewMember(String memID, String firstName, String lastName, String categoryName, int sectionNumber, DateTime expire)
        {
            String expireDate;
            expireDate = expire.Year + "-" + expire.Month + "-" + expire.Day + " " + expire.Hour + ":" + expire.Minute + ":" + expire.Second;
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO members VALUES('" + memID.Replace("'", "''") + "', '" + firstName.Replace("'", "''") + "', '" + lastName.Replace("'", "''") + "', '" + categoryName.Replace("'", "''") + "', " + sectionNumber + ", current_date, '" + expireDate.Replace("'", "''") + "' , current_date , 0, 0, 0)", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: updateMember
         * Parameters: member's id, first name, last name, category type, section number, time member will expire
         * Output: void
         * Exception: MySqlException
         * Description: This method updates a particular member's information in the members table
         */
        public void updateMember(String memID, String firstName, String lastName, String categoryName, int secNumber, DateTime expire)
        {
            String expireDate;
            expireDate = expire.Year + "-" + expire.Month + "-" + expire.Day + " " + expire.Hour + ":" + expire.Minute + ":" + expire.Second;
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE members SET first_name = '" + firstName.Replace("'", "''") + "', last_name = '" + lastName.Replace("'", "''") + "', cat_name = '" + categoryName.Replace("'", "''") + "', section_number = " + secNumber + ", expire_date = '" + expireDate.Replace("'", "''") + "' WHERE member_id = '" + memID.Replace("'", "''") + "'", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
        }//end updateMember

        /*
         * Author: Russ Utt
         * Method Name: getMemberInfo
         * Parameters: member's id
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method gets all information about a particular member
         */
        public DataTable getMemberInfo(String memID)
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter("SELECT first_name, last_name, cat_name, total_current_visits, expire_date FROM members WHERE member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0", conn);
                dataAdapter.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }//end getMemberInfo

        /*
         * Author: Russ Utt
         * Method Name: doesMemberExist
         * Parameters: member's id
         * Output: bool
         * Exception: MySqlException
         * Description: This method will check to see if an unarchived member with this particular member id exists in the
         * members table
         */
        public bool doesMemberExist(String memID)
        {
            DataTable dt = new DataTable();
            bool exists = false;
            connectToMySql();

            try
            {
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter("SELECT * FROM members WHERE member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0", conn);
                dataAdapter.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    exists = true;
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return exists;
        }//doesMemberExist

        /*
         * Author: Russ Utt
         * Method Name: getLatestLoggingEntry
         * Parameters: member's id
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method gets the latest visit entry for a particular user
         */
        public DataTable getLatestLoggingEntry(String memID)
        {
            int visitID = getMaxVisitID(memID);
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter("SELECT time_in, time_out, value FROM visits WHERE visit_id = " + visitID, conn);
                dataAdapter.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;

        }

        /*
         * Author: Russ Utt
         * Method Name: getMaxVisitID
         * Parameters: member's id
         * Output: int
         * Exception: MySqlException
         * Description: This method gets the latest visit_id for a particular member
         */
        public int getMaxVisitID(String memID)
        {
            int visitID = 0;
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter("SELECT MAX(visit_id) FROM visits WHERE member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0", conn);
                dataAdapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow myRow = dt.Rows[0];
                    try
                    {
                        visitID = Int32.Parse(myRow[0].ToString());
                    }
                    catch (FormatException)
                    {
                        visitID = -1;
                    }

                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return visitID;
        }//getMaxVisitID

        /*
         * Author: Russ Utt
         * Method Name: getMemberActivities
         * Parameters: member's id
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method gets all additional activity information associated with a particular member
         * to be used in other methods
         */
        public DataTable getMemberActivities(String memID)
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {

                MySqlDataAdapter dataAdapter = new MySqlDataAdapter("SELECT activity_description, date_assigned, value FROM activity_categories NATURAL JOIN member_activities WHERE member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0", conn);
                dataAdapter.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }//end getMemberActivities

        /*
         * Author: Russ Utt
         * Method Name: isCreditMember
         * Parameters: member's id
         * Output: bool
         * Exception: MySqlException
         * Description: This method checks to see if a particual member is a credit member
         */
        public bool isCreditMember(String memID)
        {
            bool isCreditMember = false;
            String categoryType = "";
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT cat_name FROM members WHERE member_id = '" + memID.Replace("'", "''") + "'", conn);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow myRow = dt.Rows[0];
                    categoryType = myRow[0].ToString();
                }
                if (categoryType == "credit")
                {
                    isCreditMember = true;
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return isCreditMember;
        }//end isCreditMember

        /*
         * Author: Russ Utt
         * Method Name: closeFailedLogoutAndGiveVisitValue
         * Parameters: member's id
         * Output: bool
         * Exception: MySqlException
         * Description: This method closes out visits when a member forgets to logout.  It needs to check if the member is 
         * a credit member or not, if they are a credit member they get a zero visit, if they are not a credit member they get 
         * a visit
         */
        public bool closeFailedLogoutAndGiveVisitValue(String memID)
        {
            bool success = false;
            bool isCredit = isCreditMember(memID);
            int visitID = getMaxVisitID(memID);

            connectToMySql();

            try
            {
                if (isCredit)
                {
                    MySqlCommand command = new MySqlCommand("UPDATE visits SET time_out = '" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:59', value = 0 WHERE member_id = '" + memID.Replace("'", "''") + "' AND visit_id = " + visitID, conn);
                    command.ExecuteNonQuery();
                    success = true;
                }
                else
                {
                    MySqlCommand command = new MySqlCommand("UPDATE visits SET time_out = '" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:59', value = 1 WHERE member_id = '" + memID.Replace("'", "''") + "' AND visit_id = " + visitID, conn);
                    command.ExecuteNonQuery();
                    success = true;
                }

            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            updateVisitTotal(memID, 1);  //update the member's total_current_visits
            return success;
        }//closeFailedLogoutAndGiveVisitValue

        /*
         * Author: Russ Utt
         * Method Name: loginMember
         * Parameters: member id
         * Output: bool
         * Exception: MySqlException
         * Description: This method inserts a row into the visits table.  It puts '0001-01-01 00:00:00' in for the logout
         * (which will later be used to check for members who forgot to logout)
         */
        public bool loginMember(String memID)
        {
            bool success = false;
            String loginTime;
            loginTime = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;

            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO visits VALUES(null, '" + memID.Replace("'", "''") + "', '" + loginTime.Replace("'", "''") + "', '0001-01-01 00:00:00', 0, 0)", conn);
                command.ExecuteNonQuery();
                success = true;
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return success;
        }//end loginMember

        /*
         * Author: Russ Utt
         * Method Name: logoutMember
         * Parameters: member id, value
         * Output: void
         * Exception: MySqlException
         * Description: This method is used when a member logs out, it updates the time_out to the current time
         * and updates the value appropriatley
         */
        public bool logoutMember(String memID, int value)
        {
            bool success = false;
            String logoutTime;
            logoutTime = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
            int visitID = getMaxVisitID(memID);

            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE visits SET time_out = '" + logoutTime.Replace("'", "''") + "', value = " + value + " WHERE visit_id = " + visitID, conn);
                command.ExecuteNonQuery();
                success = true;
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            updateVisitTotal(memID, value); //update the member's total_current_visits
            return success;
        }//end logoutMember

        /*
         * Author: Russ Utt
         * Method Name: getZeroVisits
         * Parameters: member id
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method gets all the zero visits for a particular member (checks to see if a member
         * has any visits that still have a time_out = '0001-01-01 00:00:00'
         */
        public DataTable getZeroVisits(String memID)
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT time_in, time_out FROM visits WHERE time_out = '0001-01-01 00:00:00' AND member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0 AND time_in < curdate() ORDER BY time_in DESC", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: isAdminPasswordCorrect
         * Parameters: password
         * Output: void
         * Exception: MySqlException
         * Description: This method checks to see if the password given is the correct administrator password
         */
        public bool isAdminPasswordCorrect(String pass)
        {
            bool correct = false;
            String password = "";
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM users WHERE user_role = 'admin';", conn);
                da.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    DataRow myRow = dt.Rows[0];

                    password = myRow["password"].ToString();
                        
                    correct = true;
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            
           
            return (verifyHash(pass, password));
           

        }

        /*
         * Author: Russ Utt
         * Method Name: archiveMember
         * Parameters: member id
         * Output: void
         * Exception: MySqlException
         * Description: This method will archive a particular member, his visits, and his additional activities.  
         * It also needs to move their total_current_visits to total_visits and sets total_current_visits = 0
         */
        public void archiveMember(String memID)
        {
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("update members set total_current_visits =0 where member_id = '"+ memID +
                    "' and total_current_visits <0;", conn);
                command.ExecuteNonQuery();
                command = new MySqlCommand("UPDATE members SET is_archived = 1, total_visits = (total_visits + total_current_visits), total_current_visits = 0, expire_date = current_date WHERE member_id = '" + memID.Replace("'", "''") + "'", conn);
                command.ExecuteNonQuery();
                command = new MySqlCommand("UPDATE visits SET is_archived = 1 WHERE member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0", conn);
                command.ExecuteNonQuery();
                command = new MySqlCommand("UPDATE member_activities SET is_archived = 1 WHERE member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0", conn);
                command.ExecuteNonQuery();

            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
        }//end archiveMember

        /*
         * Author: Russ Utt
         * Method Name: getFailedLogouts
         * Parameters: none
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method gets a list of all "unarchived" missed logouts (where time_out =
         * '0001-01-01 00:00:00'
         */
        public DataTable getFailedLogouts()
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT visit_id, member_id, cat_name FROM visits NATURAL JOIN members WHERE time_out = '0001-01-01 00:00:00' AND is_archived = 0", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }//end getFailedLogouts

        /*
         * Author: Russ Utt
         * Method Name: getActivityID
         * Parameters: description of the activity
         * Output: void
         * Exception: MySqlException
         * Description: This will check for all un-archived failed logouts (anything with a logout
         * time of 0001-01-01 00:00:00) and closes them and gives a visit value of 1
         * if they are a credit member and a 0 if they are a credit member, I'm giving
         * a time_out of the current date and time of 23:59:59 */
        public void closeAllFailedLogouts()
        {
            DataTable dt = getFailedLogouts();
            if (dt.Rows.Count > 0)
            {
                connectToMySql();
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        if (dr[2].ToString() == "non-credit")
                        {
                            MySqlCommand command = new MySqlCommand("UPDATE visits SET time_out = '" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:59', value = 1 WHERE visit_id = " + Int32.Parse(dr[0].ToString()), conn);
                            command.ExecuteNonQuery();
                            command = new MySqlCommand("UPDATE members SET total_current_visits = (total_current_visits + 1) WHERE member_id = '" + dr[1].ToString() + "'", conn);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            MySqlCommand command = new MySqlCommand("UPDATE visits SET time_out = '" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:59', value = 0 WHERE visit_id = " + Int32.Parse(dr[0].ToString()), conn);
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (MySqlException)
                    {

                    }
                }
                disconnectMySql();
            }
        }//end closeAllFailedLogouts

        /*
         * Author: Russ Utt
         * Method Name: isMemberArchived
         * Parameters: member id
         * Output: bool
         * Exception: MySqlException
         * Description: This method will check to see if a member is archived (is_archived = 1)
         */
        public bool isMemberAchived(String memID)
        {
            DataTable dt = new DataTable();
            bool isArchived = false;
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM members WHERE is_archived = 1 AND member_id = '" + memID.Replace("'", "''") + "'", conn);
                da.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    isArchived = true;
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return isArchived;
        }//end isMemberArchived

        /*
         * Author: Russ Utt
         * Method Name: activeMemberFromArchive
         * Parameters: member id
         * Output: void
         * Exception: MySqlException
         * Description: This method will "pull" a member from archived to unarchived (is_archived = 1 to is_archived = 0).
         * It also set the date_activated to the current date and sets a new expire_date
         */
        public void activeMemberFromArchive(String memID)
        {
            String expire = getDefaultExpireDate();
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE members SET is_archived = 0, date_activated = current_date, expire_date = '" + expire.Replace("'", "''") + "' WHERE member_id = '" + memID.Replace("'", "''") + "'", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: purge2
         * Parameters: date/time of purge
         * Output: void
         * Exception: MySqlException
         * Description: This method will delete all archived members, visits, and additional activities prior to a certain date.
         * 
         * notes from josh montgomery: this method of perging will possibly delete 
         * visits and activities from members who do NOT fall into the purge criteria.
         * also this purge will fail if ANY of the visits or activities are greater than
         * the purge date. this in part to the archive method changing the expire date to the
         * current date when the member is archived. that is not the only cause, so i have
         * made a new purge that addresses these issues.
         * 
         */
        public bool purge2(DateTime time)
        {
            String purgeTime = time.Year + "-" + time.Month + "-" + time.Day + " 23:59:59";
            //MessageBox.Show(purgeTime);
            purgeVisits(purgeTime);
            purgeActivities(purgeTime);
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("DELETE FROM members WHERE expire_date < '" + purgeTime.Replace("'", "''") + "' AND is_archived = 1", conn);
                command.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
        * Author: Russ Utt
        * Method Name: purgeVisits
        * Parameters: date/time of purge
        * Output: void
        * Exception: MySqlException
        * Description: This method will delete all archived visits that have a time_in prior to the given date
        */
        public void purgeVisits(String time)
        {
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("DELETE FROM visits WHERE time_in < '" + time.Replace("'", "''") + "' AND is_archived = 1", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: purge
         * Parameters: date/time of purge
         * Output: void
         * Exception: MySqlException
         * Description: This method will delete all archived member_activities that have a date_assigned prior to a given date.
         */
        public void purgeActivities(String time)
        {
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("DELETE FROM member_activities WHERE date_assigned < '" + time.Replace("'", "''") + "' AND is_archived = 1", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
            }
            finally
            {
                disconnectMySql();
            }
        }
        /* this is a bells and whistle feature that I don't believe will be implemented */
        public void automaticBackup()
        {
        }
        /* this is a bells and whistle feature that I don't believe will be implemented */
        public void restoreBackup()
        {
        }

        /* this is a bells and whistle feature that I don't believe will be implemented */
        public void importMembers()
        {
        }

        

        //I don't know who wrote this or what it is used for
        public DataTable getAllCurLastFirstNames()
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT member_id, first_name, last_name FROM members WHERE is_archived = 0", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        //I don't know who wrote this or what it is used for
        public DataTable getAllCurMemberId()
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM members WHERE is_archived = 0", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: changeVisitValue
         * Parameters: member id, value of visit
         * Output: void
         * Exception: MySqlException
         * Description: This method will update the value of a visit for a particular member
         */
        public void changeVisitValue(String memID, int value)
        {
            int visitID = getMaxVisitID(memID);

            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE visits SET value = " + value + " WHERE visit_id = " + visitID, conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            updateVisitTotal(memID, value);
        }

        /*
         * Author: Russ Utt
         * Method Name: setIsArchived
         * Parameters: member id, archive flag
         * Output: void
         * Exception: MySqlException
         * Description: This method will either set is_archive to 0 or 1 for a particular member
         */
        public void setIsArchived(String memID, int setArchive)
        {
            if (setArchive == 1)
            {
                archiveMember(memID);
            }
            else
            {
                activeMemberFromArchive(memID);
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: memberVisitInfoForSummaryReport
         * Parameters: time_in, time_out, member id
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method will get all member and visit info for a particular member
         */
        public DataTable memberVisitInfoForSummaryReport(DateTime start, DateTime end, String memID)
        {
            DataTable dt = new DataTable();

            connectToMySql();
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT member_id, first_name, last_name, section_number, time_in, time_out, value FROM members NATURAL JOIN visits WHERE member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0 AND time_in BETWEEN '" + start.Year + "-" + start.Month + "-" + start.Day + " 00:00:00' AND '" + end.Year + "-" + end.Month + "-" + end.Day + " 23:59:59'", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: additionalActivitiesForMemberSummary
         * Parameters: time from, time to, member id
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method will get all member_activities for a particular member between 2 dates
         */
        public DataTable additionalActivitiesForMemberSummary(DateTime start, DateTime end, String memID)
        {
            DataTable dt = new DataTable();
            connectToMySql();
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT activity_description, date_assigned, value FROM activity_categories NATURAL JOIN member_activities WHERE member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0 AND date_assigned BETWEEN '" + start.Year + "-" + start.Month + "-" + start.Day + "' AND '" + end.Year + "-" + end.Month + "-" + end.Day + "'", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: getDefaultExpireDate
         * Parameters: none
         * Output: String
         * Exception: MySqlException
         * Description: This method gets the default expiration date that is set in the database
         */
        public String getDefaultExpireDate()
        {
            String expireDate = "";
            DateTime time = new DateTime();
            DataTable dt = new DataTable();
            connectToMySql();
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT expiration_date FROM dates", conn);
                da.Fill(dt);
                //MessageBox.Show(dt.Rows.Count.ToString());
                if (dt.Rows.Count == 1)
                {
                    DataRow myRow = dt.Rows[0];
                    time = Convert.ToDateTime(myRow[0].ToString());
                    if (time <= System.DateTime.Now)
                    {
                        setDefaultExpDate(System.DateTime.Now);
                        expireDate = System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + " 23:59:59";
                    }
                    else
                    {
                        expireDate = time.Year + "-" + time.Month + "-" + time.Day + " 23:59:59";
                    }
                }
                else if (dt.Rows.Count == 0)
                {
                    createDefaultExpirationDate(System.DateTime.Now);
                    expireDate = System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + " 23:59:59";
                }

            }
            catch (MySqlException)
            {
                expireDate = System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + " 23:59:59";
            }
            finally
            {
                disconnectMySql();
            }
            return expireDate;
        }

        private void createDefaultExpirationDate(DateTime dateTime)
        {
            String date = System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + " 23:59:59";
            connectToMySql();
            try
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO dates VALUES('" + date + "', '" + date + "')", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: getSections
         * Parameters: none
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method will gets all the "visible" sections in the database
         */
        public DataTable getSections()
        {
            DataTable dt = new DataTable();
            connectToMySql();
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT section_number FROM section WHERE is_visible = 1 ORDER BY 1", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: getCurrentMinMinutesPerVisit
         * Parameters: none
         * Output: int
         * Exception: MySqlException
         * Description: This method gets the minimum number of minutes a credit member needs to workout in order 
         * to receive a visit
         */
        public int getCurrentMinMinutesPerVisit()
        {
            DataTable dt = new DataTable();
            int min = -1;
            connectToMySql();
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT settings_value FROM settings WHERE settings_name = 'min_time_for_visit'", conn);
                da.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    DataRow myRow = dt.Rows[0];
                    min = Int32.Parse(myRow[0].ToString());
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return min;
        }

        /*
         * Author: Russ Utt
         * Method Name: getMaxVisitsAllowed
         * Parameters: none
         * Output: int
         * Exception: MySqlException
         * Description: This method gets the maximum number of visits a credit member can earn in a day
         */
        public int getMaxVisitsAllowed()
        {
            DataTable dt = new DataTable();
            int max = -1;
            connectToMySql();
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT settings_value FROM settings WHERE settings_name = 'max_visits_per_day'", conn);
                da.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    DataRow myRow = dt.Rows[0];
                    max = Int32.Parse(myRow[0].ToString());
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return max;
        }

        /*
         * Author: Russ Utt
         * Method Name: getCurrentUsers
         * Parameters: none
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method gets all the users from the database (users are those who can login into the 
         * management console)
         */
        public DataTable getCurrentUsers()
        {
            DataTable dt = new DataTable();
            connectToMySql();
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM users", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: getCurrentCategories
         * Parameters: none
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method gets all the visible (is_visible = 1) categories from the database
         */
        public DataTable getCurrentCategories()
        {
            DataTable dt = new DataTable();
            connectToMySql();
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT activity_description, activity_value FROM activity_categories WHERE is_visible = 1", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: archiveExpiredMembers
         * Parameters: none
         * Output: void
         * Exception: MySqlException
         * Description: This method will archive all members who are past the expiration date, as well as their visits
         * and additional activities
         */
        public void archiveExpiredMembers()
        {

            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE members SET is_archived = 1, total_visits = (total_visits + total_current_visits), total_current_visits = 0, expire_date = current_date WHERE expire_date < current_date", conn);
                command.ExecuteNonQuery();
                command = new MySqlCommand("UPDATE archiveMembersVisits SET is_archived = 1", conn);
                command.ExecuteNonQuery();
                command = new MySqlCommand("UPDATE archiveMembersActivities SET is_archived = 1", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
        }

       

        /*
         * Author: Russ Utt
         * Method Name: getUserInfo
         * Parameters: user's name
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method gets all the user info (a user is someone who can login into the 
         * management console)
         */
        public DataTable getUserInfo(String userName)
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT user_role, password FROM users WHERE user_name = '" + userName.Replace("'", "''") + "'", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: getSections
         * Parameters: none
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method will gets just the user names (a user is someone who can login into
         * the management console)
         */
        public DataTable getAllUserNames()
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT user_name FROM users WHERE user_role != 'admin'", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: nonCreditLogin
         * Parameters: member id
         * Output: bool
         * Exception: MySqlException
         * Description: This method will inserts a row into the visits table when a non-credit member logins. 
         * A non-credit member gets a visit at login.
         */
        public bool nonCreditLogin(String memID)
        {
            bool success = false;
            String loginTime;
            loginTime = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;

            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO visits VALUES(null, '" + memID.Replace("'", "''") + "', '" + loginTime.Replace("'", "''") + "', '0001-01-01 00:00:00', 1, 0)", conn);
                command.ExecuteNonQuery();
                success = true;
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            updateVisitTotal(memID, 1);
            return success;
        }

        /*
         * Author: Russ Utt
         * Method Name: nonCreditLogout
         * Parameters: member id
         * Output: bool
         * Exception: MySqlException
         * Description: This method logs out a non-credit member.  Since a visit is already recorded it just updates the
         * time_out to the current time
         */
        public bool nonCreditLogout(String memID)
        {
            bool success = false;
            String logoutTime;
            logoutTime = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
            int visitID = getMaxVisitID(memID);

            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE visits SET time_out = '" + logoutTime.Replace("'", "''") + "' WHERE visit_id = " + visitID, conn);
                command.ExecuteNonQuery();
                success = true;
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return success;
        }

        /*
         * Author: Russ Utt
         * Method Name: addAMemberVisit
         * Parameters: member id, value, time_in, time_out
         * Output: void
         * Exception: MySqlException
         * Description: This method allows a user to give a member a visit
         */
        public void addAMemberVisit(String memID, int value, DateTime time_in, DateTime time_out)
        {
            String timeIn, timeOut;
            timeIn = time_in.Year + "-" + time_in.Month + "-" + time_in.Day + " " + time_in.Hour + ":" + time_in.Minute + ":" + time_in.Second;
            timeOut = time_out.Year + "-" + time_out.Month + "-" + time_out.Day + " " + time_out.Hour + ":" + time_out.Minute + ":" + time_out.Second;
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO visits VALUES(null, '" + memID.Replace("'", "''") + "', '" + timeIn.Replace("'", "''") + "', '" + timeOut.Replace("'", "''") + "', " + value + ", 0)", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            updateVisitTotal(memID, value);
        }

        /*
         * Author: Russ Utt
         * Method Name: getSections
         * Parameters: none
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method gets all visible member categories
         */
        public DataTable getMemberCategories()
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM categories WHERE is_visible = 1", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: getAllCreditMemberInfo
         * Parameters: member id
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method gets all member info for a particular credit member
         */
        public DataTable getAllCreditMemberInfo(String memID)
        {
            DataTable dt = null;
            connectToMySql();

            try
            {
                dt = new DataTable();

                if (memID == "All")
                {

                    MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM members WHERE cat_name = 'credit'", conn);
                    da.Fill(dt);
                }
                else
                {

                    MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM members WHERE member_id = '" + memID.Replace("'", "''") + "' AND cat_name = 'credit'", conn);
                    da.Fill(dt);
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: getAllCreditMemberInfo
         * Parameters: member id
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method gets all member info for a particular member
         */
        public DataTable getAllMemberInfo(String memID)
        {
            DataTable dt = null;
            connectToMySql();

            try
            {
                dt = new DataTable();

                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM members WHERE member_id = '" + memID.Replace("'", "''") + "'", conn);
                da.Fill(dt);

            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        //Author: Joe Kearns
        public DataTable getVisitInfoForCompReport(String memID, DateTime dateFrom, DateTime dateTo)
        {
            String fromDate, toDate;
            fromDate = dateFrom.Year + "-" + dateFrom.Month + "-" + dateFrom.Day + " 00:00:00";
            toDate = dateTo.Year + "-" + dateTo.Month + "-" + dateTo.Day + " 23:59:59";

            DataTable dt = null;
            connectToMySql();

            try
            {

                dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT time_in, time_out, value FROM visits WHERE member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0 AND time_in BETWEEN '" + fromDate.Replace("'", "''") + "' AND '" + toDate.Replace("'", "''") + "'", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        //Author: Joe Kearns
        public DataTable getActivitesForCompReport(String memID, DateTime dateFrom, DateTime dateTo)
        {
            String fromDate, toDate;
            fromDate = dateFrom.Year + "-" + dateFrom.Month + "-" + dateFrom.Day + " 00:00:00";
            toDate = dateTo.Year + "-" + dateTo.Month + "-" + dateTo.Day + " 23:59:59";

            DataTable dt = null;
            connectToMySql();

            try
            {
                if (memID == "All")
                {
                    dt = new DataTable();
                    MySqlDataAdapter da = new MySqlDataAdapter("Select member_id, date_assigned, activity_description, value FROM member_activities NATURAL JOIN activity_categories WHERE is_archived = 0 ORDER BY member_id,date_assigned DESC", conn);
                }
                else
                {
                    dt = new DataTable();
                    MySqlDataAdapter da = new MySqlDataAdapter("SELECT date_assigned, value, activity_description,member_id FROM member_activities NATURAL JOIN activity_categories WHERE member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0 AND date_assigned BETWEEN '" + fromDate.Replace("'", "''") + "' AND '" + toDate.Replace("'", "''") + "'", conn);
                    da.Fill(dt);
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: getNumberMembers
         * Parameters: none
         * Output: int
         * Exception: MySqlException
         * Description: This method gets the total number of rows in the member table
         */
        public int getNumberMembers()
        {
            int count = 0;
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT COUNT(member_id) FROM members", conn);
                da.Fill(dt);
                DataRow dr = dt.Rows[0];
                count = Int32.Parse(dr[0].ToString());
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return count;
        }

        /*
         * Author: Russ Utt
         * Method Name: dayOfWeekReport
         * Parameters: category name, name of the day of week, date from, date to
         * Output: int
         * Exception: MySqlException
         * Description: This method counts the number of a particular day there are between 2 date.  
         * For example, how many Mondays are between 9-29-2006 and 10-12-2006 
         */
        public int GetNumWeekDays(String catName, String dayName, DateTime dateFrom, DateTime dateTo)
        {
            int count = -1;
            String fromDate = dateFrom.Year + "-" + dateFrom.Month + "-" + dateFrom.Day + " 00:00:00";
            String toDate = dateTo.Year + "-" + dateTo.Month + "-" + dateTo.Day + " 23:59:59";

            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                if (catName == "All")
                {
                    MySqlDataAdapter da = new MySqlDataAdapter("SELECT COUNT(visit_id) FROM visits NATURAL JOIN members WHERE dayname(time_in) = '" + dayName.Replace("'", "''") + "' AND visits.is_archived = 0 AND time_in BETWEEN '" + fromDate.Replace("'", "''") + "' AND '" + toDate.Replace("'", "''") + "'", conn);
                    da.Fill(dt);
                    DataRow myRow = dt.Rows[0];
                    count = Int32.Parse(myRow[0].ToString());
                    return count;
                }
                else
                {
                    MySqlDataAdapter da = new MySqlDataAdapter("SELECT COUNT(visit_id) FROM visits NATURAL JOIN members WHERE dayname(time_in) = '" + dayName.Replace("'", "''") + "' AND visits.is_archived = 0 AND members.cat_name = '" + catName.Replace("'", "''") + "' AND time_in BETWEEN '" + fromDate.Replace("'", "''") + "' AND '" + toDate.Replace("'", "''") + "'", conn);
                    da.Fill(dt);
                    DataRow myRow = dt.Rows[0];
                    count = Int32.Parse(myRow[0].ToString());
                    return count;
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return count;
        }

        /*
         * Author: Russ Utt
         * Method Name: coutnDayNames
         * Parameters: name of the day of week, date from, date to
         * Output: int
         * Exception: MySqlException
         * Description: This method counts the number of days between 2 dates
         */
        public int CountTotalNumDays(String dayName, DateTime dateFrom, DateTime dateTo)
        {
            int count = -1;
            String fromDate = dateFrom.Year + "-" + dateFrom.Month + "-" + dateFrom.Day + " 00:00:00";
            String toDate = dateTo.Year + "-" + dateTo.Month + "-" + dateTo.Day + " 23:59:59";

            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT countDays ('" + fromDate.Replace("'", "''") + "', '" + toDate.Replace("'", "''") + "', '" + dayName.Replace("'", "''") + "')", conn);
                da.Fill(dt);
                DataRow myRow = dt.Rows[0];
                count = Int32.Parse(myRow[0].ToString());
                if (count == 0)
                {
                    count = 1;
                }
                return count;
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return count;

        }

        /*
         * Author: Russ Utt
         * Method Name: hourOfDayReport
         * Parameters: hour, date from, date to
         * Output: int
         * Exception: MySqlException
         * Description: This method counts how many unarchived visits had a time_in at a certain hour.
         * For example, how many people logged in between 11:00 and 11:59 between 9-29-2006 and 10-13-2006.
         */
        public int hourOfDayReport(int hourOfDay, String catName, DateTime dateFrom, DateTime dateTo)
        {
            int count = -1;
            String fromDate = dateFrom.Year + "-" + dateFrom.Month + "-" + dateFrom.Day + " 00:00:00";
            String toDate = dateTo.Year + "-" + dateTo.Month + "-" + dateTo.Day + " 23:59:59";

            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                if (catName == "All")
                {
                    MySqlDataAdapter da = new MySqlDataAdapter("SELECT COUNT(visit_id) FROM visits NATURAL JOIN members WHERE hour(time_in) = " + hourOfDay + " AND visits.is_archived = 0 AND time_in BETWEEN '" + fromDate.Replace("'", "''") + "' AND '" + toDate.Replace("'", "''") + "'", conn);
                    da.Fill(dt);
                    DataRow myRow = dt.Rows[0];
                    count = Int32.Parse(myRow[0].ToString());
                    return count;
                }
                else
                {
                    MySqlDataAdapter da = new MySqlDataAdapter("SELECT COUNT(visit_id) FROM visits NATURAL JOIN members WHERE hour(time_in) = " + hourOfDay + " AND visits.is_archived = 0 AND members.cat_name = '" + catName.Replace("'", "''") + "' AND time_in BETWEEN '" + fromDate.Replace("'", "''") + "' AND '" + toDate.Replace("'", "''") + "'", conn);
                    da.Fill(dt);
                    DataRow myRow = dt.Rows[0];
                    count = Int32.Parse(myRow[0].ToString());
                    return count;
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return count;
        }

        /*
         * Author: Russ Utt
         * Method Name: dateDiff
         * Parameters: date from, date to
         * Output: int
         * Exception: MySqlException
         * Description: This method counts the number of days between 2 dates
         */
        public int dateDiff(DateTime dateFrom, DateTime dateTo)
        {
            int diff = -1;
            String fromDate = dateFrom.Year + "-" + dateFrom.Month + "-" + dateFrom.Day + " 00:00:00";
            String toDate = dateTo.Year + "-" + dateTo.Month + "-" + dateTo.Day + " 23:59:59";

            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT DATEDIFF('" + toDate.Replace("'", "''") + "', '" + fromDate.Replace("'", "''") + "')", conn);
                da.Fill(dt);
                DataRow myRow = dt.Rows[0];
                diff = Int32.Parse(myRow[0].ToString());
                if (diff <= 0)
                {
                    diff = 1;
                }
                return diff;
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return diff = 1;

        }

        /*
         * Author: Russ Utt
         * Method Name: createFunction
         * Parameters: none
         * Output: void
         * Exception: MySqlException
         * Description: This method creates a stored function.  I'm using this because restoring a backed up copy was 
         * usually failing to restore this function.  The function counts how many days happen between 2 dates.  
         * For example how many Mondays occurred between 2 dates.
         */
        public void createFunction()
        {
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("CREATE FUNCTION countDays (x DateTime, y DateTime, day VARCHAR(10)) RETURNS INT BEGIN DECLARE count INT DEFAULT 0; WHILE (x < y) DO SET x = DATE_ADD(x, INTERVAL 1 DAY); IF DAYNAME(x) = day THEN SET count = count + 1; END IF; END WHILE; RETURN count; END;", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: doesFunctionExist
         * Parameters: none
         * Output: bool
         * Exception: MySqlException
         * Description: This method checks to see if the stored function that is created during createFunction() exists.
         */
        public bool doesFunctionExist()
        {
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("SELECT countDays('2006-05-21 12:12:12', '2006-05-28 1:12:12', 'Sunday')", conn);
                command.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
            finally
            {
                disconnectMySql();
            }

        }

        /*
         * Author: Russ Utt
         * Method Name: doesActivityExist
         * Parameters: description of activity
         * Output: bool
         * Exception: MySqlException
         * Description: This method checks to see if a certain additional activity exists in the database.
         */
        public bool doesActivityExist(String desc)
        {
            int count = -1;
            String description = desc.ToUpper();
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT activity_description FROM activity_categories WHERE UPPER(activity_description) = '" + description + "' AND is_visible = 1", conn);
                da.Fill(dt);
                count = dt.Rows.Count;
                if (count >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (MySqlException)
            {
                return true;
            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: doesActivityExistButNotVisible
         * Parameters: description of activity
         * Output: bool
         * Exception: MySqlException
         * Description: This method checks to see if a certain additional activity exists but is_visible = 0;
         */
        public bool doesActivityExistButNotVisible(String desc)
        {
            int count = -1;
            String description = desc.ToUpper();
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT activity_description FROM activity_categories WHERE UPPER(activity_description) = '" + description.Replace("'", "''") + "' AND is_visible = 0", conn);
                da.Fill(dt);
                count = dt.Rows.Count;
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (MySqlException)
            {
                return true;
            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: doesUserExist
         * Parameters: user's name
         * Output: bool
         * Exception: MySqlException
         * Description: This method checks to see if a certain user exists in the database.
         */
        public bool doesUserExist(String userName)
        {
            int count = -1;
            String user = userName.ToUpper();
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM users WHERE UPPER(user_name) = '" + user.Replace("'", "''") + "'", conn);
                da.Fill(dt);
                count = dt.Rows.Count;
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (MySqlException)
            {
                return true;
            }
            finally
            {
                disconnectMySql();
            }
        }

        //this method added by Joe to get all the credit members in a ddlist on reports page
        public DataTable getAllCreditMembers()
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT member_id FROM members WHERE cat_name='credit' AND is_archived = 0", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {
            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: getCurrentUserCategories
         * Parameters: none
         * Output: DataTable
         * Exception: MySqlException
         * Description: This method gets all info from categories (such as "credit", "non-credit", etc)
         */
        public DataTable getCurrentUserCategories()
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM categories", conn);
                da.Fill(dt);
            }
            catch (MySqlException)
            {
            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: doesSectionExist
         * Parameters: user's name
         * Output: bool
         * Exception: MySqlException
         * Description: This method checks to see if a certain section exists in the database.
         */
        public bool doesSectionExist(int secNum)
        {
            DataTable dt = new DataTable();
            int count = -1;
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT section_number FROM section WHERE section_number = " + secNum + " AND is_visible = 1", conn);
                da.Fill(dt);
                count = dt.Rows.Count;
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (MySqlException)
            {
                return true;
            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: doesSectionExistButNotVisible
         * Parameters: user's name
         * Output: bool
         * Exception: MySqlException
         * Description: This method checks to see if a certain section exists but is_visible= 0
         */
        public bool doesSectionExistButNotVisible(int sec)
        {
            int count = -1;
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT section_number FROM section WHERE section_number = " + sec + " AND is_visible = 0", conn);
                da.Fill(dt);
                count = dt.Rows.Count;
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (MySqlException)
            {
                return true;
            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: removeMembersAdditionalActivities
         * Parameters: DateTime assigned,description of activity, value, member id
         * Output: void
         * Exception: MySqlException
         * Description: This method removes a particular member activity for a particular member.
         */
        public void removeMembersAdditionalActivities(DateTime assigned, String desc, int value, String memID)
        {
            int actID = getActivityID(desc);
            String dateAssigned = assigned.Year + "-" + assigned.Month + "-" + assigned.Day + " " + assigned.Hour + ":" + assigned.Minute + ":" + assigned.Second;
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("DELETE FROM member_activities WHERE date_assigned = '" + dateAssigned.Replace("'", "''") + "' AND member_id = '" + memID.Replace("'", "''") + "' AND activity_id = " + actID + " AND value = " + value, conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            int negValue = value * -1;
            updateVisitTotal(memID, negValue);
        }

        /* The following 2 methods, ComputeHash and VerifyHash deal with encrypting
         * passwords.  The code was lifted from http://www.obviex.com/samples/hash.aspx
         * with very little modification by me.  The saltBytes passed into the 
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

        /*
         * Author: Russ Utt
         * Method Name: getActivitiesForCreditMemReport
         * Parameters: member id, date from, date to
         * Output: bool
         * Exception: MySqlException
         * Description: This method gets a particular members member_activities between 2 dates.
         */
        public DataTable getActivitesForCreditMemReport(String memID, DateTime dateFrom, DateTime dateTo)
        {
            String fromDate, toDate;
            fromDate = dateFrom.Year + "-" + dateFrom.Month + "-" + dateFrom.Day + " 00:00:00";
            toDate = dateTo.Year + "-" + dateTo.Month + "-" + dateTo.Day + " 23:59:59";

            DataTable dt = null;
            connectToMySql();

            try
            {
                if (memID == "All")
                {
                    dt = new DataTable();
                    MySqlDataAdapter da = new MySqlDataAdapter("Select member_id, date_assigned, activity_description, value FROM member_activities NATURAL JOIN activity_categories WHERE is_archived = 0 AND date_assigned BETWEEN '" + fromDate.Replace("'", "''") + "' AND '" + toDate.Replace("'", "''") + "'ORDER BY member_id,date_assigned DESC", conn);
                }
                else
                {
                    dt = new DataTable();
                    MySqlDataAdapter da = new MySqlDataAdapter("SELECT member_id, date_assigned, value, activity_description FROM member_activities NATURAL JOIN activity_categories WHERE member_id = '" + memID.Replace("'", "''") + "' AND is_archived = 0 AND date_assigned BETWEEN '" + fromDate.Replace("'", "''") + "' AND '" + toDate.Replace("'", "''") + "'", conn);
                    da.Fill(dt);
                }
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
            return dt;
        }

        /*
         * Author: Russ Utt
         * Method Name: setDefaultExpDate
         * Parameters: expire date
         * Output: int
         * Exception: MySqlException
         * Description: This method sets a new default expiration date.
         */
        public int setDefaultExpDate(DateTime date)
        {
            int success = -1;

            String dateExpired = date.Year + "-" + date.Month + "-" + date.Day + " 23:59:59";

            connectToMySql();
            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE dates SET expiration_date = " + "'" + dateExpired + "'", conn);
                command.ExecuteNonQuery();
                success = 1;
            }

            catch (MySqlException)
            {

            }

            finally
            {
                disconnectMySql();
            }
            return success;
        }

        /*
         * Author: Russ Utt
         * Method Name: isPasswordCorrect
         * Parameters: user's name, password
         * Output: bool
         * Exception: MySqlException
         * Description: This method checks to see if a user's password is correct.
         */
        public bool isPasswordCorrect(String userName, String password)
        {
            String user = userName.ToUpper();
            String userPassword = "";
            DataTable dt = new DataTable();

            connectToMySql();
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT password FROM users WHERE UPPER(user_name) = '" + user.Replace("'", "''") + "'", conn);
                da.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    DataRow dr = dt.Rows[0];
                    userPassword = dr[0].ToString();
                }
            }
            catch (MySqlException)
            {
            }
            finally
            {
                disconnectMySql();
            }
            
            return verifyHash(password, userPassword);
            
        }

        /*
         * Author: Russ Utt
         * Method Name: updateVisitTotal
         * Parameters: member id, value
         * Output: bool
         * Exception: MySqlException
         * Description: This method updates a particular member's total_current_visits by a certain value.
         */
        public void updateVisitTotal(String memID, int value)
        {
            connectToMySql();

            try
            {
                MySqlCommand command = new MySqlCommand("UPDATE members SET total_current_visits = (total_current_visits + " + value + " ) WHERE member_id = '" + memID.Replace("'", "''") + "'", conn);
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {

            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: doesAdminAccountExist
         * Parameters: none
         * Output: bool
         * Exception: MySqlException
         * Description: This method checks to see if an account exists with the user_name "Administrator". 
         */
        public bool doesAdminAccountExist()
        {
            DataTable dt = new DataTable();
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM users WHERE user_name = 'Administrator'", conn);
                da.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    return true;
                }
                else
                {
                    return createAdminAccount();
                }
            }
            catch (MySqlException)
            {
                return createAdminAccount();
            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: createAdminAccount
         * Parameters: none
         * Output: bool
         * Exception: MySqlException
         * Description: This method creates an Administrator account with the default password "password".
         * It is being used at installation to set a default Administrator password.
         */
        public bool createAdminAccount()
        {
            disconnectMySql();
            connectToMySql();

            try
            {
                String password = computeHash("password", null);
                MySqlCommand command = new MySqlCommand("INSERT INTO users VALUES('Administrator', 'admin', '" + password + "')", conn);
                command.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
            finally
            {
                disconnectMySql();
            }
        }

        /*
         * Author: Russ Utt
         * Method Name: getAdditionalActivityValue
         * Parameters: member id, activity id, date assigned
         * Output: bool
         * Exception: MySqlException
         * Description: This method gets the value of a particular member_activity id
         */
        public int getAdditionalActivityValue(String memID, int actID, String dateAssigned)
        {
            DataTable dt = new DataTable();
            int value = 0;
            connectToMySql();

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT value FROM member_activities WHERE member_id = '" + memID.Replace("'", "''") + "' AND activity_id = " + actID + " AND date_assigned = '" + dateAssigned.Replace("'", "''") + "'", conn);
                da.Fill(dt);
                DataRow dr = dt.Rows[0];
                value = Int32.Parse(dr[0].ToString());
                return value;

            }
            catch (MySqlException)
            {
            }
            finally
            {
                disconnectMySql();
            }
            return value;
        }

        

        /*****START Josh's REGION**********************************************************
         * Author: Joshua Montgomery
         * this region contains:
         * 
         * public DateTime getLatestTimeIn(String memberID)
         * public DateTime getLatestTimeOut(String memberID)
         * public int getLatestVisitValue(String memberID)
         * public int getTodaysVisitCount(String memberID)
         * public int getMaxVisitsPerDay()
         * public int getRequiredMinutesPerVisit()
         * public bool purge(DateTime time)
         * public void purgeMember(String memberID) 
         * public int getPurgeCriteriaCount(DateTime date)
        */
        #region Josh's code

        /*
        * Author: Joshua Montgomery
        * Method Name: getLatestTimein
        * Parameters: String 
        * Output: DateTime
        * Exception: Exception
        * Description:  gets the logout time of the latest
        */
        public DateTime getLatestTimeIn(String memberID)
        {
            MySqlDataReader reader;
            int visitID = getMaxVisitID(memberID);
            DateTime time_in = new DateTime();
            time_in = DateTime.MinValue;
            connectToMySql();
            String command = "SELECT time_in, time_out, value FROM visits WHERE visit_id = " + visitID;
            MySqlCommand c = new MySqlCommand(command, conn);
            reader = c.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    time_in = reader.GetDateTime(0);
                }
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
            finally
            {
                reader.Close();
                disconnectMySql();
            }
            return time_in;

        }

        /*
        * Author: Joshua Montgomery
        * Method Name: getLatestTimeOut
        * Parameters: String 
        * Output: DateTime
        * Exception: Exception
        * Description:  gets the logout time of the latest
        */
        public DateTime getLatestTimeOut(String memberID)
        {
            MySqlDataReader reader;
            int visitID = getMaxVisitID(memberID);
            DateTime time_out = new DateTime();
            time_out = DateTime.MinValue;
            connectToMySql();
            String command = "SELECT time_in, time_out, value FROM visits WHERE visit_id = " + visitID;
            MySqlCommand c = new MySqlCommand(command, conn);
            reader = c.ExecuteReader();
            try
            {
                if (reader.Read())
                {
                    time_out = reader.GetDateTime(1);
                }
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
            finally
            {
                reader.Close();
                disconnectMySql();
            }
            return time_out;

        }

        /*
        * Author: Joshua Montgomery
        * Method Name: getLatestVisitValue
        * Parameters: String 
        * Output: int
        * Exception: Exception
        * Description:  gets the minutes required  to earn 1 visit
        */
        public int getLatestVisitValue(String memberID)
        {
            MySqlDataReader reader;
            int visitID = getMaxVisitID(memberID);

            connectToMySql();
            String command = "SELECT time_in, time_out, value FROM visits WHERE visit_id = " + visitID;
            MySqlCommand c = new MySqlCommand(command, conn);
            reader = c.ExecuteReader();
            int value = 0;
            try
            {
                if (reader.Read())
                {
                    value = reader.GetInt32(2);
                }
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {
                reader.Close();
                disconnectMySql();
            }
            return value;

        }

        /*
       * Author: Joshua Montgomery
       * Method Name: getTodaysVisitCount
       * Parameters: String 
       * Output: int
       * Exception: Exception
       * Description:  gets the total of earned visits for the current day
       */
        public int getTodaysVisitCount(String memberID)
        {
            MySqlDataReader reader;

            connectToMySql();
            String command = "SELECT value FROM visits WHERE member_id = '" + memberID + "' AND time_in > curdate();";
            MySqlCommand c = new MySqlCommand(command, conn);
            reader = c.ExecuteReader();
            int value = 0;
            try
            {
                while (reader.Read())
                {
                    value += reader.GetInt32(0);
                }
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {
                reader.Close();
                disconnectMySql();
            }
            return value;
        }

        /*
        * Author: Joshua Montgomery
        * Method Name: getMaxVisitsPerDay
        * Parameters: none
        * Output: int
        * Exception: Exception
        * Description:  gets the maximum visits allowed per day
        */
        public int getMaxVisitsPerDay()
        {
            int maxVisits = 0;
            String command = "select settings_value from settings where settings_name ="
            + " 'max_visits_per_day';";
            connectToMySql();

            MySqlCommand comm = new MySqlCommand(command, conn);
            MySqlDataReader reader = comm.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    maxVisits = reader.GetUInt16(0);
                }
            }
            catch (MySqlException)
            {
                maxVisits = 2; // never goes here, but just in case
            }
            disconnectMySql();
            return maxVisits;

        }

        /*
        * Author: Joshua Montgomery
        * Method Name: getRequiredMinutesPerVisit
        * Parameters: none
        * Output: int
        * Exception: Exception
        * Description:  gets the minutes required  to earn 1 visit
        */
        public int getRequiredMinutesPerVisit()
        {
            int requiredMinutes = 0;
            String command = "select settings_value from settings where settings_name ="
            + " 'min_time_for_visit';";
            connectToMySql();

            MySqlCommand comm = new MySqlCommand(command, conn);
            MySqlDataReader reader = comm.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    requiredMinutes = reader.GetUInt16(0);
                }
            }
            catch (MySqlException)
            {
                requiredMinutes = 30;//never goes here, but just in case
            }
            disconnectMySql();
            return requiredMinutes;
        }


        //author josh montgomery
        // this is a replacement method that addresses the problems with the old purge method
        // this purge will delete ALL visits and activities for archived members whos
        // expiration date is before the purgeDate. the order is important to preserve. 
        public bool purge(DateTime time)
        {
            String purgeTime = time.Year + "-" + time.Month + "-" + time.Day + " 23:59:59";
            bool pass = false;
            try
            {
                connectToMySql();
                String command = "delete from visits using visits, members where "
                + " (visits.member_id = members.member_id) and expire_date < '" + purgeTime +
                "' and members.is_archived =1";
                MySqlCommand comm = new MySqlCommand(command, conn);
                comm.ExecuteNonQuery();

                command = "delete from member_activities using member_activities, members"
                + " where (member_activities.member_id = members.member_id) and expire_date < '"
                + purgeTime + "' and members.is_archived =1";
                comm = new MySqlCommand(command, conn);
                comm.ExecuteNonQuery();

                command = "delete from members where expire_date < '" + purgeTime + "' and is_archived =1";
                comm = new MySqlCommand(command, conn);
                comm.ExecuteNonQuery();

                pass = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                disconnectMySql();
            }
            return pass;
        }

        
        //an alternate purge for one member. use the purge() method do do multible.
        // this is the leftovers from the brute force method of purging that was quaranteed to work
        //but proved to be way to inefficient. so DONT use this for more than one member please!!!
        // author Josh montgomery
        public void purgeMember(String memberID)
        {
            connectToMySql();
            String command = "delete from visits where member_id = '" + memberID + "';";
            MySqlCommand comm = new MySqlCommand(command, conn);
            comm.ExecuteNonQuery();
            command = "delete from member_activities where member_id = '" + memberID + "';";
            comm = new MySqlCommand(command, conn);
            comm.ExecuteNonQuery();
            command = "delete from members where member_id = '" + memberID + "';";
            comm = new MySqlCommand(command, conn);
            comm.ExecuteNonQuery();
            disconnectMySql();

        }

        public int getPurgeCriteriaCount(DateTime date)
        {
            int count = 0;
            String purgeDate = date.Year + "-" + date.Month + "-" + date.Day + " 23:59:59";
            connectToMySql();
            String command = "select count(member_id) from members where " +
                "expire_date < '"+ purgeDate +"' and is_archived = 1;";
            MySqlCommand comm = new MySqlCommand(command, conn);
            MySqlDataReader reader = comm.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("problem with purge count , "+  e.Message);
            }
            finally
            {
                reader.Close();
                disconnectMySql();
            }
            return count;
        }
        #endregion
        //*****END Josh's REGION**********************************************************
    
    }//endclass
}//endnamespace

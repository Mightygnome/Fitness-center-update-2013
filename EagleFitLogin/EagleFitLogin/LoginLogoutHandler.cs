using System;
using System.Collections;
using System.Text;
using EagleFitLogin;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;

class LoginLogoutHandler
{
    private DBHandler data = new DBHandler();
    private DbList dbData = null;

    private String memberFirstName;
    private String memberLastName;
    private String memberCategory;
    private int memberVisits;// members total visits
    private int valueEarnedThisVisit;// visits earned from this visit
    private DateTime memberExpiration = DateTime.MinValue;

    private DateTime visitLoginDate = DateTime.MinValue;
    private DateTime visitLogoutDate = DateTime.MinValue;
    private int visitVisitValue;// the visits from the latest visit
    private String aditionalActivities;
    private String zeroVisitsEntries;

    private int totalWorkoutLength;


    public int thisVisitsValue
    {
        get
        {
            return valueEarnedThisVisit;
        }
    }

    public String memberName
    {
        get
        {
            return memberFirstName + " " + memberLastName;
        }
    }

    public DateTime loginTime
    {
        get
        {
            return visitLoginDate;
        }
    }

    public DateTime logoutTime
    {
        get
        {
            return visitLogoutDate;
        }
    }

    public int visits
    {
        get
        {
            return memberVisits;
        }
    }

    public DateTime expiration
    {
        get
        {
            return memberExpiration;
        }
    }

    public int workoutLength
    {
        get
        {
            return totalWorkoutLength;
        }
    }

    public String category
    {
        get
        {
            return memberCategory;
        }
    }


    public LoginLogoutHandler(DBHandler d)
    {
        data = d;
    }

    public String activities
    {
        get
        {
            return aditionalActivities;
        }
    }

    public String zeroVisits
    {
        get
        {
            return zeroVisitsEntries;
        }
    }



    /*
    * Author: Joshua Montgomery
    * Method Name: getMemberData
    * Parameters: String
    * Output: bool
    * Exception: Exception
    * Description:  This gets the firstname , lastname, 
    * category, visit count, and expiration date
    */
    private bool getMemberData(String memberID)
    {
        bool isPass = true;
        if (!data.doesMemberExist(memberID)) return false;
        try
        {
            DataTable dt = new DataTable();
            dt = data.getMemberInfo(memberID);
            DataTableReader reader = new DataTableReader(dt);
            while (reader.Read())
            {
                memberFirstName = reader.GetString(0);
                memberLastName = reader.GetString(1);
                memberCategory = reader.GetString(2);
                memberVisits = reader.GetInt32(3);
                memberExpiration = reader.GetDateTime(4);
            }
            reader.Close();
        }
        catch (Exception ex)
        {
            isPass = false;
        }
        return isPass;
    }

    /*
    * Author: Joshua Montgomery
    * Method Name: getVisitData
    * Parameters: String
    * Output: bool
    * Exception: Exception
    * Description:  gets the login date, log out date, and visit value
    */
    private bool getVisitData(String memberID)
    {
        bool IsPass = true;
        try
        {
            visitLoginDate = data.getLatestTimeIn(memberID);
            visitVisitValue = data.getLatestVisitValue(memberID);
            visitLogoutDate = data.getLatestTimeOut(memberID);
        }
        catch (Exception ex)
        {
            IsPass = false;
        }
        return IsPass;
    }

    /*
    * Author: Joshua Montgomery
    * Method Name: getActivities
    * Parameters: String
    * Output: bool
    * Exception: Exception
    * Description:  gets descriptions, dates assigned, and values
    */
    private bool getActivities(String memberID)
    {
        bool isPass = true;
        DataTable dt = new DataTable();
        dt = data.getMemberActivities(memberID);
        DataTableReader reader = new DataTableReader(dt);
        try
        {
            while (reader.Read())
            {
                aditionalActivities += reader.GetString(0) + " - ";
                aditionalActivities += reader.GetDateTime(1).ToShortDateString() + " - ";
                aditionalActivities += reader.GetInt16(2) + "\r\n";
            }
        }
        catch (Exception ex)
        {
            isPass = false;
        }
        return isPass;
    }

    /*
    * Author: Joshua Montgomery
    * Method Name: getZeroVisits
    * Parameters: String
    * Output: bool
    * Exception: Exception
    * Description:  gets the login and logout date to compile a list of 
     * missed logouts. i wrote this method to force only the rows that i want
    */
    public bool getZeroVisits(String memberID)
    {
        bool ispass = true;
        DateTime start = new DateTime();
        DateTime end = new DateTime();
        DataTable dt = new DataTable();

        dt = data.getZeroVisits(memberID);
        DataTableReader reader = dt.CreateDataReader();

        try
        {
            while (reader.Read())
            {
                start = reader.GetDateTime(0);
                end = reader.GetDateTime(1);
                zeroVisitsEntries += start.ToLongDateString() + " - " +
                    start.ToLongTimeString() + " \r\n";

            }
            reader.Close();
        }
        catch (Exception ex)
        {
            ispass = false;
        }
        return ispass;
    }


    /*
   * Author: Joshua Montgomery
   * Method Name: isCredit
   * Parameters: none
   * Output: bool
   * Exception: none
   * Description:  deturmines whether a member is credit or not
   */
    public bool isCredit()
    {
        if (memberCategory.ToLower() == "credit")
        {
            return true;
        }
        return false;
    }


    /*
   * Author: Joshua Montgomery
   * Method Name: isLoggedIn
   * Parameters: String
   * Output: bool
   * Exception: none
   * Description:  deturmines whether a member is logged in or not and closes out all failed logouts
     * and gives the member their earned visits. 1 for non-credit, and zero for credit.
   */
    public bool isLoggedIn(String memberID)
    {
        if (visitLoginDate != DateTime.MinValue && (visitLoginDate.Day == DateTime.Now.Day) && visitLogoutDate == DateTime.MinValue)
        {
            return true;
        }
        return false;
    }

    /*
   * Author: Joshua Montgomery
   * Method Name: getDataReady
   * Parameters: String
   * Output: bool
   * Exception: none
   * Description:  loads all the data needed to run the logging process
   */
    public bool getDataReady(String memberID)
    {
        resetData();
        if (getMemberData(memberID))
        {
            getVisitData(memberID);
            getActivities(memberID);
            getZeroVisits(memberID);
            this.totalWorkoutLength = getWorkoutDuration();
            valueEarnedThisVisit = getVisitValue(memberID);
            return true;
        }
        return false;
    }

    /*
   * Author: Joshua Montgomery
   * Method Name: logoutMember
   * Parameters: String, int
   * Output: void
   * Exception: none
   * Description:  logs out member
   */
    public void logoutMember(String memberID, int visitValue)
    {
        data.logoutMember(memberID, visitValue);
    }

    /*
  * Author: Joshua Montgomery
  * Method Name: logoutNonCreditMember
  * Parameters: String
  * Output: void
  * Exception: none
  * Description:  logs out member
  */
    public void logoutNonCreditMember(String memberID)
    {
        data.nonCreditLogout(memberID);
    }

    /*
  * Author: Joshua Montgomery
  * Method Name: loginMember
  * Parameters: String, int
  * Output: void
  * Exception: none
  * Description:  logs in member
  */
    public void loginMember(String memberID)
    {
        data.loginMember(memberID);
    }


    /*
* Author: Joshua Montgomery
* Method Name: loginMember
* Parameters: String, int
* Output: void
* Exception: none
* Description:  logs in member
*/
    public void loginNonCreditMember(String memberID)
    {
        data.nonCreditLogin(memberID);
    }


    /*
    * Author: Joshua Montgomery
    * Method Name: getWorkoutDuration
    * Parameters: none
    * Output: int
    * Exception: none
    * Description:  gets the length of the workout in minutes
    */
    public int getWorkoutDuration()
    {
        int minutes = ((TimeSpan)(DateTime.Now - loginTime)).Minutes;
        int hours = ((TimeSpan)(DateTime.Now - loginTime)).Hours;
        for (int x = 0; x < hours; x++)
        {
            minutes += 60;
        }
        return minutes;
    }

    /*
   * Author: Joshua Montgomery
   * Method Name: getVisitValue
   * Parameters: String
   * Output: int
   * Exception: none
   * Description:  calculates the earned visit value for the workout
   */
    public int getVisitValue(String memberID)
    {
        if (!isCredit()) return 1;
        int todaysEarnedVisits = data.getTodaysVisitCount(memberID);

        int earnedVisits = 0;
        int maxVisits = data.getMaxVisitsPerDay();

        if (todaysEarnedVisits >= maxVisits) return 0;
        maxVisits -= todaysEarnedVisits;

        int visit = data.getRequiredMinutesPerVisit();

        earnedVisits = totalWorkoutLength / visit;

        if (earnedVisits > maxVisits)
        {
            return maxVisits;
        }
        return earnedVisits;

    }

    /*
    * Author: Joshua Montgomery
    * Method Name: isWarnToContinue
    * Parameters: DateTime, DateTime
    * Output: int
    * Exception: none
    * Description:  Deturmines if a members workout will not score a visit,
     * or if their time is close to earning the next visit
    */
    public int isWarnToContinue()// used to prompt user if they want to continue
    {
        int maxVisits = data.getMaxVisitsPerDay();
        int visit = data.getRequiredMinutesPerVisit();

        if ((totalWorkoutLength < visit))
        {
            return 0;
        }
        else
        {
            for (int x = 1; x <= maxVisits; x++)
            {
                if (totalWorkoutLength < (visit * x) && totalWorkoutLength >= ((visit * x) - 10))
                {
                    return 1;
                }
            }
        }

        return 2;
    }

    /*
   * Author: Joshua Montgomery
   * Method Name: performArchive
   * Parameters: none
   * Output: void
   * Exception: Exception
   * Description:  will archive all expired members
   */
    public void performArchive()
    {
        DateTime expireDate = DateTime.MinValue;
        String memberID = "";
        DataTable id = new DataTable();

        id = data.getAllCurMemberId();
        DataTableReader reader = id.CreateDataReader();
        try
        {
            while (reader.Read())
            {
                memberID = reader.GetString(0);
                expireDate = reader.GetDateTime(6);
                if (expireDate < DateTime.Today)
                {
                    data.archiveMember(memberID);
                }
            }
            reader.Close();
        }
        catch (Exception ex)
        {
        }
    }

    /*
    * Author: Joshua Montgomery
    * Method Name: resetData
    * Parameters: none
    * Output: void
    * Exception: none
    * Description:  resets all data
    */
    public void resetData()
    {
        memberFirstName = "";
        memberLastName = "";
        memberCategory = "";
        memberVisits = 0;
        memberExpiration = DateTime.MinValue;
        visitLoginDate = DateTime.MinValue;
        visitLogoutDate = DateTime.MinValue;
        visitVisitValue = 0;
        totalWorkoutLength = 0;
        aditionalActivities = "";
        zeroVisitsEntries = "";
        valueEarnedThisVisit = 0;
    }

}

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

namespace EagleFit_Management
{
    /// <summary>
    /// Interaction logic for ClassInformation.xaml
    /// </summary>
    public partial class ClassInformation : Window
    {
        private DBHandler db;

        public ClassInformation(DBHandler db)
        {
            InitializeComponent();

            this.db = db;
            

            comboBx_quarter.Items.Add("Fall");
            comboBx_quarter.Items.Add("Winter");
            comboBx_quarter.Items.Add("Spring");
            comboBx_quarter.Items.Add("Summer");

            //add time information into combo boxes
            comboBx_startTime.Items.Add("N/A");
            comboBx_startTime.Items.Add("07:00:00");
            comboBx_startTime.Items.Add("08:00:00");
            comboBx_startTime.Items.Add("09:00:00");
            comboBx_startTime.Items.Add("10:00:00");
            comboBx_startTime.Items.Add("11:00:00");
            comboBx_startTime.Items.Add("12:00:00");
            comboBx_startTime.Items.Add("01:00:00");
            comboBx_startTime.Items.Add("02:00:00");
            comboBx_startTime.Items.Add("03:00:00");
            comboBx_startTime.Items.Add("04:00:00");
            comboBx_startTime.Items.Add("05:00:00");
            comboBx_startTime.Items.Add("05:15:00");
            comboBx_startTime.Items.Add("06:00:00");
            comboBx_startTime.Items.Add("06:30:00");

            comboBx_endTime.Items.Add("N/A");
            comboBx_endTime.Items.Add("07:50:00");
            comboBx_endTime.Items.Add("08:50:00");
            comboBx_endTime.Items.Add("09:50:00");
            comboBx_endTime.Items.Add("10:50:00");
            comboBx_endTime.Items.Add("11:50:00");
            comboBx_endTime.Items.Add("12:50:00");
            comboBx_endTime.Items.Add("01:50:00");
            comboBx_endTime.Items.Add("02:50:00");
            comboBx_endTime.Items.Add("03:50:00");
            comboBx_endTime.Items.Add("04:50:00");
            comboBx_endTime.Items.Add("05:50:00");
            comboBx_endTime.Items.Add("06:50:00");
            comboBx_endTime.Items.Add("07:30:00");

        }

        #region Menu Options
        //-------------------------------------- Menu Options --------------------------------------------

        //File -> Close
        private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        {
            //db.disconnectMySql();
            Close();
        }

        //Help
        private void MenuItem_Click_Help(object sender, RoutedEventArgs e)
        {
            String h1 = "Course Number: 3 digit course number\n";
            h1 += "Course Name: The name of the course (Example: Fast Fitness)\n";
            h1 += "Section: the section number of the course (1 digit in lenght)\n";
            h1 += "Quarter End Date: the last day of the quarter that a student can get credit for a class.\n\n";
            h1 += "Group Exercise courses require a start and end time. This information is used by EagleFit Login";
            h1 += " to prevent students from logging in more than 10 mins prior to their class.\n";

            MessageBox.Show(h1);
        }


        #endregion


        #region Form Code
        //-------------------------------------------------------------------------------------------------

        /*Tied to: btn_submitAddCourse
         */
        private void btn_submitAddCourse_Click(object sender, RoutedEventArgs e)
        {
            //db.connectToMySql();
            lbl_addStatus.Content = "";
            
            try
            {
                int courseID = Int32.Parse(txtBx_courseID.Text);
                String className = txtBx_courseName.Text;
                int section = Int32.Parse(txtBx_secNum.Text);
                String quarterEndDate = datePkr_quarterEndDate.SelectedDate.ToString(); //formats string as mm/dd/yyyy 12:00:00         
                int credits = Int32.Parse(txtBx_credits.Text);

                String quarter = (String)comboBx_quarter.SelectedItem;
                int year = Int32.Parse(txtBx_year.Text);

                String startTime = (String)comboBx_startTime.SelectedItem;
                if (startTime.Equals("N/A"))
                    startTime = "00:00:00";
                String endTime = (String)comboBx_endTime.SelectedItem;
                if (endTime.Equals("N/A"))
                    endTime = "00:00:00";

                //send string from DatePicker box to parser to reformat into DB DATE format
                parseForDB(quarterEndDate);

                lbl_addStatus.Content = "Course successfully added.";

                //clear fields
                txtBx_courseID.Text = "";
                txtBx_courseName.Text = "";
                txtBx_secNum.Text = "";
                txtBx_credits.Text = "";
                txtBx_year.Text = "";


                /*
                //pass information to the DBHandler class
                bool add = db.addClassToDatabase(courseID, className, section, quarterEndDate, credits, startTime, endTime, year, quarter);

                if (add)
                {
                    lbl_addStatus.Content = "Course successfully added.";
                    //clear fields
                    txtBx_courseID.Text = "";
                    txtBx_courseName.Text = "";
                    txtBx_secNum.Text = "";
                    txtBx_credits.Text = "";
                    txtBx_year.Text = "";
                }
                else
                {
                    MessageBox.Show("Add false");
                }
                 */
            }
            catch (Exception x)
            {
                String m = x.Message;
                lbl_addStatus.Content = "Error adding course. Check to make sure all fields are correct.";
                //MessageBox.Show(m);
            }
                        

        }//end method btn_submitAddCourse_Click



        /*DatePicker returns a string in the following formats:
         m/d/yyyy 00:00:00 AM
         m/dd/yyyy 00:00:00 AM
         mm/d/yyyy 00:00:00 AM
         mm/dd/yyyy 00:00:00 AM
         The Date field in the database requires the format yyyy-mm-dd
         This method looks at the string returned from the datepicker and converts it to the correct format
         */
        private String parseForDB(String date)
        {
            String qed = "";
            char[] c = date.ToCharArray();
            if (c[1].Equals('/') && c[3].Equals('/'))//for m/d/yyyy 00:00:00 AM
            {
                qed = "" + c[4] + c[5] + c[6] + c[7] + "-0" + c[0]  + "-0" + c[2];                                               
            }
            else if (c[1].Equals('/') && c[4].Equals('/'))// m/dd/yyyy 00:00:00 AM
            {
                qed = "" + c[5] + c[6] + c[7] + c[8] + "-0" + c[0] + "-" + c[2] + c[3];
            }
            else if (c[2].Equals('/') && c[4].Equals('/'))//mm/d/yyyy 00:00:00 AM
            {
                qed = "" + c[5] + c[6] + c[7] + c[8] + "-" + c[0] + c[1] + "-0" + c[3];
            }
            else if (c[2].Equals('/') && c[5].Equals('/'))//mm/dd/yyyy 00:00:00 AM
            {
                qed = "" + c[6] + c[7] + c[8] + c[9] + "-" + c[0] + c[1] + "-" + c[3] + c[4];
            }
            //MessageBox.Show(qed);
            return qed;
            
        }

        #endregion




    }//end class
}

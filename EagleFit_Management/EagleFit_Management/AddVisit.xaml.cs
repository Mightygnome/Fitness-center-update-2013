/*
 EagleFit_Management: AddVisit
  
 Fall 2013 - Winter 2014
 
 Description: This class is used to enter/correct visit information for a student enrolled in a for credit class
              offered by the EWU Recreation Center. 
              The user enters a student ID into the text box then clicks the query button to pull activity information
              pertaining to that student. The user can then add/edit activity information and then save it to the
              database.
 
 Linked .cs/xaml Files: AddVisit.xaml, AddVisit.xaml.cs
 
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

namespace EagleFit_Management
{
    /// <summary>
    /// Interaction logic for AddActivityOrVisit.xaml
    /// </summary>
    public partial class AddActivityOrVisit : Window
    {

        private int studentID;

        //constructor
        public AddActivityOrVisit()
        {
            InitializeComponent();
            lbl_idNotFound.IsEnabled = false;
            btn_addNewVisit.IsEnabled = false;
            txtBx_visitNum.IsEnabled = false;
            btn_editVisit.IsEnabled = false;
            btn_saveVisit.IsEnabled = false;
            datePkr_visitDate.IsEnabled = false;
            comboBx_logInTime.IsEnabled = false;
            comboBx_logOutTime.IsEnabled = false;
            txtBx_value.IsEnabled = false;
        }


        //----------------------------------- Menu Items --------------------------------------------

        //File -> Exit
        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //Help
        private void MenuItem_Click_Help(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("User Manual Info goes here");
        }

        



        //----------------------------------------- Form Elements ----------------------------------
        
        /*Tied to btn_query
          This method takes the user entered Student ID parses it into an it then passes the Student ID into
          a database query to pull activities for that student.
         */
        private void btn_Query_Click(object sender, RoutedEventArgs e)
        {
            lbl_idNotFound.IsEnabled = false;
            lbl_idNotFound.Content = "";

            //grab ID string from text-box
            String tempID = txtBx_studentID.Text;
            //check to make sure the string is not empty and that it is 8 characters in length
            if (!tempID.Equals("") && tempID.Length == 8)
            {
                try
                {
                    //parse the Student ID into an int                             
                    studentID = Int32.Parse(tempID);

                    //enable elements
                    enableEditingButtons();
                }
                catch (Exception d)
                {
                    String m = d.Message;//used to get rid of warnings
                    lbl_idNotFound.IsEnabled = true;
                    lbl_idNotFound.Content = "* ID Not Valid";
                }
            }
            else //invalid entry
            {
                lbl_idNotFound.IsEnabled = true;
                lbl_idNotFound.Content = "* ID Not Valid";
            }
        }//end btn_Query_Click




        /*This method enables the form elements that are initially disabled. It is called only when a valid student 
          ID number has been entered and the query button has been clicked.
         */
        private void enableEditingButtons()
        {
            btn_addNewVisit.IsEnabled = true;
            txtBx_visitNum.IsEnabled = true;
            btn_editVisit.IsEnabled = true;            
        }




        /*Tied to: btn_saveVisit
         
         */
        private void btn_saveVisit_Click(object sender, RoutedEventArgs e)
        {
            //disable elements
            datePkr_visitDate.IsEnabled = false;
            comboBx_logInTime.IsEnabled = false;
            comboBx_logOutTime.IsEnabled = false;
            txtBx_value.IsEnabled = false;
            btn_saveVisit.IsEnabled = false;
        }




        /*Tied to: btn_addNewVisit
         
         */
        private void btn_addNewVisit_Click(object sender, RoutedEventArgs e)
        {
            //enable editing elements
            datePkr_visitDate.IsEnabled = true;
            comboBx_logInTime.IsEnabled = true;
            comboBx_logOutTime.IsEnabled = true;
            txtBx_value.IsEnabled = true;
            btn_saveVisit.IsEnabled = true;

        }




        /*Tied to: btn_editVisit_Click
         
         */
        private void btn_editVisit_Click(object sender, RoutedEventArgs e)
        {
            //enable editing elements
            datePkr_visitDate.IsEnabled = true;
            comboBx_logInTime.IsEnabled = true;
            comboBx_logOutTime.IsEnabled = true;
            txtBx_value.IsEnabled = true;
            btn_saveVisit.IsEnabled = true;
        }



        
        /*DatePicker returns a string in the following formats:
         m/d/yyyy 00:00:00 AM
         m/dd/yyyy 00:00:00 AM
         mm/d/yyyy 00:00:00 AM
         mm/dd/yyyy 00:00:00 AM
         The Date field in the database requires the format yyyy-mm-dd
         This method looks at the string returned from the datepicker and converts it to the correct format
         */
        /*
        private String parseForDB(String date)
        {
            String qed = "";
            char[] c = date.ToCharArray();
            if (c[1].Equals('/') && c[3].Equals('/'))//for m/d/yyyy 00:00:00 AM
            {
                qed = "" + c[4] + c[5] + c[6] + c[7] + "-0" + c[0] + "-0" + c[2];
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
        */


    }//end class

}//end namespace

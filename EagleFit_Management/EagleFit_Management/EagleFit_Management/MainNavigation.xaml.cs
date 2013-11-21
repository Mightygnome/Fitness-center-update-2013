/*
 EagleFit_Management: Main Navigation
  
 Fall 2013 - Winter 2014
 
 Description: This class houses all of the form controls for the main Navigation of the program
 
 Linked .cs/xaml Files: MainNavigation.xaml, 
 
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
    /// Interaction logic for MainNavigation.xaml
    /// </summary>
    public partial class MainNavigation : Window
    {
        public DBHandler db;

        public MainNavigation()
        {
            InitializeComponent();
            db = new DBHandler();
        }

        

        //-------------------------Menu Options-----------------------------------------------

        //File
        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            db.disconnectMySql();
            Close();
            
        }


        //Database
        private void MenuItem_Click_DatabaseSettings(object sender, RoutedEventArgs e)
        {
            //link to database connection window goes here
        }



        //About
        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            String s = "EagleFit Management\n";
            s += "Developed Fall 2013 - Winter 2014 by EWU CS Dept Students.\n\n";
            s += "This software tracks student time/usage of the Fitness Center\n and attendance in Group Excerise classes";
            MessageBox.Show(s);

        }

        //Help
        private void MenuItem_Click_Help(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("EagleFit Management");
        }




        //-----------------------------------Form Elements-------------------------------------


        /*Tied to btn_addMultipleUsers
         This button will bring up the addMultipleUsers form
         */
        private void btn_addMultipleUsers_Click(object sender, RoutedEventArgs e)
        {
            AddMultipleUsersAtOnce nForm = new AddMultipleUsersAtOnce();
            nForm.Show();
        }


        /* Tied to btn_addActivity
         This button will bring up the AddActivityOrVisit form
         */
        private void Button_Click_AddVisit(object sender, RoutedEventArgs e)
        {
            AddActivityOrVisit aav = new AddActivityOrVisit();
            aav.Show();
        }


        /* Tied to: btn_reports
           This method will bring up the Reports form
         */
        private void btn_reports_Click(object sender, RoutedEventArgs e)
        {
            Reports r1 = new Reports();
            r1.Show();
        }


        /*Tied to: btn_userManager
          This method will bring up the UserManager form to add/delete user information & change passwords          
         */
        private void btn_userManager_Click(object sender, RoutedEventArgs e)
        {
            UserManager um = new UserManager(db);
            um.Show();
        }



        /*Tied to: btn_AddClassInfo
         This method brings up the ClassInformation Form
         */
        private void btn_AddClassInfo_Click(object sender, RoutedEventArgs e)
        {
            ClassInformation ci = new ClassInformation();
            ci.Show();
        }

        

        


    }//end partial Class
}

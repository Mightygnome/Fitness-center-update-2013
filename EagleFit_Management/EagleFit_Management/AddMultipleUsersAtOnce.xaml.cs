﻿/*
 EagleFit_Management: AddMultipleUsersAtOnce
  
 Fall 2013 - Winter 2014
 
 Description: This class controls all of the fields pertaining to adding multiple users to the database.
 
 Linked .cs/xaml Files: AddMultipleUsersAtOnce.xaml, AddMultipleUsersAtOnce.xaml.cs, 
 
 Created By: Sarah Henderson
 * 
 background: #FF7CDEB1
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
    /// Interaction logic for AddMultipleUsersAtOnce.xaml
    /// </summary>
    public partial class AddMultipleUsersAtOnce : Window
    {
        //class variables
        string fName1, fName2, fName3, fName4, fName5, fName6, fName7, fName8;
        string lName1, lName2, lName3, lName4, lName5, lName6, lName7, lName8;
        //int memNum1, memNum2, memNum3, memNum4, memNum5, memNum6, memNum7, memNum8;
        int totalAdded = 0;
        
        //Initialize Form
        public AddMultipleUsersAtOnce()
        {
            InitializeComponent();
            lbl_totalAdded.IsEnabled = false;
            
        }

        //------------------------------------ Menu Items --------------------------------------------

        //Menu Items
        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }



        // ---------------------------------- Class Methods -------------------------------------------



        /*
         This method grabs the text entered into each of the text boxes and then passes the data to another method
         that will add the data to the database.
         If the addToDatabase method returns true, the textBoxes are then emptied. If the addToDatabase method returns 
         false, the data should be left in the textBoxes to be corrected.
         */
        private void btn_Add_All_Click(object sender, RoutedEventArgs e)
        {
            totalAdded = 0; //re-initialize the count

            //strings to hold the member ID before it is cast to an int
            string temp1, temp2, temp3, temp4, temp5, temp6, temp7, temp8;
            //booleans to indicate whether or not the information can be successfully added to the database
            bool one = false, two = false, three = false, four = false, five = false, six = false, seven = false, eight = false;

            fName1 = txtBx_firstN1.Text;
            lName1 = txtBx_lastN1.Text;
            temp1 = txtBx_memNum1.Text;
            one = addToDataBase(fName1, lName1, temp1);
            
            fName2 = txtBx_firstN2.Text;
            lName2 = txtBx_lastN2.Text;
            temp2 = txtBx_memNum2.Text;
            two = addToDataBase(fName2, lName2, temp2);
            
            fName3 = txtBx_firstN3.Text;
            lName3 = txtBx_lastN3.Text;
            temp3 = txtBx_memNum3.Text;
            three = addToDataBase(fName3, lName3, temp3);
            
            fName4 = txtBx_firstN4.Text;
            lName4 = txtBx_lastN4.Text;
            temp4 = txtBx_memNum4.Text;
            four = addToDataBase(fName4, lName4, temp4);
            
            fName5 = txtBx_firstN5.Text;
            lName5 = txtBx_lastN5.Text;
            temp5 = txtBx_memNum5.Text;
            five = addToDataBase(fName5, lName5, temp5);
            
            fName6 = txtBx_firstN6.Text;
            lName6 = txtBx_lastN6.Text;
            temp6 = txtBx_memNum6.Text;
            six = addToDataBase(fName6, lName6, temp6);
            
            fName7 = txtBx_firstN7.Text;
            lName7 = txtBx_lastN7.Text;
            temp7 = txtBx_memNum7.Text;
            seven = addToDataBase(fName7, lName7, temp7);
            
            fName8 = txtBx_firstN8.Text;           
            lName8 = txtBx_lastN8.Text;
            temp8 = txtBx_memNum8.Text;
            eight = addToDataBase(fName8, lName8, temp8);

            resetFormFields(one, two, three, four, five, six, seven, eight);
            lbl_totalAdded.IsEnabled = true;
            lbl_totalAdded.Content = "" + totalAdded + " Records successfully added.";
        }//end btn_ADD_All_Click





        /*This method checks to make sure that the text boxes have data in them. 
         If the strings are empty then it does nothing with them. 
         If the strings have data in them, then it passes the data into the database.
         */
        private bool addToDataBase(string fName, string lName, string temp)
        {
            int memNum;
            //check to make sure that the strings are not empty and that the ID string is at least 8 character in length
            if (!fName.Equals("") && !lName.Equals("") && !temp.Equals("") && temp.Length == 8)
            {
                try
                {
                    //parse the Student ID into an int                             
                     memNum = Int32.Parse(temp);
                }
                catch (Exception e)
                {
                    String m = e.Message;
                    return false;
                }
                //pass to database

                totalAdded++;
                return true;
            }//end if
            else
            {
                return false;
            }

        }//end addToDataBase



        /* This method reset the fields on the form that have already been passed into the database.
           If there is an error in one of the fields the data should remain in the fields for the user to correct.
         */
        public void resetFormFields(bool one, bool two, bool three, bool four, bool five, bool six, bool seven, bool eight)
        {
            if (one == true)
            {
                txtBx_firstN1.Text = "";
                txtBx_lastN1.Text = "";
                txtBx_memNum1.Text = "";
            }
            if (two == true)
            {
                txtBx_firstN2.Text = "";
                txtBx_lastN2.Text = "";
                txtBx_memNum2.Text = "";
            }
            if (three == true)
            {
                txtBx_firstN3.Text = "";
                txtBx_lastN3.Text = "";
                txtBx_memNum3.Text = "";
            }
            if (four == true)
            {
                txtBx_firstN4.Text = "";
                txtBx_lastN4.Text = "";
                txtBx_memNum4.Text = "";
            }
            if (five == true)
            {
                txtBx_firstN5.Text = "";
                txtBx_lastN5.Text = "";
                txtBx_memNum5.Text = "";
            }
            if (six == true)
            {
                txtBx_firstN6.Text = "";
                txtBx_lastN6.Text = "";
                txtBx_memNum6.Text = "";
            }
            if (seven == true)
            {
                txtBx_firstN7.Text = "";
                txtBx_lastN7.Text = "";
                txtBx_memNum7.Text = "";
            }
            if (eight == true)
            {
                txtBx_firstN8.Text = "";
                txtBx_lastN8.Text = "";
                txtBx_memNum8.Text = "";
            }
        }//end resetFromFields


        
        /*
         This method controls what happens when a user clicks the done button.
         This method closes the AddMultipleUsersAtOnce form and returns the user to the mainNavigation form.
         */
        private void btn_done_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        




    }//end class
}//end namespace

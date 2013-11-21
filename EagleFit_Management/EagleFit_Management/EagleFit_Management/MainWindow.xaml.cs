/*
 EagleFit_Management: Main Window
  
 Fall 2013 - Winter 2014
 
 Description: This class is for the initial Login window that comes up when a user first launches the program
 
 Linked .cs/xaml Files: MainWindow.xaml, 
 
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EagleFit_Management
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        
        //------------------------------------------ Menu Items ------------------------------------------------

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }


        /* Database -> Connection Settings
           This method bring up the Database setting windows so a user can edit the IP address 
           and database connections.
         */
        private void MenuItem_Click_DBSettings(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The settings window should come up here");
        }


        //------------------------------------------- Form Methods --------------------------------------------


        /*
            This method controls what happens when a user clicks the button to log in
         */
        private void btn_Click_Login(object sender, RoutedEventArgs e)
        {
            MainNavigation nForm = new MainNavigation();
            nForm.Show();

            Close();//close the Main Login Window
        }

        


    }//end class
}

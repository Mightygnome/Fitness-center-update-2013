/*
 EagleFit_Management: Reports
  
 Fall 2013 - Winter 2014
 
 Description: This class is used select which report type to be generated.
              Reports are then generated using Excel
 
 Linked .cs/xaml Files: Reports.xaml, 
 
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
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : Window
    {
        public Reports()
        {
            InitializeComponent();
            comboBx_reportType.Items.Add("Credit Member Summary");
            comboBx_reportType.Items.Add("Comprehensive Member Summary");
            comboBx_reportType.Items.Add("Hourly Traffic");
            comboBx_reportType.Items.Add("Daily Traffic");
        }


        //--------------------------------------- Menu Items ------------------------------------------


        //File -> Close
        private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        


        //--------------------------------------- Form Elements ----------------------------------------


        /*Tied to: btn_getReport
         */
        private void btn_getReport_Click(object sender, RoutedEventArgs e)
        {

        }

    }//end class
}//end namespace

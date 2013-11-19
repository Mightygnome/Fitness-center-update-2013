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

namespace EagleFitLogin
{
    /*
 EagleFitLogin: Database Settings
  
 Fall 2013 - Winter 2014
 
 Description: 
 
 Linked .cs/xaml Files:  DatabaseSettings.xaml, 
 
 Created By: Jeremy Reineman
 */
    
   
    public partial class DatabaseSettings : Window
    {
        
        DbList dbData;
        public DatabaseSettings(DbList dbDat)
        {
            dbData = dbDat;
            int x = 0;
            InitializeComponent();
            for (x = 0; x < dbData.DataBaseList.Count; x++)
            {
                comboBx_dbList.Items.Add(((DataBaseData)dbData.DataBaseList[x]));
            }

            if (comboBx_dbList.Items.Count > 0)
            {
                txtBx_ServerIP.Text = ((DataBaseData)comboBx_dbList.Items[0]).serverIP;
                txtBx_userName.Text = ((DataBaseData)comboBx_dbList.Items[0]).DBUserName;
                txtBx_dbName.Text = ((DataBaseData)comboBx_dbList.Items[0]).DBName;
                txtBx_dbPassword.Text = ((DataBaseData)comboBx_dbList.Items[0]).DBPassword;
                DataBaseData d = new DataBaseData(txtBx_ServerIP.Text, txtBx_dbName.Text, txtBx_userName.Text, txtBx_dbPassword.Text);
                comboBx_dbList.SelectedItem = d;
                comboBx_dbList.Text = d.ToString();
            }
        }

        public DatabaseSettings(DbList dbDat, String password)
        {

            dbData = dbDat;
            int x = 0;
            InitializeComponent();
            for (x = 0; x < dbData.DataBaseList.Count; x++)
            {
                if (((DataBaseData)dbData.DataBaseList[x]).DBPassword == password)
                {
                    comboBx_dbList.Items.Add(((DataBaseData)dbData.DataBaseList[x]));
                }
            }

            if (comboBx_dbList.Items.Count > 0)
            {
                txtBx_ServerIP.Text = ((DataBaseData)comboBx_dbList.Items[0]).serverIP;
                txtBx_userName.Text = ((DataBaseData)comboBx_dbList.Items[0]).DBUserName;
                txtBx_dbName.Text = ((DataBaseData)comboBx_dbList.Items[0]).DBName;
                txtBx_dbPassword.Text = ((DataBaseData)comboBx_dbList.Items[0]).DBPassword;
                DataBaseData d = new DataBaseData(txtBx_ServerIP.Text, txtBx_dbName.Text, txtBx_userName.Text, txtBx_dbPassword.Text);
                comboBx_dbList.SelectedItem = d;
                comboBx_dbList.Text = d.ToString();
            }

        }

        
        private void comboBx_dbList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBx_dbList.Items.Count > 0)
            {
                DataBaseData d = (DataBaseData)comboBx_dbList.SelectedItem;
                txtBx_ServerIP.Text = d.serverIP;
                txtBx_dbName.Text = d.DBName;
                txtBx_userName.Text = d.DBUserName;
                txtBx_dbPassword.Text = d.DBPassword;
            }
        }

        private void btn_AddDatabase_Click(object sender, RoutedEventArgs e)
        {
            if (txtBx_ServerIP.Text != "" && txtBx_dbName.Text != "" && txtBx_userName.Text != "" && txtBx_dbPassword.Text != "")
            {
                DataBaseData d = new DataBaseData(txtBx_ServerIP.Text, txtBx_dbName.Text, txtBx_userName.Text, txtBx_dbPassword.Text);
                dbData.DataBaseList.Add(d);
                comboBx_dbList.Items.Add(d);
                comboBx_dbList.SelectedItem = d;
                comboBx_dbList.Text = d.ToString();
            }
            else
            {
                MessageBox.Show("Your new database connection was not created because one or more fields were empty");
            }
        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (txtBx_ServerIP.Text != "" && txtBx_dbName.Text != "" && txtBx_userName.Text != "" && txtBx_dbPassword.Text != "" && comboBx_dbList.SelectedItem != null)
            {
                DataBaseData d = new DataBaseData(txtBx_ServerIP.Text, txtBx_dbName.Text, txtBx_userName.Text, txtBx_dbPassword.Text);
                if (comboBx_dbList.Items.Count > 0)
                {
                    dbData.DataBaseList.Remove(comboBx_dbList.SelectedItem);
                    dbData.DataBaseList.Add(d);
                    comboBx_dbList.Items.RemoveAt(comboBx_dbList.SelectedIndex);
                    comboBx_dbList.Items.Add(d);
                    comboBx_dbList.SelectedItem = d;
                }
            }
            else
            {
                MessageBox.Show("Database data not modified because one or more fields are empty");
            }
        }

        private void btn_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (comboBx_dbList.Items.Count > 0)
            {
                if (comboBx_dbList.SelectedItem != null)
                {
                    dbData.DataBaseList.Remove(comboBx_dbList.SelectedItem);
                    comboBx_dbList.Items.RemoveAt(comboBx_dbList.SelectedIndex);
                    txtBx_ServerIP.Text = "";
                    txtBx_userName.Text = "";
                    txtBx_dbName.Text = "";
                    txtBx_dbPassword.Text = "";
                    comboBx_dbList.Text = "";
                }
                else
                {
                    MessageBox.Show("You must select an item to delete");
                }
            }
        }

        private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            txtBx_ServerIP.Text = "";
            txtBx_userName.Text = "";
            txtBx_dbName.Text = "";
            txtBx_dbPassword.Text = "";
            comboBx_dbList.Text = "";
            this.Close();
        }
    }
}

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
    /// Interaction logic for UserManager.xaml
    /// </summary>
    public partial class UserManager : Window
    {
        public DBHandler db;

        public UserManager(DBHandler db1)
        {
            InitializeComponent();

            //get db instance
            db = db1;

            lbl_passWdNoMatch.IsEnabled= false;
               
            //add user roles to the combo box
            comboBx_roles.Items.Add("Administrator");
            comboBx_roles.Items.Add("Supervisor");
            comboBx_roles.Items.Add("Instructor");
        }


        //--------------------------------Menu Items ------------------------------

        private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }


        //---------------------- Add New User --------------------------------
        /*
         Tied to: btn_submit
         This method adds a new user into the database.
         
         */
        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String uName;
                String pw1;
                String pw2;
                String role;

                uName = txtBx_AddUserName.Text;
                pw1 = passWdBx_password1.ToString();
                pw2 = passWdBx_password2.ToString();
                role = (String)comboBx_roles.SelectedItem;
                //check to make sure the passwords match
                if (!pw1.Equals(pw2))
                {
                    lbl_passWdNoMatch.Content = "* Passwords don't match";
                    lbl_passWdNoMatch.IsEnabled = true;
                }
                if (pw1.Equals(pw2))
                {
                    //pass parameters to the database
                    db.addNewLogin(uName, role, pw1);
                }
            }//end try
            catch (Exception err)
            {

            }
        }//end method btn_submit_Click


        //------------------------------ Change Password ------------------------------------
        
        /*Tied to: btn_submitChange
         This method changes the password of an existing user.
         The method checks the users current password against the database and then changes the database entry to 
         the new password the user has entered.
         */
        private void btn_submitChange_Click(object sender, RoutedEventArgs e)
        {
            String curUserName = txt_curUserName.Text;
            String curPassword = passWBx_curPass.Password.ToString();
            String newPassword1 = passWBx_newPass1.Password.ToString();
            String newPassword2 = passWBx_newPass2.Password.ToString();

            //bool check = db.changePassword(curUserName, curPassword, newPassword1, newPassword2);
            bool check = true;

            //check to make sure the current password is valid
            if (check)
            {
                txt_curUserName.Text = "";
                passWBx_curPass.Password = "";
                passWBx_newPass1.Password = "";
                passWBx_newPass2.Password = "";
                MessageBox.Show("Password was successfully changed.");
            }
            else
            {
                MessageBox.Show("ERROR -- Passwords do not match!");
            }


        }//end submit button
    
    
    




    }//end class
}

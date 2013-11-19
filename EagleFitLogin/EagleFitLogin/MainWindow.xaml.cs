using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace EagleFitLogin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    //;000652482=9002? string read in from swiped ID card

    public partial class MainWindow : Window
    {
        private const int _lengthOfMemberID = 8;
        private const int SECOND = 1000;
        private const int MINUTE = 60000;
        private Timer clock = new Timer();
        private Timer readyTimeout = new Timer();
        private Timer memberIdTimer = new Timer();
        private Timer infoTimeout = new Timer();
        private Timer archiveTrigger = new Timer();
        
        private DBHandler data = new DBHandler();
        private CardSwipeHandler card;
        private LoginLogoutHandler logging;
        private DatabaseSwitcher switcher;

        private DispatcherTimer Timer = new DispatcherTimer();

        //*******
        bool delay = false;
        //********
        bool isTimedOut_Ready = false;
        bool isTimedOut_Info = false;
        bool isFailure = false;
        bool isLogout = false;
        bool isCredit = false;
        bool isCancel = true;
        bool blink = false;
        bool isArchiving = true;
        string memberID = "";
        private string readyToSwipe = "Ready To Swipe Card";
        private string idleMessege = "System Is Idle";
        private string memberName;
        private DbList dbData;
        

        public MainWindow()
        {
            logging = new LoginLogoutHandler(data);
            switcher = new DatabaseSwitcher(data, ".myMemberSerial");
            memberName = idleMessege;
            double width = SystemParameters.PrimaryScreenWidth;
            double height = SystemParameters.PrimaryScreenHeight;
            
            if (height <= 600 || width <= 800)
            {
                //InitializeComponent2();// small
            }
            else
            {
                InitializeComponent();//large
            }

            card = new CardSwipeHandler();
            txtBx_MemberID.KeyDown += new KeyEventHandler(txtBx_MemberID_KeyDown);

            Timer.Tick += new EventHandler(Timer_Click);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();

            clearInfo();
            txtBx_MemberID.Focus();

            dbData = switcher.openDatabaseData();
            switcher.openDatabase(1, "localhost", "blah");

            StartArchiving();
        }


        private void StartArchiving()
        {
            if (switcher.isDatabaseConnected())
            {
                //MessageBox.Show("were in");
                performArchiving();
            }
            else
            {
                //MessageBox.Show("were not in");
            }
        }


        private void Timer_Click(object sender, EventArgs e)
        {
            DateTime d;
            d = DateTime.Now;
            txtBlk_Clock.Text = d.ToString("hh:mm:ss tt");
        }



        private void txtBx_MemberID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string inputMemberID = card.HandleInput(txtBx_MemberID.Text);
                if (inputMemberID != string.Empty)
                {
                    txtBx_MemberID.Text = inputMemberID.ToString();
                    //PerformLogging(inputMemberID);
                }
                else
                {
                    MessageBox.Show("Invalid ID");
                }
                txtBx_MemberID.MaxLength = _lengthOfMemberID;
            }
            else if (e.Key == Key.Oem1)
            {
                txtBx_MemberID.Text = "";
                txtBx_MemberID.MaxLength = _lengthOfMemberID + 2;
            }
            else
            {
                if (e.Key < Key.D0 || e.Key > Key.D9)
                {
                    if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                    {
                        int isNumber = 0;
                        e.Handled = !int.TryParse(e.Key.ToString(), out isNumber);
                    }
                }
            }
        }



        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            string inputMemberID = card.HandleInput(txtBx_MemberID.Text);
            if (inputMemberID != string.Empty)
            {
                txtBx_MemberID.Text = inputMemberID.ToString();
                //PerformLogging(inputMemberID);
            }
            else
            {
                MessageBox.Show("Invalid ID");
            }
        }


        private void performArchiving()
        {
            if (isArchiving)
            {
                logging.performArchive();
                isArchiving = false;
                //Invalidate(true);
            }
        }


        public void resetState()
        {
            isTimedOut_Ready = true;
            isTimedOut_Info = false;
            isFailure = false;
            isLogout = false;
            isCredit = false;
            isCancel = true;
            blink = false;
        }


        private void PerformLogging(string memberID)
        {
            if (!isArchiving)
            {
                MessageBox.Show("1");
                resetState();

                //****delay for bug fix*******                
                if (memberIdTimer.Enabled)
                {
                    isTimedOut_Ready = false;
                    isCancel = false;
                    delay = true;
                    //Invalidate(true);
                    clearInfo();
                    return;
                }
                //***************************

                int status;
                if (logging.getDataReady(memberID))
                {
                    MessageBox.Show("2");
                    memberName = logging.memberName;
                    isCancel = false;
                    if (logging.isLoggedIn(memberID))
                    {

                        isLogout = false;
                        isCancel = true;// for display prior
                        fillMemberData(memberID);
                        isCancel = false;// ahh all better
                        if (logging.isCredit())
                        {
                            status = logging.isWarnToContinue();// once had params
                            //handleCancelLogin(status);
                        }
                    }
                    if (!isCancel)
                    {
                        if (logging.isCredit())
                        {
                            isCredit = true;
                            if (logging.isLoggedIn(memberID))
                            {
                                logging.logoutMember(memberID, logging.thisVisitsValue);//getVisitValue(memberID));
                                isLogout = true;
                            }
                            else
                            {
                                logging.loginMember(memberID);
                                isLogout = false;
                            }
                        }
                        else
                        {
                            isCredit = false;
                            if (logging.isLoggedIn(memberID))
                            {
                                logging.logoutNonCreditMember(memberID);
                                isLogout = true;
                            }
                            else
                            {
                                logging.loginNonCreditMember(memberID);
                                isLogout = false;
                            }
                        }
                    }
                }
                else
                {
                    isFailure = true;
                    isCancel = false;
                }

                if (!isFailure)
                {
                    fillMemberData(memberID);

                }
                else
                {
                    clearInfo();
                }
                prepPaintTimers();
            }// end is archive
        }


       
        // works but not integrated yet
        private void handleCancelLogin(int status)
        {
            if (status == 0 || status == 1)
            {
                txtBx_MemberID.Text = "";

                if (MessageBox.Show("\n\nWould you really want to log out?\n", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    isCancel = false;
                    isLogout = true;
                }
                else
                {
                    isCancel = true;
                }
            }
            else
            {
                isCancel = false;
            }
        }
         
        /*
        void panel1_Paint(object sender, PaintEventArgs e)
        {
            int w = ((Panel)sender).Width;
            int h = ((Panel)sender).Height;
            //Point p = ((Panel)sender).Location;
            Graphics g = e.Graphics;
            SolidBrush solidBrush = new SolidBrush(Color.Lime);

            if ((!isTimedOut_Ready && !isCancel) || isArchiving)
            {
                if (isFailure || isArchiving || delay)
                {
                    solidBrush.Color = Color.Red;
                    g.FillRectangle(solidBrush, 0, 0, w, h);
                    solidBrush.Color = Color.Black;
                    if (isArchiving)
                    {
                        g.DrawString("Archiving", new Font(FontFamily.GenericSerif, 50), solidBrush, 0, 0);
                        g.DrawString("Please Wait", new Font(FontFamily.GenericSerif, 30), solidBrush, 0, 90);
                    }
                    else if (isFailure)
                    {
                        g.DrawString("No Such Member", new Font(FontFamily.GenericSerif, 50), solidBrush, 0, 0);
                        g.DrawString("Login Unsuccessful", new Font(FontFamily.GenericSerif, 30), solidBrush, 0, 90);
                    }
                    else
                    {
                        g.DrawString("System Is Busy", new Font(FontFamily.GenericSerif, 50), solidBrush, 0, 0);
                        g.DrawString("Please re-swipe card", new Font(FontFamily.GenericSerif, 30), solidBrush, 0, 90);
                    }
                    isFailure = false;
                    delay = false;

                }
                else
                {
                    g.FillRectangle(solidBrush, 0, 0, w, h);
                    solidBrush.Color = Color.Black;
                    g.DrawString(memberName, new Font(FontFamily.GenericSerif, 50), solidBrush, 0, 0);
                    String memberStatus = "";
                    if (isCredit)
                    {
                        memberStatus += "Credit ";
                    }
                    else
                    {
                        memberStatus += "Non-Credit ";
                    }

                    if (isLogout)
                    {
                        memberStatus += "Member Logout Successful";
                    }
                    else
                    {
                        memberStatus += "Member Login Successful";
                    }
                    g.DrawString(memberStatus, new Font(FontFamily.GenericSerif, 30), solidBrush, 0, 90);

                }
            }
            else
            {
                solidBrush.Color = Color.Yellow;
                g.FillRectangle(solidBrush, 0, 0, w, h);
                solidBrush.Color = Color.Black;
                if (isTimedOut_Info)
                {
                    g.DrawString(idleMessege, new Font(FontFamily.GenericSerif, 50), solidBrush, 0, 0);
                    g.DrawString("Ready For Next Scan", new Font(FontFamily.GenericSerif, 30), solidBrush, 0, 90);
                }
                else
                {
                    g.DrawString(memberName, new Font(FontFamily.GenericSerif, 50), solidBrush, 0, 0);
                    g.DrawString("Ready For Next Scan", new Font(FontFamily.GenericSerif, 30), solidBrush, 0, 90);
                }
            }
        }
        */


        private void prepPaintTimers()
        {
            isTimedOut_Ready = false;
            isTimedOut_Info = false;
            readyTimeout.Stop();
            infoTimeout.Stop();
            readyTimeout.Start();
            infoTimeout.Start();
            memberIdTimer.Stop();
            memberIdTimer.Start();
            //this.Invalidate(true);
        }
        /*
        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            card.interceptAndFocus(sender, textBox1);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            card.interceptAndFocus(sender, textBox1);
        }
        */


        private void clearInfo()
        {
            txtblk_TimeInValue.Text = "";
            txtblk_TimeOutValue.Text = "";
            txtblk_WorkoutLengthValue.Text = "";
            txtblk_TotalVisitsValue.Text = "";
            txtblk_MemberName.Text = readyToSwipe;
            txtblk_MemberID.Text = idleMessege;
            txtblk_MemberResult.Text = "";
        }


        // this is messy because of numerous edits... but it works fine.
        private void fillMemberData(string memberID)
        {
            memberName = logging.memberName;
            if (isCancel)
            {
                txtblk_TimeInValue.Text = logging.loginTime.ToString("hh:mm:ss tt");
                txtblk_TimeOutValue.Text = "";
                txtblk_WorkoutLengthValue.Text = logging.workoutLength.ToString() + " minutes";
                txtblk_TotalVisits.Text = logging.visits.ToString();

            }
            else if (isLogout)
            {
                txtblk_TimeInValue.Text = logging.loginTime.ToString("hh:mm:ss tt");
                txtblk_TimeOutValue.Text = DateTime.Now.ToString("hh:mm:ss tt");
                txtblk_WorkoutLengthValue.Text = logging.workoutLength.ToString() + " minutes";
                if (logging.isCredit())
                {
                    txtblk_TotalVisits.Text = "" + (logging.visits + logging.thisVisitsValue);
                }
                else
                {
                    txtblk_TotalVisits.Text = logging.visits.ToString();
                }
            }
            else
            {
                txtblk_TimeInValue.Text = DateTime.Now.ToString("hh:mm:ss tt");
                txtblk_TimeOutValue.Text = "";
                txtblk_WorkoutLengthValue.Text = "";
                if (!logging.isCredit())
                {
                    txtblk_TotalVisits.Text = "" + (logging.visits + logging.thisVisitsValue);
                }
                else
                {
                    txtblk_TotalVisits.Text = logging.visits.ToString();
                }
            }
            lbox_AdditionalActivity.DataContext = logging.activities;
        }
    }
}

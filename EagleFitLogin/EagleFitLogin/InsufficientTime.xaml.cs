
using System.Windows;


namespace EagleFitLogin
{
    /// <summary>
    /// Interaction logic for InsufficientTime.xaml
    /// </summary>
    public partial class InsufficientTime : Window
    {
        public InsufficientTime(int status)
        {
            InitializeComponent();
            if (status == 0)
            {



                lbl_close.Content = "No visits earned this workout";
            }
            else
            {
                lbl_close.Content = "You are close to earning a visit";
            }
        }

        private void btn_yes_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_no_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

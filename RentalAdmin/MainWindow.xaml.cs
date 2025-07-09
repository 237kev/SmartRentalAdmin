using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RentalAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string CheminPhotoLogo = "C:\\Users\\Kevin\\Pictures\\appartement\\logo\\logo.png";
        public MainWindow()
        {
            
            InitializeComponent();
            this.DataContext = this;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

            string identifiantAdmin = UsernameTextBox.Text;
            string motDePasse = PasswordBox.Password;

            try
            {
                if (identifiantAdmin == "admin" && motDePasse == "1234")
                {
                    //                    il est question de creer une instance du tableau de bord
                    Dashboard dashboard = new Dashboard();
                    //                    puis l'afficher et
                    dashboard.Show();
                    //                   ensuite fermer la fenetre de connection
                    this.Close();


                }
                else
                {
                    MessageBox.Show("Identifiant ou mot de passe incorrect");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


    }
}
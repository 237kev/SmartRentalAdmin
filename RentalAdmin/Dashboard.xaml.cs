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
using System.Data;
using Npgsql;

namespace RentalAdmin   
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        //Host=localhost
        //Username = postgres
        //Password= **********
        //Database = databaseLegrandpre

        // le string de connection
        private string connectionString = "Host=localhost;Username=postgres;Password=123456789;Database=databaseLegrandpre";
        public Dashboard()
        {
            InitializeComponent();
            LoadHebergement();
        }

        private void LoadHebergement()
        {
            using (var npgsqlConnection = new NpgsqlConnection(connectionString))
            {
                //ouverture de la connection
                npgsqlConnection.Open();
                // TODO: rediger le query souhaité
                string query = "SELECT ID_hebergement, Nom, Type, Prix_par_nuit, Capacite, Description FROM Hebergements";
                using (var npgsqlCmd = new NpgsqlCommand(query, npgsqlConnection))
                using (var npgaqlAdapter = new NpgsqlDataAdapter(npgsqlCmd))
                {
                    // créer une table de donnée
                    DataTable dt = new DataTable();
                    //remplir la table de donnée
                    npgaqlAdapter.Fill(dt);
                    //afficher dans DataGrid
                    HebergementsDataGrid.ItemsSource = dt.DefaultView;
                }
            }
        }


    }
}

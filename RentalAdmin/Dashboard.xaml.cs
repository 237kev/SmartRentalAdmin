using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
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
        private readonly string connectionString = "Host=localhost;Username=postgres;Password=123456789;Database=databaseLegrandpre";
        public Dashboard()
        {
            InitializeComponent();
            LoadHebergements();
            LoadReservation();
            LoadClients();
            LoadEquipement();
        }

        private void LoadHebergements()
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


        private void EnregistrerHerbergement_Click(object sender, RoutedEventArgs e)
        {
            using (var npgsqlConnection = new NpgsqlConnection(connectionString))
            {
                npgsqlConnection.Open(); // ouverture de la connection

                // parcourir toute les lignes du Datagrid
                foreach (DataRowView rowView in HebergementsDataGrid.ItemsSource)
                {
                    var row = rowView;

                    // Verifier données obligatoires si la colonne du nom et du type ne sont remplies passe a l'iteration suivante
                    if (string.IsNullOrWhiteSpace(row["nom"].ToString()) || string.IsNullOrWhiteSpace(row["type"].ToString()))
                        continue;

                    // il faut extraire les champs du tableau (le nom et le type)
                    string nom = row["nom"].ToString() ?? ""; // ?? "" -> si le string es null alors attribu lui: "" pour eviter 
                    string type = row["type"].ToString() ?? "";
                    decimal prix = Convert.ToDecimal(row["prix_par_nuit"]);
                    int capacite = Convert.ToInt32(row["Capacite"]);
                    string description = row["Description"].ToString() ?? "";

                    // detecter si un nouvel enregistrement ou une modification
                    if (row["ID_hebergement"] == DBNull.Value || string.IsNullOrEmpty(row["ID_hebergement"].ToString()))
                    {
                        //ajouter nouvelle ligne --> INSERT
                        string insertQuery = @"  INSERT INTO Hebergements    (Nom,   Type,   Prix_par_nuit,  Capacite,   Description)
                                                    VALUES                      (@nom,  @Type,  @Prix,          @Capacite,  @Description)";

                        using (var npgsqlCommand = new NpgsqlCommand(insertQuery, npgsqlConnection))
                        {
                            npgsqlCommand.Parameters.AddWithValue("Nom", nom);
                            npgsqlCommand.Parameters.AddWithValue("Type", type);
                            npgsqlCommand.Parameters.AddWithValue("Prix", prix);
                            npgsqlCommand.Parameters.AddWithValue("Capacite", capacite);
                            npgsqlCommand.Parameters.AddWithValue("Description", description);
                            npgsqlCommand.ExecuteNonQuery();
                        }



                    }
                    else
                    {
                        int iD_hebergement = Convert.ToInt32(row["ID_hebergement"]);
                        // update la base de données
                        string updateQuery = @" UPDATE Hebergements SET 
                                                Nom =@Nom,   
                                                Type=@Type,   
                                                Prix_par_nuit = @Prix,  
                                                Capacite = @Capacite,   
                                                Description = @Description
                                                WHERE ID_Hebergement = @ID";


                        using (var npgsqlCommand = new NpgsqlCommand(updateQuery, npgsqlConnection))
                        {
                            npgsqlCommand.Parameters.AddWithValue("ID", iD_hebergement);
                            npgsqlCommand.Parameters.AddWithValue("Nom", nom);
                            npgsqlCommand.Parameters.AddWithValue("Type", type);
                            npgsqlCommand.Parameters.AddWithValue("Prix", prix);
                            npgsqlCommand.Parameters.AddWithValue("Capacite", capacite);
                            npgsqlCommand.Parameters.AddWithValue("Description", description);
                            npgsqlCommand.ExecuteNonQuery();
                        }

                    }
                }
            }
            MessageBox.Show("Les changements ont été enregistrés !");
            LoadHebergements();
        }

        private void SupprimerHebergement_Click(object sender, RoutedEventArgs e)
        {
            // verifier si un row du tableau a ete selectionner?
            if (HebergementsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un hébergement à supprimer");
                return;
            }

            // confirmer la suppression
            var result = MessageBox.Show("Êtes - vous sûr de vouloir supprimer cet hébergement ? ",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;
            // je recupere le row selection et son Id
            var selectedRow = (DataRowView)HebergementsDataGrid.SelectedItem;

            // es ce le celulle ID a une valeur ou est vide
            if (selectedRow["ID_hebergement"] == DBNull.Value || string.IsNullOrEmpty(selectedRow["ID_hebergement"].ToString()))
            {
                MessageBox.Show("Impossible de supprimer : l'ID est manquant. referez l'admin de la base de donnée");
                return;

            }

            // suprimer de la ligne dans la base de donnée
            using (var npgsqlConnection = new NpgsqlConnection(connectionString))
            {

                npgsqlConnection.Open();
                int selectedID = Convert.ToInt32(selectedRow["ID_hebergement"]);
                string deleteQuery = @" DELETE FROM Hebergements
                                        WHERE ID_hebergement = @selectedID";


                using (var npgsqlCommand = new NpgsqlCommand(deleteQuery, npgsqlConnection))
                {
                    npgsqlCommand.Parameters.AddWithValue("selectedID", selectedID);
                    int affectedRows = npgsqlCommand.ExecuteNonQuery();

                    if (affectedRows > 0)
                        MessageBox.Show("\"Hébergement supprimé avec succès !");
                    else
                        MessageBox.Show("Erreur : aucun enregistrement supprimé.");
                }
            }

            // rafraichir le datagrid
            LoadHebergements();

        }

        private void EnregistrerReservation_Click(object sender, EventArgs e)
        {
            try
            {

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }
        private void SupprimerReservation_Click(object sender, EventArgs eventArgs)
        {
            LoadReservation();
        }

        private void LoadClients()
        {
            try
            {
                using (var npgsqlConnection = new NpgsqlConnection(connectionString))
                {
                    npgsqlConnection.Open();
                    string query = @"SELECT * FROM Clients";
                    var npgsqlCommand = new NpgsqlCommand(query, npgsqlConnection);
                    var npgsqlAdapter = new NpgsqlDataAdapter(npgsqlCommand);
                    DataTable dataTable = new DataTable();
                    npgsqlAdapter.Fill(dataTable);

                    ClientsDataGrid.ItemsSource = dataTable.DefaultView;
                }

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void LoadEquipement()
        {
            try
            {
                using (var npgsqlConnection = new NpgsqlConnection(connectionString))
                {
                    npgsqlConnection.Open();
                    string query = @"SELECT * FROM Equipements";
                    var npgsqlCommand = new NpgsqlCommand(query, npgsqlConnection);
                    var npgsqlAdapter = new NpgsqlDataAdapter(npgsqlCommand);
                    DataTable dataTable = new DataTable();
                    npgsqlAdapter.Fill(dataTable);

                    EquipementsDataGrid.ItemsSource = dataTable.DefaultView;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void LoadReservation()
        {
            try
            {
                using (var npgsqlConnection = new NpgsqlConnection(connectionString))
                {
                    npgsqlConnection.Open();
                    //string query = @"
                    //                SELECT *

                    //                FROM
                    //                    Reservations";
                    string query = @"SELECT
	                                    c.nom,
	                                    c.email,
	                                    c.telephone,
	                                    r.date_debut,
	                                    r.date_fin,
                                        r.statut
                                    FROM
                                        clients c 
                                    JOIN reservations r ON c.id_client = r.id_client";

                    using (var npgsqlCmd = new NpgsqlCommand(query, npgsqlConnection))
                    using (var npgsqlAdapter = new NpgsqlDataAdapter(npgsqlCmd))
                    {
                        DataTable dt = new DataTable();
                        npgsqlAdapter.Fill(dt);
                        ReservationsDataGrid.ItemsSource = dt.DefaultView;
                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabControl = sender as TabControl; // make the TabControl as Sender for the SelectionChangedEventArgs


            if (tabControl.SelectedItem is TabItem selectedTab)
            {
                string header = selectedTab.Header.ToString() ?? "";

                switch (header)
                {
                    case "Hébergements":
                        panelButtonHebergements.Visibility = Visibility.Visible;
                        panelButtonReservations.Visibility = Visibility.Collapsed;
                        panelButtonClients.Visibility = Visibility.Collapsed;
                        panelButtonEquipements.Visibility = Visibility.Collapsed;
                        break;

                    case "Réservations":
                        panelButtonHebergements.Visibility = Visibility.Collapsed;
                        panelButtonReservations.Visibility = Visibility.Visible;
                        panelButtonClients.Visibility = Visibility.Collapsed;
                        panelButtonEquipements.Visibility = Visibility.Collapsed;
                        break;
                    case "Clients":
                        panelButtonHebergements.Visibility = Visibility.Collapsed;
                        panelButtonReservations.Visibility = Visibility.Collapsed;
                        panelButtonClients.Visibility = Visibility.Visible;
                        panelButtonEquipements.Visibility = Visibility.Collapsed;
                        break;
                    case "Equipements":
                        panelButtonHebergements.Visibility = Visibility.Collapsed;
                        panelButtonReservations.Visibility = Visibility.Collapsed;
                        panelButtonClients.Visibility = Visibility.Collapsed;
                        panelButtonEquipements.Visibility = Visibility.Visible;
                        break;

                }


            }

        }

        private void VoirPhotos_Click(object sender, EventArgs e)
        {

            var selectedRow = (DataRowView)HebergementsDataGrid.SelectedItem;

            if (selectedRow == null)
            {
                MessageBox.Show("Selection un Hebergement dans la liste");
                return;
            }

            int idHebergement = Convert.ToInt32(selectedRow["id_hebergement"]);
            var photoWindow = new Window1(idHebergement,connectionString);
            photoWindow.Show();

        }

        private void ImprimerFacture_Click(object sender, EventArgs e)
        {

        }

    //private void SupprimerEquipement_Click(object sender, EventArgs eventArgs)
    //{

        //}
        //private void EnregistrerEquipement_Click(object sender, EventArgs eventArgs)
        //{

        //}
        //private void SupprimerClient_Click(object sender, EventArgs eventArgs)
        //{

        //}
        //private void EnregistrerClients_Click(object sender, EventArgs eventArgs)
        //{

        //}


    }
}

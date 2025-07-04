using Microsoft.Graph;
using Microsoft.Graph.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Image = System.Windows.Controls.Image;

namespace RentalAdmin
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private int _id = 0;
        private string _conn = "";
        public Photo PhotoActuelle { get; set; }
        private int indexPhoto = 0;
        public ObservableCollection<Photo> Photos { get; set; } = new ObservableCollection<Photo>();
        public Window1(int id, string conn)
        {
            InitializeComponent();
            _id = id;
            _conn = conn;
            LoadPhotosFromDB();
            this.DataContext = this;

        }

        private void LoadPhotosFromDB()
        {
            using (var connection = new NpgsqlConnection(_conn))
            {
                connection.Open();
                string query = "SELECT chemin_fichier FROM Photos WHERE id_hebergement = @id";
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("id", _id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Photos.Add(new Photo
                            {
                                CheminFichier = reader.GetString(0)
                            });
                        }
                    }
                }
            }
        }

        private void Photo_Click(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            var photo = image?.DataContext as Photo;
            if (photo != null)
            {
                indexPhoto = Photos.IndexOf(photo);
                PhotoActuelle = photo;
                DiapoPanel.Visibility = Visibility.Visible;
                this.DataContext = null;
                this.DataContext = this;
            }
        }
        private void PhotoSuivante_Click(object sender, RoutedEventArgs e)
        {
            if (indexPhoto < Photos.Count - 1)
                indexPhoto++;
            PhotoActuelle = Photos[indexPhoto];
            this.DataContext = null;
            this.DataContext = this;
        }

        private void PhotoPrecedente_Click(object sender, RoutedEventArgs e)
        {
            if (indexPhoto > 0)
                indexPhoto--;
            PhotoActuelle = Photos[indexPhoto];
            this.DataContext = null;
            this.DataContext = this;
        }

        private void FermerDiapo_Click(object sender, RoutedEventArgs e)
        {
            DiapoPanel.Visibility = Visibility.Collapsed;
        }
    }
    public class Photo
    {
        public int ID_photo { get; set; }
        public int ID_hebergement { get; set; }
        public string CheminFichier { get; set; }
    }

}

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

namespace RentalAdmin
{
    /// <summary>
    /// Interaction logic for FactureWindow.xaml
    /// </summary>
    public partial class FactureWindow : Window
    {
        public FactureWindow(string nomDuClient, string email, DateTime dateDebut, DateTime datefin, string nomHebergement, decimal prixParNuit)
        {
            InitializeComponent();

            Facture facture = new Facture();
            facture.ClientNom = nomDuClient;
            facture.ClientEmail = email;
            facture.HebergementNom = nomHebergement;
            facture.DateDebut = dateDebut;
            facture.DateFin = datefin;
            facture.PrixParNuit = prixParNuit;
            
            this.DataContext = facture;

        }
    }

    public class Facture
    {
        public string ClientNom { get; set; }
        public string ClientEmail { get; set; }
        public string ClientTelephone { get; set; }

        public string HebergementNom { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public decimal PrixParNuit { get; set; }

        public decimal Total => (decimal)(DateFin - DateDebut).TotalDays * PrixParNuit;
    }

}

using CustomerApp.Models;
using CustomerApp.Services;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;

namespace CustomerApp.Views
{
    // MainWindow er hovedvinduet i WPF-appen.
    // Den viser kunder, reoler og salg via faneblade (TabControl).
    public partial class MainWindow : Window
    {
        private DatabaseCustomers db;                  // Database-objekt til at hente og opdatere data
        private List<Customer> customers;     // Liste over kunder
        private List<ShelfRental> shelves;    // Liste over reoler

        // --- KONSTRUKTØR ---
        public MainWindow()
        {
            InitializeComponent();            // Initialiser GUI-komponenter

            db = new DatabaseCustomers();              // Opret database-forbindelse

            // --- Hent kunder ---
            customers = db.GetCustomers();    // Hent kunder fra DB (eller offline fallback)
            TxtStatusCustomers.Text = db.LatestStatus; // Vis status i GUI
            UpdateCustomerList();             // Opdater ListBox med kunder

            // --- Hent reoler ---
            shelves = db.GetShelves();        // Hent reoler fra DB (eller offline fallback)
            TxtStatusShelves.Text = db.LatestStatus;   // Vis status i GUI
            UpdateShelfList();                // Opdater ListBox med reoler
        }

        // --- KUNDER ---
        // Opdaterer ListBox med kunder
        private void UpdateCustomerList()
        {
            LstCustomers.Items.Clear();       // Ryd ListBox først
            foreach (var c in customers)      // Tilføj hver kunde som tekst
                LstCustomers.Items.Add(c.ToString());
        }

        // Event-handler for knappen "Opdater Kunder"
        private void BtnUpdateCustomers_Click(object sender, RoutedEventArgs e)
        {
            customers = db.GetCustomers();            // Hent opdateret liste fra DB
            TxtStatusCustomers.Text = db.LatestStatus; // Opdater status
            UpdateCustomerList();                      // Opdater ListBox
        }

        // Event-handler for knappen "Tilføj Kunde"
        private void BtnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Opret ny kunde fra inputfelter
            var newCustomer = new Customer
            {
                Name = TxtName.Text,
                Email = TxtEmail.Text,
                Phone = TxtPhone.Text
            };

            db.AddCustomer(newCustomer);             // Gem kunde i DB
            customers = db.GetCustomers();           // Hent opdateret liste
            TxtStatusCustomers.Text = db.LatestStatus; // Opdater status
            UpdateCustomerList();                    // Opdater GUI

            // Ryd inputfelter og sæt placeholder-tekst
            TxtName.Text = "Name";
            TxtEmail.Text = "Email";
            TxtPhone.Text = "Phone";
        }

        // --- REOLER ---
        // Opdaterer ListBox med reoler
        private void UpdateShelfList()
        {
            LstShelves.Items.Clear();                // Ryd ListBox først
            foreach (var s in shelves)               // Tilføj hver reol som tekst
                LstShelves.Items.Add(s.ToString());
        }

        // Event-handler for knappen "Opdater Reoler"
        private void BtnUpdateShelves_Click(object sender, RoutedEventArgs e)
        {
            shelves = db.GetShelves();               // Hent reoler fra DB
            TxtStatusShelves.Text = db.LatestStatus; // Opdater status
            UpdateShelfList();                       // Opdater ListBox
        }

        // Event-handler for knappen "Udlej Reol"
        private void BtnRentShelf_Click(object sender, RoutedEventArgs e)
        {
            // Forsøg at læse ShelfId og CustomerId fra tekstfelter
            if (int.TryParse(TxtShelfId.Text, out int shelfId) &&
                int.TryParse(TxtCustomerId.Text, out int customerId))
            {
                db.RentShelf(shelfId, customerId);   // Udlej reol i DB
                shelves = db.GetShelves();           // Opdater liste
                TxtStatusShelves.Text = db.LatestStatus;
                UpdateShelfList();                   // Opdater GUI
            }
            else
            {
                TxtStatusShelves.Text = "Ugyldigt input for reol eller kunde.";
            }
        }

        // --- SALG ---
        // Event-handler for knappen "Registrer Salg"
        private void BtnRegisterSale_Click(object sender, RoutedEventArgs e)
        {
            // Forsøg at læse ShelfId og beløb fra tekstfelter
            if (int.TryParse(TxtShelfIdSale.Text, out int shelfId) &&
                double.TryParse(TxtSaleAmount.Text, out double amount))
            {
                db.RegisterSale(shelfId, amount);    // Registrer salg i DB
                TxtStatusSale.Text = db.LatestStatus;
            }
            else
            {
                TxtStatusSale.Text = "Ugyldigt input for reol eller beløb.";
            }
        }

        // --- PLACEHOLDER TEKST ---
        // Ryd standardtekst når TextBox får fokus
        private void TxtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as System.Windows.Controls.TextBox;
            if (tb != null && (tb.Text == "Name" || tb.Text == "Email" || tb.Text == "Phone" ||
                               tb.Text == "ShelfId" || tb.Text == "CustomerId" || tb.Text == "Amount"))
                tb.Text = ""; // Ryd tekst ved fokus
        }

        // Sæt standardtekst tilbage hvis TextBox taber fokus og er tom
        private void TxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as System.Windows.Controls.TextBox;
            if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
            {
                if (tb.Name == "TxtName") tb.Text = "Name";
                if (tb.Name == "TxtEmail") tb.Text = "Email";
                if (tb.Name == "TxtPhone") tb.Text = "Phone";
                if (tb.Name == "TxtShelfId") tb.Text = "ShelfId";
                if (tb.Name == "TxtCustomerId") tb.Text = "CustomerId";
                if (tb.Name == "TxtShelfIdSale") tb.Text = "ShelfId";
                if (tb.Name == "TxtSaleAmount") tb.Text = "Amount";
            }
        }
    }
}

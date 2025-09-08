using CustomerApp.Services; // korrekt reference til Database

namespace CustomerApp.Models
{
    // Klasse, der repræsenterer en kunde
    public class Customer
    {
        // Egenskaber (properties) for kunden
        public int Id { get; set; }         // Kundens unikke ID i databasen
        public string Name { get; set; }    // Kundens navn
        public string Email { get; set; }   // Kundens email
        public string Phone { get; set; }   // Kundens telefonnummer

        // Tom konstruktør – bruges når man vil oprette en kunde uden at angive data med det samme
        public Customer() { }

        // Konstruktør, der tager navn, email og telefon som parametre
        public Customer(string name, string email, string phone)
        {
            Name = name;       // Sætter Name
            Email = email;     // Sætter Email
            Phone = phone;     // Sætter Phone
        }

        // ToString-metode, der bestemmer, hvordan objektet vises som tekst
        // Her viser vi Name, Email og Phone adskilt af bindestreg
        public override string ToString()
        {
            return $"{Name} - {Email} - {Phone}";
        }
    }
}

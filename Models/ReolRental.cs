using System;

namespace CustomerApp.Models
{
    // Klasse der repræsenterer en reol i systemet
    public class ShelfRental
    {
        // --- PROPERTIES ---
        public int Id { get; set; }
        // Unikt ID for reolen i databasen

        public int Number { get; set; }
        // Reolnummer, bruges til at identificere reolen i GUI og DB

        public bool HasClothingRack { get; set; }
        // Angiver om reolen har en bøjlestang
        // true = bøjlestang, false = almindelige hylder

        public bool IsRented { get; set; }
        // Angiver om reolen er udlejet
        // true = udlejet, false = ledig

        public int? CustomerId { get; set; }
        // ID på kunden der lejer reolen
        // Nullable (kan være null) hvis reolen er ledig

        // --- STATISKE METODER ---

        /// <summary>
        /// Beregner månedlig pris baseret på antal reoler.
        /// </summary>
        /// <param name="shelfCount">Antal reoler kunden lejer</param>
        /// <returns>Pris pr. måned i kr.</returns>
        public static double CalculatePricePerMonth(int shelfCount)
        {
            if (shelfCount == 1) return 850;           // 1 reol = 850 kr/md
            else if (shelfCount <= 3) return 825;     // 2-3 reoler = 825 kr/md
            else return 800;                          // 4+ reoler = 800 kr/md
        }

        /// <summary>
        /// Beregner kommission på et salg.
        /// </summary>
        /// <param name="salesAmount">Salgsbeløb i kr.</param>
        /// <returns>Kommission (10%) i kr.</returns>
        public static double CalculateCommission(double salesAmount)
        {
            return salesAmount * 0.10; // 10% af salgsbeløbet
        }

        // --- OVERRIDE METODER ---
        /// <summary>
        /// Returnerer en tekstbeskrivelse af reolen.
        /// </summary>
        /// <returns>Reolnummer, type og status</returns>
        public override string ToString()
        {
            string type = HasClothingRack ? "Clothing Rack" : "Shelves";
            string status = IsRented ? $"Rented to customer {CustomerId}" : "Available";
            return $"Shelf {Number} - {type} - {status}";
        }
    }
}

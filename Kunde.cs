using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReolMarked
{
    // Klassen Kunde repræsenterer en person der kan leje en reol
    public class Kunde
    {
        public string Navn { get; set; }  // Kundens navn

        // Konstruktør
        public Kunde(string navn)
        {
            Navn = navn;
        }
    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReolMarked
{
    // Enum der beskriver typen af reol
    public enum ReolType // bruges her for at definere reolens type klart, sikkerhed for gyldige værdier og gøre koden nemmere at læse og vedligeholde.
    {
        Hylder,
        Boejlestang
    }

    // Klassen Reol repræsenterer en enkelt reol i butikken
    public class Reol
    {
        public int Id { get; set; }            // Reolens unikke ID
        public ReolType Type { get; set; }     // Reolens type (hylder eller bøjlestang)
        public bool ErUdlejet { get; set; }    // Status: true = udlejet, false = ledig
        public string Placering { get; set; }  // Hvor reolen står i butikken

        // Konstruktør til at oprette en reol
        public Reol(int id, ReolType type, string placering)
        {
            Id = id;
            Type = type;
            Placering = placering;
            ErUdlejet = false; // Som standard er en nyoprettet reol ledig
        }

        // Gør det nemt at vise reolens info i en liste
        public override string ToString()
        {
            return $"Reol {Id} - {Type} - {Placering} - {(ErUdlejet ? "UDLEJET" : "LEDIG")}";
        }
    }
}



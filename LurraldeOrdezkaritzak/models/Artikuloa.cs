using System;
using SQLite;

namespace lurraldeOrdezkaritzak
{
    [SQLite.Table("Artikuloa")]
    public class Artikuloa
    {
        [PrimaryKey, AutoIncrement, SQLite.Column("id")]
        public int Id { get; set; }

        [SQLite.Column("izena")]
        public string Izena { get; set; }

        [SQLite.Column("kategoria")]
        public string Kategoria { get; set; }

        [SQLite.Column("prezioa")]
        public double Prezioa { get; set; }

        [SQLite.Column("stock")]
        public int Stock { get; set; }

        public Artikuloa() { }

        public Artikuloa(string izena, string kategoria, double prezioa, int stock)
        {
            Izena = izena;
            Kategoria = kategoria;
            Prezioa = prezioa;
            Stock = stock;
        }
    }
}

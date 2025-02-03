using System;
using System.Xml.Serialization;
using SQLite;

namespace lurraldeOrdezkaritzak
{
    [Table("Artikuloa")]
    [XmlRoot("Artikuloa")]
    public class Artikuloa
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        [XmlIgnore]
        public int Id { get; set; }

        [Column("izena")]
        [XmlElement("izena")]
        public string Izena { get; set; }

        [Column("kategoria")]
        [XmlElement("kategoria")]
        public string Kategoria { get; set; }

        [Column("prezioa")]
        [XmlElement("prezioa")]
        public double Prezioa { get; set; }

        [Column("stock")]
        [XmlElement("stock")]
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

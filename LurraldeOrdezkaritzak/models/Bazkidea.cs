using System;
using System.Collections.Generic;
using SQLite;

namespace lurraldeOrdezkaritzak
{
    [SQLite.Table("Bazkidea")]
    public class Bazkidea
    {
        [PrimaryKey, AutoIncrement, SQLite.Column("id")]
        public int Id { get; set; }

        [SQLite.Column("izena")]
        public string Izena { get; set; }

        [SQLite.Column("email")]
        public string Email { get; set; }

        [SQLite.Column("telefonoa")]
        public string Telefonoa { get; set; }

        [SQLite.Column("helbidea")]
        public string Helbidea { get; set; }

        [SQLite.Column("bazkideMota")]
        public string BazkideMota { get; set; }

        [SQLite.Column("komerzialaId")]
        public int KomerzialaId { get; set; }

        public Bazkidea() { }

        public Bazkidea(int id, string izena, string email, string telefonoa, string helbidea, string bazkideMota, int komerzialaId)
        {
            Id = id;
            Izena = izena;
            Email = email;
            Telefonoa = telefonoa;
            Helbidea = helbidea;
            BazkideMota = bazkideMota;
            KomerzialaId = komerzialaId;
        }
    }
}

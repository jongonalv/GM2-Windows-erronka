using System;
using lurraldeOrdezkaritzak;
using SQLite;

[SQLite.Table("EskaeraEgoitza")]
public class EskaeraEgoitza
{
    [PrimaryKey, AutoIncrement, SQLite.Column("id")]
    public int Id { get; set; }

    [SQLite.Column("Kantitatea")]
    public int Kantitatea { get; set; }

    [SQLite.Column("Iritsiera-data")]
    public DateTime Iritsiera_data { get; set; }

    [SQLite.Column("Entregatuta")]
    public bool Entregatuta { get; set; }

    [Ignore]
    public List<Artikuloa> Artikuloa { get; set; } = new List<Artikuloa>();

    public EskaeraEgoitza() { }

}


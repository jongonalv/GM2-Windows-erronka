using SQLite;
using System;

[Table("EskaeraArtikuloa")]
public class EskaeraArtikuloa
{
    [PrimaryKey, AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    [Column("guztira")]
    public double Guztira { get; set; }

    [Column("kantitatea")]
    public int Kantitatea { get; set; } = 1;

    [Column("deskontua")]
    public int Deskontua { get; set; } = 0;

    [Column("artikuloa_id")]
    public int ArtikuloaId { get; set; }

    [Column("eskaera_id")]
    public int EskaeraId { get; set; }
}

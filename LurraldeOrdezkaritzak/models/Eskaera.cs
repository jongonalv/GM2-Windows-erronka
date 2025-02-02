using System;
using System.Collections.Generic;
using SQLite;

[Table("Eskaera")]
public class Eskaera
{
    [PrimaryKey, AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    [Column("eskaeraData")]
    public long EskaeraDataTimestamp { get; set; }

    [Ignore]
    public DateTime EskaeraData
    {
        get => DateTimeOffset.FromUnixTimeSeconds(EskaeraDataTimestamp).DateTime;
        set => EskaeraDataTimestamp = ((DateTimeOffset)value).ToUnixTimeSeconds();
    }

    [Column("bazkideaId")]
    public int BazkideaId { get; set; }

    [Column("kontzeptua")]
    public string Kontzeptua { get; set; }

    [Column("egoera")]
    public string Egoera { get; set; }

    [Ignore]
    public List<EskaeraArtikuloa> EskaeraArtikuloak { get; set; } = new List<EskaeraArtikuloa>();

    [Column("guztira")]
    public double Guztira { get; set; }
}
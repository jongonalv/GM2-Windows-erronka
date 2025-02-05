using System;
using System.Collections.Generic;
using SQLite;
using Ikaslea.KomertzialakApp.Models.Enums;

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
    public string EgoeraString
    {
        get => Egoera.ToString();
        set => Egoera = Enum.TryParse<Egoera>(value, true, out var egoera) ? egoera : throw new ArgumentException($"Invalid egoera: {value}");
    }

    [Ignore]
    public Egoera Egoera { get; set; }

    [Ignore]
    public List<EskaeraArtikuloa> EskaeraArtikuloak { get; set; } = new List<EskaeraArtikuloa>();

    [Ignore]
    public int KantitateTotala => EskaeraArtikuloak?.Sum(ea => ea.Kantitatea) ?? 0;

    [Column("guztira")]
    public double Guztira { get; set; }
}

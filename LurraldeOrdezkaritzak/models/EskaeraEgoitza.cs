using SQLite;

[SQLite.Table("EskaeraEgoitza")]
public class EskaeraEgoitza
{
    [PrimaryKey, AutoIncrement, SQLite.Column("id")]
    public int Id { get; set; }

    [SQLite.Column("Kantitatea")]
    public int Kantitatea { get; set; }

    [SQLite.Column("Iritsiera_data")]
    public long IritsieraDataUnix { get; set; }

    [SQLite.Column("Entregatuta")]
    public bool Entregatuta { get; set; }

    [Column("artikuloa_id")]
    public int ArtikuloaId { get; set; }
    public DateTime Iritsiera_data
    {
        get => DateTimeOffset.FromUnixTimeSeconds(IritsieraDataUnix).DateTime;
        set => IritsieraDataUnix = new DateTimeOffset(value).ToUnixTimeSeconds();
    }

    public EskaeraEgoitza() { }
}
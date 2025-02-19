using SQLite;

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

    [Ignore]
    public DateTime Iritsiera_data
    {
        get
        {
            return IritsieraDataUnix > 0
                ? DateTimeOffset.FromUnixTimeSeconds(IritsieraDataUnix).DateTime
                : DateTime.MinValue;
        }
        set
        {
            IritsieraDataUnix = value != DateTime.MinValue
                ? new DateTimeOffset(value).ToUnixTimeSeconds()
                : 0;
        }
    }

    public EskaeraEgoitza() { }
}

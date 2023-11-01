namespace Common.POCOs;
public class DateTimePoco : POCO
{
    public static DateTimePoco UTCNow => new(DateTime.UtcNow);

    public DateTime DateTime { get; set; }

    public DateTimePoco(DateTime dateTime) => DateTime = dateTime;
}

namespace TestApi.Messages;

public class RoomMessage
{
    public long Id { get; set; }
    public DateTime Date { get; set; }

    public string RoomId { get; set; }

    public string Message { get; set; }
}

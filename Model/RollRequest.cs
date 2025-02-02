namespace TestApi.Model
{
    public class RollRequest
    {
        public string RoomId { get; set; }
        public string PlayerId { get; set; }
        public List<BatchOfDice> Dice { get; set; }
    }

    public class BatchOfDice
    {
        public int NumberOfSides { get; set; }
        public int NumberOfDice { get; set; }
    }
}

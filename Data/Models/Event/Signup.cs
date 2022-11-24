namespace Data.Models.Event
{
    /// <summary>
    /// Represents a single occurrence of a person in a namelist of an event.
    /// </summary>
    public class Signup
    {
        public string Name { get; set; }
        public string TelegramUserId { get; set; }
        public DateTime? ArrivedAt { get; set; }
    }
}

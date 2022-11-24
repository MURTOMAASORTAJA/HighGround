using Data.Interfaces;

namespace Data.Models.Event
{
    public class Event : IIdentifiable, ITimeStampable
    {
        public Guid Id { get; set; }
        public Timestamps Timestamps { get; set; }
        public Information Information { get; set; }
        public RSVPBook RSVP { get; set; }
        public TicketPrices? TicketPrices { get; set; }

        #region Constructors
        public Event()
        {
            Id = Guid.NewGuid();
            Timestamps = new Timestamps() { Created = DateTime.UtcNow };
            Information = new Information();
            RSVP = new RSVPBook();
            
        }
        public Event(Information information)
        {
            Information = information;
            Id = Guid.NewGuid();
            Timestamps = new Timestamps() { Created = DateTime.UtcNow };
            RSVP = new RSVPBook();
        }
        #endregion
    }
}

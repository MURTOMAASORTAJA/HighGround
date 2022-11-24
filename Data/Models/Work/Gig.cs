using Data.Interfaces;

namespace Data.Models.Work
{
    public class Gig : IIdentifiable
    {
        /// <summary>
        /// Definition of this gig, consisting of a title and description.
        /// Consider this as the "cover art" for this gig.
        /// </summary>
        public GigDefinition Definition { get; set; }
        
        public Guid Id { get; set; }

        /// <summary>
        /// Guid identifier of a gig that is responsible of this gig.
        /// </summary>
        public Guid? ResponsibleGigId { get; set; }

        /// <summary>
        /// Guid identifier of a gig this gig is dependent on. 
        /// The depended gig must be finished before this gig can be started.
        /// </summary>
        public Guid? DependedGig { get; set; }

        /// <summary>
        /// Appointed start time of this gig.
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// Appointed finish time of this gig.
        /// </summary>
        public DateTime? Finish { get; set; }

        public Gig()
        {
            Id = Guid.NewGuid();
            Definition = new GigDefinition();
        }
    }
}

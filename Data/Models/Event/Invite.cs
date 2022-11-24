using Data.Interfaces;

namespace Data.Models.Event
{
    /// <summary>
    /// Represents a single entry, i.e. an invite or a signup.
    /// </summary>
    public class Entry : IIdentifiable, ITimeStampable
    {
        /// <summary>
        /// Contains timestamp information about a particular entry object.
        /// </summary>
        public Timestamps Timestamps { get; set; }

        /// <summary>
        /// Name of the person who is signing up or being invited.
        /// </summary>
        public string PersonName { get; set; } = "";

        /// <summary>
        /// Telegram user identifier or username.
        /// </summary>
        public string TelegramUserId { get; set; } = "";

        /// <summary>
        /// Amount of people this person can take with them as avec.
        /// </summary>
        public int GrantedAvecAmount { get; set; }

        /// <summary>
        /// Amount of people this person can invite to the event.
        /// </summary>
        public int GrantedInviteAmount { get; set; }

        public Guid Id { get; set; }

        public Entry()
        {
            Timestamps = new Timestamps();
        }
    }
}
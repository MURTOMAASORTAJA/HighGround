using Data.Interfaces;

namespace Data.Models
{
    public class Person : ITimeStampable, IIdentifiable
    {
        /// <summary>
        /// Contains timestamp information about a particular object.
        /// </summary>
        public Timestamps Timestamps { get; set; }

        /// <summary>
        /// Name of the person who is signing up or being invited.
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// Telegram user identifier or username.
        /// </summary>
        public string TelegramUserId { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool EmailWorkNotifications { get; set; }
        
        public Guid Id { get; set; }

        public Person()
        {
            Timestamps = new Timestamps();
            PersonName = "";
            TelegramUserId = "";
            PhoneNumber = "";
            EmailAddress = "";
        }
    }
}

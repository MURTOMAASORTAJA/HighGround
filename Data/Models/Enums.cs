namespace Data.Models
{
    public class Enums
    {
        public enum SignupInclusivity
        {
            /// <summary>
            /// No signup needed.
            /// </summary>
            NoSignup = 0,

            /// <summary>
            /// Signup is required, but anyone can sign up.
            /// </summary>
            OpenSignup = 1,

            /// <summary>
            /// Invitation is required, and only the people who have been invited can sign up.
            /// </summary>
            InviteOnly = 2,
        }

        public enum EntryMethod
        {
            NoEntry = 0,

            /// <summary>
            /// No tickets required for entry.
            /// </summary>
            FreeEntry = 1,

            /// <summary>
            /// Participating in the event requires a ticket, acquired by the person at the event, or before the event.
            /// </summary>
            Ticket = 2,

            /// <summary>
            /// Participating in the event requires a ticket, acquired by the person before the event.
            /// </summary>
            PreacquiredTicket = 3
        }

        public enum SignupResponse
        {
            Unanswered = 0,
            Going = 1,
            Maybe = 2,
            NotGoing = 3
        }

        public enum InvitedBy
        {
            EventAdmin = 0,
            DesignatedInviter = 1,
            AnotherInvitee = 2,
            Themself = 3,    
        }

        public enum ProductionStage
        {
            /// <summary>
            /// Pre-event stage, which can happen days or even months before the event.
            /// </summary>
            PreEvent,

            /// <summary>
            /// The time right before the event, i.e. the hours preceding the event during the same day.
            /// </summary>
            RightBeforeEvent,

            /// <summary>
            /// The time between start and finish of the event.
            /// </summary>
            DuringEvent,

            /// <summary>
            /// The time between the finish of the event and a few hours after it.
            /// </summary>
            RightAfterEvent,

            /// <summary>
            /// Post-event stage, which means days or even months after the event.
            /// </summary>
            PostEvent
        }

        public enum ReviewResult
        {
            NotReviewed,
            Ok,
            Failed,
            NeedsMoreWork,
        }
    }
}

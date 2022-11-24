using static Data.Models.Enums;

namespace Data.Models.Event
{
    public class RSVPBook
    {
        public SignupInclusivity Inclusivity { get; set; } = SignupInclusivity.NoSignup;
        public EntryMethod EntryRequirement { get; set; }
        public Entry[] Entries { get; set; }

        public RSVPBook()
        {
            Inclusivity = SignupInclusivity.NoSignup;
            EntryRequirement = EntryMethod.NoEntry;
            Entries = Array.Empty<Entry>();
        }
    }
}

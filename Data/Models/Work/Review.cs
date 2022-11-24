using static Data.Models.Enums;

namespace Data.Models.Work
{
    public class Review
    {
        public Guid Reviewer { get; set; }
        public string Message { get; set; } = "";
        public ReviewResult Result { get; set; }
    }
}

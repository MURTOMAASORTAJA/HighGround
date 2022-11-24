using Data.Models;

namespace Data.Interfaces
{
    public interface ITimeStampable
    {
        public Timestamps Timestamps { get; set; }
    }
}

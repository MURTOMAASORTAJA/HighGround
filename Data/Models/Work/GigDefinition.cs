using Data.Interfaces;

namespace Data.Models.Work
{
    public class GigDefinition : IInformative
    {
        public Information Information { get; set; }

        public string Instructions { get; set; }

        /// <summary>
        /// The definition of the finished gig, for the worker to recognize the stage or state where the gig needs no further work.
        /// </summary>
        public string DefinitionOfDone { get; set; }


        public GigDefinition()
        {
            Instructions = "";
            Information = new Information();
            DefinitionOfDone = "";
        }
    }
}

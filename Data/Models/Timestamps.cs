using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Timestamps
    {
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public Timestamps()
        {
            Created = DateTime.Now;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetModel
{
    public  class VisitService
    {
        public int Id { get; set; }

        public int VisitId { get; set; }

        public int ServiceId { get; set; }

        public decimal ServicePrice { get; set; }

        public virtual Visit Visit { get; set; }

        public virtual Service Service { get; set; }
    }
}

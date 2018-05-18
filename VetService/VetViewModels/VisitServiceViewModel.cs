using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VetService.VetViewModels
{
    public class VisitServiceViewModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int VisitId { get; set; }

        [DataMember]
        public int ServiceId { get; set; }

        [DataMember]
        public string ServiceName { get; set; }

        [DataMember]
        public decimal ServicePrice { get; set; }
    }
}

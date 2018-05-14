using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VetService.VetViewModels
{
    [DataContract]
    public class PayViewModel
    {
        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public string ClientFIO { get; set; }

        [DataMember]
        public string DateCreate { get; set; }

        [DataMember]
        public decimal Sum { get; set; }
    }
}

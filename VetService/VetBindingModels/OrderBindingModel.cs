using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VetService.VetBindingModels
{
    [DataContract]
    public class OrderBindingModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int ClientId { get; set; }

        [DataMember]
        public DateTime CreditEnd { get; set; }

        [DataMember]
        public int VisitId { get; set; }

        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public decimal Summa { get; set; }
    }
}

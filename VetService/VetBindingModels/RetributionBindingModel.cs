using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VetService.VetBindingModels
{
    [DataContract]
    public class RetributionBindingModel
    {
        [DataMember]
        public int ClientId { get; set; }

        [DataMember]
        public int Calculation { get; set; }

        [DataMember]
        public int Penalty { get; set; }
    }
}

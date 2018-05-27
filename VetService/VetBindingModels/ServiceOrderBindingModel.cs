using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VetService.VetBindingModels
{
    [DataContract]
    public class ServiceOrderBindingModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int ServiceId { get; set; }

        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public int Price { get; set; }
    }
}

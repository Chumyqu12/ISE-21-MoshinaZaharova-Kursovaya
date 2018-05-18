using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VetService.VetBindingModels
{
    [DataContract]
    public class VisitBindingModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string VisitName { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public List<VisitServiceBindingModel> VisitService { get; set; }
    }
}

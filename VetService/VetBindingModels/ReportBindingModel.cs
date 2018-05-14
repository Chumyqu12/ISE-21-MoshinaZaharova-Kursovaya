using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VetService.VetBindingModels
{
    [DataContract]
    public class ReportBindingModel
    {
        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string FontPath { get; set; }

        [DataMember]
        public DateTime DateFrom { get; set; }

        [DataMember]
        public DateTime DateTo { get; set; }
    }
}

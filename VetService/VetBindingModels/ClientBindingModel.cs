using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VetService.VetBindingModels
{
    [DataContract]
    public class ClientBindingModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string ClientFIO { get; set; }

        [DataMember]
        public string UserName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VetService.App;

namespace VetService.VetBindingModels
{
    [DataContract]
    public class ClientCreateModel : AppUser
    {
        [DataMember]
        public string ClientFIO { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VetService.VetViewModels
{
    [DataContract]
    public class ClientViewModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string ClientFIO { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public int Bonuses { get; set; }

        [DataMember]
        public string Active { get; set; }
    }
}

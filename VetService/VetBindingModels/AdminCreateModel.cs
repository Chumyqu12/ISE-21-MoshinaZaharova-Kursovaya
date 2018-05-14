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
    public class AdminCreateModel : AppUser
    {
        [DataMember]
        public string AdminFIO { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VetAdminView
{
    [DataContract]
    public class TokenResponse
    {
        [DataMember]
        public string Access_token { get; set; }

        [DataMember]
        public string Token_type { get; set; }

        [DataMember]
        public string UserName { get; set; }
    }
}

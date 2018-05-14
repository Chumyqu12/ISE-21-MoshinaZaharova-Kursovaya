﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VetService.VetBindingModels
{
    [DataContract]
    public class AdminBindingModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string AdminFIO { get; set; }

        [DataMember]
        public string UserName { get; set; }
    }
}

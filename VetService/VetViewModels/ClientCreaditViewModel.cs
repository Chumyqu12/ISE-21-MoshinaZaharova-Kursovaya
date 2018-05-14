﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VetService.VetViewModels
{
    [DataContract]
    public class ClientCreaditViewModel
    {
        [DataMember]
        public int ClientId { get; set; }

        [DataMember]
        public string ClientFIO { get; set; }

        [DataMember]
        public string Mail { get; set; }

        [DataMember]
        public List<OrderCreditViewModel> OrderCredits { get; set; }
    }
    [DataContract]
    public class OrderCreditViewModel
    {
        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public List<ServiceOrderViewModel> Services { get; set; }

        [DataMember]
        public string DateCreate { get; set; }

        [DataMember]
        public decimal Total { get; set; }

        [DataMember]
        public decimal TotalPaid { get; set; }

        [DataMember]
        public decimal Credit { get; set; }
    }


}

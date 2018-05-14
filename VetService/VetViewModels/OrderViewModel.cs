using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VetService.VetViewModels
{
    [DataContract]
    public class OrderViewModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int ClientId { get; set; }

        [DataMember]
        public string ClientFIO { get; set; }

        [DataMember]
        public string Mail { get; set; }

        [DataMember]
        public string OrderStatus { get; set; }

        [DataMember]
        public string DateCreate { get; set; }

        [DataMember]
        public string CreditEnd { get; set; }

        [DataMember]
        public decimal Sum { get; set; }

        [DataMember]
        public decimal Paid { get; set; }

        [DataMember]
        public decimal Credit { get; set; }

        [DataMember]
        public List<ServiceOrderViewModel> ServiceOrders { get; set; }

        [DataMember]
        public DateTime CreditDate { get; set; }
    }
}

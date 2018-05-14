using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetModel
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        public string ServiceName { get; set; }

        [Required]
        public decimal Price { get; set; }

        [ForeignKey("ServiceId")]
        public virtual List<ServiceOrder> ServiceOrders { get; set; }
    }
}

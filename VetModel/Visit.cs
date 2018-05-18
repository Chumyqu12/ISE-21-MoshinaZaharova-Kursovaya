using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetModel
{
    public class Visit
    {
        public int Id { get; set; }

        [Required]
        public string VisitName { get; set; }

        [Required]
        public decimal Price { get; set; }

        [ForeignKey("VisitId")]
        public virtual List<Order> Orders { get; set; }

        [ForeignKey("VisitId")]
        public virtual List<VisitService> VisitService { get; set; }
    }
}

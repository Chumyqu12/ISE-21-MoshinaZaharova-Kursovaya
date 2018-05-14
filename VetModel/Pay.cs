using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetModel
{
    public class Pay
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public DateTime DateCreate { get; set; }

        public decimal Summ { get; set; }

        public virtual Order Order { get; set; }
    }
}

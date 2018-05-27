using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetModel
{
    public class ServiceOrder
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }

        public int OrderId { get; set; }

        public int Count { get; set; }

        public int Price { get; set; }

        public virtual Service Service { get; set; }

        public virtual Order Order { get; set; }
    }
}

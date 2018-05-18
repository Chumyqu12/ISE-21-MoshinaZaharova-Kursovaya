﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetModel
{
    public class Order
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public DateTime DateCreate { get; set; }

        public DateTime CreditEnd { get; set; }

        public virtual Client Client { get; set; }

        [ForeignKey("OrderId")]
        public virtual List<Pay> Pays { get; set; }

        public int VisitId { get; set; }

        public int Count { get; set; }

        public decimal Summa { get; set; }

        public OrderStatus Status { get; set; }

        public virtual Visit Visit { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetService.VetViewModels
{
    public class ClientOrdersViewModel
    {
        public string ClientName { get; set; }

        public string DateOfCreate { get; set; }

        public List<string> ServiceName { get; set; }

        public List<int> Summa { get; set; }

        public string Status { get; set; }
    }
}

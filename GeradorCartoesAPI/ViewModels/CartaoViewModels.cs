using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeradorCartoesAPI.ViewModels
{
    public class ReadCartaoViewModel
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }

        public DateTime Data { get; set; }
        public string Numero { get; set; }
    }
}

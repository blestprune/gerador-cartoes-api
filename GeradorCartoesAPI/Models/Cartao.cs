using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeradorCartoesAPI.Models
{
    public class Cartao
    {
        public int Id { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public string Numero { get; set; }

        public virtual Cliente Cliente { get; set; }
    }
}

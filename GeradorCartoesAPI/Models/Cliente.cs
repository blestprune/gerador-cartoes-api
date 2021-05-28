using System.ComponentModel.DataAnnotations;

namespace GeradorCartoesAPI.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }
    }
}

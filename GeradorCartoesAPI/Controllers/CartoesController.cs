using GeradorCartoesAPI.Context;
using GeradorCartoesAPI.Models;
using GeradorCartoesAPI.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GeradorCartoesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartoesController : ControllerBase
    {
        private readonly GeradorCartoesContext _context;

        public CartoesController(GeradorCartoesContext context)
        {
            _context = context;
        }

        [HttpPost("{email}")]
        public ActionResult<Cartao> CreateCartao(string email)
        {
            var cliente = _context.Clientes.Where(a => a.Email == email).FirstOrDefault();

            if (cliente == null)
            {
                Cliente novoCliente = new Cliente()
                {
                    Email = email
                };

                _context.Clientes.Add(novoCliente);
                _context.SaveChanges();

                cliente = _context.Clientes.Where(a => a.Email == email).FirstOrDefault();
            }

            bool cartaoExistente;
            string novoNumero;

            do
            {
                novoNumero = Gerador.GerarCartao();
                cartaoExistente = _context.Cartoes.Where(c => c.Numero == novoNumero).FirstOrDefault() != null;
            } while (cartaoExistente);

            Cartao novoCartao = new Cartao
            {
                Cliente = cliente,
                Numero = novoNumero
            };

            _context.Cartoes.Add(novoCartao);
            _context.SaveChanges();

            return new ObjectResult(novoCartao) { StatusCode = StatusCodes.Status201Created };
        }
    }
}

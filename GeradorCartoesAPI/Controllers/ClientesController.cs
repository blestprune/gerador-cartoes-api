using GeradorCartoesAPI.Context;
using GeradorCartoesAPI.Models;
using GeradorCartoesAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace GeradorCartoesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly GeradorCartoesContext _context;

        public ClientesController(GeradorCartoesContext context)
        {
            _context = context;
        }

        [HttpGet("{email}")]
        public ActionResult<IEnumerable<ReadCartaoViewModel>> GetClientePorEmail(string email)
        {
            List<ReadCartaoViewModel> cartoesViewModel = new List<ReadCartaoViewModel>();

            var cliente = _context.Clientes.Where(a => a.Email == email).FirstOrDefault();

            if (cliente == null)
            {
                return NotFound();
            }

            var cartoes = _context.Cartoes.Where(c => c.Cliente == cliente).OrderBy(c => c.Data).ToList();

            foreach (Cartao c in cartoes)
            {
                ReadCartaoViewModel cartaoVm = new ReadCartaoViewModel();
                cartaoVm.Id = c.Id;
                cartaoVm.Numero = c.Numero;
                cartaoVm.Data = c.Data;
                cartaoVm.ClienteId = c.Cliente.Id;
                cartoesViewModel.Add(cartaoVm);
            }

            return Ok(cartoesViewModel);
        }
    }
}

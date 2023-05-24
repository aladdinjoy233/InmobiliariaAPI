using System.Security.Claims;
using InmobiliariaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAPI.Controllers;

[ApiController]
[Route("[Controller]")]
public class PagosController : ControllerBase
{
	private readonly DataContext _context;
	private readonly IConfiguration _config;
	public PagosController(DataContext context, IConfiguration config)
	{
		_context = context;
		_config = config;
	}

	// GET: Pagos/
	[HttpGet]
	public IEnumerable<Pago> Get()
	{
		return _context.Pagos;
	}

	// POST: Pagos/Obtener/{contrato_id}
	[HttpPost("Obtener/{contrato_id}")]
	[Authorize]
	public IActionResult obtenerPorContrato(int contrato_id)
	{
		try
		{
			int.TryParse(User.FindFirstValue("Id"), out int userId);

			var usuario = User.Identity != null
				? _context.Propietarios.Find(userId)
				: null;
			if (usuario == null) return NotFound();

			var contrato = _context.Contratos.Include(c => c.Inmueble).FirstOrDefault(c => c.Id_Contrato == contrato_id);
			if (contrato == null) return NotFound();

			if (contrato.Inmueble.Id_Propietario != usuario.Id_Propietario) return Unauthorized();

			var pagos = _context.Pagos.Where(p => p.Id_Contrato == contrato_id);

			return Ok(pagos);
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}
}
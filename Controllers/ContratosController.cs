using System.Security.Claims;
using InmobiliariaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAPI.Controllers;

[ApiController]
[Route("[Controller]")]
public class ContratosController : ControllerBase
{
	private readonly DataContext _context;
	private readonly IConfiguration _config;
	public ContratosController(DataContext context, IConfiguration config)
	{
		_context = context;
		_config = config;
	}

	// GET: Contratos/Obtener/{inmueble_id}
	[HttpGet("Obtener/{inmueble_id}")]
	[Authorize]
	public IActionResult ObtenerPorInmueble(int inmueble_id)
	{
		try
		{
			int.TryParse(User.FindFirstValue("Id"), out int userId);

			var usuario = User.Identity != null
				? _context.Propietarios.Find(userId)
				: null;

			if (usuario == null) return NotFound();

			var inmueble = _context.Inmuebles.Find(inmueble_id);

			if (inmueble == null) return NotFound();
			if (usuario.Id_Propietario != inmueble.Id_Propietario) return Unauthorized(); // Verificar que el inmueble sea del usuario

			var contrato = _context.Contratos
				.Include(i => i.Inquilino)
				.FirstOrDefault(i => i.Id_Inmueble == inmueble_id);

			if (contrato == null) return NotFound();

			return Ok(contrato);
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}
}
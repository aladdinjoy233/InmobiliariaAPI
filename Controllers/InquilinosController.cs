using System.Security.Claims;
using InmobiliariaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAPI.Controllers;

[ApiController]
[Route("[Controller]")]
public class InquilinosController : ControllerBase
{
	private readonly DataContext _context;
	private readonly IConfiguration _config;
	public InquilinosController(DataContext context, IConfiguration config)
	{
		_context = context;
		_config = config;
	}

	// GET: Inquilinos/Obtener/{inmueble_id}
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

			var inquilino = _context.Contratos
				.Include(c => c.Inquilino)
				.Where(c => c.Id_Inmueble == inmueble_id)
				.Select(c => c.Inquilino)
				.FirstOrDefault();

			if (inquilino == null) return NotFound();

			return Ok(inquilino);
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}
}
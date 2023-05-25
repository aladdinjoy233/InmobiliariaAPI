using System.Security.Claims;
using InmobiliariaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAPI.Controllers;

[ApiController]
[Route("[Controller]")]
public class InmueblesController : ControllerBase
{
	private readonly DataContext _context;
	private readonly IConfiguration _config;
	public InmueblesController(DataContext context, IConfiguration config)
	{
		_context = context;
		_config = config;
	}

	// GET: Inmuebles/
	[HttpGet]
	[Authorize]
	public IActionResult GetInmuebles()
	{
		try
		{
			int.TryParse(User.FindFirstValue("Id"), out int userId);
			var usuario = User.Identity != null
				? _context.Propietarios.Find(userId)
				: null;

			if (usuario == null) return NotFound();

			return Ok(_context.Inmuebles.Include(i => i.Propietario).Where(e => e.Propietario.Id_Propietario == usuario.Id_Propietario));
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}

	// PUT: Inmuebles/Cambiar_Estado/{id}
	[HttpPut("Cambiar_Estado/{id}")]
	[Authorize]
	public IActionResult CambiarEstado(int id, [FromBody] bool active)
	{
		try
		{
			int.TryParse(User.FindFirstValue("Id"), out int userId);
			var usuario = User.Identity != null
				? _context.Propietarios.Find(userId)
				: null;

			if (usuario == null) return NotFound();

			var inmueble = _context.Inmuebles.Find(id);

			if (inmueble == null) return NotFound();

			if (inmueble.Id_Propietario != usuario.Id_Propietario) return Forbid();

			inmueble.Activo = active;
			_context.SaveChanges();

			return Ok(inmueble);
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}

	// GET: Inmuebles/Alquilados
	[HttpGet("Alquilados")]
	[Authorize]
	public IActionResult GetAlquilados()
	{
		try
		{

			int.TryParse(User.FindFirstValue("Id"), out int userId);
			var usuario = User.Identity != null
				? _context.Propietarios.Find(userId)
				: null;

			if (usuario == null)
				return NotFound();

			var currentDate = DateTime.Today;

			var inmuebles = _context.Contratos
				.Include(c => c.Inmueble)
					.ThenInclude(i => i.Propietario)
				.Where(c => c.Inmueble.Propietario.Id_Propietario == usuario.Id_Propietario)
				.Where(c => c.Activo && c.Fecha_Inicio <= currentDate && c.Fecha_Fin >= currentDate)
				.Select(c => c.Inmueble)
				.ToList();


			return Ok(inmuebles);

		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}



}
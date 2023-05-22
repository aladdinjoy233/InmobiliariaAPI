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
	private string hashSalt = "";
	public InmueblesController(DataContext context, IConfiguration config)
	{
		_context = context;
		_config = config;
	}

	// GET: Inmuebles/
	[HttpGet()]
	public IEnumerable<Inmueble> Get()
	{
		return _context.Inmuebles;
		// return _context.Inmuebles.Include(i => i.Propietario); // Para obtener info del propietario con el inmueble
	}

	// POST: Inmuebles/
	[HttpPost]
	[Authorize]
	public IActionResult GetInmuebles()
	{
		try
		{
			var usuario = User.Identity != null
				? _context.Propietarios.Find(Int32.Parse(User.FindFirstValue("Id")))
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
			var usuario = User.Identity != null
				? _context.Propietarios.Find(Int32.Parse(User.FindFirstValue("Id")))
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

}
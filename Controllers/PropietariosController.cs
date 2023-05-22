/*

Un regalito de mi amigo 💖🤗:

eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoidGVzdDRAZ21haWwuY29tIiwiRnVsbE5hbWUiOiJFbHBybyBQaWV0YXJpbyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlByb3BpZXRhcmlvIiwiZXhwIjoxNjgzODM3OTQ1LCJpc3MiOiJpbm1vYmlsaWFyaWFkb3ROZXQiLCJhdWQiOiJtb2JpbGVBUFAifQ.tZI_nNjB83453ZpmU6njf0aqa78py2bh-5ee_f9GTPo

*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InmobiliariaAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaAPI.Controllers;

[ApiController]
[Route("[Controller]")]
public class PropietariosController : ControllerBase
{
	private readonly DataContext _context;
	private readonly IConfiguration _config;
	private string hashSalt = "";
	public PropietariosController(DataContext context, IConfiguration config)
	{
		_context = context;
		_config = config;
		hashSalt = _config["Salt"] ?? "";
	}

	// GET: Propietarios/ (todos los propietarios)
	[HttpGet()]
	public IEnumerable<Propietario> Get()
	{
		return _context.Propietarios;
	}

	// GET: Propietarios/:id (un propietario solo)
	[HttpGet("{id}")]
	public IActionResult Get(int id)
	{
		var propietario = _context.Propietarios.Find(id);
		if (propietario == null) return NotFound();
		return Ok(propietario);
	}

	// PUT: Propietarios/HashPasswords
	// [HttpPut("HashPasswords")]
	// public IActionResult HashPasswords()
	// {
	// 	// Obtener todos los propietarios
	// 	var propietarios = _context.Propietarios.ToList();

	// 	// Hashear el DNI y actualizarlo como contraseña
	// 	foreach (var propietario in propietarios)
	// 	{
	// 		string hashedDni = Convert.ToBase64String(KeyDerivation.Pbkdf2(
	// 			password: propietario.Dni,
	// 			salt: System.Text.Encoding.ASCII.GetBytes(hashSalt),
	// 			prf: KeyDerivationPrf.HMACSHA1,
	// 			iterationCount: 10000,
	// 			numBytesRequested: 256 / 8
	// 		));

	// 		// Actualizar contraseña
	// 		propietario.Password = hashedDni;

	// 		// Update the Propietario object in the database
	// 		_context.Entry(propietario).State = EntityState.Modified;
	// 	}

	// 	// Guardar los cambios
	// 	_context.SaveChanges();

	// 	return Ok();
	// }

	// POST: Propietarios/Login
	[HttpPost("Login")]
	// public IActionResult Login([FromForm] LoginView loginView)
	public IActionResult Login(LoginView loginView)
	{
		var propietario = _context.Propietarios.FirstOrDefault(x => x.Correo == loginView.Correo);

		if (propietario == null) return NotFound();

		string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
			password: loginView.Password,
			salt: System.Text.Encoding.ASCII.GetBytes(hashSalt),
			prf: KeyDerivationPrf.HMACSHA1,
			iterationCount: 10000,
			numBytesRequested: 256 / 8
		));

		if (hashed != propietario.Password) return BadRequest("Dato incorrecto");

		string secretKey = _config["TokenAuthentication:SecretKey"] ?? throw new ArgumentNullException(nameof(secretKey));
		var securityKey = secretKey != null ? new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey)) : null;
		var credenciales = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, propietario.Correo),
			new Claim("Id", propietario.Id_Propietario.ToString())
		};

		var token = new JwtSecurityToken(
			issuer: _config["TokenAuthentication:Issuer"],
			audience: _config["TokenAuthentication:Audience"],
			claims: claims,
			expires: DateTime.Now.AddMinutes(60),
			signingCredentials: credenciales
		);

		return Ok(new JwtSecurityTokenHandler().WriteToken(token));
	}

	// GET: Popietarios/Perfil
	[HttpGet("Perfil")]
	[Authorize]
	public IActionResult Perfil()
	{
		var propietario = User.Identity != null
			? _context.Propietarios
				.Where(x => x.Correo == User.Identity.Name)
				.Select(x => new PropietarioView(x))
				.FirstOrDefault()
			: null;

		if (propietario == null)
		{
			return NotFound();
		}

		return Ok(propietario);
	}

	// POST: Propietarios/Edit
	[HttpPost("Edit")]
	[Authorize]
	public IActionResult Edit(EditView propietario)
	{
		// var propietarioDb = User.Identity != null
		// 	? _context.Propietarios.Find(Int32.Parse((User.Claims.FirstOrDefault(c => c.Type == "Id").Value)))
		// 	: null;
		var propietarioDb = User.Identity != null
			? _context.Propietarios.Find(User.FindFirstValue("Id"))
			: null;

		if (propietarioDb == null) return NotFound();

		if (propietario.Id_Propietario != propietarioDb.Id_Propietario) return BadRequest();

		if (
			string.IsNullOrEmpty(propietario.Dni) ||
			string.IsNullOrEmpty(propietario.Nombre) ||
			string.IsNullOrEmpty(propietario.Apellido) ||
			string.IsNullOrEmpty(propietario.Correo) ||
			string.IsNullOrEmpty(propietario.Telefono)
		)
		{
			return BadRequest("Nungun campo puede ser vacio");
		}

		propietarioDb.Dni = propietario.Dni;
		propietarioDb.Nombre = propietario.Nombre;
		propietarioDb.Apellido = propietario.Apellido;
		propietarioDb.Correo = propietario.Correo;
		propietarioDb.Telefono = propietario.Telefono;

		_context.Propietarios.Update(propietarioDb);
		_context.SaveChanges();

		return Ok(propietario);
	}



}


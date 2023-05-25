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

	// POST: Propietarios/Login
	[HttpPost("Login")]
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
		try
		{
			int.TryParse(User.FindFirstValue("Id"), out int userId);
			var propietarioDb = User.Identity != null
				? _context.Propietarios.Find(userId)
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
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
	}



}
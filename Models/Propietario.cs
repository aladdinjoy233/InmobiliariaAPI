using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.Models;

public class Propietario
{
	[Key]
	public int Id_Propietario { get; set; }
	public string Dni { get; set; } = "";
	public string Nombre { get; set; } = "";
	public string Apellido { get; set; } = "";
	public string Correo { get; set; } = "";
	public string Telefono { get; set; } = "";
	public string Password { get; set; } = "";
}
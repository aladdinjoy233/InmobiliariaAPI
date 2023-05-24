using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.Models;

public class Inquilino
{
	[Key]
	public int Id_Inquilino { get; set; }
	public string Dni { get; set; } = "";
	public string Nombre { get; set; } = "";
	public string Apellido { get; set; } = "";
	public string ? Correo { get; set; }
	public string Telefono { get; set; } = "";
}
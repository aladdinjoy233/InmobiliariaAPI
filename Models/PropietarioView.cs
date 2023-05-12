using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAPI.Models;

public class PropietarioView
{
	[Key]
	public int Id_Propietario { get; set; }
	public string Dni { get; set; } = "";
	public string Nombre { get; set; } = "";
	public string Apellido { get; set; } = "";
	public string Correo { get; set; } = "";
	public string Telefono { get; set; } = "";

	public PropietarioView() {}

	public PropietarioView(Propietario propietario)
	{
		Id_Propietario = propietario.Id_Propietario;
		Dni = propietario.Dni;
		Nombre = propietario.Nombre;
		Apellido = propietario.Apellido;
		Correo = propietario.Correo;
		Telefono = propietario.Telefono;
	}
}
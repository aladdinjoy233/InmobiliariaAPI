namespace InmobiliariaAPI.Models;

public class EditView
{
	public int Id_Propietario { get; set; }
	public string Dni { get; set; } = "";
	public string Nombre { get; set; } = "";
	public string Apellido { get; set; } = "";
	public string Correo { get; set; } = "";
	public string Telefono { get; set; } = "";
}
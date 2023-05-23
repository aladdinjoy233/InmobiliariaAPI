using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAPI.Models;

public enum enUsos
{
	Residencial = 1,
	Comercial = 2,
	Industrial = 3,
	Vacacional = 4,
	Educativo = 5,
	Deportivo = 6,
	Sanitario = 7,
	Cultural = 8
}

public enum enTipos
{
	Casa = 1,
	Departamento = 2,
	Oficina = 3,
	Local = 4,
	Terreno = 5,
	Galpon = 6,
	Edificio = 7,
	Hotel = 8,
	Quinta = 9
}

public class Inmueble
{
	[Key]
	public int Id_Inmueble { get; set; }

	[ForeignKey(nameof(Propietario))]
	public int Id_Propietario { get; set; }
	public Propietario Propietario { get; set; } = null!;

	public string Direccion { get; set; } = "";
	public int Uso { get; set; }
	public int Tipo { get; set; }
	public int ? Ambientes { get; set; }
	public decimal ? Latitud { get; set; }
	public decimal ? Longitud { get; set; }
	public decimal ? Precio { get; set; }
	public bool Activo { get; set; }
	public String Imagen { get; set; } = "";

	public string UsoNombre => Uso > 0 ? ((enUsos)Uso).ToString() : "";
	public string TipoNombre => Tipo !> 0 ? ((enTipos)Tipo).ToString() : "";

	public static IDictionary<int, string> ObtenerUsos()
	{
		SortedDictionary<int, string> usos = new SortedDictionary<int, string>();
		Type tipoEnumUso = typeof(enUsos);
		foreach (var valor in Enum.GetValues(tipoEnumUso))
		{
			usos.Add((int)valor, Enum.GetName(tipoEnumUso, valor) ?? "");
		}
		return usos;
	}

	public static IDictionary<int, string> ObtenerTipos()
	{
		SortedDictionary<int, string> tipos = new SortedDictionary<int, string>();
		Type tipoEnumTipo = typeof(enTipos);
		foreach (var valor in Enum.GetValues(tipoEnumTipo))
		{
			tipos.Add((int)valor, Enum.GetName(tipoEnumTipo, valor) ?? "");
		}
		return tipos;
	}
}
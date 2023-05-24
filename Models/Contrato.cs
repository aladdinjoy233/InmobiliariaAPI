using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAPI.Models;

public class Contrato
{
	[Key]
	public int Id_Contrato { get; set; }

	[ForeignKey(nameof(Inmueble))]
	public int Id_Inmueble { get; set; }
	public Inmueble Inmueble { get; set; } = null!;

	[ForeignKey(nameof(Inquilino))]
	public int Id_Inquilino { get; set; }
	public Inquilino Inquilino { get; set; } = null!;

	public DateTime Fecha_Inicio { get; set; }
	public DateTime Fecha_Fin { get; set; }
	public decimal Monto_Mensual { get; set; }
	public bool Activo { get; set; }
}
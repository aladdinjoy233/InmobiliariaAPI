using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAPI.Models;

public class Pago
{
	[Key]
	public int Id_Pago { get; set; }

	[ForeignKey(nameof(Contrato))]
	public int Id_Contrato { get; set; }
	public Contrato Contrato { get; set; } = null!;
	public int Numero { get; set; }
	public DateTime Fecha { get; set; }
	public decimal Importe { get; set; }

}
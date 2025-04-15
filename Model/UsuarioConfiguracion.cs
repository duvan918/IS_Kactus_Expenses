using System.ComponentModel.DataAnnotations;

namespace IS_Kactus_Expenses.Model;

public class UsuarioConfiguracion
{
    public int IdConfiguracion { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdCompania { get; set; }

    public string? TipoDocumento { get; set; }

    public string? CentroOperacion { get; set; }

    public string? Servicios { get; set; }

    public string? UnidadNegocio { get; set; }

    public string? IdCondicionPago { get; set; }

    public string? Motivo { get; set; }

    public string? TipoProveedor { get; set; }

    public string? CentroCostos { get; set; }

    public string? Moneda { get; set; }
}

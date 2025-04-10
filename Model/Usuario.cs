namespace IS_Kactus_Expenses.Model
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        public int? IdCompania { get; set; }

        public int? IdPerfil { get; set; }

        public string? Nit { get; set; }

        public string? Clave { get; set; }

        public string? Cedula { get; set; }

        public string? RazonSocial { get; set; }

        public string? NombreCompleto { get; set; }

        public string? CiudadBase { get; set; }

        public string? Direccion { get; set; }

        public string? Celular { get; set; }

        public string? Correo { get; set; }

        public string? Ipuser { get; set; }

        public bool? BitEstado { get; set; }

        public string? Observaciones { get; set; }

        public bool? BitTercero { get; set; }

        public int? Cupo { get; set; }

        public int? Grupo { get; set; }

        public bool? BitAprobacion { get; set; }

        public int? IdPadrino { get; set; }

        public DateTime? Ts { get; set; }

        public bool? Activo { get; set; }

        public DateTime? TsActivo { get; set; }
    }
}

namespace ApiLogros.Model
{
    public class HistorialPuntos
    {
        public int IdHistorial { get; set; }

        public int UsuarioId { get; set; }

        public int Puntos { get; set; }

        public string TipoMovimiento { get; set; }

        public string Descripcion { get; set; }

        public DateTime Fecha { get; set; }
    }
}

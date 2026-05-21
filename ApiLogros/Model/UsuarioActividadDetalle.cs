namespace ApiLogros.Model
{
    public class UsuarioActividadDetalle
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public int Puntos { get; set; }

        public DateTime FechaAsignacion { get; set; }

        public bool Completada { get; set; }

        public DateTime? FechaCompletada { get; set; }
    }
}

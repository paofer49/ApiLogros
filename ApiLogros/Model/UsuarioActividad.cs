namespace ApiLogros.Model
{
    public class UsuarioActividad
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public int ActividadId { get; set; }

        public DateTime FechaAsignacion { get; set; }

        public bool Completada { get; set; }

        public DateTime? FechaCompletada { get; set; }
    }
}

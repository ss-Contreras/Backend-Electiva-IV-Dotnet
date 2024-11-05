using SonrisasBackendv01.Models;

namespace SonrisasBackendv01.Dtos
{
    public class LeerPacienteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public int Edad { get; set; }
        public DateTime FechaCita { get; set; }
        public OpcionBinaria NuevoPaciente { get; set; }
        public OpcionBinaria PacienteRecomendado { get; set; }
        public string MotivoConsulta { get; set; }
        public string RutaImagen { get; set; }
        public string RutaLocalImagen { get; set; }
        public string Telefono { get; set; }
        public string CorreoElectronico { get; set; }
        public string Direccion { get; set; }

        // Odontólogo asignado
        public string NombreOdontologo { get; set; }
    }
}

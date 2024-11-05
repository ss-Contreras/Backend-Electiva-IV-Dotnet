using SonrisasBackendv01.Models;

namespace SonrisasBackendv01.Dtos
{
    public class ResumenPacienteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public int Edad { get; set; }
        public DateTime FechaCita { get; set; }
        public OpcionBinaria NuevoPaciente { get; set; }
    }
}

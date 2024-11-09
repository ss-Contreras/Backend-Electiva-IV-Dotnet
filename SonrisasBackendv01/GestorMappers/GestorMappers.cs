using AutoMapper;
using SonrisasBackendv01.Dtos;
using SonrisasBackendv01.DTOs;
using SonrisasBackendv01.Models;
using SonrisasBackendv01.Models.Dtos;

namespace SonrisasBackendv01.GestorMappers
{
    public class GestorMappers : Profile
    {
        public GestorMappers()
        {
            CreateMap<Paciente, ActualizarPacienteDto>().ReverseMap();
            CreateMap<Paciente, CrearPacienteDto>().ReverseMap();
			CreateMap<Paciente, LeerPacienteDto>()
		   .ForMember(dest => dest.CorreoElectronico, opt => opt.MapFrom(src => src.Email))
		   .ForMember(dest => dest.NombreOdontologo, opt => opt.MapFrom(src => src.Odontologo.Nombre));
			CreateMap<Paciente, ResumenPacienteDto>().ReverseMap();

            CreateMap<Odontologo, OdontologoDto>().ReverseMap();
            CreateMap<Odontologo, CrearOdontologoDto>().ReverseMap();

            CreateMap<Consultorio, ConsultorioDto>().ReverseMap();
            CreateMap<Consultorio, CrearConsultorioDto>().ReverseMap();

			CreateMap<Historial, HistorialDto>()
				.ForMember(dest => dest.NombrePaciente, opt => opt.MapFrom(src => src.Paciente.Nombre))
				.ForMember(dest => dest.NombreOdontologo, opt => opt.MapFrom(src => src.Odontologo.Nombre));
			CreateMap<Historial, CrearHistorialDto>().ReverseMap();

			CreateMap<Cita, CitasDto>()
				.ForMember(dest => dest.NombrePaciente, opt => opt.MapFrom(src => src.Paciente.Nombre))
				.ForMember(dest => dest.NombreOdontologo, opt => opt.MapFrom(src => src.Odontologo.Nombre));
			CreateMap<Cita, LeerCitaDto>()
				.ForMember(dest => dest.NombrePaciente, opt => opt.MapFrom(src => src.Paciente.Nombre))
				.ForMember(dest => dest.CorreoElectronicoPaciente, opt => opt.MapFrom(src => src.Paciente.Email)) 
				.ForMember(dest => dest.NombreOdontologo, opt => opt.MapFrom(src => src.Odontologo.Nombre));
			CreateMap<CrearCitaDto, Cita>();
			CreateMap<ActualizarCitaDto, Cita>();

			CreateMap<Radiografia, LeerRadiografiaDto>()
							.ForMember(dest => dest.NombrePaciente, opt => opt.MapFrom(src => $"{src.Paciente.Nombre}"))
							.ForMember(dest => dest.CorreoElectronicoPaciente, opt => opt.MapFrom(src => src.Paciente.Email));

			CreateMap<CrearRadiografiaDto, Radiografia>();
			CreateMap<ActualizarRadiografiaDto, Radiografia>();
		}
    }
}

using AutoMapper;
using CompanyApp.DTO;
using CompanyApp.Models;

namespace CompanyApp.Helper {
	public class MappingProfiles : Profile {
        public MappingProfiles() {
			CreateMap<Truck, TruckDTO>().ReverseMap();
		}
    }
}

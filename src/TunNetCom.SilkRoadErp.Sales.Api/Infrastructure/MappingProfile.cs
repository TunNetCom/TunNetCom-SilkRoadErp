using AutoMapper;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Facture, InvoiceResponse>();
        }
    }
}

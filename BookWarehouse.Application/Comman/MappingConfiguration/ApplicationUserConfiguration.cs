using BookWarehouse.Domain.Entities;
using Ecom.BLL.ViewModel.Authentication;
using Mapster;

namespace BookWarehouse.Application.Comman.MappingConfiguration
{
    public class ApplicationUserConfigurationL : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<RegisterVM, ApplicationUser>()
                .Map(dest => dest.UserName, src => src.Email);
        }
    }
}

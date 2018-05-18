namespace BizyBoard.Models.Mappings
{
    using AutoMapper;
    using Bizy.OuinneBiseSharp.Extensions;
    using DbEntities;
    using ViewModels;

    public class ViewModelToEntityMappingProfile : Profile
    {
        public ViewModelToEntityMappingProfile()
        {
            CreateMap<RegistrationViewModel, AppUser>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email))
                .ForMember(au => au.ErpPassword,
                    map => map.MapFrom(vm =>
                        vm.WinBizPassword.Encrypt(
                            "BgIAAACkAABSU0ExAAQAAAEAAQBZ3myd6ZQA0tUXZ3gIzu1sQ7larRfM5KFiYbkgWk+jw2VEWpxpNNfDw8M3MIIbbDeUG02y/ZW+XFqyMA/87kiGt9eqd9Q2q3rRgl3nWoVfDnRAPR4oENfdXiq5oLW3VmSKtcBl2KzBCi/J6bbaKmtoLlnvYMfDWzkE3O1mZrouzA==")));
        }
    }
}
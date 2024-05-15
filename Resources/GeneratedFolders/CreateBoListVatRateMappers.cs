using AutoMapper;

namespace AccountStructureManagementManagement.Application.Commands.BoListVatRateCommands.CreateBoListVatRate;
public class BoListVatRateMappers : Profile
{
    public BoListVatRateMappers()
    {
        this.CreateMap<CreateBoListVatRateCommand, BoListVatRate>();
        this.CreateMap<BoListVatRate, CreateBoListVatRateCommandResult>();
    }
}

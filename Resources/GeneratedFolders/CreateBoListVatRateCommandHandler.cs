using AutoMapper;
using Sodexo.BackOffice.Abstraction.Commands;
using Sodexo.BackOffice.Abstraction.Data;
using System.Threading;
using System.Threading.Tasks;

namespace AccountStructureManagementManagement.Application.Commands.BoListVatRateCommands.CreateBoListVatRate;
public class BoListVatRateCreateCommandHandler : ICommandHandler<CreateBoListVatRateCommand, CreateBoListVatRateCommandResult>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public BoListVatRateCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<CreateBoListVatRateCommandResult> Handle(CreateBoListVatRateCommand command, CancellationToken cancellationToken)
    {
        var entity = this.mapper.Map<BoListVatRate>(command);
         // Pk değeri Sequence ile besleniyorsa aşağıdaki satır aktif edilmeli. Eğer AutoIncremented ise aşağıdaki satır silinmeli
        //entity.TrSeqId = await this.unitOfWork.Database.ExecuteNextValAsync("BOLISTVATRATE_ID").ConfigureAwait(false);

        #region Before Trigger Operations
        #endregion

        #region Entity Operations
        entity.SetTriggerConversionStatus(TriggerConversionStatus.Before);
        await this.unitOfWork.GetCommandRepository<BoListVatRate>().AddAsync(entity).ConfigureAwait(false);
        #endregion

        #region After Trigger Operations
        #endregion

        return this.mapper.Map<CreateBoListVatRateCommandResult>(entity);
    }
}

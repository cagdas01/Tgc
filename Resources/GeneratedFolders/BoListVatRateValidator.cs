using FluentValidation;

namespace AccountStructureManagementManagement.Application.Commands.BoListVatRateCommands.CreateBoListVatRate;
public class CreateBoListVatRateValidator : BaseValidator<CreateBoListVatRateCommand>
{
    public CreateBoListVatRateValidator()
    {
        RuleFor(x => x.TrSeqId).NotNull();
        RuleFor(x => x.VatRateId).NotNull();
        RuleFor(x => x.Vat).NotNull();
        RuleFor(x => x.Definition).NotNull();
        RuleFor(x => x.Definition).MaximumLength(30);
        RuleFor(x => x.Active).NotNull();
        RuleFor(x => x.NewVatDate).NotNull();
        RuleFor(x => x.VatNo).NotNull();
        RuleFor(x => x.OpType).NotNull();
        RuleFor(x => x.OpDate).NotNull();
        RuleFor(x => x.OpuCode).NotNull();
        RuleFor(x => x.OpuCode).MaximumLength(14);
        RuleFor(x => x.TrOpType).NotNull();
        RuleFor(x => x.TrDate).NotNull();
        RuleFor(x => x.TrStatus).NotNull();
        RuleFor(x => x.TrFileName).NotNull();
        RuleFor(x => x.TrFileName).MaximumLength(50);
        RuleFor(x => x.SourceType).NotNull();
        RuleFor(x => x.SourceType).MaximumLength(1);
    }
}

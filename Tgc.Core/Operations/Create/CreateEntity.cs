using System.Text;

namespace Tgc.Core.Operations.Create
{
    public class CreateEntity : CreateEntityTrigger
    {
        protected override string BuildCommandHandlerClass()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using AutoMapper;");
            sb.AppendLine("using Sodexo.BackOffice.Abstraction.Commands;");
            sb.AppendLine("using Sodexo.BackOffice.Abstraction.Data;");
            sb.AppendLine("using Sodexo.BackOffice.AccountStructureManagement.Domain;");
            sb.AppendLine("using Sodexo.BackOffice.AccountStructureManagement.Infrastructure;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"{CommandNameSpace}");
            sb.AppendLine();
            sb.AppendLine($"public class {CommandType}{EntityName}CommandHandler : ICommandHandler<{CommandType}{EntityName}Command, {CommandType}{EntityName}CommandResult>");
            sb.AppendLine("{");
            sb.AppendLine($"    private readonly IUnitOfWork<{Context}> unitOfWork;");
            sb.AppendLine("    private readonly IMapper mapper;");
            sb.AppendLine();
            sb.AppendLine($"    public {CommandType}{EntityName}CommandHandler(IUnitOfWork<{Context}> unitOfWork, IMapper mapper)");
            sb.AppendLine("    {");
            sb.AppendLine("        this.unitOfWork = unitOfWork;");
            sb.AppendLine("        this.mapper = mapper;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine($"    public async Task<{CommandType}{EntityName}CommandResult> Handle({CommandType}{EntityName}Command command, CancellationToken cancellationToken)");
            sb.AppendLine("    {");
            sb.AppendLine($"        var entity = this.mapper.Map<{EntityName}>(command);");
            sb.AppendLine($"        await this.unitOfWork.GetCommandRepository<{EntityName}>().AddAsync(entity).ConfigureAwait(false);");
            sb.AppendLine($"        return this.mapper.Map<{CommandType}{EntityName}CommandResult>(entity);");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
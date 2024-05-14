using System.Text;
using Tgc.Core.Constants;

namespace Tgc.Core.Operations.Update
{
    public class UpdateEntity : UpdateEntityTrigger
    {
        protected override string BuildCommandHandlerClass()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using AutoMapper;");
            sb.AppendLine("using Sodexo.BackOffice.Abstraction.Commands;");
            sb.AppendLine("using Sodexo.BackOffice.Abstraction.Data;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"namespace {moduleName}{StringConstants.Management}.Application.Commands.{entityName}Commands.Update{entityName};");
            sb.AppendLine($"public class Update{entityName}MinimalCommandHandler : ICommandHandler<Update{entityName}Command, Update{entityName}CommandResult>");
            sb.AppendLine("{");
            sb.AppendLine($"    private readonly IUnitOfWork<{moduleName}{StringConstants.Context}> unitOfWork;");
            sb.AppendLine("    private readonly IMapper mapper;");
            sb.AppendLine("    private readonly ICommandSender commandSender;");
            sb.AppendLine();
            sb.AppendLine($"    public Update{entityName}MinimalCommandHandler(IUnitOfWork<{moduleName}{StringConstants.Context}> unitOfWork, IMapper mapper, ICommandSender commandSender)");
            sb.AppendLine("    {");
            sb.AppendLine("        this.unitOfWork = unitOfWork;");
            sb.AppendLine("        this.mapper = mapper;");
            sb.AppendLine("        this.commandSender = commandSender;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine($"    public async Task<Update{entityName}CommandResult> Handle(Update{entityName}Command command, CancellationToken cancellationToken)");
            sb.AppendLine("    {");
            sb.AppendLine($"        var repo = this.unitOfWork.GetCommandRepository<{entityName}>();");
            sb.AppendLine($"        var entity = await repo.FindAsync(x => x.{entityName}Id == command.{entityName}Id).ConfigureAwait(false);");
            sb.AppendLine();
            sb.AppendLine("        if (entity == null)");
            sb.AppendLine("        {");

            sb.AppendLine($"            {moduleName}{StringConstants.Management}ErrorCode.{entityName}NotFound.Throw(command.{entityName}Id);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        this.mapper.Map(command, entity);");
            sb.AppendLine("        await repo.UpdatePartialAsync(entity).ConfigureAwait(false);");
            sb.AppendLine();
            sb.AppendLine($"        return this.mapper.Map<Update{entityName}CommandResult>(entity);");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
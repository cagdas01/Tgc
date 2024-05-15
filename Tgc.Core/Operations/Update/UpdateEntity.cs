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
            sb.AppendLine($"{CommandNameSpace}");
            sb.AppendLine();
            sb.AppendLine($"public class {CommandType}{EntityName}CommandHandler : ICommandHandler<{CommandType}{EntityName}Command, {CommandType}{EntityName}CommandResult>");
            sb.AppendLine("{");
            sb.AppendLine($"    private readonly IUnitOfWork<{Context}> unitOfWork;");
            sb.AppendLine("    private readonly IMapper mapper;");
            sb.AppendLine("    private readonly ICommandSender commandSender;");
            sb.AppendLine();
            sb.AppendLine($"    public {CommandType}{EntityName}CommandHandler(IUnitOfWork<{Context}> unitOfWork, IMapper mapper, ICommandSender commandSender)");
            sb.AppendLine("    {");
            sb.AppendLine("        this.unitOfWork = unitOfWork;");
            sb.AppendLine("        this.mapper = mapper;");
            sb.AppendLine("        this.commandSender = commandSender;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine($"    public async Task<{CommandType}{EntityName}CommandResult> Handle({CommandType}{EntityName}Command command, CancellationToken cancellationToken)");
            sb.AppendLine("    {");
            sb.AppendLine($"        var repo = this.unitOfWork.GetCommandRepository<{EntityName}>();");
            sb.AppendLine($"        var entity = await repo.FindAsync(x => x.{PrimaryKey} == command.{PrimaryKey}, cancellationToken: cancellationToken).ConfigureAwait(false);");
            sb.AppendLine();
            sb.AppendLine("        if (entity == null)");
            sb.AppendLine("        {");

            sb.AppendLine($"            {ModuleName}ErrorCode.{EntityName}NotFound.Throw(command.{PrimaryKey});");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        this.mapper.Map(command, entity);");
            sb.AppendLine("        await repo.UpdatePartialAsync(entity).ConfigureAwait(false);");
            sb.AppendLine();
            sb.AppendLine($"        return this.mapper.Map<{CommandType}{EntityName}CommandResult>(entity);");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
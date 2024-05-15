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
            sb.AppendLine($"public class {CommandType}{entityName}CommandHandler : ICommandHandler<{CommandType}{entityName}Command, {CommandType}{entityName}CommandResult>");
            sb.AppendLine("{");
            sb.AppendLine($"    private readonly IUnitOfWork<{context}> unitOfWork;");
            sb.AppendLine("    private readonly IMapper mapper;");
            sb.AppendLine("    private readonly ICommandSender commandSender;");
            sb.AppendLine();
            sb.AppendLine($"    public {CommandType}{entityName}CommandHandler(IUnitOfWork<{context}> unitOfWork, IMapper mapper, ICommandSender commandSender)");
            sb.AppendLine("    {");
            sb.AppendLine("        this.unitOfWork = unitOfWork;");
            sb.AppendLine("        this.mapper = mapper;");
            sb.AppendLine("        this.commandSender = commandSender;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine($"    public async Task<{CommandType}{entityName}CommandResult> Handle({CommandType}{entityName}Command command, CancellationToken cancellationToken)");
            sb.AppendLine("    {");
            sb.AppendLine($"        var repo = this.unitOfWork.GetCommandRepository<{entityName}>();");
            sb.AppendLine($"        var entity = await repo.FindAsync(x => x.{primaryKey} == command.{primaryKey}, cancellationToken: cancellationToken).ConfigureAwait(false);");
            sb.AppendLine();
            sb.AppendLine("        if (entity == null)");
            sb.AppendLine("        {");

            sb.AppendLine($"            {moduleName}ErrorCode.{entityName}NotFound.Throw(command.{primaryKey});");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        this.mapper.Map(command, entity);");
            sb.AppendLine("        await repo.UpdatePartialAsync(entity).ConfigureAwait(false);");
            sb.AppendLine();
            sb.AppendLine($"        return this.mapper.Map<{CommandType}{entityName}CommandResult>(entity);");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
using System.Text;
using Tgc.Core.Base;
using Tgc.Core.Constants;

namespace Tgc.Operations.Update;

public class UpdateEntityTrigger : CqrsBase
{
    protected override string GetNamespacePrefix()
    {
        return $"{moduleName}{StringConstants.Management}.Application.Commands.{entityName}Commands.Update{entityName}";
    }
    protected override string GetFileNameSuffix()
    {
        return "Update";
    }
    protected override string BuildCommandClass()
    {
        var excludedProperties = new HashSet<string>(new[] { "TriggerConversionStatus" }, StringComparer.OrdinalIgnoreCase);

        var sb = new StringBuilder();
        sb.AppendLine("using Sodexo.BackOffice.Abstraction.Commands;");
        sb.AppendLine();
        sb.AppendLine($"namespace {GetNamespacePrefix()};");
        sb.AppendLine($"public class Update{entityName}Command : CommandBase<Update{entityName}CommandResult>");
        sb.AppendLine("{");

        foreach (var prop in properties)
        {
            if (!excludedProperties.Contains(prop.Key))
            {
                string nullableAnnotation = prop.Value.isRequired ? "" : "?";
                sb.AppendLine($"    public {prop.Value.type}{nullableAnnotation} {prop.Key} {{ get; set; }}");
            }
        }

        sb.AppendLine("}");
        return sb.ToString();
    }
    protected override string BuildCommandHandlerClass()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using AutoMapper;");
        sb.AppendLine("using Sodexo.BackOffice.Abstraction.Commands;");
        sb.AppendLine("using Sodexo.BackOffice.Abstraction.Data;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine("using System.Threading;");
        sb.AppendLine("using System.Threading.Tasks;");
        sb.AppendLine();
        sb.AppendLine($"namespace {GetNamespacePrefix()};");
        sb.AppendLine($"public class Update{entityName}CommandHandler : ICommandHandler<Update{entityName}Command, Update{entityName}CommandResult>");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly IUnitOfWork<{moduleName}{StringConstants.Context}> unitOfWork;");
        sb.AppendLine("    private readonly IMapper mapper;");
        sb.AppendLine();
        sb.AppendLine($"    public Update{entityName}CommandHandler(IUnitOfWork<{moduleName}{StringConstants.Context}> unitOfWork, IMapper mapper)");
        sb.AppendLine("    {");
        sb.AppendLine("        this.unitOfWork = unitOfWork;");
        sb.AppendLine("        this.mapper = mapper;");
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
        sb.AppendLine("        #region Before Trigger Operations");
        sb.AppendLine("        // Optional actions before update");
        sb.AppendLine("        #endregion");
        sb.AppendLine();
        sb.AppendLine("        #region Entity Operations");
        sb.AppendLine("        this.mapper.Map(command, entity);");
        sb.AppendLine("        await repo.UpdateAsync(entity).ConfigureAwait(false);");
        sb.AppendLine("        #endregion");
        sb.AppendLine();
        sb.AppendLine("        #region After Trigger Operations");
        sb.AppendLine("        // Optional actions after update");
        sb.AppendLine("        #endregion");
        sb.AppendLine();
        sb.AppendLine($"        return this.mapper.Map<Update{entityName}CommandResult>(entity);");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }
    protected override string BuildCommandResultClass()
    {
        var excludedProperties = new HashSet<string>(new string[] { "TriggerConversionStatus" }, StringComparer.OrdinalIgnoreCase);

        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine($"namespace {GetNamespacePrefix()};");
        sb.AppendLine($"public class Update{entityName}CommandResult");
        sb.AppendLine("{");

        foreach (var prop in properties)
        {
            if (!excludedProperties.Contains(prop.Key))
            {
                string nullableAnnotation = prop.Value.isRequired ? "" : "?";
                sb.AppendLine($"    public {prop.Value.type}{nullableAnnotation} {prop.Key} {{ get; set; }}");
            }
        }

        sb.AppendLine("}");
        return sb.ToString();
    }
    protected override string BuildMapperClass()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using AutoMapper;");
        sb.AppendLine();
        sb.AppendLine($"namespace {GetNamespacePrefix()};");
        sb.AppendLine($"public class {entityName}Mappers : Profile");
        sb.AppendLine("{");
        sb.AppendLine($"    public {entityName}Mappers()");
        sb.AppendLine("    {");
        sb.AppendLine($"        CreateMap<Update{entityName}Command, {entityName}>();");
        sb.AppendLine($"        CreateMap<{entityName}, Update{entityName}CommandResult>();");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }
}
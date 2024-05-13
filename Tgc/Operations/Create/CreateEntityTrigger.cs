using System;
using System.Text;
using System.Threading.Tasks;
using Tgc.Core.Base;
using Tgc.Core.Constants;

namespace Tgc.Operations.Create;

public class CreateEntityTrigger : CqrsBase
{
    protected override string GetNamespacePrefix()
    {
        return $"{moduleName}{StringConstants.Management}.Application.Commands.{entityName}Commands.Create{entityName}";
    }

    protected override string GetFileNameSuffix()
    {
        return "Create";
    }

    protected override string BuildCommandClass()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using Sodexo.BackOffice.Abstraction.Commands;");
        sb.AppendLine();
        sb.AppendLine($"namespace {GetNamespacePrefix()};");
        sb.AppendLine($"public class Create{entityName}Command : CommandBase<Create{entityName}CommandResult>");
        sb.AppendLine("{");
        foreach (var prop in properties)
        {
            if (!prop.Key.Equals($"{entityName}Id", StringComparison.OrdinalIgnoreCase))
            {
                var nullableAnnotation = prop.Value.isRequired ? "" : "?";
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
        sb.AppendLine("using System.Threading;");
        sb.AppendLine("using System.Threading.Tasks;");
        sb.AppendLine();
        sb.AppendLine($"namespace {GetNamespacePrefix()};");
        sb.AppendLine($"public class {entityName}CreateCommandHandler : ICommandHandler<Create{entityName}Command, Create{entityName}CommandResult>");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly IUnitOfWork unitOfWork;");
        sb.AppendLine("    private readonly IMapper mapper;");
        sb.AppendLine();
        sb.AppendLine($"    public {entityName}CreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)");
        sb.AppendLine("    {");
        sb.AppendLine("        this.unitOfWork = unitOfWork;");
        sb.AppendLine("        this.mapper = mapper;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public async Task<Create{entityName}CommandResult> Handle(Create{entityName}Command command, CancellationToken cancellationToken)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var entity = this.mapper.Map<{entityName}>(command);");
        sb.AppendLine($"        entity.{entityName}Id = await this.unitOfWork.Database.ExecuteNextValAsync(\"{entityName.ToUpper()}_ID\").ConfigureAwait(false);");
        sb.AppendLine();
        sb.AppendLine("        #region Before Trigger Operations");
        sb.AppendLine("        // Add any specific actions before the entity is added");
        sb.AppendLine("        #endregion");
        sb.AppendLine();
        sb.AppendLine("        #region Entity Operations");
        sb.AppendLine($"        entity.SetTriggerConversionStatus(TriggerConversionStatus.Before);");
        sb.AppendLine($"        await this.unitOfWork.GetCommandRepository<{entityName}>().AddAsync(entity).ConfigureAwait(false);");
        sb.AppendLine("        #endregion");
        sb.AppendLine();
        sb.AppendLine("        #region After Trigger Operations");
        sb.AppendLine("        // Add any specific actions after the entity is added");
        sb.AppendLine("        #endregion");
        sb.AppendLine($"        return this.mapper.Map<Create{entityName}CommandResult>(entity);");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    protected override string BuildCommandResultClass()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine($"namespace {GetNamespacePrefix()};");
        sb.AppendLine($"public class Create{entityName}CommandResult");
        sb.AppendLine("{");
        foreach (var prop in properties)
        {
            string nullableAnnotation = prop.Value.isRequired ? "" : "?";
            sb.AppendLine($"    public {prop.Value.type}{nullableAnnotation} {prop.Key} {{ get; set; }}");
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
        sb.AppendLine($"        CreateMap<Create{entityName}Command, {entityName}>();");
        sb.AppendLine($"        CreateMap<{entityName}, Create{entityName}CommandResult>();");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private string BuildValidatorClass()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using FluentValidation;");
        sb.AppendLine();
        sb.AppendLine($"namespace {GetNamespacePrefix()};");
        sb.AppendLine($"public class Create{entityName}Validator : BaseValidator<Create{entityName}Command>");
        sb.AppendLine("{");
        sb.AppendLine($"    public Create{entityName}Validator()");
        sb.AppendLine("    {");
        foreach (var prop in properties)
        {
            sb.AppendLine($"        RuleFor(x => x.{prop.Key}).NotNull();");
            if (prop.Value.maxLength > 0)
            {
                sb.AppendLine($"        RuleFor(x => x.{prop.Key}).MaximumLength({prop.Value.maxLength});");
            }
        }
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    public override async Task Process(string filePath)
    {
        await base.Process(filePath);
        CreateFile($"{entityName}Validator.cs", BuildValidatorClass());
    }
}


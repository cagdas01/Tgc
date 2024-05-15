using System;
using System.Collections.Generic;
using System.Text;
using Tgc.Core.Base;

namespace Tgc.Core.Operations.Update
{
    public class UpdateEntityTrigger : CqrsBase
    {        
        protected override string CommandType { get; set; } = "Update";
       
        protected override string BuildCommandHandlerClass()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using AutoMapper;");
            sb.AppendLine("using Sodexo.BackOffice.Abstraction.Commands;");
            sb.AppendLine("using Sodexo.BackOffice.Abstraction.Data;");
            sb.AppendLine("using Sodexo.BackOffice.Abstraction.Enums;");
            sb.AppendLine("using Sodexo.BackOffice.Abstraction.Extensions;");
            sb.AppendLine("using Sodexo.BackOffice.AccountStructureManagement.Domain;");
            sb.AppendLine("using Sodexo.BackOffice.AccountStructureManagement.Infrastructure;");
            sb.AppendLine("using Sodexo.BackOffice.Core.Extentions;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"{CommandNameSpace}");
            sb.AppendLine();
            sb.AppendLine($"public class {CommandType}{entityName}CommandHandler : ICommandHandler<{CommandType}{entityName}Command, {CommandType}{entityName}CommandResult>");
            sb.AppendLine("{");
            sb.AppendLine($"    private readonly IUnitOfWork<{context}> unitOfWork;");
            sb.AppendLine($"    private readonly IMapper mapper;");
            sb.AppendLine();
            sb.AppendLine($"    public {CommandType}{entityName}CommandHandler(IUnitOfWork<{context}> unitOfWork, IMapper mapper)");
            sb.AppendLine("    {");
            sb.AppendLine("        this.unitOfWork = unitOfWork;");
            sb.AppendLine("        this.mapper = mapper;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine($"    public async Task<{CommandType}{entityName}CommandResult> Handle({CommandType}{entityName}Command command, CancellationToken cancellationToken)");
            sb.AppendLine("    {");
            sb.AppendLine($"        //INFO: entity = oldValue, command = newValue");
            sb.AppendLine($"        var repo = this.unitOfWork.GetCommandRepository<{entityName}>();");
            sb.AppendLine($"        var entity = await repo.FindAsync(x => x.{primaryKey} == command.{primaryKey}, cancellationToken: cancellationToken).ConfigureAwait(false);");
            sb.AppendLine();
            sb.AppendLine("        if (entity == null)");
            sb.AppendLine("        {");
            sb.AppendLine($"            {moduleName}ErrorCode.{entityName}NotFound.Throw(command.{primaryKey});");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        #region Before Trigger Operations");
            sb.AppendLine("        #endregion");
            sb.AppendLine();
            sb.AppendLine("        #region Entity Operations");
            sb.AppendLine();
            sb.AppendLine("        this.mapper.Map(command, entity);");
            sb.AppendLine("        entity.SetTriggerConversionStatus(TriggerConversionStatus.Before);");
            sb.AppendLine("        await repo.UpdatePartialAsync(entity).ConfigureAwait(false);");
            sb.AppendLine();
            sb.AppendLine("        #endregion");
            sb.AppendLine();
            sb.AppendLine("        #region After Trigger Operations");
            sb.AppendLine("        #endregion");
            sb.AppendLine();
            sb.AppendLine($"       return this.mapper.Map<{CommandType}{entityName}CommandResult>(entity);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        protected override string BuildCommandClass()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Sodexo.BackOffice.Abstraction.Commands;");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"{CommandNameSpace}");
            sb.AppendLine();
            sb.AppendLine($"public class {this.CommandType}{entityName}Command : CommandBase<{this.CommandType}{entityName}CommandResult>");
            sb.AppendLine("{");

            var excludedProperties = this.GetExcludedProperties();

            foreach (var prop in properties)
            {
                if (!excludedProperties.Contains(prop.Key))
                {
                    sb.AppendLine($"    public {prop.Value.type} {prop.Key} {{ get; set; }}");
                }
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        protected override HashSet<string> GetExcludedProperties()
        {
            return new HashSet<string>(new[] { "TriggerConversionStatus" }, StringComparer.OrdinalIgnoreCase);
        }
    }
}
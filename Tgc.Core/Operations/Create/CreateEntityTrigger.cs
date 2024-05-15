using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tgc.Core.Base;
using Tgc.Core.Constants;
using Tgc.Core.Extensions;

namespace Tgc.Core.Operations.Create
{
    public class CreateEntityTrigger : CqrsBase
    {
        protected override string CommandType { get; set; } = "Create";

        public override void Process(string mappingInfo)
        {
            base.Process(mappingInfo);
            string commandBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, moduleName, "Application", "Commands", $"{entityName}Commands", $"{CommandType}{entityName}");
            FileExtensions.CreateFile(Path.Combine(commandBasePath, $"{CommandType}{entityName}Validator.cs"), BuildValidatorClass());
        }

        protected override string BuildCommandHandlerClass()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using AutoMapper;");
            sb.AppendLine("using Sodexo.BackOffice.Abstraction.Commands;");
            sb.AppendLine("using Sodexo.BackOffice.Abstraction.Data;");
            sb.AppendLine("using Sodexo.BackOffice.Abstraction.Enums;");
            sb.AppendLine("using Sodexo.BackOffice.AccountStructureManagement.Domain;");
            sb.AppendLine("using Sodexo.BackOffice.AccountStructureManagement.Infrastructure;");
            sb.AppendLine("using Sodexo.BackOffice.Core.Extentions;");
            sb.AppendLine("using Sodexo.BackOffice.Data.Extentions;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"{CommandNameSpace}");
            sb.AppendLine();
            sb.AppendLine($"public class {CommandType}{entityName}CommandHandler : ICommandHandler<{CommandType}{entityName}Command, {CommandType}{entityName}CommandResult>");
            sb.AppendLine("{");
            sb.AppendLine($"   private readonly IUnitOfWork<{context}> unitOfWork;");
            sb.AppendLine("    private readonly IMapper mapper;");
            sb.AppendLine();
            sb.AppendLine($"   public {CommandType}{entityName}CommandHandler(IUnitOfWork<{context}> unitOfWork, IMapper mapper)");
            sb.AppendLine("    {");
            sb.AppendLine("        this.unitOfWork = unitOfWork;");
            sb.AppendLine("        this.mapper = mapper;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine($"   public async Task<{CommandType}{entityName}CommandResult> Handle({CommandType}{entityName}Command command, CancellationToken cancellationToken)");
            sb.AppendLine("    {");
            sb.AppendLine($"        var entity = this.mapper.Map<{entityName}>(command);");
            sb.AppendLine($"        // Pk değeri Sequence ile besleniyorsa aşağıdaki satır aktif edilmeli. Eğer AutoIncremented ise aşağıdaki satır silinmeli");
            sb.AppendLine($"        //entity.{primaryKey} = await this.unitOfWork.Database.ExecuteNextValAsync(\"{entityName.ToUpper()}_ID\").ConfigureAwait(false);");
            sb.AppendLine();
            sb.AppendLine("        #region Before Trigger Operations");
            sb.AppendLine("        #endregion");
            sb.AppendLine();
            sb.AppendLine("        #region Entity Operations");
            sb.AppendLine();
            sb.AppendLine($"        entity.SetTriggerConversionStatus(TriggerConversionStatus.Before);");
            sb.AppendLine($"        await this.unitOfWork.GetCommandRepository<{entityName}>().AddAsync(entity).ConfigureAwait(false);");
            sb.AppendLine();
            sb.AppendLine("        #endregion");
            sb.AppendLine();
            sb.AppendLine("        #region After Trigger Operations");
            sb.AppendLine("        #endregion");
            sb.AppendLine();
            sb.AppendLine($"        return this.mapper.Map<{CommandType}{entityName}CommandResult>(entity);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
        
        private string BuildValidatorClass()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using FluentValidation;");
            sb.AppendLine("using Sodexo.BackOffice.Core.Validation;");
            sb.AppendLine();
            sb.AppendLine($"{CommandNameSpace}");
            sb.AppendLine();
            sb.AppendLine($"public class {CommandType}{entityName}CommandValidator : BaseValidator<{CommandType}{entityName}Command>");
            sb.AppendLine("{");
            sb.AppendLine($"    public {CommandType}{entityName}CommandValidator()");
            sb.AppendLine("    {");
            foreach (var prop in properties)
            {
                sb.AppendLine($"        this.RuleFor(x => x.{prop.Key}).NotNull();");
                if (prop.Value.maxLength > 0)
                {
                    sb.AppendLine($"        this.RuleFor(x => x.{prop.Key}).MaximumLength({prop.Value.maxLength});");
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
        
        protected override HashSet<string> GetExcludedProperties(string triggerOperationType)
        {
            return new HashSet<string>(new string[] { primaryKey, "CruCode", "LuuCode", "LuuDate", "CrDate", "TriggerConversionStatus" }, StringComparer.OrdinalIgnoreCase);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tgc.Core.Base;
using Tgc.Core.Extensions;

namespace Tgc.Core.Operations.Create
{
    public class CreateEntityTrigger : CqrsBase
    {
        protected override string CommandType { get { return "Create"; } }
        public override void Process()
        {
            base.Process();
            var validatorBasePath = Path.Combine(@"C:\TriggerConversionGenerator", ModuleName, "Application", "Commands", $"{EntityName}Commands", $"{CommandType}{EntityName}", $"{CommandType}{EntityName}Validator.cs");

            FileExtensions.CreateFile(validatorBasePath, BuildValidatorClass());
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
            sb.AppendLine($"public class {CommandType}{EntityName}CommandHandler : ICommandHandler<{CommandType}{EntityName}Command, {CommandType}{EntityName}CommandResult>");
            sb.AppendLine("{");
            sb.AppendLine($"   private readonly IUnitOfWork<{Context}> unitOfWork;");
            sb.AppendLine("    private readonly IMapper mapper;");
            sb.AppendLine();
            sb.AppendLine($"   public {CommandType}{EntityName}CommandHandler(IUnitOfWork<{Context}> unitOfWork, IMapper mapper)");
            sb.AppendLine("    {");
            sb.AppendLine("        this.unitOfWork = unitOfWork;");
            sb.AppendLine("        this.mapper = mapper;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine($"   public async Task<{CommandType}{EntityName}CommandResult> Handle({CommandType}{EntityName}Command command, CancellationToken cancellationToken)");
            sb.AppendLine("    {");
            sb.AppendLine($"        var entity = this.mapper.Map<{EntityName}>(command);");
            sb.AppendLine($"        // Pk değeri Sequence ile besleniyorsa aşağıdaki satır aktif edilmeli. Eğer AutoIncremented ise aşağıdaki satır silinmeli");
            sb.AppendLine($"        //entity.{PrimaryKey} = await this.unitOfWork.Database.ExecuteNextValAsync(\"{EntityName.ToUpper()}_ID\").ConfigureAwait(false);");
            sb.AppendLine();
            sb.AppendLine("        #region Before Trigger Operations");
            sb.AppendLine("        #endregion");
            sb.AppendLine();
            sb.AppendLine("        #region Entity Operations");
            sb.AppendLine();
            sb.AppendLine($"        entity.SetTriggerConversionStatus(TriggerConversionStatus.Before);");
            sb.AppendLine($"        await this.unitOfWork.GetCommandRepository<{EntityName}>().AddAsync(entity).ConfigureAwait(false);");
            sb.AppendLine();
            sb.AppendLine("        #endregion");
            sb.AppendLine();
            sb.AppendLine("        #region After Trigger Operations");
            sb.AppendLine("        #endregion");
            sb.AppendLine();
            sb.AppendLine($"        return this.mapper.Map<{CommandType}{EntityName}CommandResult>(entity);");
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
            sb.AppendLine($"public class {CommandType}{EntityName}CommandValidator : BaseValidator<{CommandType}{EntityName}Command>");
            sb.AppendLine("{");
            sb.AppendLine($"    public {CommandType}{EntityName}CommandValidator()");
            sb.AppendLine("    {");

            var excludedProps = this.GetExcludedProperties();
            foreach (var prop in ColumnProperties)
            {
                if (!excludedProps.Contains(prop.Key))
                {
                    if (prop.Value.type == "string")
                    {
                        sb.Append($"        this.RuleFor(x => x.{prop.Key})");

                        if (prop.Value.isRequired)
                            sb.Append(".NotNull()");

                        if (prop.Value.maxLength > 0)
                            sb.Append($".MaximumLength({prop.Value.maxLength});");

                        sb.AppendLine();
                    }
                }
            }

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
            sb.AppendLine($"public class {this.CommandType}{EntityName}Command : CommandBase<{this.CommandType}{EntityName}CommandResult>");
            sb.AppendLine("{");

            var excludedProperties = this.GetExcludedProperties();

            var defaultValues = ParsingExtensions.ExtractDefaultValues(MappingInfo, ColumnProperties);

            if (defaultValues.Count > 0)
            {
                sb.AppendLine($"    public {this.CommandType}{EntityName}Command()");
                sb.AppendLine("    {");

                foreach (var defaultValue in defaultValues)
                {
                    if (!excludedProperties.Contains(defaultValue.Key))
                    {
                        var value = defaultValues[defaultValue.Key];
                        sb.AppendLine($"        this.{defaultValue.Key} = {value};");
                    }
                }

                sb.AppendLine("    }");
                sb.AppendLine();
            }


            foreach (var prop in ColumnProperties)
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
            return new HashSet<string>(new string[] { PrimaryKey, "CruCode", "LuuCode", "LuuDate", "CrDate", "TriggerConversionStatus" }, StringComparer.OrdinalIgnoreCase);
        }
    }
}
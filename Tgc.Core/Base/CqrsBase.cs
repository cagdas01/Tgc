using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tgc.Core.Extensions;

namespace Tgc.Core.Base
{
    public abstract class CqrsBase
    {
        public string moduleName;
        public string entityName;
        public string primaryKey;
        protected string context;
        protected string CommandNameSpace;
        public string MappingInfo;

        public Dictionary<string, (string type, bool isRequired, int maxLength)> properties;

        public virtual void Process()
        {
            entityName = MappingInfo.ExtractEntityName();
            properties = MappingInfo.ParseProperties();
            primaryKey = MappingInfo.ExtractPrimaryKey();
            context = this.moduleName.GetContextName();
            CommandNameSpace = $"namespace Sodexo.BackOffice.{moduleName}.Application.Commands.{entityName}Commands.{CommandType}{entityName};";

            string commandBasePath = Path.Combine(@"C:\TriggerConversionGenerator", moduleName, "Application", "Commands", $"{entityName}Commands", $"{CommandType}{entityName}");
            string mapperBasePath = Path.Combine(@"C:\TriggerConversionGenerator", moduleName, "Application", "Mappers");

            FileExtensions.CreateFile(Path.Combine(commandBasePath, $"{this.CommandType}{entityName}Command.cs"), BuildCommandClass());  // Primary key is passed to the method
            FileExtensions.CreateFile(Path.Combine(commandBasePath, $"{this.CommandType}{entityName}CommandHandler.cs"), BuildCommandHandlerClass());
            FileExtensions.CreateFile(Path.Combine(commandBasePath, $"{this.CommandType}{entityName}CommandResult.cs"), BuildCommandResultClass());
            FileExtensions.CreateFile(Path.Combine(mapperBasePath, $"{entityName}Mapper.cs"), BuildMapperClass());
        }

        protected abstract HashSet<string> GetExcludedProperties();
        protected abstract string CommandType { get; set; }

        protected abstract string BuildCommandClass();
        
        protected abstract string BuildCommandHandlerClass();
        protected string BuildCommandResultClass()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"{CommandNameSpace}");
            sb.AppendLine();
            sb.AppendLine($"public class {this.CommandType}{entityName}CommandResult");
            sb.AppendLine("{");
            foreach (var prop in properties)
            {
                sb.AppendLine($"    public {prop.Value.type} {prop.Key} {{ get; set; }}");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }
        protected string BuildMapperClass()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using AutoMapper;");
            sb.AppendLine($"using Sodexo.BackOffice.{moduleName}.Application.Commands.{entityName}Commands.Create{entityName};");
            sb.AppendLine($"using Sodexo.BackOffice.{moduleName}.Application.Commands.{entityName}Commands.Update{entityName};");
            sb.AppendLine("using Sodexo.BackOffice.AccountStructureManagement.Domain;");
            sb.AppendLine();
            sb.AppendLine($"namespace Sodexo.BackOffice.{moduleName}.Application.Mappers;");
            sb.AppendLine();
            sb.AppendLine($"public class {entityName}Mapper : Profile");
            sb.AppendLine("{");
            sb.AppendLine($"    public {entityName}Mapper()");
            sb.AppendLine("    {");
            sb.AppendLine($"        this.CreateMap<Create{entityName}Command, {entityName}>();");
            sb.AppendLine($"        this.CreateMap<{entityName}, Create{entityName}CommandResult>();");
            sb.AppendLine();
            sb.AppendLine($"        this.CreateMap<Update{entityName}Command, {entityName}>();");
            sb.AppendLine($"        this.CreateMap<{entityName}, Update{entityName}CommandResult>();");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
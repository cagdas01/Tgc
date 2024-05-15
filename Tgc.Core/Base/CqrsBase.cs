using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tgc.Core.Extensions;

namespace Tgc.Core.Base
{
    public abstract class CqrsBase
    {
        public string ModuleName { get; set; }
        public string MappingInfo { get; set; }

        protected Dictionary<string, (string type, bool isRequired, int maxLength)> ColumnProperties { get; set; }
        protected string EntityName { get; set; }
        protected string PrimaryKey { get; set; }
        protected string Context { get; set; }
        protected string CommandNameSpace { get; set; }

        protected abstract string CommandType { get; }


        public virtual void Process()
        {
            EntityName = MappingInfo.ExtractEntityName();
            ColumnProperties = MappingInfo.ParseProperties();
            PrimaryKey = MappingInfo.ExtractPrimaryKey();
            Context = this.ModuleName.GetContextName();
            CommandNameSpace = $"namespace Sodexo.BackOffice.{ModuleName}.Application.Commands.{EntityName}Commands.{CommandType}{EntityName};";

            string commandBasePath = Path.Combine(@"C:\TriggerConversionGenerator", ModuleName, "Application", "Commands", $"{EntityName}Commands", $"{CommandType}{EntityName}");
            string mapperBasePath = Path.Combine(@"C:\TriggerConversionGenerator", ModuleName, "Application", "Mappers");
            string apiBasePath = Path.Combine(@"C:\TriggerConversionGenerator", ModuleName, "Api", "Controllers");

            FileExtensions.CreateFile(Path.Combine(commandBasePath, $"{this.CommandType}{EntityName}Command.cs"), BuildCommandClass());  // Primary key is passed to the method
            FileExtensions.CreateFile(Path.Combine(commandBasePath, $"{this.CommandType}{EntityName}CommandHandler.cs"), BuildCommandHandlerClass());
            FileExtensions.CreateFile(Path.Combine(commandBasePath, $"{this.CommandType}{EntityName}CommandResult.cs"), BuildCommandResultClass());
            FileExtensions.CreateFile(Path.Combine(mapperBasePath, $"{EntityName}Mapper.cs"), BuildMapperClass());
            FileExtensions.CreateFile(Path.Combine(apiBasePath, $"{EntityName}Controller.cs"), BuildControllerClass());
        }

        protected abstract HashSet<string> GetExcludedProperties();
        protected abstract string BuildCommandClass();
        protected abstract string BuildCommandHandlerClass();

        protected virtual string BuildCommandResultClass()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"{CommandNameSpace}");
            sb.AppendLine();
            sb.AppendLine($"public class {this.CommandType}{EntityName}CommandResult");
            sb.AppendLine("{");
            foreach (var prop in ColumnProperties)
            {
                sb.AppendLine($"    public {prop.Value.type} {prop.Key} {{ get; set; }}");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }
        protected virtual string BuildMapperClass()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using AutoMapper;");
            sb.AppendLine($"using Sodexo.BackOffice.{ModuleName}.Application.Commands.{EntityName}Commands.Create{EntityName};");
            sb.AppendLine($"using Sodexo.BackOffice.{ModuleName}.Application.Commands.{EntityName}Commands.Update{EntityName};");
            sb.AppendLine("using Sodexo.BackOffice.AccountStructureManagement.Domain;");
            sb.AppendLine();
            sb.AppendLine($"namespace Sodexo.BackOffice.{ModuleName}.Application.Mappers;");
            sb.AppendLine();
            sb.AppendLine($"public class {EntityName}Mapper : Profile");
            sb.AppendLine("{");
            sb.AppendLine($"    public {EntityName}Mapper()");
            sb.AppendLine("    {");
            sb.AppendLine($"        this.CreateMap<Create{EntityName}Command, {EntityName}>();");
            sb.AppendLine($"        this.CreateMap<{EntityName}, Create{EntityName}CommandResult>();");
            sb.AppendLine();
            sb.AppendLine($"        this.CreateMap<Update{EntityName}Command, {EntityName}>();");
            sb.AppendLine($"        this.CreateMap<{EntityName}, Update{EntityName}CommandResult>();");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
        protected virtual string BuildControllerClass()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using Sodexo.BackOffice.Abstraction.Commands;");
            sb.AppendLine($"using Sodexo.BackOffice.{ModuleName}.Application.Commands.{EntityName}Commands.Create{EntityName};");
            sb.AppendLine($"using Sodexo.BackOffice.{ModuleName}.Application.Commands.{EntityName}Commands.Update{EntityName};");
            sb.AppendLine("using Sodexo.BackOffice.ApiBase;");
            sb.AppendLine("using Sodexo.BackOffice.ApiBase.Messaging;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"namespace Sodexo.BackOffice.{ModuleName}.Api.Controllers;");
            sb.AppendLine();
            sb.AppendLine($"public class {EntityName}Controller : ApiControllerBase");
            sb.AppendLine("{");
            sb.AppendLine($"    private readonly ICommandSender commandSender;");
            sb.AppendLine();
            sb.AppendLine($"    public {EntityName}Controller(ICommandSender commandSender)");
            sb.AppendLine("    {");
            sb.AppendLine("        this.commandSender = commandSender;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    [HttpPost]");
            sb.AppendLine($"    public async Task<Response<Create{EntityName}CommandResult>> Create{EntityName}([FromBody] Create{EntityName}Command command, CancellationToken cancellationToken)");
            sb.AppendLine("    {");
            sb.AppendLine("        var data = await this.commandSender.SendAsync(command, cancellationToken).ConfigureAwait(false);");
            sb.AppendLine("        return this.ProduceResponse(data);");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    [HttpPut]");
            sb.AppendLine($"    public async Task<Response<Update{EntityName}CommandResult>> Update{EntityName}([FromBody] Update{EntityName}Command command, CancellationToken cancellationToken)");
            sb.AppendLine("    {");
            sb.AppendLine("        var data = await this.commandSender.SendAsync(command, cancellationToken).ConfigureAwait(false);");
            sb.AppendLine("        return this.ProduceResponse(data);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
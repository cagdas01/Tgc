using System.Collections.Generic;
using System.IO;
using Tgc.Core.Extensions;

namespace Tgc.Core.Base
{
    public abstract class CqrsBase
    {
        public string moduleName;
        public string entityName;
        public string primaryKey;
        public Dictionary<string, (string type, bool isRequired, int maxLength)> properties;

        public virtual void Process(string mappingInfo)
        {
            entityName = ParsingExtensions.ExtractEntityName(mappingInfo);
            properties = ParsingExtensions.ParseProperties(mappingInfo);
            primaryKey = ParsingExtensions.ExtractPrimaryKey(mappingInfo);

            CreateFile($"{GetFileNameSuffix()}{entityName}Command.cs", BuildCommandClass());  // Primary key is passed to the method
            CreateFile($"{GetFileNameSuffix()}{entityName}CommandHandler.cs", BuildCommandHandlerClass());
            CreateFile($"{GetFileNameSuffix()}{entityName}CommandResult.cs", BuildCommandResultClass());
            CreateFile($"{GetFileNameSuffix()}{entityName}Mappers.cs", BuildMapperClass());
        }
        protected abstract string GetNamespacePrefix();
        protected abstract string GetFileNameSuffix();
        protected abstract string BuildCommandClass();
        protected abstract string BuildCommandHandlerClass();
        protected abstract string BuildCommandResultClass();
        protected abstract string BuildMapperClass();
        protected void CreateFile(string fileName, string content)
        {
            var filePath = Path.GetFullPath($@"..\..\..\Resources\GeneratedFolders");
            FileExtensions.WriteToFile(filePath, content, fileName);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tgc.Core.Constants;
using Tgc.Core.Extensions;

namespace Tgc.Core.Base;

public abstract class CqrsBase
{
    public string moduleName;
    public string entityName;
    public Dictionary<string, (string type, bool isRequired, int maxLength)> properties;

    public virtual async Task Process(string filePath)
    {
        Console.WriteLine($"Processing {GetFileNameSuffix()}{entityName} for {moduleName}{StringConstants.Management}");
        if (File.Exists(filePath))
        {
            var mappingInfo = FileExtensions.ReadFileContent(filePath);

            entityName = mappingInfo.ExtractEntityName();
            properties = mappingInfo.ParseProperties();

            CreateFile($"{GetFileNameSuffix()}{entityName}Command.cs", BuildCommandClass());
            CreateFile($"{GetFileNameSuffix()}{entityName}CommandHandler.cs", BuildCommandHandlerClass());
            CreateFile($"{GetFileNameSuffix()}{entityName}CommandResult.cs", BuildCommandResultClass());
            CreateFile($"{GetFileNameSuffix()}{entityName}Mappers.cs", BuildMapperClass());

            Console.WriteLine("All files successfully created.");
        }
        else
        {
            Console.WriteLine("File not found: " + filePath);
        }
    }

    protected abstract string GetNamespacePrefix();
    protected abstract string GetFileNameSuffix();

    protected abstract string BuildCommandClass();
    protected abstract string BuildCommandHandlerClass();
    protected abstract string BuildCommandResultClass();
    protected abstract string BuildMapperClass();

    protected void CreateFile(string fileName, string content)
    {
        string filePath = Path.GetFullPath(@$"..\..\..\Resources\GeneratedFolders\{fileName}");
        FileExtensions.WriteToFile(filePath, content, fileName);
    }
}

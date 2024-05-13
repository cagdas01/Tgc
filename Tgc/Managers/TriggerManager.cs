using System;
using System.Threading.Tasks;
using Tgc.Core.Base;
using Tgc.Core.Constants;
using Tgc.Core.Enums;
using Tgc.Operations.Create;
using Tgc.Operations.Update;


namespace Tgc.Managers;

public class TriggerManager : ITriggerService
{
    private readonly string filePath = Path.GetFullPath(@$"..\..\..\Resources\input.txt");
    public async Task InitializeAndRun()
    {
        Console.WriteLine("Choose the module:");
        string[] modules =
        {
            $"{StringConstants.AccountStructure}", $"{StringConstants.Card}", $"{StringConstants.Invoice}", $"{StringConstants.Merchant}", $"{StringConstants.Notification}", $"{StringConstants.Order}", $"{StringConstants.Delivery}"
        };
        for (int i = 0; i < modules.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {modules[i]}");
        }

        int moduleChoice = Convert.ToInt32(Console.ReadLine()) - 1;
        string moduleName = modules[moduleChoice];

        Console.WriteLine("Choose the operation:\n1. CreateTrigger\n2. UpdateTrigger  \n3. Create\n4. Update");
        string input = Console.ReadLine();
        if (!Enum.TryParse(input, out OperationType operationType))
        {
            Console.WriteLine("Invalid operation type selected. Please enter a valid number (e.g., 1 for CreateTrigger, 2 for UpdateTrigger, 3 for Create, 4 for Update).");
            return;
        }

        await RunTrigger(moduleName, operationType);
    }
    private async Task RunTrigger(string moduleName, OperationType operationType)
    {
        CqrsBase trigger = operationType switch
        {
            OperationType.CreateTrigger => new CreateEntityTrigger(),
            OperationType.UpdateTrigger => new UpdateEntityTrigger(),
            OperationType.Create => new CreateEntity(),
            OperationType.Update => new UpdateEntity(),
            _ => throw new NotImplementedException($"Operation {operationType} not supported.")
        };

        trigger.moduleName = moduleName;
        await trigger.Process(filePath);
    }
}

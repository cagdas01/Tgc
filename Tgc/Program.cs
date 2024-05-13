using Tgc.Managers;
using Microsoft.Extensions.DependencyInjection;

var provider = (new ServiceCollection().AddScoped<ITriggerService, TriggerManager>().BuildServiceProvider()).GetRequiredService<ITriggerService>();
provider.InitializeAndRun();

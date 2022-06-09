// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;

await Host.CreateDefaultBuilder(args)
  .UseWindowsService()
  .ConfigureServices(services =>
{
  services.AddTransient<DynuUpdater.Service.DynuAPI>();
  services.AddHostedService<DynuUpdater.App>();
}).RunConsoleAsync();



//var q = Host.CreateDefaultBuilder(args).ConfigureServices(services =>
//{
//  services.AddHostedService<DynuUpdater.App>();
//}).Build();

//await q.RunAsync();


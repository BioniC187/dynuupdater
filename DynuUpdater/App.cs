using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynuUpdater
{
  internal class App : BackgroundService
  {
    private Timer timer;
    private Service.DynuAPI _dynuAPI;
    private ILogger<App> _logger;
    private string[] domainsToUpdate;
    private long updateFrequency = 10*60*1000;

    public App(Service.DynuAPI dynuAPI, ILogger<App> logger, IConfiguration configuration)
    {
      _dynuAPI = dynuAPI;
      _logger = logger;
      var q = configuration;
      Init();
    }

    private void Init()
    {
      _logger.LogInformation("Starting...");
      _logger.LogInformation("Initializing...");

      _dynuAPI.APIKey = Environment.GetEnvironmentVariable("API_KEY") ?? "";
      var domains = Environment.GetEnvironmentVariable("DOMAINS");
      var timeInMinutesText = Environment.GetEnvironmentVariable("UPDATE_FREQUENCY");
      var errorText = "";

      if (!string.IsNullOrEmpty(timeInMinutesText))
      {
        var parseResult = long.TryParse(timeInMinutesText, out long timeInMinutes); 
        if (parseResult && timeInMinutes > 0)
        {
          updateFrequency = timeInMinutes * 60 * 1000;
        }
        else
        {
          errorText = $"UPDATE_FREQUENCY is not valid.{Environment.NewLine}";
        }
      }
      else
      {
        _logger.LogInformation("UPDATE_FREQUENCY not set. Using default of 10 minutes.");
      }

      if (string.IsNullOrEmpty(_dynuAPI.APIKey))
      {
        errorText += $"API_KEY not set.{Environment.NewLine}";
      }

      if (string.IsNullOrEmpty(domains))
      {
        errorText += $"DOMAINS not set.{Environment.NewLine}";
      }

      if (!string.IsNullOrEmpty(errorText))
      {
        _logger.LogError(errorText);
        throw new Exception(errorText);
      }

      domainsToUpdate = domains.Split('\u002C');
      for (int i = 0; i < domainsToUpdate.Length; i++)
      {
        domainsToUpdate[i] = domainsToUpdate[i].Trim();
      }
      _logger.LogInformation("Done");

      //_logger.LogDebug("Got API_KEY and DOMAINS");
      //_logger.LogDebug($"API_KEY: {_dynuAPI.APIKey}");
      //_logger.LogDebug($"DOMAINS: {domains}");
      //_logger.LogDebug($"UPDATE_FREQUENCY: {updateFrequency}");
    }

    async void OnTimer(object state)
    {
      _logger.LogInformation("Updating domains...");
      var userDomains = await _dynuAPI.GetEntries();

      foreach (var domain in domainsToUpdate)
      {
        _logger.LogInformation($"Processing: {domain}");
        var userDomain = userDomains.FirstOrDefault(x => x.Name == domain);
        if (userDomain != null)
        {
          //_logger.LogDebug("Getting public IP Address...");
          var publicIPAddress = await _dynuAPI.GetPublicIPAddress();
          //_logger.LogDebug($"Public IPAddress: {publicIPAddress}");
          if (!string.IsNullOrEmpty(publicIPAddress))
          {
            //_logger.LogDebug($"Updating up for domain on Dynu: {domain}-{publicIPAddress}");
            var updateResult = await _dynuAPI.UpdateIPAddress(userDomain.Id, userDomain.Name, publicIPAddress);
            if (!updateResult)
            {
              _logger.LogError("Unable to update IP address on Dynu.");
            }
            //_logger.LogDebug($"Done updating: {domain}-{publicIPAddress}");
          }
          else
          {
            _logger.LogError("Unable to retrieve public IP address. Will try again at next update.");
          }
        }
      }

      _logger.LogInformation("Done updating all domains.");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      timer = new Timer(OnTimer, stoppingToken, 2000, updateFrequency);

      return Task.CompletedTask;
    }
  }
}

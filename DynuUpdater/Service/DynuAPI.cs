using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynuUpdater.Service
{
  internal class DynuAPI
  {
    private string DynuAPiUrl = "https://api.dynu.com/v2";
    public string APIKey { get; set; }// = "WdXW35g6Y4ZVVc53564cdVb564g74T37";
    public DynuAPI()
    {

    }

    public async Task<List<Models.Domain>> GetEntries()
    {
      HttpClient client = new HttpClient();
      client.DefaultRequestHeaders.Add("API-Key", APIKey);

      HttpResponseMessage result = await client.GetAsync(DynuAPiUrl + "/dns");

      if (result.IsSuccessStatusCode)
      {
        var contentStream = await result.Content.ReadAsStringAsync();
        var domains = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.GetDomainsListResponse>(contentStream);
        return domains?.Domains;
      }
      else
      {
        return null;
      }
    }

    public async Task<bool> UpdateIPAddress(int id, string domain, string publicIPAddress)
    {
      HttpClient client = new HttpClient();
      client.DefaultRequestHeaders.Add("API-Key", APIKey);

      var model = new Models.UpdateDNSIPRequest();
      model.Name = domain;
      model.IPv4Address = publicIPAddress;

      var payload = Newtonsoft.Json.JsonConvert.SerializeObject(model); // model serialized.
      var content = new StringContent(payload, Encoding.UTF8, "application/json");

      HttpResponseMessage response = await client.PostAsync(DynuAPiUrl + $"/dns/{id}", content);

      if (response.IsSuccessStatusCode)
      {
        var contentStream = await response.Content.ReadAsStringAsync();
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.DynuResponse>(contentStream);
        return result.StatusCode == 200;
      }
      else
      {
        return false;
      }
    }

    public async Task<string> GetPublicIPAddress()
    {
      HttpClient client = new HttpClient();

      HttpResponseMessage result = await client.GetAsync("https://www.trackip.net/ip?json");

      if (result.IsSuccessStatusCode)
      {
        var contentStream = await result.Content.ReadAsStringAsync();
        var ipResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.PublicIPAddressResponse>(contentStream);
        return ipResponse?.IP;
      }
      else
      {
        return null;
      }
    }
  }
}

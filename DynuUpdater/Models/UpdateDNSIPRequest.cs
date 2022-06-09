using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynuUpdater.Models
{
  internal class UpdateDNSIPRequest
  {
    /// <summary>
    /// The domain name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Only set this if we are explicitly setting an ip adress. If no value is set, the client machine public ip address will be used.
    /// </summary>
    public string IPv4Address { get; set; }
  }
}

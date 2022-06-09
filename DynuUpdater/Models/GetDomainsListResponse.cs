using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynuUpdater.Models
{
  internal class GetDomainsListResponse : DynuResponse
  {
    public List<Domain> Domains { get; set; }
  }
}

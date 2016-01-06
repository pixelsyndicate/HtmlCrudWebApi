using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTMLTableSamples.EntityClasses
{
public class Product
{
  public int ProductId { get; set; }
  public string ProductName { get; set; }
  public DateTime IntroductionDate { get; set; }
  public string Url { get; set; }
}
}
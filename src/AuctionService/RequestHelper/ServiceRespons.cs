using System;

namespace AuctionService.RequestHelper;

public class ServiceRespons<T> 
{
  public  T data;
  public string Message { get; set; }

  public bool Sccess {get;set;}
}

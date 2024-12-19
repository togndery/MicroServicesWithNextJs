using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using SearchService.Models;

namespace SearchService.Reqhelper
{
    public class MappingProfiles :Profile
    {
       public MappingProfiles()
       {
          CreateMap<AuctionCreated ,Item>();
       }   
    }
}
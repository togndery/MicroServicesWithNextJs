using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Reqhelper;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>>SearchItem([FromQuery]SearchParm searchParms)
        {
          
           var query  = DB.PagedSearch<Item ,Item>();
          
           
           if(!string.IsNullOrEmpty(searchParms.SearchTerm) ){
             query.Match(Search.Full ,searchParms.SearchTerm).SortByTextScore();
           }

           query = searchParms.OrderBy switch
           {
               "make" => query.Sort(x=>x.Ascending(b=>b.Make)),
               "new" => query.Sort(c=>c.Descending(e=>e.CreatedAt)),
               _ => query.Sort(c=>c.Ascending(a=>a.AuctionEnd))
           };

           query = searchParms.FilterBy switch
           {
            "finished" => query.Match(x=>x.AuctionEnd < DateTime.UtcNow),
             "endingSoon" => query.Match(x=>x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd >DateTime.UtcNow),
             _=> query.Match(x=>x.AuctionEnd > DateTime.UtcNow)
           };


           if(!string.IsNullOrEmpty(searchParms.Seller)){
            query.Match(x =>x.Seller ==searchParms.Seller);
           }

           
           if(!string.IsNullOrEmpty(searchParms.Winner)){
            query.Match(x =>x.Winner ==searchParms.Winner);
           }

         

           query.PageNumber(searchParms.PageNumber);
           query.PageSize(searchParms.PageSize);
           var result = await query.ExecuteAsync();   

           return Ok(new {
              result = result.Results,
              pageCount = result.PageCount,
              totalCount = result.TotalCount
           });  
        }
    }
}
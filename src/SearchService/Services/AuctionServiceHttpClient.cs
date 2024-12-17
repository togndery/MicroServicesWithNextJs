using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services
{
    public class AuctionServiceHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AuctionServiceHttpClient(HttpClient httpClient , IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<Item>> GetItemsForSearchDb(){

            var lastUpdate = await DB.Find<Item,string>()
            .Sort(x=>x.Descending(x => x.UpdatedAt))
            .Project(x=>x.UpdatedAt.ToString())
            .ExecuteFirstAsync();

            return await _httpClient.GetFromJsonAsync<List<Item>>(_configuration["AuctionServicesUrl"] + "/api/auctions?date=" + lastUpdate);
        }
    }
}
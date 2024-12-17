using System;
using System.Text.Json;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data;

public class DbInitialzer
{
      public static async Task InitDb(WebApplication app)
      {
         await DB.InitAsync("SearchDb" ,MongoDB.Driver.MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MonogDbConnection")));

        await DB.Index<Item>()
        .Key(x =>x.Make ,KeyType.Text)
        .Key(x =>x.Model ,KeyType.Text)
        .Key(x =>x.Color ,KeyType.Text)
        .CreateAsync();

        //chaeck if data exsit in the databse 

        var count  = await DB.CountAsync<Item>();

        if(count == 0)
        {
            Console.WriteLine("no data to Seed ");
            var itemData= await File.ReadAllTextAsync("Data/auction.json");

            var opt = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

            var item = JsonSerializer.Deserialize<List<Item>>(itemData,opt);

            await DB.SaveAsync(item);
        }



      }
}
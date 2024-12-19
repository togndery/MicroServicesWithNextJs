using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionUpdateConsumer : IConsumer<AuctionUpdate>
    {
        private readonly IMapper _mapper;

        public AuctionUpdateConsumer(IMapper mapper)
        {
            _mapper = mapper;
        } 
        public async Task Consume(ConsumeContext<AuctionUpdate> context)
        {
            Console.WriteLine("====> Consuming auction update" + context.Message.Id);

            var item = _mapper.Map<Item>(context.Message);

            var result = await DB.Update<Item>()
                         .Match(a =>a.ID == context.Message.Id)
                         .ModifyOnly(x => new {

                            x.Color,
                            x.Make,
                            x.Model,
                            x.Year,
                            x.Mileage
                         },item)
                         .ExecuteAsync();

            if(!result.IsAcknowledged){
                throw new MessageException(typeof(AuctionUpdate),"Monog Db Error Updating");
            }             
            
        }
    }
}
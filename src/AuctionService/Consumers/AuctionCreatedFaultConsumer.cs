using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
    {
        public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
        {
           Console.WriteLine("=====>  Consuming fault creating");

           var exp = context.Message.Exceptions.First();

           if(exp.ExceptionType == "System.ArgumentException"){

             context.Message.Message.Model ="FooBar";
             await context.Publish(context.Message.Message);
           }
           else{
            Console.WriteLine("====> not argument  update the UI" );
           }
        }
    }
}
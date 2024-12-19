using AuctionService.Consumers;
using AuctionService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AuctionDbContext>(opt => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefulatConnection"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddMassTransit(x=>{
     
     x.AddEntityFrameworkOutbox<AuctionDbContext>(ops=> 
     {
           ops.QueryDelay = TimeSpan.FromSeconds(10);
           ops.UsePostgres();
           ops.UseBusOutbox();
     });
     x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();

    x.UsingRabbitMq((context ,cfg)=> {
        cfg.ConfigureEndpoints(context);
    });
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction" ,false));
});

var app = builder.Build();

try
{
    DbInitializer.IntDb(app);
}
catch (Exception ex)
{
    
    Console.WriteLine(ex.Message);
}


app.UseAuthorization();

app.MapControllers();




app.Run();

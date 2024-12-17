using AuctionService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AuctionDbContext>(opt => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefulatConnection"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddMassTransit(x=>{
    x.UsingRabbitMq((context ,cfg)=> {
        cfg.ConfigureEndpoints(context);
    });
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

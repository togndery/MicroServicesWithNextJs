using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.RequestHelper;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _autoMapper;

        public AuctionsController(AuctionDbContext context ,IMapper autoMapper)
        {
            _context = context;
            _autoMapper = autoMapper;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>>GetallAuctions(string date)
        {

            var query = _context.Auctions.OrderBy(x =>x.Item.Make).AsQueryable();

            if(!string.IsNullOrEmpty(date)){
                query = query.Where(x=>x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
            }
        //    var auctionResult = await _context.Auctions.Include(x => x.Item).OrderBy(x =>x.Item.Make).ToListAsync();

        //    return _autoMapper.Map<List<AuctionDto>>(auctionResult) ;

              return await query.ProjectTo<AuctionDto>(_autoMapper.ConfigurationProvider).ToListAsync() ;
            
            
        }


        [HttpGet("{id}",Name ="GetActionByIdAsync")]

        public async Task<ActionResult<AuctionDto>> GetActionByIdAsync(Guid id)
        {
             var auction = await _context.Auctions.Include(x=>x.Item).FirstOrDefaultAsync(c=>c.Id == id); 
              
            if(auction== null){
                return NotFound();
            }

             return _autoMapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>>CreateAuction(CreateAuctionDto auctionDto)
        {   
           
            var auction = _autoMapper.Map<Auction>(auctionDto);
           
            //TODO :Add Current user as Seller 
            auction.Seller = "test";
            

           await _context.Auctions.AddAsync(auction); 
            var result = await _context.SaveChangesAsync()> 0;

            if(!result) return BadRequest("Could not save change");
            

          //return (_autoMapper.Map<AuctionDto>(auction));
            return CreatedAtAction(nameof(GetActionByIdAsync),new {id= auction.Id},_autoMapper.Map<AuctionDto>(auction));
           
        }

        [HttpPut("{id}")]

        public async Task<ActionResult>UpdateAuction(Guid id,UpdateAuctionDto updateAuctionDto)
        {
           var auction =await _context.Auctions.Include(c=>c.Item).FirstOrDefaultAsync(x=>x.Id == id);

           if(auction==null)
           {
              return NotFound();
           }
           //TODO: check sellar == userName

           auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
           auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
           auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
           auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
           auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

           var result = await _context.SaveChangesAsync() > 0;

           if(result) return Ok();

           return BadRequest("Problem saving change");

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id){

          var auction = await _context.Auctions.FindAsync(id);

          if(auction==null) return NotFound();

          //TODO check if seller  == username
          _context.Auctions.Remove(auction);
          
          var result = await _context.SaveChangesAsync() > 0;

          if(result)
                return Ok();



           return BadRequest("Car not found");

        }
    }
}

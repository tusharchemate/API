using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase

    {

        private readonly StoreContext _context;

        public BasketController(StoreContext context)
        {
            _context = context;
        }



        [HttpGet]
        public async Task<ActionResult<Basket>> GetBasket()
        {
            int buyerId = int.Parse(Request.Cookies["buyerId"]);

            var basket = await _context.Baskets
            .Include(i => i.Items)
            .ThenInclude(p => p.Product)
            .FirstOrDefaultAsync(x => x.BuyerId == buyerId);

            return basket;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
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
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            var basket = await RetriveBasket();

            return new BasketDto
            {

                Id = basket.Id,
                BuyerId = basket.BuyerId,
                Items = basket.Items.Select(item => new BasketItemDto
                {

                    ProductId = item.ProductId,
                    Name = item.Product.Name,
                    Price = item.Product.Price,

                    PictuerUrl = item.Product.PictureURL,
                    Type = item.Product.Type,
                    Brand = item.Product.Brand,
                    Quantity = item.Product.QuantityInStock

                }).ToList()
            };
        }


        private async Task<Basket> RetriveBasket()
        {
            // int buyerId = int.Parse(Request.Cookies["buyerId"]);
            return await _context.Baskets
            .Include(i => i.Items)
            .ThenInclude(p => p.Product)
            .FirstOrDefaultAsync(x => x.BuyerId == Request.Cookies["buyerId"]);
        }

        [HttpPost]
        public async Task<ActionResult> AddItemToBasket(int productId, int quantity)
        {
            // get basket
            var basket = await RetriveBasket();

            if (basket == null) basket = CreateBasket();
            // create basket
            // get product 

            var product = await _context.Products.FindAsync(productId);

            if (product == null) return NotFound();

            basket.AddItem(product, quantity);

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return StatusCode(201);

            return BadRequest(new ProblemDetails { Title = "Problem Saving Item" });

        }

        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItem(int productId, int quantity)
        {

            var basket = await RetriveBasket();

            if (basket == null) return NotFound();

            basket.RemoveItem(productId, quantity);


            var result = await _context.SaveChangesAsync() > 0;


            if (result) return Ok();

            return BadRequest(new ProblemDetails { Title = "Something went Wrong" });


        }

        private Basket CreateBasket()
        {
            var buyerId = Guid.NewGuid().ToString();

            var cookieeOptions = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(30) };

            Response.Cookies.Append("buyerId", buyerId, cookieeOptions);

            var basket = new Basket { BuyerId = buyerId };

            _context.Baskets.Add(basket);

            return basket;
        }
    }
}
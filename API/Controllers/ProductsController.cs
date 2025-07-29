using System.ComponentModel;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly StoreContext context;

    public ProductsController(StoreContext context)
    {
        this.context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        // StoreContext returns all products in db
        // adding products from dbset to list
        return await context.Products.ToListAsync();
    }

    [HttpGet("{id:int}")] // e.g. api/products/2 getting prod by id
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        // getting product by id
        var product = await context.Products.FindAsync(id);
        if (product == null) return NotFound();
        return product;
    }

    // Creating new product
    [HttpPost]
    // apicontroller pulls "Product" from request via black magic so it knows
    // to parse out a product because of the param
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        //adding new product to context/dbset
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExists(id))
            return BadRequest("Failed to update product");

        context.Entry(product).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null) return NotFound();
        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return NoContent();
    }

    // these bool functions are still weird to me
    // true if any existing product has same id as passed param
    private bool ProductExists(int id)
    {
        return context.Products.Any(x => x.Id == id);
    }
}

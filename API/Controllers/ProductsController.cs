using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts()
    {
        // StoreContext returns all products in db
        // adding products from dbset to list
        return Ok(await repo.GetProductsAsync());
    }

    [HttpGet("{id:int}")] // e.g. api/products/2 getting prod by id
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        // getting product by id
        var product = await repo.GetProductByIdAsync(id);
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
        repo.AddProduct(product);
        if (await repo.SaveChangesAsync())
        {
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }
        else
        {
            return BadRequest("Issue creating product");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExists(id))
            return BadRequest("Failed to update product");

        if (await repo.SaveChangesAsync())
        {
            return NoContent();
        }
        else
        {
            return BadRequest("Issue updating product");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repo.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        repo.DeleteProduct(product);
        if (await repo.SaveChangesAsync())
        {
            return NoContent();
        }
        else
        {
            return BadRequest("Issue deleting product");
        }
    }

    // these bool functions are still weird to me
    // true if any existing product has same id as passed param
    private bool ProductExists(int id)
    {
        return repo.ProductExists(id);
    }
}

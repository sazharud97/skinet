using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    public async Task SeedAsync(StoreContext context)
    {
        if (!context.Products.Any())
        {
            // reading all text from seed data file
            var productsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
            // parse JSON into Product objects, just like Mobivity
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);
            // cant work w/ null
            if (products == null) return;

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

    }
}

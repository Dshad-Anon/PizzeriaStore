using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Models;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<PizzaDB>(options => options.UseInMemoryDatabase("items"));
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo{
        Title = "Pizzeria Store",
        Version = "v1",
        Description = "The API checkpoint for CRUD operations.",
    });  
});

var app = builder.Build();
if(app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pizzeria Version 1");
    });
}

// This retrieves list of pizza in form of list.
app.MapGet("/pizzas", async (PizzaDB db) => await db.Pizzas.ToListAsync()); 
//GET REQUEST USING THE ID:
app.MapGet("/pizza/{id}", async (PizzaDB db, int id)=> {
     await db.Pizzas.FindAsync(id);  
});
// To Post the data.
app.MapPost("/pizzas", async (PizzaDB db, Pizza pizza) => {
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}",pizza);
});
//Update the data cheking the id.
app.MapPut("/pizza/{id}", async (PizzaDB db, Pizza updatedPizza,int id) => 
{
    var pizza = await db.Pizzas.FindAsync(id);
    if(pizza == null) return Results.NotFound();
    pizza.Name = updatedPizza.Name;
    pizza.Description = updatedPizza.Description;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
//Delete a pizza from the list.
app.MapDelete("/pizza/{id}", async (PizzaDB db,int id) => {
    var pizza = await db.Pizzas.FindAsync(id);
    if(pizza == null) return Results.NotFound();
    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    return Results.Ok();
});
app.Run();

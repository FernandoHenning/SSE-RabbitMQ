using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SSE.Orders.Data;
using SSE.Orders.Models;
using SSE.Orders.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var initializer = services.GetRequiredService<DatabaseInitializer>();
    initializer.Initialize();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// GET /Orders - Retrieve all orders
app.MapGet("/Orders", (IOrderRepository orderRepository) =>
{
    var orders = (List<Order>)orderRepository.GetOrders();
    return orders.Count == 0 ? Results.NotFound("No orders found.") : Results.Ok(orders);
});

// POST /Orders - Create a new order
app.MapPost("/Orders", (IOrderRepository orderRepository, [FromBody] CreateOrderDto newOrder) =>
{
    if (string.IsNullOrEmpty(newOrder.OrderNumber))
    {
        return Results.BadRequest("Invalid order data.");
    }

    var order = new Order
    {
        OrderDate = newOrder.OrderDate,
        OrderNumber = newOrder.OrderNumber,
        ProductDescription = newOrder.ProductDescription,
        ShippingAddress = newOrder.ShippingAddress
    };

    orderRepository.CreateOrder(order);
    return Results.Created();
});



app.Run();
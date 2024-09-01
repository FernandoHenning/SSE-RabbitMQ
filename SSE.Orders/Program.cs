using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SSE.Orders.Data;
using SSE.Orders.Messaging;
using SSE.Orders.Models;
using SSE.Orders.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<IMessageProducer, MessageProducer>();
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
app.MapPost("/Orders", async (IOrderRepository orderRepository, IMessageProducer messageProducer, ILogger<Program> logger, [FromBody] CreateOrderDto newOrder) =>
{
    
    if ( !Validator.TryValidateObject(newOrder, new ValidationContext(newOrder), null, true))
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

    try
    {
        orderRepository.CreateOrder(order);
        var message = $"Order {order.OrderNumber} created: {order.ProductDescription}";
        await messageProducer.SendNewOrderMessageAsync(message);
    }
    catch (Exception ex)
    {
        logger.LogError("{ErrorMessage}",ex.Message);
        return Results.Text("Error creating the order.", statusCode: 500);
    }
    
    return Results.Created($"/Orders/{order.OrderNumber}", order); 
});



app.Run();
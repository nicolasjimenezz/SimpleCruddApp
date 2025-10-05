using Microsoft.EntityFrameworkCore;
using SimpleCrudApp.Data;
using SimpleCrudApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=todo.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Create DB if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Static files + middleware
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();

// CRUD endpoints
app.MapGet("/todos", async (AppDbContext db) =>
    await db.TodoItems.ToListAsync()).WithName("GetTodos");

app.MapPost("/todos", async (AppDbContext db, TodoItem item) =>
{
    db.TodoItems.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{item.Id}", item);
}).WithName("CreateTodo");

app.MapPut("/todos/{id}", async (AppDbContext db, int id, TodoItem inputItem) =>
{
    var item = await db.TodoItems.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.Title = inputItem.Title;
    item.IsDone = inputItem.IsDone;
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithName("UpdateTodo");

app.MapDelete("/todos/{id}", async (AppDbContext db, int id) =>
{
    var item = await db.TodoItems.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.TodoItems.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithName("DeleteTodo");

app.Run();

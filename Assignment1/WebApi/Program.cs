using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

List<TaskItem> tasks = new();

app.MapGet("/api/tasks", () => tasks);

app.MapPost("/api/tasks", (TaskItem newTask) =>
{
    newTask.Id = tasks.Count + 1;
    if (newTask.Description == null) newTask.Description = "";
    if (newTask.DueDate == DateTime.MinValue)
        newTask.DueDate = DateTime.UtcNow.AddDays(1);

    tasks.Add(newTask);
    return Results.Created($"/api/tasks/{newTask.Id}", newTask);
});

app.MapPut("/api/tasks/{id}", (int id, TaskItem updatedTask) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task is null) return Results.NotFound();
    task.Description = updatedTask.Description;
    task.IsCompleted = updatedTask.IsCompleted;
    task.DueDate = updatedTask.DueDate;
    return Results.Ok(task);
});

app.MapDelete("/api/tasks/{id}", (int id) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task is null) return Results.NotFound();
    tasks.Remove(task);
    return Results.Ok();
});

app.MapGet("/", () => "API is running!");
app.Run();

public class TaskItem
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public bool IsCompleted { get; set; } = false;
    public DateTime DueDate { get; set; }
}


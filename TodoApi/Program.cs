using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);


///swagger ////////////
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// הזרקת ה-DbContext לשירותים
builder.Services.AddDbContext<ToDoDbContex>(options =>
    options.UseMySql("Server=localhost;Database=ToDoDB;User=root;Password=aA1795aA;",
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql")));

//// cors /////////////
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin() 
                   .AllowAnyMethod() 
                   .AllowAnyHeader(); 
        });
});


var app = builder.Build();

app.UseCors("AllowAllOrigins"); 


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

///controller ////////
app.MapGet("/", () => "welcome");
// Route לשליפת כל המשימות
app.MapGet("/tasks", async (ToDoDbContex db) =>
{
    return await db.Items.ToListAsync(); // מחזיר את כל המשימות
});

// Route להוספת משימה חדשה
app.MapPost("/tasks", async (ToDoDbContex db, Item newTask) =>
{
    db.Items.Add(newTask);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{newTask.Id}", newTask); // מחזיר את המשימה שנוספה
});

// Route לעדכון משימה
app.MapPut("/tasks/{id}", async (ToDoDbContex db, int id, Item updatedTask) =>
{
    var task = await db.Items.FindAsync(id);
    if (task is null) return Results.NotFound();

    task.Name = updatedTask.Name; // עדכון שם המשימה
    task.IsComplet = updatedTask.IsComplet; // עדכון סטטוס ההשלמה
    await db.SaveChangesAsync();
    return Results.NoContent(); // מחזיר תשובה ריקה
});

// Route למחיקת משימה
app.MapDelete("/tasks/{id}", async (ToDoDbContex db, int id) =>
{
    var task = await db.Items.FindAsync(id);
    if (task is null) return Results.NotFound();

    db.Items.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent(); // מחזיר תשובה ריקה
});

app.Run();
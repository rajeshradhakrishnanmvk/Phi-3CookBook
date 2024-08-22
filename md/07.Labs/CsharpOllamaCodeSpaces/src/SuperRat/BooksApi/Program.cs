// ref: https://www.youtube.com/watch?v=JG2TeGBs8MU (ASP.NET 6 Minimal Web API with SQLite)
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LibraryDbContext>(option => option.UseSqlite(connectionString));

// Add Cors
builder.Services.AddCors(o => o.AddPolicy("Policy", builder => {
  builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
}));


var app = builder.Build();

app.UseCors("Policy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var books = app.MapGroup("/api/books");

app.MapGet("/", BookService.GetAllBooks);
app.MapGet("/library/{library}", BookService.GeBooksByLibrary);
app.MapGet("/{id}", BookService.GetBookById);
app.MapPost("/", BookService.InsertBook);
app.MapPut("/{id}", BookService.UpdateBook);
app.MapDelete("/{id}", BookService.DeleteBook);

app.Run();
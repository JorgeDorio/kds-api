using Kds;
using Kds.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<Settings>(
    builder.Configuration.GetSection("Settings"));

builder.Services.AddSingleton<StationService>();
builder.Services.AddSingleton<OrderService>();
builder.Services.AddSingleton<FlowService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<AuthService>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

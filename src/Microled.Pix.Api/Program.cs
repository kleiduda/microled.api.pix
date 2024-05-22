using Microled.Pix.Application;
using Microled.Pix.Application.Interface;
using Microled.Pix.Infra.Helpers;
using Microled.Pix.Infra.Helpers.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

//dependency injection
builder.Services.AddScoped<IPixBradescoHelper, PixBradescoHelpers>();
builder.Services.AddScoped<IPixBradescoService, PixBradescoService>();

builder.Services.AddScoped<IPixItauHelper, PixItauHelpers>();
builder.Services.AddScoped<IPixItauService, PixItauService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

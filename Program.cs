var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<EnergyMixClient>();
builder.Services.AddScoped<EnergyMixService>();

builder.Services.AddCors(options =>
{
    var allowedOriginsValue = builder.Configuration["Cors:AllowedOrigins"];

    if (string.IsNullOrWhiteSpace(allowedOriginsValue))
    {
        throw new InvalidOperationException("CORS allowed origins are not configured.");
    }

    var allowedOrigins = allowedOriginsValue.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    options.AddPolicy("Frontend",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.MapControllers();

app.Run();

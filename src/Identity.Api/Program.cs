using NEFORmal.ua.Identity.Api.Apis;
using NEFORmal.ua.Identity.Api.Configs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureDatabase      (builder.Configuration);
builder.Services.ConfigureAuthorization (builder.Configuration);
builder.Services.ConfigureIdentity      ();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

IdentityApi.MapRoutes(app);

app.Run();

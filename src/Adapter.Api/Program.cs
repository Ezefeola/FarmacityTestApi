using Adapter.Api.Extensions;
using CompositionRoot;

var builder = WebApplication.CreateBuilder(args);

#region Services Area

builder.Services.AddApiConfig(builder.Configuration);
builder.Services.AddCompositionRoot(builder.Configuration);

#endregion Services Area

var app = builder.Build();

#region Middlewares Area

app.AddApiWebApplicationConfig();

#endregion Middlewares Area

await app.RunAsync();
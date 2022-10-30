using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Pizza.Api;
using Slugify;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IAmazonDynamoDB>( _ =>
  new AmazonDynamoDBClient(RegionEndpoint.USEast1));
builder.Services.AddSingleton<IDynamoDBContext>(p =>
  new DynamoDBContext(p.GetService<IAmazonDynamoDB>()));
builder.Services.AddScoped<ISlugHelper, SlugHelper>();
builder.Services.AddScoped<PizzaHandler>();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

var app = builder.Build();

app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda");

using (var serviceScope = app.Services.CreateScope())
{
  var services = serviceScope.ServiceProvider;
  var pizzaApi = services.GetRequiredService<PizzaHandler>();

  app.MapPost("/pizzas", pizzaApi.MakePizza);
  app.MapGet("/pizzas/{url}", pizzaApi.TastePizza);
}

app.Run();

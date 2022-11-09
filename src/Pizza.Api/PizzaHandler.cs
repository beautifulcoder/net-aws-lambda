namespace Pizza.Api;

public class PizzaHandler
{
  private readonly ISlugHelper _slugHelper;
  private readonly IDynamoDBContext _context;
  private readonly ILogger<PizzaHandler> _logger;

  public PizzaHandler(
    IDynamoDBContext context,
    ISlugHelper slugHelper,
    ILogger<PizzaHandler> logger)
  {
    _context = context;
    _slugHelper = slugHelper;
    _logger = logger;
  }

  public async Task<IResult> MakePizza(PizzaModel pizza)
  {
    if (string.IsNullOrWhiteSpace(pizza.Name)
      || pizza.Ingredients.Count == 0)
    {
      return Results.ValidationProblem(new Dictionary<string, string[]>
      {
        {nameof(pizza), new [] {"To make a pizza include name and ingredients"}}
      });
    }

    pizza.Url = _slugHelper.GenerateSlug(pizza.Name);

    await _context.SaveAsync(pizza);
    _logger.LogInformation($"Pizza made! {pizza}");

    return Results.Created($"/pizzas/{pizza.Url}", pizza);
  }

  public async Task<IResult> TastePizza(string url)
  {
    var pizza = await _context.LoadAsync<PizzaModel?>(url);

    return pizza == null
      ? Results.NotFound()
      : Results.Ok(pizza);
  }
}

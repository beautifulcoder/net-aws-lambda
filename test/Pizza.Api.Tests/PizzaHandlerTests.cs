namespace Pizza.Api.Tests;

public class PizzaHandlerTests
{
  private readonly Mock<IDynamoDBContext> _context;
  private readonly PizzaHandler _handler;

  public PizzaHandlerTests()
  {
    var slugHelper = new Mock<ISlugHelper>();
    _context = new Mock<IDynamoDBContext>();
    var logger = new Mock<ILogger<PizzaHandler>>();

    _handler = new PizzaHandler(
      _context.Object,
      slugHelper.Object,
      logger.Object);
  }

  [Fact]
  public async Task MakePizzaCreated()
  {
    // arrange
    var pizza = new PizzaModel
    {
      Name = "Name",
      Ingredients = new List<string> {"toppings"}
    };

    // act
    var result = await _handler.MakePizza(pizza);

    // assert
    Assert.Equal("CreatedResult", result.GetType().Name);
  }

  [Fact]
  public async Task MakePizzaBadRequest()
  {
    // arrange
    var pizza = new PizzaModel
    {
      Name = "Name"
    };

    // act
    var result = await _handler.MakePizza(pizza);

    // assert
    Assert.Equal("ObjectResult", result.GetType().Name);
  }

  [Fact]
  public async Task TastePizzaOk()
  {
    // arrange
    _context
      .Setup(m => m.LoadAsync<PizzaModel?>("url", default))
      .ReturnsAsync(new PizzaModel());

    // act
    var result = await _handler.TastePizza("url");

    // assert
    Assert.Equal("OkObjectResult", result.GetType().Name);
  }

  [Fact]
  public async Task TastePizzaNotFound()
  {
    // arrange
    _context
      .Setup(m => m.LoadAsync<PizzaModel?>("url", default))
      .ReturnsAsync((PizzaModel?)null);

    // act
    var result = await _handler.TastePizza("url");

    // assert
    Assert.Equal("NotFoundObjectResult", result.GetType().Name);
  }
}

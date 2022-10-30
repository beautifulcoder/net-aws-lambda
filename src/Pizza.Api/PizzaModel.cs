using Amazon.DynamoDBv2.DataModel;

namespace Pizza.Api;

[DynamoDBTable("pizzas")]
public class PizzaModel
{
  [DynamoDBHashKey]
  [DynamoDBProperty("url")]
  public string Url { get; set; } = string.Empty;

  [DynamoDBProperty("name")]
  public string Name { get; set; } = string.Empty;

  [DynamoDBProperty("ingredients")]
  public List<string> Ingredients { get; set; } = new();

  public override string ToString() =>
    $"{Name}: {string.Join(',', Ingredients)}";
}

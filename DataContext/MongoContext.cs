namespace Caketime.Context;

public interface IMongoContext
{
    IMongoClient Client { get; }
    IMongoDatabase Database { get; }
    IMongoCollection<Recipe> RecipeCollection { get; }
    IMongoCollection<Instruction> InstructionCollection { get; }
    IMongoCollection<Ingredient> IngredientCollection { get; }
    IMongoCollection<Category> CategoryCollection { get; }
}

public class MongoContext : IMongoContext
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly DatabaseSettings _settings;

    public IMongoClient Client
    {
        get
        {
            return _client;
        }
    }

    public IMongoDatabase Database => _database;

    public MongoContext(IOptions<DatabaseSettings> dbOptions)
    {
        _settings = dbOptions.Value;
        _client = new MongoClient(_settings.ConnectionString);
        _database = _client.GetDatabase(_settings.DatabaseName);
    }

    public IMongoCollection<Recipe> RecipeCollection
    {
        get
        {
            return _database.GetCollection<Recipe>(_settings.RecipeCollection);
        }
    }
    public IMongoCollection<Instruction> InstructionCollection
    {
        get
        {
            return _database.GetCollection<Instruction>(_settings.InstructionCollection);
        }
    }
    public IMongoCollection<Ingredient> IngredientCollection
    {
        get
        {
            return _database.GetCollection<Ingredient>(_settings.IngredientCollection);
        }
    }
    public IMongoCollection<Category> CategoryCollection
    {
        get
        {
            return _database.GetCollection<Category>(_settings.CategoryCollection);
        }
    }

}
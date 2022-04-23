namespace Caketime.Models;

public class Instruction
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Manual { get; set; }
}
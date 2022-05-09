namespace Caketime.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Omschrijving { get; set; }
    public int Stock { get; set; }
    public double Prijs { get; set; }
    public bool Promo { get; set; }
}
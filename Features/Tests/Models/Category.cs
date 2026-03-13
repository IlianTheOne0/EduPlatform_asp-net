namespace TestingPlatform.Features.Tests.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Test> Tests { get; set; } = new List<Test>();

    public override string ToString() => $"Category: {Name} (ID: {Id})";
}
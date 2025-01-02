namespace RealtyHub.Core.Models;

public class PropertyImage : Entity
{
    public long Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long PropertyId { get; set; }
     public Property Property { get; set; } = new();
     public bool IsActive { get; set; }
}
using BuildingBlocks.Domain;

namespace Modules.Identity.Domain.Entities;

public class City : Entity<int>
{
    public string Name { get; private set; } = string.Empty;
    public int PlateCode { get; private set; }
    public bool IsActive { get; private set; }

    private City() { }

    public City(int id, string name, int plateCode, bool isActive = true)
    {
        Id = id;
        Name = name;
        PlateCode = plateCode;
        IsActive = isActive;
    }
}

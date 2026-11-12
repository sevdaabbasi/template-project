using BuildingBlocks.Domain;

namespace Modules.Identity.Domain.Entities;

public class Interest : Entity<int>
{
    public string Name { get; private set; } = string.Empty;
    public string IconName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private Interest() { }

    public Interest(int id, string name, string iconName, bool isActive = true)
    {
        Id = id;
        Name = name;
        IconName = iconName;
        IsActive = isActive;
    }
}

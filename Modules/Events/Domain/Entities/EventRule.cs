using BuildingBlocks.Domain;

namespace Modules.Events.Domain.Entities;

public class EventRule : Entity<Guid>
{
    public Guid EventId { get; private set; }
    public string RuleText { get; private set; } = string.Empty;
    public bool IsCustom { get; private set; }
    public int DisplayOrder { get; private set; }

    private EventRule() { }

    public EventRule(Guid eventId, string ruleText, bool isCustom = false, int displayOrder = 0)
    {
        EventId = eventId;
        RuleText = ruleText.Trim();
        IsCustom = isCustom;
        DisplayOrder = displayOrder;
    }
}
using System.Text;
using BuildingBlocks.Domain;
using Modules.Events.Domain.Enums;
using Modules.Events.Domain.Events;

namespace Modules.Events.Domain.Entities;

public class Event : AggregateRoot<Guid>
{
    public Guid OrganizerId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public DateTime EventDate { get; private set; }
    public string LocationUrl { get; private set; } = string.Empty;
    public string? LocationName { get; private set; }
    public int? CityId { get; private set; }
    public int? InterestId { get; private set; }
    public int? Capacity { get; private set; }
    public bool IsPaid { get; private set; }
    public decimal? Price { get; private set; }
    public string? GroupChatUrl { get; private set; }
    public string? ExtraNotes { get; private set; }
    public EventStatus Status { get; private set; } = EventStatus.Published;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<EventRule> _rules = new();
    public IReadOnlyCollection<EventRule> Rules => _rules.AsReadOnly();

    private readonly List<EventParticipant> _participants = new();
    public IReadOnlyCollection<EventParticipant> Participants => _participants.AsReadOnly();

    private Event() { }

    public static Event Create(
        Guid organizerId,
        string title,
        DateTime eventDate,
        string locationUrl,
        string? locationName = null,
        int? cityId = null,
        int? interestId = null,
        int? capacity = null,
        bool isPaid = false,
        decimal? price = null,
        string? groupChatUrl = null,
        string? extraNotes = null)
    {
        var ev = new Event
        {
            Id = Guid.NewGuid(),
            OrganizerId = organizerId,
            Title = title.Trim(),
            EventDate = eventDate,
            LocationUrl = locationUrl.Trim(),
            LocationName = locationName?.Trim(),
            CityId = cityId,
            InterestId = interestId,
            Capacity = capacity,
            IsPaid = isPaid,
            Price = isPaid ? price : null,
            GroupChatUrl = groupChatUrl?.Trim(),
            ExtraNotes = extraNotes?.Trim(),
            Status = EventStatus.Published,
            CreatedAt = DateTime.UtcNow
        };

        ev.AddDomainEvent(new EventCreatedDomainEvent(ev.Id, ev.OrganizerId, ev.Title, ev.EventDate));
        return ev;
    }

    public void AddRule(string ruleText, bool isCustom = false)
    {
        if (string.IsNullOrWhiteSpace(ruleText)) return;
        var order = _rules.Count + 1;
        _rules.Add(new EventRule(Id, ruleText, isCustom, order));
    }

    public void AddParticipant(Guid userId)
    {
        if (Status != EventStatus.Published)
            throw new InvalidOperationException("Yalnızca yayında olan etkinliklere katılım sağlanabilir.");

        if (Capacity.HasValue && _participants.Count >= Capacity.Value)
            throw new InvalidOperationException("Etkinlik kontenjanı dolmuştur.");

        if (_participants.Any(p => p.UserId == userId))
            throw new InvalidOperationException("Bu etkinliğe zaten katıldınız.");

        _participants.Add(new EventParticipant(Id, userId));
    }

    public void RemoveParticipant(Guid userId)
    {
        var participant = _participants.FirstOrDefault(p => p.UserId == userId);
        if (participant != null)
        {
            _participants.Remove(participant);
        }
    }

    public void Cancel(string reason)
    {
        Status = EventStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new EventCancelledDomainEvent(Id, reason));
    }

    /// <summary>
    /// Ekrandaki WhatsApp formatında kopyalanabilir canlı duyuru metnini üretir.
    /// </summary>
    public string GenerateWhatsAppAnnouncement()
    {
        return BuildWhatsAppAnnouncement(
            Title,
            EventDate,
            LocationName,
            LocationUrl,
            Capacity,
            IsPaid,
            Price,
            _rules.OrderBy(r => r.DisplayOrder).Select(r => r.RuleText),
            ExtraNotes,
            GroupChatUrl
        );
    }

    /// <summary>
    /// Statik duyuru şablonu oluşturucu — henüz DB'ye kaydedilmemiş önizlemeler için de kullanılabilir.
    /// </summary>
    public static string BuildWhatsAppAnnouncement(
        string title,
        DateTime eventDate,
        string? locationName,
        string? locationUrl,
        int? capacity,
        bool isPaid,
        decimal? price,
        IEnumerable<string>? rules,
        string? extraNotes,
        string? groupChatUrl)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Değerli Katılımcılarımız;\n");
        sb.AppendLine($"🗓️ *{title}* etkinliğimiz *{eventDate:dd.MM.yyyy - HH:mm}* tarihinde gerçekleşecektir.\n");

        if (!string.IsNullOrWhiteSpace(locationName))
        {
            sb.AppendLine($"📍 *Mekan:* {locationName}");
        }

        if (!string.IsNullOrWhiteSpace(locationUrl))
        {
            sb.AppendLine($"📍 *Konum:* {locationUrl}");
        }

        if (capacity.HasValue && capacity > 0)
        {
            sb.AppendLine($"👥 *Kontenjan:* {capacity} Kişi");
        }

        if (isPaid && price.HasValue)
        {
            sb.AppendLine($"💳 *Ücret:* {price:N0} ₺");
        }

        var ruleList = rules?.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
        if (ruleList != null && ruleList.Any())
        {
            sb.AppendLine("\n⚠️ *Kurallar:*");
            foreach (var rule in ruleList)
            {
                sb.AppendLine($"• {rule.Trim()}");
            }
        }

        if (!string.IsNullOrWhiteSpace(extraNotes))
        {
            sb.AppendLine($"\n📝 *Not:* {extraNotes.Trim()}");
        }

        if (!string.IsNullOrWhiteSpace(groupChatUrl))
        {
            sb.AppendLine($"\n💬 *Etkinlik Grubu:* {groupChatUrl.Trim()}");
        }

        sb.AppendLine("\nKatılımlarınızı bekliyoruz! 🌿");
        return sb.ToString();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Identity.Domain.Entities;

namespace Modules.Identity.Infrastructure.Persistence.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities", "identity");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        // Türkiye'nin 81 İli (Plaka Kodları ile)
        builder.HasData(
            new City(1, "Adana", 1, true),
            new City(2, "Adıyaman", 2, true),
            new City(3, "Afyonkarahisar", 3, true),
            new City(4, "Ağrı", 4, true),
            new City(5, "Amasya", 5, true),
            new City(6, "Ankara", 6, true),
            new City(7, "Antalya", 7, true),
            new City(8, "Artvin", 8, true),
            new City(9, "Aydın", 9, true),
            new City(10, "Balıkesir", 10, true),
            new City(11, "Bilecik", 11, true),
            new City(12, "Bingöl", 12, true),
            new City(13, "Bitlis", 13, true),
            new City(14, "Bolu", 14, true),
            new City(15, "Burdur", 15, true),
            new City(16, "Bursa", 16, true),
            new City(17, "Çanakkale", 17, true),
            new City(18, "Çankırı", 18, true),
            new City(19, "Çorum", 19, true),
            new City(20, "Denizli", 20, true),
            new City(21, "Diyarbakır", 21, true),
            new City(22, "Edirne", 22, true),
            new City(23, "Elazığ", 23, true),
            new City(24, "Erzincan", 24, true),
            new City(25, "Erzurum", 25, true),
            new City(26, "Eskişehir", 26, true),
            new City(27, "Gaziantep", 27, true),
            new City(28, "Giresun", 28, true),
            new City(29, "Gümüşhane", 29, true),
            new City(30, "Hakkari", 30, true),
            new City(31, "Hatay", 31, true),
            new City(32, "Isparta", 32, true),
            new City(33, "Mersin", 33, true),
            new City(34, "İstanbul", 34, true),
            new City(35, "İzmir", 35, true),
            new City(36, "Kars", 36, true),
            new City(37, "Kastamonu", 37, true),
            new City(38, "Kayseri", 38, true),
            new City(39, "Kırklareli", 39, true),
            new City(40, "Kırşehir", 40, true),
            new City(41, "Kocaeli", 41, true),
            new City(42, "Konya", 42, true),
            new City(43, "Kütahya", 43, true),
            new City(44, "Malatya", 44, true),
            new City(45, "Manisa", 45, true),
            new City(46, "Kahramanmaraş", 46, true),
            new City(47, "Mardin", 47, true),
            new City(48, "Muğla", 48, true),
            new City(49, "Muş", 49, true),
            new City(50, "Nevşehir", 50, true),
            new City(51, "Niğde", 51, true),
            new City(52, "Ordu", 52, true),
            new City(53, "Rize", 53, true),
            new City(54, "Sakarya", 54, true),
            new City(55, "Samsun", 55, true),
            new City(56, "Siirt", 56, true),
            new City(57, "Sinop", 57, true),
            new City(58, "Sivas", 58, true),
            new City(59, "Tekirdağ", 59, true),
            new City(60, "Tokat", 60, true),
            new City(61, "Trabzon", 61, true),
            new City(62, "Tunceli", 62, true),
            new City(63, "Şanlıurfa", 63, true),
            new City(64, "Uşak", 64, true),
            new City(65, "Van", 65, true),
            new City(66, "Yozgat", 66, true),
            new City(67, "Zonguldak", 67, true),
            new City(68, "Aksaray", 68, true),
            new City(69, "Bayburt", 69, true),
            new City(70, "Karaman", 70, true),
            new City(71, "Kırıkkale", 71, true),
            new City(72, "Batman", 72, true),
            new City(73, "Şırnak", 73, true),
            new City(74, "Bartın", 74, true),
            new City(75, "Ardahan", 75, true),
            new City(76, "Iğdır", 76, true),
            new City(77, "Yalova", 77, true),
            new City(78, "Karabük", 78, true),
            new City(79, "Kilis", 79, true),
            new City(80, "Osmaniye", 80, true),
            new City(81, "Düzce", 81, true)
        );
    }
}

public class InterestConfiguration : IEntityTypeConfiguration<Interest>
{
    public void Configure(EntityTypeBuilder<Interest> builder)
    {
        builder.ToTable("Interests", "identity");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.IconName)
            .HasMaxLength(50);

        // Designs: Doğa, Kültür, Spor, Sanat
        builder.HasData(
            new Interest(1, "Doğa", "leaf", true),
            new Interest(2, "Kültür", "landmark", true),
            new Interest(3, "Spor", "dumbbell", true),
            new Interest(4, "Sanat", "palette", true)
        );
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Modules.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAll81CitiesSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "identity",
                table: "Cities",
                columns: new[] { "Id", "IsActive", "Name", "PlateCode" },
                values: new object[,]
                {
                    { 1, true, "Adana", 1 },
                    { 2, true, "Adıyaman", 2 },
                    { 3, true, "Afyonkarahisar", 3 },
                    { 4, true, "Ağrı", 4 },
                    { 5, true, "Amasya", 5 },
                    { 8, true, "Artvin", 8 },
                    { 9, true, "Aydın", 9 },
                    { 10, true, "Balıkesir", 10 },
                    { 11, true, "Bilecik", 11 },
                    { 12, true, "Bingöl", 12 },
                    { 13, true, "Bitlis", 13 },
                    { 14, true, "Bolu", 14 },
                    { 15, true, "Burdur", 15 },
                    { 17, true, "Çanakkale", 17 },
                    { 18, true, "Çankırı", 18 },
                    { 19, true, "Çorum", 19 },
                    { 20, true, "Denizli", 20 },
                    { 22, true, "Edirne", 22 },
                    { 23, true, "Elazığ", 23 },
                    { 24, true, "Erzincan", 24 },
                    { 25, true, "Erzurum", 25 },
                    { 26, true, "Eskişehir", 26 },
                    { 27, true, "Gaziantep", 27 },
                    { 28, true, "Giresun", 28 },
                    { 29, true, "Gümüşhane", 29 },
                    { 30, true, "Hakkari", 30 },
                    { 31, true, "Hatay", 31 },
                    { 32, true, "Isparta", 32 },
                    { 33, true, "Mersin", 33 },
                    { 36, true, "Kars", 36 },
                    { 37, true, "Kastamonu", 37 },
                    { 38, true, "Kayseri", 38 },
                    { 39, true, "Kırklareli", 39 },
                    { 40, true, "Kırşehir", 40 },
                    { 41, true, "Kocaeli", 41 },
                    { 42, true, "Konya", 42 },
                    { 43, true, "Kütahya", 43 },
                    { 44, true, "Malatya", 44 },
                    { 45, true, "Manisa", 45 },
                    { 46, true, "Kahramanmaraş", 46 },
                    { 47, true, "Mardin", 47 },
                    { 48, true, "Muğla", 48 },
                    { 49, true, "Muş", 49 },
                    { 50, true, "Nevşehir", 50 },
                    { 51, true, "Niğde", 51 },
                    { 52, true, "Ordu", 52 },
                    { 53, true, "Rize", 53 },
                    { 54, true, "Sakarya", 54 },
                    { 55, true, "Samsun", 55 },
                    { 56, true, "Siirt", 56 },
                    { 57, true, "Sinop", 57 },
                    { 58, true, "Sivas", 58 },
                    { 59, true, "Tekirdağ", 59 },
                    { 60, true, "Tokat", 60 },
                    { 62, true, "Tunceli", 62 },
                    { 63, true, "Şanlıurfa", 63 },
                    { 64, true, "Uşak", 64 },
                    { 65, true, "Van", 65 },
                    { 66, true, "Yozgat", 66 },
                    { 67, true, "Zonguldak", 67 },
                    { 68, true, "Aksaray", 68 },
                    { 69, true, "Bayburt", 69 },
                    { 70, true, "Karaman", 70 },
                    { 71, true, "Kırıkkale", 71 },
                    { 72, true, "Batman", 72 },
                    { 73, true, "Şırnak", 73 },
                    { 74, true, "Bartın", 74 },
                    { 75, true, "Ardahan", 75 },
                    { 76, true, "Iğdır", 76 },
                    { 77, true, "Yalova", 77 },
                    { 78, true, "Karabük", 78 },
                    { 79, true, "Kilis", 79 },
                    { 80, true, "Osmaniye", 80 },
                    { 81, true, "Düzce", 81 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Cities",
                keyColumn: "Id",
                keyValue: 81);
        }
    }
}

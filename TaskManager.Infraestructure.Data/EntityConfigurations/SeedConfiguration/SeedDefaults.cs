namespace TaskManager.Infraestructure.Data.EntityConfigurations.SeedConfiguration
{
    public static class SeedDefaults
    {
        private static readonly TimeZoneInfo ColombiaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

        private static readonly DateTime BaseCreationDateUtc =
            new DateTime(2025, 09, 01, 0, 0, 0, DateTimeKind.Utc);

        public static readonly DateTime CreationDate =
            TimeZoneInfo.ConvertTimeFromUtc(BaseCreationDateUtc, ColombiaTimeZone);

        // Usuario por defecto
        public static readonly Guid DefaultUserId =
            Guid.Parse("123e4567-e89b-12d3-a456-426655440000");
    }
}
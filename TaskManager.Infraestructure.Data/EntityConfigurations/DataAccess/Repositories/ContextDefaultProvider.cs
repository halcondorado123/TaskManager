using Microsoft.AspNetCore.Http;
using TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Contracts;

namespace TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Repositories
{
    public class ContextDefaultProvider : IContextDefaultProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly TimeZoneInfo ColombiaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

        public ContextDefaultProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Fecha actual en UTC (para guardar en BD)
        public DateTime UtcNow => DateTime.UtcNow;

        // Fecha actual convertida a Colombia (para mostrar en UI si quieres)
        public DateTime NowColombia =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ColombiaTimeZone);

        // Usuario actual -> por ahora un GUID fijo
        public Guid CurrentUserId
        {
            get
            {
                // 🚩 cuando no tengas autenticación
                var defaultUser = Guid.Parse("00000000-0000-0000-0000-000000000001");

                // 🚩 cuando tengas autenticación, lo descomentas
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsed))
                    return parsed;

                return defaultUser;
            }
        }
    }
}

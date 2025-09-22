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

        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime NowColombia =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ColombiaTimeZone);

        public Guid CurrentUserId
        {
            get
            {
                var defaultUser = Guid.Parse("00000000-0000-0000-0000-000000000001");

                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsed))
                    return parsed;

                return defaultUser;
            }
        }
    }
}

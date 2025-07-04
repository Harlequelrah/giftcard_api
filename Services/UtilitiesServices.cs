using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using giftcard_api.Data;



namespace giftcard_api.Services
{
    public interface IRoleService
    {
        Task<string> GetRoleNameByIdAsync(int? idRole);
    }

    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetRoleNameByIdAsync(int? idRole)
        {
            var role = await _context.Roles
                .Where(r => r.Id == idRole)
                .Select(r => r.RoleNom)
                .FirstOrDefaultAsync();

            return role;
        }
    }
    public static class UtilityDate
    {
        public static string GetDate(string format = "yyyy-MM-dd HH:mm:ss")
        {
            return DateTime.UtcNow.ToString(format);
        }
        public static string FormatDate(DateTime date ,string format = "yyyy-MM-dd HH:mm:ss")
        {
            return date.ToString(format);
        }


    }




}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TMAProject.DataAccess;
using TMAProject.Models.Entities;

namespace TMAProject.Comomn.DBInitilizer
{
    public class DBInitilizer : IDBInitilizer
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DBInitilizer> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DBInitilizer(ApplicationDbContext context, ILogger<DBInitilizer> logger, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        void IDBInitilizer.Initialize()
        {
            try
            {
                if(_context.Database.GetPendingMigrations().Any())
                    _context.Database.Migrate();

                if (!_roleManager.RoleExistsAsync(Role.Admin).GetAwaiter().GetResult())
                    _roleManager.CreateAsync(new(Role.Admin)).GetAwaiter().GetResult();
                
                if (!_roleManager.RoleExistsAsync(Role.Customer).GetAwaiter().GetResult())
                    _roleManager.CreateAsync(new(Role.Customer)).GetAwaiter().GetResult();

                var admin = _userManager
                    .FindByEmailAsync("tmaadmin@tma.com")
                    .GetAwaiter().GetResult();

                if(admin is null)
                {
                    admin = new ApplicationUser
                    {
                        Email = "tmaadmin@tma.com",
                        UserName = "TmaAdmin",
                        EmailConfirmed = true,
                        FirstName = "Tma",
                        LastName = "Admin",
                        IsActive = true,
                    };

                    var result = _userManager
                        .CreateAsync(admin, "Admin@2792003")
                        .GetAwaiter()
                        .GetResult();

                    if (result.Succeeded)
                    {
                        _userManager.AddToRoleAsync(admin, Role.Admin).GetAwaiter().GetResult();
                    }

                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
        }
    }
}

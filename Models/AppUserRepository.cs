using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WorkMate.Models
{

    /// <summary>
    /// BL class for User Modification and setup
    /// </summary>
    public class AppUserRepository: IDisposable
    {
        private readonly AppUserManager _userManager;

        public AppUserRepository()
        {
            _userManager = AppUserManager.Create();
        }

        public async Task<IdentityResult> RegisterUserAsync(AppUsers user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<AppUsers> FindUserAsync(string username, string password)
        {
            return await _userManager.FindAsync(username, password);
        }

        public async Task<AppUsers> FindByIdAsync(int userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<AppUsers> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<IdentityResult> AddToRoleAsync(int userId, string role)
        {
            return await _userManager.AddToRoleAsync(userId, role);
        }
        public async Task<bool> IsInRoleAsync(int userId, string role)
        {
            return await _userManager.IsInRoleAsync(userId, role);
        }
        public async Task<IdentityResult> UpdateUserAsync(AppUsers user)
        {
            return await _userManager.UpdateAsync(user);
        }
        public async Task<IdentityResult> DeleteUserAsync(AppUsers user)
        {
            return await _userManager.DeleteAsync(user);
        }
        public void Dispose()
        {
            _userManager.Dispose();
        }
    }



}
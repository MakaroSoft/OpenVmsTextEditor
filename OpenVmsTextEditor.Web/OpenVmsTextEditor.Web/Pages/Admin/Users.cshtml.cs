using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OpenVmsTextEditor.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly ILogger<UsersModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersModel(
            ILogger<UsersModel> logger,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public sealed class UserWithRolesViewModel
        {
            public string UserId { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? UserName { get; set; }
            public List<string> Roles { get; set; } = new();
            public bool IsActivated { get; set; }
            public List<string> AllowedFolders { get; set; } = new();
        }

        public string? CurrentUserId { get; private set; }
        public List<UserWithRolesViewModel> Users { get; private set; } = new();
        public List<string> AllRoles { get; private set; } = new();

        public async Task OnGetAsync(CancellationToken ct)
        {
            _logger.LogDebug("Users GET");

            CurrentUserId = _userManager.GetUserId(User);

            AllRoles = await _roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(n => n)
                .ToListAsync(ct);

            var allUsers = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync(ct);

            var result = new List<UserWithRolesViewModel>(allUsers.Count);
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var claims = await _userManager.GetClaimsAsync(user);
                result.Add(new UserWithRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles.OrderBy(r => r).ToList(),
                    IsActivated = user.EmailConfirmed,
                    AllowedFolders = claims
                        .Where(c => string.Equals(c.Type, "AllowedFolder", StringComparison.Ordinal))
                        .Select(c => c.Value)
                        .OrderBy(v => v)
                        .ToList()
                });
            }

            Users = result;
        }

        public async Task<IActionResult> OnPostUpdateRolesAsync(string userId, List<string>? selectedRoles, string? allowedFolders, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("Missing userId");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var allRoles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync(ct);
            var currentRoles = await _userManager.GetRolesAsync(user);
            var desiredRoles = (selectedRoles ?? new List<string>()).Where(r => allRoles.Contains(r)).Distinct().ToList();

            // Prevent removing Admin from your own account
            var currentUserId = _userManager.GetUserId(User);
            if (string.Equals(currentUserId, userId, StringComparison.Ordinal) && currentRoles.Contains("Admin") && !desiredRoles.Contains("Admin"))
            {
                ModelState.AddModelError(string.Empty, "You cannot remove the Admin role from your own account.");
                await OnGetAsync(ct);
                return Page();
            }

            var rolesToAdd = desiredRoles.Except(currentRoles).ToList();
            var rolesToRemove = currentRoles.Except(desiredRoles).ToList();

            if (rolesToAdd.Count > 0)
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, string.Join("; ", addResult.Errors.Select(e => e.Description)));
                    await OnGetAsync(ct);
                    return Page();
                }
            }

            if (rolesToRemove.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, string.Join("; ", removeResult.Errors.Select(e => e.Description)));
                    await OnGetAsync(ct);
                    return Page();
                }
            }

            // If a user has only the 'User' role, they must have a list of allowed folders.
            var onlyUserRole = desiredRoles.Count == 1 && desiredRoles.Contains("User");
            if (onlyUserRole)
            {
                var parsedFolders = (allowedFolders ?? string.Empty)
                    .Replace("\r", string.Empty)
                    .Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => f.Trim())
                    .Where(f => !string.IsNullOrWhiteSpace(f))
                    .Distinct(StringComparer.Ordinal)
                    .ToList();

                var existingClaims = await _userManager.GetClaimsAsync(user);
                var allowedFolderClaims = existingClaims.Where(c => string.Equals(c.Type, "AllowedFolder", StringComparison.Ordinal)).ToList();

                // Remove claims no longer present (if parsedFolders is empty, this removes all)
                var claimsToRemove = allowedFolderClaims.Where(c => !parsedFolders.Contains(c.Value, StringComparer.Ordinal)).ToList();
                if (claimsToRemove.Count > 0)
                {
                    var rc = await _userManager.RemoveClaimsAsync(user, claimsToRemove);
                    if (!rc.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, string.Join("; ", rc.Errors.Select(e => e.Description)));
                        await OnGetAsync(ct);
                        return Page();
                    }
                }

                // Add new claims
                var existingValues = allowedFolderClaims.Select(c => c.Value).ToHashSet(StringComparer.Ordinal);
                var claimsToAdd = parsedFolders.Where(v => !existingValues.Contains(v)).Select(v => new System.Security.Claims.Claim("AllowedFolder", v)).ToList();
                if (claimsToAdd.Count > 0)
                {
                    var ac = await _userManager.AddClaimsAsync(user, claimsToAdd);
                    if (!ac.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, string.Join("; ", ac.Errors.Select(e => e.Description)));
                        await OnGetAsync(ct);
                        return Page();
                    }
                }
            }

            TempData["StatusMessage"] = $"Updated roles for {user.Email}";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string userId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("Missing userId");
            }

            var currentUserId = _userManager.GetUserId(User);
            if (string.Equals(currentUserId, userId, StringComparison.Ordinal))
            {
                TempData["StatusMessage"] = "You cannot delete your own account.";
                return RedirectToPage();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            TempData["StatusMessage"] = result.Succeeded
                ? $"Deleted user {user.Email}"
                : string.Join("; ", result.Errors.Select(e => e.Description));

            return RedirectToPage();
        }
    }
}

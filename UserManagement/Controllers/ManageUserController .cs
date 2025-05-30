using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace UserManagement.Controllers
{
    [Authorize]
    public class ManageUserController : Controller
    {
        private readonly DBContext _db;
        private readonly UserManager<UserDetails> _userManager;

        public ManageUserController(DBContext dbContext, UserManager<UserDetails> userManager)
        {
            _db = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> ShowUsers()
        {
            try
            {
                ViewBag.Users = await _db.Users
                    .Select(u => new 
                    { 
                        u.Name, 
                        u.Email, 
                        u.LastLoginDate,
                        u.IsBlocked
                    })
                    .OrderByDescending(u => u.LastLoginDate) 
                    .ToListAsync();
                    
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty,"Failed to load users. Please try again.");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockUsers([FromForm] List<string> selectedEmails)
        {
            if (selectedEmails == null || !selectedEmails.Any())
            {
                ModelState.AddModelError(string.Empty, "No users selected for blocking.");
                return RedirectToAction("ShowUsers");
            }

            try
            {
                await _db.Users.Where(u => selectedEmails.Contains(u.Email) && !u.IsBlocked)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(u => u.IsBlocked, u => true));

                var users = _userManager.Users.Where(u => selectedEmails.Contains(u.Email));
                foreach (var user in users)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                }

                TempData["SuccessMessage"] = $"Successfully blocked {selectedEmails.Count} user(s).";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Failed to block users. Please try again.");
            }

            return RedirectToAction("ShowUsers");
        }


        [HttpPost , ValidateAntiForgeryToken]
        public async Task<IActionResult> UnblockUsers([FromForm] List<string> selectedEmails)
        {
            if (selectedEmails == null || !selectedEmails.Any())
            {
                ModelState.AddModelError(string.Empty,"No users selected for unblocking.");
                return RedirectToAction("ShowUsers");
            }

            try
            {
                int updatedCount = await _db.Users
                    .Where(u => selectedEmails.Contains(u.Email) && u.IsBlocked)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(u => u.IsBlocked, u => false));

                TempData["SuccessMessage"] = $"Successfully unblocked {updatedCount} user(s).";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty,"Failed to unblock users. Please try again.");
            }

            return RedirectToAction("ShowUsers");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUsers([FromForm] List<string> selectedEmails)
        {
            if (selectedEmails == null || !selectedEmails.Any())
            {
                ModelState.AddModelError(string.Empty,"No users selected for deletion.");
                return RedirectToAction("ShowUsers");
            }

            try
            {
                int deletedCount = await _db.Users
                    .Where(u => selectedEmails.Contains(u.Email))
                    .ExecuteDeleteAsync();
                
                TempData["SuccessMessage"] = $"Successfully deleted {deletedCount} user(s).";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty,"Failed to delete users. Please try again.");
            }

            return RedirectToAction("ShowUsers");
        }
    }
}
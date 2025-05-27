using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Controllers
{
    public class ManageUser : Controller
    {
        private readonly DBContext _db;

        public ManageUser(DBContext dbContext)
        {
            _db = dbContext;
        }

        [HttpGet("ShowUsers")]
        public async Task<IActionResult> ShowUsers()
        {
            var users = await _db.Users.ToListAsync();
            return View(users);
        }

        [HttpPost("BlockUser")]
        public IActionResult BlockUser(List<UserViewModel> users)
        {
            var emails = users.Select(x => x.Email).ToList();

            var usersToBlock = _db.Users
                .Where(u => emails.Contains(u.Email) && u.IsBlocked == false)
                .ToList();

            foreach (var user in usersToBlock)
            {
                user.IsBlocked = true;
            }

            _db.Users.UpdateRange(usersToBlock);
            _db.SaveChanges();
            return RedirectToAction("ShowUsers");
        }

        [HttpPost("UnblockUser")]
        public IActionResult UnblockUser(List<UserViewModel> users)
        {
            var emails = users.Select(x => x.Email).ToList();

            var usersToUnblock = _db.Users
                .Where(u => emails.Contains(u.Email) && u.IsBlocked == true)
                .ToList();

            foreach (var user in usersToUnblock)
            {
                user.IsBlocked = false;
            }

            _db.Users.UpdateRange(usersToUnblock);
            _db.SaveChanges();
            return RedirectToAction("ShowUsers");
        }

        [HttpPost("DeleteUser")]
        public IActionResult DeleteUser(List<UserViewModel> users)
        {
            var emails = users.Select(x => x.Email).ToList();

            var usersToDelete = _db.Users
                .Where(u => emails.Contains(u.Email))
                .ToList();

            _db.Users.RemoveRange(usersToDelete);
            _db.SaveChanges();
            return RedirectToAction("ShowUsers");
        }
    }
}

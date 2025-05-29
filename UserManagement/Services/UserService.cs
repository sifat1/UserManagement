using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
using System.Collections.Generic;
using System.Linq;

public class UserService
{
    private DBContext _db;
    public UserService(DBContext db)
    {
        _db = db;
    }

    public bool IfUserExists(string Email)
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == Email && u.IsBlocked != true);

        return user != null ? true : false;
    }
}
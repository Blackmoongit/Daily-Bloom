using DailyBloom.Data;
using DailyBloom.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyBloom.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    public AuthService(AppDbContext db) => _db = db;

    public async Task<(bool Success, string Message, User? User)> RegisterAsync(
        string name, string username, string password, DateTime dob, string? profilePicturePath)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return (false, "Please fill in all required fields.", null);

        var exists = await _db.Users.AnyAsync(u => u.Username == username);
        if (exists) return (false, "That username is already taken.", null);

        var user = new User
        {
            Name = name,
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            DateOfBirth = dob,
            CreatedDate = DateTime.Now,
            ProfilePicturePath = profilePicturePath
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return (true, "Account created!", user);
    }

    public async Task<(bool Success, string Message, User? User)> LoginAsync(string username, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (false, "Invalid username or password.", null);

        return (true, "Welcome back!", user);
    }
}

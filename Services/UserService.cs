using MongoDB.Driver;
using NoteManagementSystem.Config;
using NoteManagementSystem.Models;
using System.Threading.Tasks;

namespace NoteManagementSystem.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(DatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>(settings.UsersCollection);
        }

        public async Task<User> GetByIdAsync(string id) =>
            await _users.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<User> GetByEmailAsync(string email) =>
            await _users.Find(x => x.Email == email).FirstOrDefaultAsync();

        public async Task<User> CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }


    public async Task<bool> ValidateCredentialsAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return false; // Prevent passing null values to BCrypt
        }

        var user = await GetByEmailAsync(email.ToLower());
        if (user == null)
        {
            return false; // User not found
        }

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            throw new InvalidOperationException("Stored password hash is null or empty.");
        }

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    }
}

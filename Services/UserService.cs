using MongoDB.Driver;


public class UserService
{
    private readonly IMongoCollection<User> _users;
    private readonly IMongoCollection<BlacklistedToken> _blacklistCollection;

    public UserService(MongoDbSettings MongoDbSettings)
    {
        
        var client = new MongoClient(MongoDbSettings.ConnectionString);

        var database = client.GetDatabase(MongoDbSettings.DatabaseName);
        _users = database.GetCollection<User>(MongoDbSettings.CollectionName);
        _blacklistCollection = database.GetCollection<BlacklistedToken>("BlacklistedTokens");
    }



    public async Task<User> GetUserByUsernameAsync(string username)
    {
        return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
    }
    public async Task CreateUserAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }
    public async Task BlacklistTokenAsync(string token)
    {
        var blacklistedToken = new BlacklistedToken
        {
            Id = Guid.NewGuid().ToString(),
            Token = token,
            BlacklistedAt = DateTime.UtcNow
        };

        await _blacklistCollection.InsertOneAsync(blacklistedToken);
    }

    // Check if a token is blacklisted
    public async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        return await _blacklistCollection
            .Find(t => t.Token == token)
            .AnyAsync();
    }
}
public class BlacklistedToken
{
    public required string Id { get; set; }
    public required string Token { get; set; }
    public DateTime BlacklistedAt { get; set; }
}





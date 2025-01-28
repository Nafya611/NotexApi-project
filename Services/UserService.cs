using MongoDB.Driver;


public class UserService
{
    private readonly IMongoCollection<User> _users;

    public UserService(MongoDbSettings MongoDbSettings)
    {
        
        var client = new MongoClient(MongoDbSettings.ConnectionString);

        var database = client.GetDatabase(MongoDbSettings.DatabaseName);
        _users = database.GetCollection<User>(MongoDbSettings.CollectionName);
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
    }
}

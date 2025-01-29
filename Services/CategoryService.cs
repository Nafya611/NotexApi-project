using MongoDB.Driver;
using NoteManagementSystem.Config;
using NoteManagementSystem.Models;

namespace NoteManagementSystem.Services
{
    public class CategoryService
    {
        private readonly IMongoCollection<Category> _categories;

        public CategoryService(DatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _categories = database.GetCollection<Category>(settings.CategoriesCollection);
        }

        public async Task<List<Category>> GetAsync(string userId) =>
            await _categories.Find(x => x.UserId == userId).ToListAsync();

        public async Task<Category> GetAsync(string id, string userId) =>
            await _categories.Find(x => x.Id == id && x.UserId == userId).FirstOrDefaultAsync();

        public async Task<Category> CreateAsync(Category category)
        {
            await _categories.InsertOneAsync(category);
            return category;
        }

        public async Task UpdateAsync(string id, Category categoryIn, string userId) =>
            await _categories.ReplaceOneAsync(x => x.Id == id && x.UserId == userId, categoryIn);

        public async Task RemoveAsync(string id, string userId) =>
            await _categories.DeleteOneAsync(x => x.Id == id && x.UserId == userId);
    }
}

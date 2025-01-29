using MongoDB.Driver;
using NoteManagementSystem.Config;
using NoteManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteManagementSystem.Services
{
    public class NoteService
    {
        private readonly IMongoCollection<Note> _notes;

        public NoteService(DatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _notes = database.GetCollection<Note>(settings.NotesCollection);
        }

        public async Task<List<Note>> GetAsync(string userId) =>
            await _notes.Find(x => x.UserId == userId).ToListAsync();

        public async Task<Note> GetAsync(string id, string userId) =>
            await _notes.Find(x => x.Id == id && x.UserId == userId).FirstOrDefaultAsync();

        public async Task<List<Note>> SearchAsync(string userId, string searchTerm)
        {
            var filter = Builders<Note>.Filter.And(
                Builders<Note>.Filter.Eq(x => x.UserId, userId),
                Builders<Note>.Filter.Or(
                    Builders<Note>.Filter.Regex(x => x.Title, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<Note>.Filter.Regex(x => x.Content, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
                )
            );

            return await _notes.Find(filter).ToListAsync();
        }

        public async Task<List<Note>> GetByCategoryAsync(string categoryId, string userId) =>
            await _notes.Find(x => x.CategoryId == categoryId && x.UserId == userId).ToListAsync();

        public async Task<Note> CreateAsync(Note note)
        {
            note.CreatedAt = DateTime.UtcNow;
            note.UpdatedAt = note.CreatedAt;
            await _notes.InsertOneAsync(note);
            return note;
        }

        public async Task UpdateAsync(string id, Note noteIn, string userId)
        {
            noteIn.UpdatedAt = DateTime.UtcNow;
            await _notes.ReplaceOneAsync(x => x.Id == id && x.UserId == userId, noteIn);
        }

        public async Task RemoveAsync(string id, string userId) =>
            await _notes.DeleteOneAsync(x => x.Id == id && x.UserId == userId);
    }
}

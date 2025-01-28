using MongoDB.Bson;
using MongoDB.Driver;
public interface INoteService
{
    Task<Note> CreateNoteAsync(Note note);
    Task<Note> GetNoteByIdAsync(string id);
    Task<IEnumerable<Note>> GetAllNotesAsync();
    Task<Note?> UpdateNoteAsync(string id, Note note);
    Task<bool> DeleteNoteAsync(string id);
}



public class NoteService : INoteService
{
    private readonly IMongoCollection<Note> _notesCollection;

    public NoteService(MongoDbSettings settings)
    {
       
        var client = new MongoClient(settings.ConnectionString);
        var db = client.GetDatabase(settings.DatabaseName);
        _notesCollection = db.GetCollection<Note>("notes");
    }


    // Create a new note
    public async Task<Note> CreateNoteAsync(Note note)
    {
        try
        {
            note.CreatedAt = DateTime.UtcNow;
            note.UpdatedAt = DateTime.UtcNow;
            await _notesCollection.InsertOneAsync(note);
            return note;
        }
        catch (Exception ex)
        {
            // Log the exception to see what's going wrong
            Console.WriteLine($"Error occurred while inserting the note: {ex.Message}");
            throw; // Re-throw the exception or handle it as needed
        }
    }


    // Get a note by its ID
    public async Task<Note> GetNoteByIdAsync(string id)
    {
        var objectId = new ObjectId(id);
        var note = await _notesCollection.Find(n => n.Id == objectId).FirstOrDefaultAsync();
        return note;
    }

    // Get all notes
    public async Task<IEnumerable<Note>> GetAllNotesAsync()
    {
        var notes = await _notesCollection.Find(_ => true).ToListAsync();
        return notes;
    }

    // Update an existing note
    public async Task<Note?> UpdateNoteAsync(string id, Note note)
    {
        var existingNote = await GetNoteByIdAsync(id);
        if (existingNote == null)
        {
            return null; // Note not found
        }

        existingNote.Title = note.Title;
        existingNote.Content = note.Content;
        existingNote.UpdatedAt = DateTime.UtcNow;

        await _notesCollection.ReplaceOneAsync(n => n.Id == new ObjectId(id), existingNote);
        return existingNote;
    }

    // Delete a note by its ID
    public async Task<bool> DeleteNoteAsync(string id)
    {
        var objectId = new ObjectId(id);
        var result = await _notesCollection.DeleteOneAsync(n => n.Id == objectId);
        return result.DeletedCount > 0;
    }
}

namespace NoteManagementSystem.Config
{
    public class DatabaseSettings
    {
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        public string? NotesCollection { get; set; }
        public string? CategoriesCollection { get; set; }
        public string? UsersCollection { get; set; }
    }
}

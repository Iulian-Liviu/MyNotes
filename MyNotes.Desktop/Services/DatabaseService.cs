using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MyNotes.Desktop.Models;

namespace MyNotes.Desktop.Services
{
    public static class DatabaseService
    {
        private static HttpClient httpClient = new HttpClient();

        public static event EventHandler? DatabaseOperation;
        public static async Task<List<Note>> GetAllNotes()
        {
            var response = await httpClient.GetAsync("http://localhost:5000/api/notes");
            if (response.IsSuccessStatusCode)
            {
                var databaseNotes = await JsonSerializer.DeserializeAsync<List<Note>>(await response.Content.ReadAsStreamAsync());
                if (databaseNotes != null && databaseNotes.Count > 0)
                {
                    return databaseNotes;
                }
            }
            return new List<Note>();
        }

        public static async Task<bool> AddNote(NoteUpload note)
        {
            if (!string.IsNullOrEmpty(note.Title))
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                using var stream = new MemoryStream();
                await JsonSerializer.SerializeAsync(stream, note);

                // Reset the stream position to read the output
                stream.Position = 0;

                // Read the JSON string from the stream
                using var streamReader = new StreamReader(stream);
                string noteAsJson = await streamReader.ReadToEndAsync();
                var response = await httpClient.PostAsync("http://localhost:5000/api/notes", new StringContent(noteAsJson, Encoding.UTF8, "application/json"));


                if (response.IsSuccessStatusCode)
                {
                    DatabaseOperation?.Invoke(null, new EventArgs());

                    return true;
                }

            }
            return false;

        }

        public static async Task<bool> UpdateNote(Guid noteId, NoteUpload note)
        {
            if (!string.IsNullOrEmpty(note.Title))
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                using var stream = new MemoryStream();
                await JsonSerializer.SerializeAsync(stream, note);

                // Reset the stream position to read the output
                stream.Position = 0;

                // Read the JSON string from the stream
                using var streamReader = new StreamReader(stream);
                string noteAsJson = await streamReader.ReadToEndAsync();
                var response = await httpClient.PutAsync("http://localhost:5000/api/notes?noteId=" + noteId, new StringContent(noteAsJson, Encoding.UTF8, "application/json"));


                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    DatabaseOperation?.Invoke(null, new EventArgs());

                    return true;
                }

            }
            return false;

        }

        public static async Task<bool> RemoveNote(Guid noteId)
        {
            var response = await httpClient.DeleteAsync("http://localhost:5000/api/notes?noteId=" + noteId.ToString());

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                DatabaseOperation?.Invoke(null, new EventArgs());

                return true;
            }
            return false;
        }
    }

}

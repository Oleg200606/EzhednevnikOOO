using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace EzhednevnikOOO
{
    public class Note
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }

    public static class DataManager
    {
        private static string filePath = "\\\\Mac\\Home\\Desktop\\notes.json";

        public static async Task SaveNotesAsync(List<Note> notes)
        {

            await Task.Run(() =>
            {
                var json = JsonSerializer.Serialize(notes);
                File.WriteAllText(filePath, json);
            });
        }

        public static async Task<List<Note>> LoadNotesAsync()
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(filePath))
                {
                    return new List<Note>();
                }

                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<Note>>(json) ?? new List<Note>();
            });
        }
    }

    public partial class MainWindow : Window
    {
        private List<Note> _notes = new List<Note>();

        public MainWindow()
        {
            InitializeComponent();
            LoadNotes();
        }

        private async void LoadNotes()
        {
            _notes = await DataManager.LoadNotesAsync();
            UpdateListBox(DATA.SelectedDate.GetValueOrDefault(DateTime.Now));
        }

        private void UpdateListBox(DateTime date)
        {
            Vivod.Items.Clear();
            foreach (var note in _notes.Where(n => n.Date.Date == date.Date))
            {
                Vivod.Items.Add(note.Title);
            }
        }

        private void Creater_Click(object sender, RoutedEventArgs e)
        {
            Vvod.Text = ""; 
            Vvod.IsEnabled = true;
        }

        private async void Sozdanie_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Vvod.Text))
            {
                var newNote = new Note
                {
                    Title = $"Заметка {DateTime.Now}", 
                    Description = Vvod.Text,
                    Date = DATA.SelectedDate.GetValueOrDefault(DateTime.Now)
                };
                _notes.Add(newNote);
                await DataManager.SaveNotesAsync(_notes);
                UpdateListBox(newNote.Date);
                Vvod.IsEnabled = false; 
            }
        }

        private async void Deletee_Click(object sender, RoutedEventArgs e)
        {
            var selectedTitle = Vivod.SelectedItem as string;
            var noteToRemove = _notes.FirstOrDefault(n => n.Title == selectedTitle);
            if (noteToRemove != null)
            {
                _notes.Remove(noteToRemove);
                await DataManager.SaveNotesAsync(_notes);
                UpdateListBox(DATA.SelectedDate.GetValueOrDefault(DateTime.Now));
            }
        }

        private void DATA_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DATA.SelectedDate.HasValue)
            {
                UpdateListBox(DATA.SelectedDate.Value);
            }
        }

        private void Vivod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTitle = Vivod.SelectedItem as string;
            var note = _notes.FirstOrDefault(n => n.Title == selectedTitle);
            if (note != null)
            {
                Vvod.Text = note.Description;
            }
        }
    }

}

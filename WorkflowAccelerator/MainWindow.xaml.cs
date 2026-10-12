using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace WorkflowAccelerator
{
    public partial class MainWindow : Window
    {
        private string _selectedFilePath = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        // Step 1: Select File
        private void BtnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
                TxtFilePath.Text = Path.GetFileName(_selectedFilePath);
                BtnProcess.IsEnabled = true; // Enable the process button
                Log("File selected: " + _selectedFilePath);
            }
        }

        // Step 2: Process Data (The "Heavy Workflow")
        private async void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            BtnProcess.IsEnabled = false;
            BtnSelectFile.IsEnabled = false;
            LogList.Items.Clear();
            ProgressBar.Value = 0;

            Log("Starting workflow...");

            // We run this on a background thread so the UI doesn't freeze!
            await Task.Run(() => ProcessCsvWorkflow());

            Log("Workflow completed successfully!");
            BtnSelectFile.IsEnabled = true;
            BtnProcess.IsEnabled = true;
        }

        // The actual logic (Multithreading & Database)
        private void ProcessCsvWorkflow()
        {
            var lines = File.ReadAllLines(_selectedFilePath);
            int totalLines = lines.Length;

            using (var db = new AppDbContext())
            {
                // Ensure database is created
                db.Database.EnsureCreated();

                for (int i = 1; i < totalLines; i++) // Skip header (i=1)
                {
                    string line = lines[i];

                    // Simulate heavy processing time
                    System.Threading.Thread.Sleep(50);

                    // Create a record and save to PostgreSQL
                    var record = new ProcessedRecord
                    {
                        OriginalData = line,
                        ProcessedResult = line.ToUpper() + " - PROCESSED", // Simple transformation
                        ProcessedAt = DateTime.Now
                    };

                    db.Records.Add(record);

                    // Update UI using Dispatcher (since we are on a background thread)
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Log($"Processed line {i}: {line}");
                        ProgressBar.Value = ((double)i / (totalLines - 1)) * 100;
                    });
                }
                db.SaveChanges(); // Save all to DB at once
            }
        }

        // Helper to add to log list
        private void Log(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LogList.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                LogList.ScrollIntoView(LogList.Items[LogList.Items.Count - 1]);
            });
        }
    }
}
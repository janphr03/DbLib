using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace DbVisualizer
{
    public partial class Form1 : Form
    {
        private string connectionString = "Server=localhost;Database=testprotocol;Uid=root;Pwd=password;"; // Passe den Connection-String an deine DB an

        public Form1()
        {
            InitializeComponent();
            LoadData(); // Daten beim Start laden
        }

        /// <summary>
        /// Lädt die Daten aus der MySQL-Tabelle in das DataGridView
        /// </summary>
        private void LoadData()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM tester"; // Passe die Tabelle an
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            // Daten in das DataGridView binden
                            dataGridView.DataSource = dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Daten: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}


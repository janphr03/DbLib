namespace DbVisualizer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // Hinzufügen eines DataGridView-Steuerelements
        private DataGridView dataGridView;

        /// <summary>
        /// Bereinigt die verwendeten Ressourcen
        /// </summary>
        /// <param name="disposing">true, wenn verwaltete Ressourcen freigegeben werden sollen; andernfalls false</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Stelle sicher, dass die Zeile und Spalte gültig sind
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Holen der Zelle, die angeklickt wurde
                var cellValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                // Beispiel: Zeige den Wert der angeklickten Zelle in einer MessageBox an
                MessageBox.Show($"Zelle angeklickt: {cellValue}", "Cell Click");
            }
        }


        #region Windows Form Designer generated code

        /// <summary>
        /// Benötigte Methode für die Designer-Unterstützung – nicht mit dem Code-Editor ändern
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(0, 0);
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidth = 62;
            dataGridView.Size = new Size(1074, 586);
            dataGridView.TabIndex = 0;
            dataGridView.CellContentClick += dataGridView_CellContentClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1074, 586);
            Controls.Add(dataGridView);
            Name = "Form1";
            Text = "MySQL Tabellenanzeige";
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
}




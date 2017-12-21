using System;
using System.Windows.Forms;

namespace StudySpace
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 newForm = new Form2();
            newForm.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            const string message =
        "Are you sure that you would like to close the form?";
            const string caption = "Mercury Closing";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            // If the no button was pressed ...
            if (result == DialogResult.No)
            {
                // cancel the closure of the form.
                e.Cancel = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

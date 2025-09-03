using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OffCrypt
{
    public partial class EncryptedtextPassword : Form
    {
        public EncryptedtextPassword()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        public TextBox EncryptedTextBox
        {
            get { return Encryptedtxtfrm; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Aktivoi theme runtime aikana
            offCryptTheme1.AttachToForm(this);

          
        }

        private void modernButton1_Click(object sender, EventArgs e)
        {
            // Check if the TextBox has content
            if (!string.IsNullOrEmpty(Encryptedtxtfrm.Text))
            {
                // Copy the text to clipboard
                Clipboard.SetText(Encryptedtxtfrm.Text);

                // Show confirmation message
                MessageBox.Show("Text copied to clipboard successfully!", "Copy Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Show warning if there's no text to copy
                MessageBox.Show("No text to copy!", "Copy Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
    }
}

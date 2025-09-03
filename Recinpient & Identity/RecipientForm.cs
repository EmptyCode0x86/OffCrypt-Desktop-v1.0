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
    public partial class RecipientForm : Form
    {
        public RecipientForm()
        {
            InitializeComponent();

            // Aktivoi theme
            this.Load += RecipientForm_Load;
        }

        private void RecipientForm_Load(object sender, EventArgs e)
        {
            // Aktivoi theme runtime aikana
            offCryptTheme1.AttachToForm(this);

            // Aseta otsikko
            offCryptTheme1.FormTitle = "Recipient's Public Key";

            // Lataa leikepöydältä mahdollinen public key
            LoadPublicKeyFromClipboard();
        }

        /// <summary>
        /// Lataa public key:n leikepöydältä jos se sisältää PEM-muotoisen avaimen
        /// </summary>
        private void LoadPublicKeyFromClipboard()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText().Trim();

                    // Tarkista onko leikepöydällä PEM-muotoinen public key
                    if (clipboardText.Contains("-----BEGIN PUBLIC KEY-----") &&
                        clipboardText.Contains("-----END PUBLIC KEY-----"))
                    {
                        Recptxtbx.Text = clipboardText;

                        // Ilmoita käyttäjälle
                        this.BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show("✓ Public key automatically loaded from clipboard!\n\n" +
                                           "You can now use this key for RSA encryption.",
                                           "Public Key Loaded",
                                           MessageBoxButtons.OK,
                                           MessageBoxIcon.Information);
                        }));
                    }
                }
            }
            catch
            {
                // Ignore clipboard errors
            }
        }

        /// <summary>
        /// Hakee syötetyn public key:n
        /// </summary>
        public string GetRecipientPublicKey()
        {
            return Recptxtbx.Text.Trim();
        }

        /// <summary>
        /// Asettaa vastaanottajan public key:n
        /// </summary>
        public void SetRecipientPublicKey(string publicKeyPEM)
        {
            Recptxtbx.Text = publicKeyPEM;
        }

        /// <summary>
        /// Tarkistaa onko syötetty valid public key
        /// </summary>
        public bool IsValidPublicKey()
        {
            string key = GetRecipientPublicKey();

            if (string.IsNullOrEmpty(key))
                return false;

            // Accept both standard PEM and alternative formats
            bool isStandardPEM = key.Contains("-----BEGIN PUBLIC KEY-----") &&
                                key.Contains("-----END PUBLIC KEY-----");
            bool isAlternativePEM = key.Contains("-----BEGIN") && key.Contains("PUBLIC KEY-----") &&
                                   key.Contains("-----END") && !isStandardPEM;

            return isStandardPEM || isAlternativePEM;
        }

        /// <summary>
        /// Tallentaa vastaanottajan public key:n väliaikaisesti (jos tarvitaan)
        /// </summary>
        public static string LastRecipientPublicKey { get; set; } = string.Empty;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Tallenna public key muistiin
            LastRecipientPublicKey = GetRecipientPublicKey();
            base.OnFormClosing(e);
        }

     
    }
}

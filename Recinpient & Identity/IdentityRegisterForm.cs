using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OffCrypt
{
    public partial class IdentityRegisterForm : Form
    {
        public IdentityRegisterForm()
        {
            InitializeComponent();
            InitializeEvents();
            
            // Activate theme IMMEDIATELY in constructor
            try
            {
                offCryptTheme1.AttachToForm(this);
            }
            catch (Exception ex)
            {
                // Fallback if theme fails
                System.Diagnostics.Debug.WriteLine($"Theme setup failed: {ex.Message}");
            }
        }

        private void InitializeEvents()
        {
            RegisterIDBtn.Click += RegisterIDBtn_Click;

            // Remove ReadOnly state from RegisterNameTxtbx
            RegisterNameTxtbx.ReadOnly = false;
        }


        private void IdentityRegisterForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Theme is already set in constructor, no need to repeat
                // Could have other initializations if needed
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application initialization failed: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RegisterIDBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Get name from text field (if empty, use null)
                string displayName = string.IsNullOrWhiteSpace(RegisterNameTxtbx.Text?.Trim()) ?
                    null : RegisterNameTxtbx.Text.Trim();

                // Create PGP identity with KeyRingManager (Random passphrase)
                var metadata = KeyringManager.CreateIdentity(
                    displayName,
                    PassphraseMode.Random,
                    out char[] generatedPassphrase);

                // Identity will be displayed in Form1 when this dialog closes

                // Show information to user
                var displayText = string.IsNullOrEmpty(displayName)
                    ? $"Random ID: {metadata.RandomId}"
                    : $"{displayName} [{metadata.RandomId}]";

                var passphraseText = new string(generatedPassphrase);

                MessageBox.Show(
                    $"✅ PGP Identity created successfully!\n\n" +
                    $"🆔 {displayText}\n" +
                    $"🔑 Key ID: {metadata.KeyId}\n" +
                    $"👤 Fingerprint: {metadata.FingerprintHex[..16]}...\n\n" +
                    $"🔐 Generated Passphrase:\n{passphraseText}\n\n" +
                    $"⚠️ Save this passphrase in a secure location!\n" +
                    $"Your identity has been created successfully.",
                    "PGP Identity Created",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Clear passphrase from memory
                Array.Clear(generatedPassphrase, 0, generatedPassphrase.Length);

                // Clear name text field for next use
                RegisterNameTxtbx.Clear();
                
                // Close form and return to main application
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Identity creation failed: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Identity display methods removed - handled by Form1's IdentityManager

        public bool HasExistingIdentity()
        {
            try
            {
                if (!Directory.Exists(KeyringManager.KeyringDir))
                    return false;

                var pubFiles = Directory.GetFiles(KeyringManager.KeyringDir, "*.ocpub");
                return pubFiles.Length > 0;
            }
            catch
            {
                return false;
            }
        }

      
    }
}

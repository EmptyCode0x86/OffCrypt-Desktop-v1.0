using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

namespace OffCrypt
{
    public partial class Form1 : Form
    {
        // Modern crypto class (ECDH + HKDF + AES-GCM) - updated dynamically
        private PasswordSecure _modernCrypto;
        private ECDHMode _currentEcdhMode = ECDHMode.StaticP256;

        // Legacy support for old messages
        private readonly PasswordSecure _legacyCrypto = new();

        // Password in char[] format for safer handling
        private char[]? _currentPassword;

        // X25519 functionality
        private X25519Manager? _x25519Manager;

        // Identity management
        private IdentityManager? _identityManager;

        // Contact trust information management
        private ContactManager? _contactManager;

        // Temporarily stored recipient public key
        private string? _recipientPublicKey;

        // Recipient identity information (Random ID, DisplayName, etc.)
        private RecipientIdentity? _recipientIdentity;

        // Constants for legacy compatibility
        private const byte LEGACY_VERSION_PASSWORD = 0x01;
        private const byte LEGACY_VERSION_ENHANCED = 0x03;
        private const byte MODERN_VERSION = 0x10;
        private const byte MODERN_DISAPPEARING_VERSION = 0x12; // Added missing constant

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
               
                offCryptTheme1.AttachToForm(this);

             
                ApplicationLogger.Initialize();
                ApplicationLogger.LogInfo("Application", "OffCryptDesktop starting...");

               
                offCryptTheme1.AttachToForm(this);
                ApplicationLogger.LogInitializationStep("Theme activated");


             
                if (RSAMethodComboBox != null && RSAMethodComboBox.Items.Count > 0)
                {
                    RSAMethodComboBox.SelectedIndex = 0;
                }

                if (PasswordCombobx != null && PasswordCombobx.Items.Count > 0)
                {
                    PasswordCombobx.SelectedIndex = 0; 
                }

                //PasswordCombobx

                // Initialize X25519 functions with disappearing messages UI components
                _x25519Manager = new X25519Manager(
                    RSPublickeymsgbox,
                    RSWritemsgbox,
                    RSDecryptmsgbox,
                    GetCurrentPasswordAsCharArray,
                    DispX255txtbox,
                    HourX255Chck,
                    DayX255Chck,
                    WeekX255Chck,
                    MonthX255Chck,
                    ToggleDispX255
                );

                // Initialize identity management
                _identityManager = new IdentityManager(this, CurntIdntTxt);

                // Initialize contact trust information management
                _contactManager = new ContactManager();
                ApplicationLogger.LogInitializationStep("ContactManager initialized");

                // Load existing public key automatically
                LoadExistingPublicKey();

                // Initialize ECDH and INI settings
                InitializeECDHAndINISettings();

                // Attach event handlers
                SetupEventHandlers();

                // Load existing identity (no need to check existence, as Program.cs handles it)
                _identityManager?.LoadCurrentIdentity();

                // Update RSA/X25519 button initial state
                UpdateRSAButtonStates();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application initialization failed: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadExistingPublicKey()
        {
            try
            {
                if (_x25519Manager?.HasPublicKey() == true)
                {
                    var publicKeyPEM = _x25519Manager.GetCurrentPublicKeyPEM();
                    if (!string.IsNullOrEmpty(publicKeyPEM))
                    {
                        RSPublickeymsgbox.Text = publicKeyPEM;

                        this.BeginInvoke(new Action(() =>
                        {
                            var result = MessageBox.Show(
                                "✓ Public key loaded successfully!\n\n" +
                                "Your public key is now visible and ready for sharing.\n\n" +
                                "Would you like to copy it to clipboard?",
                                "Public Key Available",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);

                            if (result == DialogResult.Yes)
                            {
                                _x25519Manager?.CopyMyPublicKeyToClipboard();
                            }
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Public key loading failed: {ex.Message}");
            }
        }


        private void SetupEventHandlers()
        {
            try
            {
                // Password setting
                Setpasswordbutton.Click += Setpasswordbutton_Click;

                // Encryption (modern method)
                Encryptbutton1.Click += Encryptbutton1_Click;
                Createcryptedfile1.Click += Createcryptedfile_Click;
                modernButton6.Click += modernButton6_Click;

                // Decryption (legacy + modern support)
                DecryptE.Click += DecryptE_Click;
                ImportE.Click += ImportE_Click;
                CopyDecryptedtxt.Click += CopyDecryptedtxt_Click;

                // RSA-4096 functionality
                RSGenerate.Click += RSGenerate_Click;
                RSshowmessage.Click += RSshowmessage_Click;
                RSimport.Click += RSimport_Click;
                RsEncrypt.Click += RsEncrypt_Click;
                RSDecrypt.Click += RSDecrypt_Click;
                RsCreatefile.Click += RsCreatefile_Click;
                RsCopydecryptedmsg.Click += RsCopydecryptedmsg_Click;
                cryptButtonc1.Click += CryptButtonc1_Click;

                // PGP Identity functionality
                CreateNewIDbtn.Click += CreateNewIDbtn_Click;

                // Identity copy button
                Copyidentbtn.Click += Copyidentbtn_Click;

                // Public key management
                RSPublickeymsgbox.DoubleClick += RSPublicKeyMsgbox_DoubleClick;
                RSPublickeymsgbox.MouseClick += RSPublicKeyMsgbox_MouseClick;

                // RSWritemsgbox dynamic button updates
                RSWritemsgbox.TextChanged += RSWritemsgbox_TextChanged;

                // Recipient management
                RecpientBtn.Click += RecpientBtn_Click;

                // Data wipe function
                RemoveDataButton.Click += RemoveDataButton_Click;

                // Disappearing messages
                SetupDisappearingMessageHandlers();

                // ECDH ComboBox
                if (ECDHCombobx != null)
                    ECDHCombobx.SelectedIndexChanged += ECDHCombobx_SelectedIndexChanged;

                // INI Settings buttons
                if (Saveinibtn != null)
                    Saveinibtn.Click += Saveinibtn_Click;
                if (Loadinibtn != null)
                    Loadinibtn.Click += Loadinibtn_Click;

                // Enter key support in password fields
                Setpasswordtxt.KeyPress += (s, e) =>
                {
                    if (e.KeyChar == (char)Keys.Enter)
                        Setpasswordbutton_Click(s, e);
                };

                Decryptpasstxt.KeyPress += (s, e) =>
                {
                    if (e.KeyChar == (char)Keys.Enter)
                        DecryptE_Click(s, e);
                };

                Writemsgbox.KeyPress += (s, e) =>
                {
                    if (e.KeyChar == (char)Keys.Enter && Control.ModifierKeys == Keys.Control)
                        Encryptbutton1_Click(s, e);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Event handler setup failed: {ex.Message}");
            }
        }

        #region Password management (secure char[] handling)

        private void Setpasswordbutton_Click(object sender, EventArgs e)
        {
            try
            {
                var passwordInput = Setpasswordtxt.Text.Trim();

                if (string.IsNullOrEmpty(passwordInput))
                {
                    MessageBox.Show("Enter password first!", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (passwordInput.Length < 8)
                {
                    MessageBox.Show("Password must be at least 8 characters long!", "Weak password",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Clear old password securely
                ClearCurrentPassword();

                // Save new password in char[] format and clear string immediately
                _currentPassword = passwordInput.ToCharArray();
                // Try to clear string (although not fully reliable)
                passwordInput = string.Empty;

                MessageBox.Show("Password set successfully!", "Success",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear password field immediately for security
                Setpasswordtxt.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Password setting failed: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private char[] GetCurrentPasswordAsCharArray()
        {
            if (_currentPassword == null || _currentPassword.Length == 0)
            {
                throw new InvalidOperationException("Password not set. Please set password first.");
            }

            // Return copy, not original reference
            var copy = new char[_currentPassword.Length];
            Array.Copy(_currentPassword, copy, _currentPassword.Length);
            return copy;
        }

        private void ClearCurrentPassword()
        {
            if (_currentPassword != null)
            {
                // Clear password from memory securely
                Array.Clear(_currentPassword, 0, _currentPassword.Length);
                _currentPassword = null;
            }
        }

        #endregion

        #region Modern encryption (ECDH + HKDF + AES-GCM)

        private void Encryptbutton1_Click(object sender, EventArgs e)
        {
            char[]? password = null;

            try
            {
                string message = Writemsgbox.Text.Trim();

                if (string.IsNullOrEmpty(message))
                {
                    MessageBox.Show("Enter message first!", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                password = GetCurrentPasswordAsCharArray();
                string encrypted;

                // Tarkista katoavien viestien asetukset
                try
                {
                    // Debug: Tarkista UI-komponenttien tilat
                    System.Diagnostics.Debug.WriteLine($"Debug - ToggleDispPass.Checked: {ToggleDispPass.Checked}");
                    System.Diagnostics.Debug.WriteLine($"Debug - DispPassTxtbox.Text: '{DispPassTxtbox.Text}'");
                    System.Diagnostics.Debug.WriteLine($"Debug - HourPassChck.Checked: {HourPassChck.Checked}");
                    System.Diagnostics.Debug.WriteLine($"Debug - DayPassCheck.Checked: {DayPassCheck.Checked}");
                    System.Diagnostics.Debug.WriteLine($"Debug - WeekPassChck.Checked: {WeekPassChck.Checked}");
                    System.Diagnostics.Debug.WriteLine($"Debug - MonthPassChck.Checked: {MonthPassChck.Checked}");

                    var disappearingSettings = DisappearingMessages.SettingsManager.GetPasswordSettings(
                        DispPassTxtbox, HourPassChck, DayPassCheck, WeekPassChck, MonthPassChck, ToggleDispPass);

                    System.Diagnostics.Debug.WriteLine($"Debug - disappearingSettings: {disappearingSettings?.ToString() ?? "null"}");

                    if (disappearingSettings != null)
                    {
                        // Luo katoava viesti (käyttää modernia PasswordSecure-luokkaa)
                        encrypted = DisappearingMessages.CreateDisappearingPasswordMessage(
                            message, password, disappearingSettings.ExpireAfter);

                        MessageBox.Show($"✨ Katoava viesti salattu!\n\n" +
                                       $"⏰ Vanhenee: {disappearingSettings}\n" +
                                       $"🔐 Moderni salaus käytössä (v0x12)\n\n" +
                                       $"Salattu teksti kopioitu leikepöydälle.",
                                       "Katoava viesti luotu",
                                       MessageBoxButtons.OK,
                                       MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Käytä modernia salausta (ECDH + HKDF + AES-GCM)
                        encrypted = _modernCrypto.Encrypt(message, password);

                        MessageBox.Show("Viesti salattu modernilla salauksella! " +
                                       "Salattu teksti kopioitu leikepöydälle.", "Onnistui",
                                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show($"Katoavien viestien asetusvirhe: {ex.Message}", "Virhe",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kopioi salattu teksti leikepöydälle
                Clipboard.SetText(encrypted);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Salasana puuttuu",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Encryption failed: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Clear password from memory
                if (password != null)
                    Array.Clear(password, 0, password.Length);
            }
        }

        #endregion

        #region Moderni purku (tukee sekä modernia että legacy-formaattia)

        private void DecryptE_Click(object sender, EventArgs e)
        {
            char[]? password = null;

            try
            {
                string encryptedText = GetEncryptedTextFromUI();
                var passwordInput = Decryptpasstxt.Text.Trim();

                if (string.IsNullOrEmpty(encryptedText))
                {
                    MessageBox.Show("No encrypted message found! Copy encrypted text to clipboard first.",
                                   "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(passwordInput))
                {
                    MessageBox.Show("Anna salasana!", "Virhe",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Muunna salasana char[]:ksi ja tyhjennä string välittömästi
                password = passwordInput.ToCharArray();
                passwordInput = string.Empty;
                string decrypted;

                // Tunnista viestin tyyppi ja käytä sopivaa purkumetodia
                decrypted = DecryptWithVersionDetection(encryptedText, password);

                Decryptmsg.Text = decrypted;

                // Tyhjennä salasanakenttä tietoturvan vuoksi
                Decryptpasstxt.Text = "";
            }
            catch (MessageExpiredException ex)
            {
                MessageBox.Show(ex.Message, "Viesti vanhentunut",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Decryptmsg.Text = "⚰️ Tämä viesti on vanhentunut eikä sitä voida purkaa.";
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Wrong password or data is corrupted.", "Decryption failed",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                Decryptmsg.Text = "Purku epäonnistui";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Decryption failed: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                Decryptmsg.Text = "Decryption failed";
            }
            finally
            {
                // Clear password from memory
                if (password != null)
                    Array.Clear(password, 0, password.Length);
            }
        }

        private string DecryptWithVersionDetection(string encryptedText, char[] password)
        {
            try
            {
                var encryptedData = Convert.FromBase64String(encryptedText);
                if (encryptedData.Length == 0)
                    throw new ArgumentException("Tyhjä salattu data");

                byte version = encryptedData[0];

                return version switch
                {
                    // Moderni versio (ECDH + HKDF + AES-GCM)
                    MODERN_VERSION => _modernCrypto.Decrypt(encryptedText, password),

                    // Moderni katoava viesti (ECDH + HKDF + AES-GCM + timestamp)
                    MODERN_DISAPPEARING_VERSION => DisappearingMessages.DecryptDisappearingPasswordMessage(
                        encryptedText, password),

                    // Legacy disappearing messages (vanha PBKDF2 + AES-CBC)
                    0x02 => DisappearingMessages.DecryptDisappearingPasswordMessage(
                        encryptedText, password),

                    // Legacy enhanced security (ECDH + PBKDF2 + AES-CBC + HMAC)
                    LEGACY_VERSION_ENHANCED => _legacyCrypto.Decrypt(encryptedText, password),

                    // Legacy password-based (PBKDF2 + AES-CBC + HMAC)
                    LEGACY_VERSION_PASSWORD => DecryptLegacyPasswordBased(encryptedText, password),

                    _ => throw new NotSupportedException($"Tuntematon salausversio: 0x{version:X2}")
                };
            }
            catch (FormatException)
            {
                throw new ArgumentException("Virheellinen Base64-muoto");
            }
        }

        private string DecryptLegacyPasswordBased(string encryptedText, char[] password)
        {
            // Tämä metodi säilytetään yhteensopivuudelle vanhempien versioiden kanssa
            // Käytä alkuperäistä Form1:n DecryptPasswordBased-logiikkaa

            // Jos alkuperäinen metodi on poistettu, heitetään tuki-poikkeus
            throw new NotSupportedException(
                "Legacy password-based dekryptaus ei ole enää tuettu. " +
                "Käytä uudempia salausversioita.");
        }

        #endregion

        #region Tiedosto-operaatiot

        private void Createcryptedfile_Click(object sender, EventArgs e)
        {
            char[]? password = null;

            try
            {
                string message = Writemsgbox.Text.Trim();
                if (string.IsNullOrEmpty(message))
                {
                    MessageBox.Show("Anna viesti ensin!", "Virhe",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Salatut tiedostot (*.enc)|*.enc|Kaikki tiedostot (*.*)|*.*";
                    saveDialog.FileName = $"Secure_encrypted_memo_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.enc";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        password = GetCurrentPasswordAsCharArray();
                        string encrypted;

                        // Tarkista katoavien viestien asetukset (sama logiikka kuin Encryptbutton1_Click)
                        try
                        {
                            var disappearingSettings = DisappearingMessages.SettingsManager.GetPasswordSettings(
                                DispPassTxtbox, HourPassChck, DayPassCheck, WeekPassChck, MonthPassChck, ToggleDispPass);

                            if (disappearingSettings != null)
                            {
                                // Luo katoava viesti tiedostoon
                                encrypted = DisappearingMessages.CreateDisappearingPasswordMessage(
                                    message, password, disappearingSettings.ExpireAfter);

                                File.WriteAllText(saveDialog.FileName, encrypted, Encoding.UTF8);
                                MessageBox.Show($"✨ Katoava viesti tallennettu tiedostoon!\n\n" +
                                               $"⏰ Vanhenee: {disappearingSettings}\n" +
                                               $"📁 Tiedosto: {Path.GetFileName(saveDialog.FileName)}\n" +
                                               $"🔐 Moderni salaus käytössä (v0x12)",
                                               "Katoava viesti tallennettu",
                                               MessageBoxButtons.OK,
                                               MessageBoxIcon.Information);
                            }
                            else
                            {
                                // Käytä modernia salausta tiedostoihin
                                encrypted = _modernCrypto.Encrypt(message, password);

                                File.WriteAllText(saveDialog.FileName, encrypted, Encoding.UTF8);
                                MessageBox.Show("Salattu tiedosto tallennettu modernilla salauksella!", "Onnistui",
                                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        catch (ArgumentException ex)
                        {
                            MessageBox.Show($"Katoavien viestien asetusvirhe: {ex.Message}", "Virhe",
                                           MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Salasana puuttuu",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"File save failed: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (password != null)
                    Array.Clear(password, 0, password.Length);
            }
        }

        private void ImportE_Click(object sender, EventArgs e)
        {
            try
            {
                using (var openDialog = new OpenFileDialog())
                {
                    openDialog.Filter = "Salatut tiedostot (*.enc)|*.enc|Kaikki tiedostot (*.*)|*.*";

                    if (openDialog.ShowDialog() == DialogResult.OK)
                    {
                        string encryptedContent = File.ReadAllText(openDialog.FileName, Encoding.UTF8);

                        if (!string.IsNullOrEmpty(encryptedContent))
                        {
                            Clipboard.SetText(encryptedContent);
                            MessageBox.Show("Salattu tiedosto ladattu ja kopioitu leikepöydälle! " +
                                           "Voit nyt purkaa sen.", "Onnistui",
                                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Tiedosto on tyhjä tai virheellinen!", "Virhe",
                                           MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tiedoston lataus epäonnistui: {ex.Message}", "Virhe",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region UI Event Handlers

        private void CopyDecryptedtxt_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Decryptmsg.Text) && Decryptmsg.Text != "Purku epäonnistui")
                {
                    Clipboard.SetText(Decryptmsg.Text);
                    MessageBox.Show("Decrypted message copied to clipboard!", "Success",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Ei purettua viestiä kopioitavaksi!", "Virhe",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kopiointi epäonnistui: {ex.Message}", "Virhe",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetEncryptedTextFromUI()
        {
            if (Clipboard.ContainsText())
            {
                return Clipboard.GetText().Trim();
            }
            return "";
        }

        #endregion

        #region RSA Event Handlers (ennallaan)

        private void RSGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                // SPECIAL: Ctrl+Shift+Click suorittaa X25519 kryptaustestit
                if (ModifierKeys == (Keys.Control | Keys.Shift))
                {
                    X25519Tester.RunAllTests();
                    return;
                }

                // Tarkista onko avainpari jo olemassa
                if (_x25519Manager?.HasPublicKey() == true)
                {
                    MessageBox.Show(
                        "⚠️ WARNING: X25519 key pair already exists!\n\n" +
                        "Cannot create a new key pair as it would destroy access to existing encrypted messages.\n\n" +
                        "If you need a new identity, please go to:\n" +
                        "Identity Manager > Create New Identity",
                        "Key Pair Already Exists",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                _x25519Manager?.GenerateKeyPair();

                // Päivitä nappien tila avainparin luonnin jälkeen
                UpdateRSAButtonStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"X25519 avainparin luonti epäonnistui: {ex.Message}", "Virhe",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RSshowmessage_Click(object sender, EventArgs e)
        {
            try
            {
                // Näytä salattu viesti erillisessä ikkunassa
                if (!string.IsNullOrEmpty(RSWritemsgbox.Text))
                {
                    var encryptedMessage = RSWritemsgbox.Text;

                    using (var displayForm = new EncryptedtextPassword())
                    {

                        {
                            try
                            {
                                // TESTIEN AJAMINEN: Ctrl+Shift+Alt+Click
                                if (ModifierKeys == (Keys.Control | Keys.Shift | Keys.Alt))
                                {
                                    CryptographyTests.RunAllCryptographyTests(this);
                                    return;
                                }

                                // Normaali toiminta...
                                _x25519Manager?.GenerateKeyPair();
                            }
                            catch (Exception ex)
                            {
                                ApplicationLogger.LogException("RSGenerate", ex);
                                MessageBox.Show($"Virhe: {ex.Message}");
                            }
                        }

                        displayForm.Text = "Encrypted Message";
                        displayForm.EncryptedTextBox.Text = encryptedMessage;
                        displayForm.EncryptedTextBox.ReadOnly = true;
                        displayForm.ShowDialog(this);
                    }
                }
                else
                {
                    MessageBox.Show("No encrypted message to display. Encrypt a message first!",
                                   "No Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to display message: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RSimport_Click(object sender, EventArgs e)
        {
            try
            {
                // Tuo salattu tiedosto ja kopioi leikepöydälle purkua varten
                using (var openDialog = new OpenFileDialog())
                {
                    openDialog.Filter = "PGP encrypted files (*.pgp)|*.pgp|All files (*.*)|*.*";

                    if (openDialog.ShowDialog() == DialogResult.OK)
                    {
                        string encryptedContent = File.ReadAllText(openDialog.FileName, Encoding.UTF8);

                        if (!string.IsNullOrEmpty(encryptedContent))
                        {
                            RSWritemsgbox.Text = encryptedContent;
                            MessageBox.Show("Encrypted file imported successfully!\n\n" +
                                           "The content is now ready for decryption.",
                                           "File Imported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"File import failed: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void RsEncrypt_Click(object sender, EventArgs e)
        {
            try
            {
                ApplicationLogger.LogDebug("Debug", "Tarkistan salausnappia...");
                ApplicationLogger.LogCrypto("X25519", "Aloitan salauksen");
                ApplicationLogger.LogUIEvent("EncryptButton", "Click", "Käyttäjä painoi salausnappia");

                // Tarkista kontaktin luottamus ennen salausta
                if (_contactManager != null && _recipientIdentity != null && !string.IsNullOrEmpty(_recipientPublicKey))
                {
                    var fingerprint = CalculateRecipientFingerprint(_recipientPublicKey);
                    if (fingerprint != null)
                    {
                        var trustLevel = _contactManager.GetTrustLevel(fingerprint);

                        if (trustLevel == TrustLevel.Unknown)
                        {
                            var result = MessageBox.Show(
                                $"⚠️ TUNTEMATON KONTAKTI\n\n" +
                                $"Nimi: {_recipientIdentity.DisplayName}\n" +
                                $"RandomID: {_recipientIdentity.RandomId}\n" +
                                $"Fingerprint: {fingerprint[..16]}...\n\n" +
                                $"Tämä on ensimmäinen kerta kun lähetät viestin tälle henkilölle.\n" +
                                $"Varmista että sait julkisen avaimen luotettavasta lähteestä!\n\n" +
                                $"Haluatko varmasti salata viestin tälle vastaanottajalle?",
                                "Turvallisuusvaroitus",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning);

                            if (result == DialogResult.No)
                            {
                                ApplicationLogger.LogSecurity("ContactManager", $"User declined to encrypt to unknown contact {_recipientIdentity.RandomId}");
                                return;
                            }

                            // Tallenna Known-tasolle ensimmäisen käytön jälkeen
                            _contactManager.SetTrustLevel(fingerprint,
                                _recipientIdentity.RandomId,
                                _recipientIdentity.DisplayName,
                                TrustLevel.Known,
                                "Automatically added after user confirmation");

                            ApplicationLogger.LogSecurity("ContactManager", $"New contact added: {_recipientIdentity.RandomId} ({_recipientIdentity.DisplayName})");
                        }
                        else if (trustLevel == TrustLevel.Untrusted)
                        {
                            var result = MessageBox.Show(
                                $"🚫 EI-LUOTETTU KONTAKTI\n\n" +
                                $"Nimi: {_recipientIdentity.DisplayName}\n" +
                                $"RandomID: {_recipientIdentity.RandomId}\n\n" +
                                $"Olet merkinnyt tämän kontaktin EI-LUOTETTAVAKSI.\n" +
                                $"Salataanko viesti silti?",
                                "Turvallisuusvaroitus",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Stop);

                            if (result == DialogResult.No)
                            {
                                ApplicationLogger.LogSecurity("ContactManager", $"User declined to encrypt to untrusted contact {_recipientIdentity.RandomId}");
                                return;
                            }
                        }

                        // Merkitse viesti lähetetyksi tilastointia varten
                        _contactManager.RecordMessage(fingerprint);
                    }
                }

                // Delegate to X25519Manager for modern encryption
                _x25519Manager?.EncryptAndShowMessage();

                // Päivitä nappien tila salauksen jälkeen
                UpdateRSAButtonStates();
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogException("RsEncrypt", ex);
                MessageBox.Show($"Salaus epäonnistui: {ex.Message}", "Virhe",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void RSDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                ApplicationLogger.LogDebug("Debug", "Tarkistan purkunappia...");
                ApplicationLogger.LogCrypto("X25519", "Aloitan purkamisen");
                ApplicationLogger.LogUIEvent("DecryptButton", "Click", "Käyttäjä painoi purkunappia");

                // Delegate to X25519Manager for modern decryption
                _x25519Manager?.ImportAndDecryptMessage();
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogException("RSDecrypt", ex);
                MessageBox.Show($"Purku epäonnistui: {ex.Message}", "Virhe",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateIdentityBTN_Click(object sender, EventArgs e)
        {
            try
            {
                // Kysy käyttäjältä valinnainen nimi PGP-identiteetille
                var displayName = GetPGPDisplayNameFromUser();
                if (displayName == null) return; // Käyttäjä peruutti

                // Luo PGP-identiteetti KeyRingManager:illa (Random passphrase)
                var metadata = KeyringManager.CreateIdentity(
                    displayName,
                    PassphraseMode.Random,
                    out char[] generatedPassphrase);

                // Näytä luotu julkinen avain RSPublickeymsgbox:ssa (ASCII-armor muodossa)
                DisplayPGPIdentityInUI(metadata);

                // Näytä tiedot käyttäjälle
                var displayText = string.IsNullOrEmpty(displayName)
                    ? $"ID: {metadata.RandomId}"
                    : $"Name: {displayName}\nID: {metadata.RandomId}";

                var passphraseText = new string(generatedPassphrase);

                MessageBox.Show(
                    $"✅ PGP Identity luotu onnistuneesti!\n\n" +
                    $"🆔 {displayText}\n" +
                    $"🔑 Key ID: {metadata.KeyId}\n" +
                    $"👤 Fingerprint: {metadata.FingerprintHex[..16]}...\n\n" +
                    $"🔐 Generated Passphrase:\n{passphraseText}\n\n" +
                    $"⚠️ Tallenna passphrase turvalliseen paikkaan!\n" +
                    $"Julkinen avaimesi on nyt näkyvissä yllä ja valmis jakamiseen.",
                    "PGP Identity Created",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Tyhjennä passphrase muistista
                Array.Clear(generatedPassphrase, 0, generatedPassphrase.Length);
            }
            catch (OperationCanceledException)
            {
                // Käyttäjä peruutti, ei näytetä virhettä
            }
            catch (Exception ex)
            {
                MessageBox.Show($"PGP-identiteetin luominen epäonnistui: {ex.Message}",
                               "Virhe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateNewIDbtn_Click(object sender, EventArgs e)
        {
            _identityManager?.OpenIdentityRegisterForm();
        }


        private void RSPublicKeyMsgbox_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(RSPublickeymsgbox.Text))
                {
                    Clipboard.SetText(RSPublickeymsgbox.Text);
                    MessageBox.Show("Public key copied to clipboard!", "Copied",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Ei public key:tä kopioitavaksi. Generoi avaimet ensin!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kopiointi epäonnistui: {ex.Message}");
            }
        }

        private void RSPublicKeyMsgbox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ShowPublicKeyContextMenu();
            }
        }

        private void RecpientBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (var recipientForm = new RecipientForm())
                {
                    if (recipientForm.ShowDialog() == DialogResult.OK)
                    {
                        var publicKeyText = recipientForm.GetRecipientPublicKey();

                        if (!string.IsNullOrEmpty(publicKeyText))
                        {
                            _x25519Manager?.ImportRecipientPublicKey(publicKeyText);

                            // Päivitä nappien tila vastaanottajan avaimen tuonnin jälkeen
                            UpdateRSAButtonStates();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to import recipient key: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowPublicKeyContextMenu()
        {
            try
            {
                var contextMenu = new ContextMenuStrip();
                contextMenu.BackColor = Color.FromArgb(45, 45, 48);
                contextMenu.ForeColor = Color.White;

                var copyItem = new ToolStripMenuItem("Kopioi Public Key")
                {
                    BackColor = Color.FromArgb(45, 45, 48),
                    ForeColor = Color.White
                };
                copyItem.Click += (s, e) => _x25519Manager?.CopyMyPublicKeyToClipboard();

                var infoItem = new ToolStripMenuItem("Avaimen tiedot")
                {
                    BackColor = Color.FromArgb(45, 45, 48),
                    ForeColor = Color.White
                };
                infoItem.Click += (s, e) => _x25519Manager?.ShowPublicKeyInfo();

                var folderItem = new ToolStripMenuItem("Avaa avainten kansio")
                {
                    BackColor = Color.FromArgb(45, 45, 48),
                    ForeColor = Color.White
                };
                // OpenPublicKeyFolder ei enää tuettu X25519:ssa
                folderItem.Click += (s, e) => MessageBox.Show("Key folder function is not available in X25519.", "Not supported");

                contextMenu.Items.Add(copyItem);
                contextMenu.Items.Add(infoItem);
                contextMenu.Items.Add(new ToolStripSeparator());
                contextMenu.Items.Add(folderItem);

                contextMenu.Show(RSPublickeymsgbox, new Point(0, RSPublickeymsgbox.Height));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kontekstivalikko epäonnistui: {ex.Message}");
            }
        }

        #endregion

        #region PGP Identity Helper Methods

        /// <summary>
        /// Kysyy käyttäjältä valinnainen nimi PGP-identiteetille
        /// </summary>
        private string? GetPGPDisplayNameFromUser()
        {
            // Yksinkertainen input dialog pelkälle nimelle
            using (var form = new Form())
            {
                form.Text = "Create PGP Identity";
                form.Size = new Size(400, 220);
                form.StartPosition = FormStartPosition.CenterParent;
                form.BackColor = Color.FromArgb(40, 40, 40);
                form.ForeColor = Color.White;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var titleLabel = new Label
                {
                    Text = "Enter a display name for your PGP identity (optional):",
                    Location = new Point(20, 20),
                    Size = new Size(340, 40),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F)
                };

                var nameTextBox = new TextBox
                {
                    Location = new Point(20, 70),
                    Size = new Size(340, 25),
                    BackColor = Color.FromArgb(60, 60, 60),
                    ForeColor = Color.White,
                    PlaceholderText = "Leave empty to use only random ID"
                };

                var infoLabel = new Label
                {
                    Text = "A random 8-character ID will be generated automatically.",
                    Location = new Point(20, 105),
                    Size = new Size(340, 20),
                    ForeColor = Color.LightGray,
                    Font = new Font("Segoe UI", 8F)
                };

                var okButton = new Button
                {
                    Text = "Create Identity",
                    Location = new Point(200, 140),
                    Size = new Size(120, 30),
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(34, 153, 84),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                var cancelButton = new Button
                {
                    Text = "Cancel",
                    Location = new Point(100, 140),
                    Size = new Size(80, 30),
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.FromArgb(60, 60, 60),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                form.Controls.AddRange(new Control[] { titleLabel, nameTextBox, infoLabel, okButton, cancelButton });

                if (form.ShowDialog() == DialogResult.OK)
                {
                    var displayName = nameTextBox.Text.Trim();
                    return string.IsNullOrEmpty(displayName) ? "" : displayName;
                }
            }

            return null; // Käyttäjä peruutti
        }

        /// <summary>
        /// Näyttää PGP-identiteetin julkisen avaimen RSPublickeymsgbox:ssa
        /// </summary>
        private void DisplayPGPIdentityInUI(KeyMetadata metadata)
        {
            try
            {
                // Lataa luotu julkinen avain keyring:stä ja näytä ASCII-armor muodossa
                string armoredPublicKey = KeyringManager.FindPublicArmoredByRandomId(metadata.RandomId);

                if (!string.IsNullOrEmpty(armoredPublicKey))
                {
                    // Näytetään vain armored public key ilman Comment-riviä
                    RSPublickeymsgbox.Text = armoredPublicKey;
                }
                else
                {
                    // Fallback näyttö jos armored key ei löydy
                    var identityText = string.IsNullOrEmpty(metadata.DisplayName)
                        ? $"Random ID: {metadata.RandomId}"
                        : $"Name: {metadata.DisplayName}\nRandom ID: {metadata.RandomId}";

                    RSPublickeymsgbox.Text = $"PGP Identity created successfully!\n\n{identityText}\n\nFingerprint: {metadata.FingerprintHex}\nKey ID: {metadata.KeyId}";
                }
            }
            catch (Exception ex)
            {
                RSPublickeymsgbox.Text = $"PGP Identity created, but display failed: {ex.Message}\n\nRandom ID: {metadata.RandomId}\nFingerprint: {metadata.FingerprintHex}";
                System.Diagnostics.Debug.WriteLine($"Display PGP identity error: {ex}");
            }
        }

        /// <summary>
        /// Validoi että annettu teksti on kelvollinen ASCII-armored julkinen avain
        /// </summary>
        private bool IsValidArmoredPublicKey(string keyText)
        {
            if (string.IsNullOrWhiteSpace(keyText))
                return false;

            try
            {
                // Tarkista että sisältää BEGIN/END tagit
                if (keyText.Contains("-----BEGIN") && keyText.Contains("-----END"))
                {
                    // Yritä dekoodata ArmorUtil:lla
                    if (keyText.Contains("PUBLIC KEY"))
                    {
                        var decoded = ArmorUtil.Decode(keyText, "PUBLIC KEY");
                        return decoded.Length > 0;
                    }
                    else if (keyText.Contains("PUBLIC KEY"))
                    {
                        // Standard PEM format
                        var cleanKey = keyText
                            .Replace("-----BEGIN PUBLIC KEY-----", "")
                            .Replace("-----END PUBLIC KEY-----", "")
                            .Replace("\r", "").Replace("\n", "").Replace(" ", "");
                        var decoded = Convert.FromBase64String(cleanKey);
                        return decoded.Length > 0;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // RSA parsing methods removed - functionality delegated to RSAOperations.cs

        /// <summary>
        /// Kysyy käyttäjältä passphrase yksityisavaimen purkuun
        /// </summary>
        private string GetPassphraseForDecryption()
        {
            using (var passwordDialog = new EncryptedtextPassword())
            {
                passwordDialog.Text = "Enter PGP Passphrase";
                passwordDialog.EncryptedTextBox.PlaceholderText = "Enter your PGP identity passphrase...";
                passwordDialog.EncryptedTextBox.UseSystemPasswordChar = true;
                passwordDialog.EncryptedTextBox.Text = "";

                if (passwordDialog.ShowDialog(this) == DialogResult.OK)
                {
                    var passphrase = passwordDialog.EncryptedTextBox.Text.Trim();
                    if (string.IsNullOrEmpty(passphrase))
                        throw new OperationCanceledException("Passphrase cannot be empty");

                    return passphrase;
                }

                return ""; // Käyttäjä peruutti
            }
        }

        /// <summary>
        /// Kopioi purettu viesti leikepöydälle
        /// </summary>
        private void RsCopydecryptedmsg_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(RSDecryptmsgbox.Text))
                {
                    Clipboard.SetText(RSDecryptmsgbox.Text);
                    MessageBox.Show("Decrypted message copied to clipboard!", "Copied",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No decrypted message to copy!", "No Message",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Copy failed: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Luo salattu tiedosto (.pgp)
        /// </summary>
        private void RsCreatefile_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(RSWritemsgbox.Text))
                {
                    MessageBox.Show("No encrypted message to save!\n\nEncrypt a message first.",
                                   "No Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "PGP files (*.pgp)|*.pgp|All files (*.*)|*.*";
                    saveDialog.FileName = $"encrypted_message_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.pgp";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveDialog.FileName, RSWritemsgbox.Text, Encoding.UTF8);
                        MessageBox.Show("Encrypted file saved successfully!", "File Saved",
                                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"File save failed: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Kopioi oma julkinen avain leikepöydälle
        /// </summary>
        private void CryptButtonc1_Click(object sender, EventArgs e)
        {
            _x25519Manager?.CopyMyPublicKeyToClipboard();
        }

        /// <summary>
        /// Kopioi nykyinen identiteetti leikepöydälle
        /// </summary>
        private void Copyidentbtn_Click(object sender, EventArgs e)
        {
            try
            {
                var identityText = CurntIdntTxt.Text.Trim();    
                
                if (string.IsNullOrEmpty(identityText))
                {
                    MessageBox.Show("No identity to copy!\n\nPlease create or load an identity first.", 
                                   "No Identity", 
                                   MessageBoxButtons.OK, 
                                   MessageBoxIcon.Warning);
                    return;
                }

                // Kopioi identiteetti leikepöydälle
                Clipboard.SetText(identityText);
                
                MessageBox.Show("✅ Identity copied to clipboard!\n\n" +
                               $"📋 Copied: {identityText}\n\n" +
                               "You can now share your identity with other users.",
                               "Identity Copied", 
                               MessageBoxButtons.OK, 
                               MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy identity: {ex.Message}", 
                               "Error", 
                               MessageBoxButtons.OK, 
                               MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Muut metodit (ennallaan)

        private void modernButton6_Click(object sender, EventArgs e)
        {
            EncryptedtextPassword myNewForm = new EncryptedtextPassword();

            if (Clipboard.ContainsText())
            {
                string encryptedText = Clipboard.GetText();
                myNewForm.EncryptedTextBox.Text = encryptedText;
            }

            myNewForm.Show();
        }

        private void SetupDisappearingMessageHandlers()
        {
            try
            {
                // RSA disappearing messages - mutual exclusion
                HourX255Chck.CheckedChanged += (s, e) => { if (HourX255Chck.Checked) UncheckOtherRSABoxes(HourX255Chck); };
                DayX255Chck.CheckedChanged += (s, e) => { if (DayX255Chck.Checked) UncheckOtherRSABoxes(DayX255Chck); };
                WeekX255Chck.CheckedChanged += (s, e) => { if (WeekX255Chck.Checked) UncheckOtherRSABoxes(WeekX255Chck); };
                MonthX255Chck.CheckedChanged += (s, e) => { if (MonthX255Chck.Checked) UncheckOtherRSABoxes(MonthX255Chck); };

                // Password disappearing messages - mutual exclusion
                HourPassChck.CheckedChanged += (s, e) => { if (HourPassChck.Checked) UncheckOtherPassBoxes(HourPassChck); };
                DayPassCheck.CheckedChanged += (s, e) => { if (DayPassCheck.Checked) UncheckOtherPassBoxes(DayPassCheck); };
                WeekPassChck.CheckedChanged += (s, e) => { if (WeekPassChck.Checked) UncheckOtherPassBoxes(WeekPassChck); };
                MonthPassChck.CheckedChanged += (s, e) => { if (MonthPassChck.Checked) UncheckOtherPassBoxes(MonthPassChck); };

                // Default-arvot
                DispX255txtbox.Text = "3";
                DispPassTxtbox.Text = "3";
                HourX255Chck.Checked = true;
                HourPassChck.Checked = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Katoavien viestien setup epäonnistui: {ex.Message}");
            }
        }

        private void UncheckOtherRSABoxes(ModernCheckBox selectedBox)
        {
            if (selectedBox != HourX255Chck) HourX255Chck.Checked = false;
            if (selectedBox != DayX255Chck) DayX255Chck.Checked = false;
            if (selectedBox != WeekX255Chck) WeekX255Chck.Checked = false;
            if (selectedBox != MonthX255Chck) MonthX255Chck.Checked = false;
        }

        private void UncheckOtherPassBoxes(ModernCheckBox selectedBox)
        {
            if (selectedBox != HourPassChck) HourPassChck.Checked = false;
            if (selectedBox != DayPassCheck) DayPassCheck.Checked = false;
            if (selectedBox != WeekPassChck) WeekPassChck.Checked = false;
            if (selectedBox != MonthPassChck) MonthPassChck.Checked = false;
        }

        #endregion

        #region Turvallinen cleanup

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                // Tyhjennä herkät tiedot turvallisesti
                ClearCurrentPassword();

                // Tyhjennä UI-kentät
                Writemsgbox.Text = "";
                Setpasswordtxt.Text = "";
                Decryptpasstxt.Text = "";
                Decryptmsg.Text = "";

                // Dispose kryptoluokat
                _modernCrypto?.Dispose();
                _legacyCrypto?.Dispose();
                _x25519Manager?.Dispose();

                // Pakota garbage collection (vaikka ei takaa muistin tyhjentymistä)
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch
            {
                // Ignore cleanup errors
            }

            base.OnFormClosed(e);
        }

        #endregion

        #region RSA/X25519 Event Handlers

        /// <summary>
        /// RSWritemsgbox tekstin muutos - päivitä nappien tila dynaamisesti
        /// </summary>
        private void RSWritemsgbox_TextChanged(object sender, EventArgs e)
        {
            // Päivitä nappien tila kun käyttäjä kirjoittaa tai tyhjentää viestin
            UpdateRSAButtonStates();
        }

        #endregion

        #region RSA/X25519 UI State Management

        /// <summary>
        /// Päivittää RSA/X25519-nappien tilan käyttäjäkokemusten parantamiseksi
        /// </summary>
        private void UpdateRSAButtonStates()
        {
            try
            {
                // Tarkista X25519Manager:n tila
                bool hasPrivateKey = _x25519Manager?.HasPrivateKey() ?? false;
                bool hasRecipientKey = _x25519Manager?.HasRecipientKey() ?? false;
                bool hasMessageText = !string.IsNullOrEmpty(RSWritemsgbox?.Text?.Trim());
                bool hasEncryptedData = IsRSWritemsgboxEncrypted();

                // RSGenerate pysyy aina näkyvissä - varoitus hoidetaan click-handlerissa

                // RsEncrypt - vaatii private key + recipient key + viestin
                if (RsEncrypt != null)
                {
                    RsEncrypt.Enabled = hasPrivateKey && hasRecipientKey && hasMessageText && !hasEncryptedData;
                }

                // RsCreatefile - vaatii että salaus on onnistunut
                if (RsCreatefile != null)
                {
                    RsCreatefile.Enabled = hasEncryptedData;
                }

                // RSDecrypt - vaatii vain private keyn
                if (RSDecrypt != null)
                {
                    RSDecrypt.Enabled = hasPrivateKey;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateRSAButtonStates error: {ex.Message}");
            }
        }

        /// <summary>
        /// Tarkistaa sisältääkö RSWritemsgbox salattua dataa
        /// </summary>
        private bool IsRSWritemsgboxEncrypted()
        {
            try
            {
                if (string.IsNullOrEmpty(RSWritemsgbox?.Text))
                    return false;

                var text = RSWritemsgbox.Text.Trim();

                // Yksinkertainen heuristiikka: jos teksti näyttää Base64:ltä ja on pitkä
                if (text.Length > 100 && IsLikelyBase64(text))
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Yksinkertainen Base64-tunnistus
        /// </summary>
        private bool IsLikelyBase64(string text)
        {
            try
            {
                // Base64 sisältää vain tietyt merkit
                var base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
                return text.All(c => base64Chars.Contains(c) || char.IsWhiteSpace(c));
            }
            catch
            {
                return false;
            }
        }

        #endregion

        private void RemoveDataButton_Click(object sender, EventArgs e)
        {
            // Näytä moderni varoitusikkuna
            var result = ShowModernWarningDialog();

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Suorita täydellinen datan poisto
                    PerformCompleteDataWipe();

                    // Näytä onnistumisviesti
                    ShowModernSuccessDialog();

                    // Sammuta sovellus turvallisesti
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Data wipe failed: {ex.Message}\n\nSome data may remain on the system.",
                                   "Data Wipe Error",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Error);
                }
            }
        }

        private DialogResult ShowModernWarningDialog()
        {
            // Luo moderni varoitusikkuna
            var warningForm = new Form
            {
                Text = "OffCrypt - Data Wipe Confirmation",
                Size = new Size(520, 480), // Isompi koko
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F)
            };

            // Pääotsikko
            var titleLabel = new Label
            {
                Text = "⚠️ PERMANENT DATA DESTRUCTION",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60), // Punainen varoitus
                Location = new Point(20, 20),
                Size = new Size(470, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Varoitusteksti
            var warningLabel = new Label
            {
                Text = "Are you absolutely sure you want to permanently delete ALL OffCrypt data?\n\n" +
                       "This action will IRREVERSIBLY remove:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 60),
                Size = new Size(470, 60),
                TextAlign = ContentAlignment.TopLeft
            };

            // Lista mitä poistetaan
            var dataListLabel = new Label
            {
                Text = "🔑 All RSA-4096 key pairs (public & private keys)\n" +
                       "🔐 All encrypted storage data\n" +
                       "📁 All key storage files and folders\n" +
                       "📋 All registry entries and configuration\n" +
                       "💾 All temporary files and cached data\n" +
                       "🗑️ All application memory data\n\n" +
                       "📢 ALL PREVIOUSLY ENCRYPTED MESSAGES WILL BECOME\n" +
                       "     PERMANENTLY UNRECOVERABLE!",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(240, 240, 240),
                Location = new Point(30, 130),
                Size = new Size(450, 160),
                TextAlign = ContentAlignment.TopLeft
            };

            // Varoitusteksti lopuksi
            var finalWarningLabel = new Label
            {
                Text = "This action cannot be undone. Proceed with extreme caution.",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Italic),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(20, 300),
                Size = new Size(470, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Painikkeet - alemmas ja keskitetymmin
            var yesButton = new CryptButtonC
            {
                Text = "🗑️ YES, DELETE ALL",
                Size = new Size(160, 40),
                Location = new Point(100, 350),
                PrimaryColor = Color.FromArgb(192, 57, 43), // Tummanpunainen
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                DialogResult = DialogResult.Yes
            };

            var noButton = new CryptButtonC
            {
                Text = "❌ CANCEL",
                Size = new Size(130, 40),
                Location = new Point(280, 350),
                PrimaryColor = Color.FromArgb(44, 62, 80), // Normaali
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                DialogResult = DialogResult.No
            };

            // Lisää kontrollit formiin
            warningForm.Controls.AddRange(new Control[]
            {
                titleLabel, warningLabel, dataListLabel, finalWarningLabel, yesButton, noButton
            });

            // Näytä dialogi
            return warningForm.ShowDialog(this);
        }

        private void PerformCompleteDataWipe()
        {
            try
            {
                // 1. Tyhjennä kaikki muistimuuttujat
                ClearAllSensitiveMemoryData();




                // 3. Poista registry-tiedot
                WipeRegistryData();

                // 4. Poista tiedostot ja kansiot
                WipeFileSystemData();

                // 5. Tyhjennä leikepöytä
                WipeClipboard();

                // 6. Tyhjennä UI-kentät
                WipeUIFields();

                // 7. Pakota garbage collection useita kertoja
                ForceMemoryCleanup();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Data wipe error: {ex.Message}");
                throw;
            }
        }

        private void ClearAllSensitiveMemoryData()
        {
            try
            {
                // Tyhjennä salasana muuttujat
                ClearCurrentPassword();

                // Tyhjennä RSA operations
                _x25519Manager?.Dispose();
                _x25519Manager = null;

                // Tyhjennä kryptoluokat
                _modernCrypto?.Dispose();
                _legacyCrypto?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Memory clear error: {ex.Message}");
            }
        }


        private void WipeRegistryData()
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\OffCrypt", true))
                {
                    if (key != null)
                    {
                        // Poista kaikki OffCrypt registry-avaimet
                        Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(@"SOFTWARE\OffCrypt");
                        System.Diagnostics.Debug.WriteLine("Registry data wiped successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Registry wipe error: {ex.Message}");
            }
        }

        private void WipeFileSystemData()
        {
            try
            {
                // Poista OffCrypt-kansiot eri sijainneista
                var locations = new[]
                {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OffCrypt"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OffCrypt"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OffCrypt"),
                    Path.Combine(Path.GetTempPath(), "OffCrypt"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".offcrypt")
                };

                foreach (var location in locations)
                {
                    try
                    {
                        if (Directory.Exists(location))
                        {
                            Directory.Delete(location, true);
                            System.Diagnostics.Debug.WriteLine($"Deleted directory: {location}");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Could not delete {location}: {ex.Message}");
                    }
                }

                // Poista yksittäiset tiedostot
                var files = new[]
                {
                    Path.Combine(Path.GetTempPath(), "OffCrypt_PublicKey.pem"),
                    Path.Combine(Path.GetTempPath(), "OffCrypt_PublicKey.json"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "OffCrypt_PublicKey.pem")
                };

                foreach (var file in files)
                {
                    try
                    {
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                            System.Diagnostics.Debug.WriteLine($"Deleted file: {file}");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Could not delete {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"File system wipe error: {ex.Message}");
            }
        }

        private void WipeClipboard()
        {
            try
            {
                // Tyhjennä leikepöytä turvallisesti
                for (int i = 0; i < 3; i++)
                {
                    Clipboard.Clear();
                    Clipboard.SetText(new string('X', 1024)); // Ylikirjoita random datalla
                    Thread.Sleep(10);
                }
                Clipboard.Clear();
                System.Diagnostics.Debug.WriteLine("Clipboard wiped successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Clipboard wipe error: {ex.Message}");
            }
        }

        private void WipeUIFields()
        {
            try
            {
                // Tyhjennä kaikki UI-kentät
                var textBoxes = new TextBox[]
                {
                    Writemsgbox, Setpasswordtxt, Decryptpasstxt, Decryptmsg,
                    RSPublickeymsgbox, RSWritemsgbox, RSDecryptmsgbox,
                    DispPassTxtbox, DispX255txtbox
                };

                foreach (var textBox in textBoxes)
                {
                    if (textBox != null)
                    {
                        textBox.Text = "";
                        textBox.Clear();
                    }
                }

                System.Diagnostics.Debug.WriteLine("UI fields wiped successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UI wipe error: {ex.Message}");
            }
        }

        private void ForceMemoryCleanup()
        {
            try
            {
                // Pakota garbage collection useita kertoja
                for (int i = 0; i < 5; i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    Thread.Sleep(100);
                }

                // Yritä vapauttaa työmuistia
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                GC.WaitForPendingFinalizers();

                System.Diagnostics.Debug.WriteLine("Memory cleanup completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Memory cleanup error: {ex.Message}");
            }
        }

        private void ShowModernSuccessDialog()
        {
            var successForm = new Form
            {
                Text = "OffCrypt - Data Wipe Complete",
                Size = new Size(420, 320), // Isompi koko
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White
            };

            var iconLabel = new Label
            {
                Text = "✅",
                Font = new Font("Segoe UI", 36F),
                ForeColor = Color.FromArgb(39, 174, 96),
                Location = new Point(180, 30),
                Size = new Size(60, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var messageLabel = new Label
            {
                Text = "DATA WIPE COMPLETED SUCCESSFULLY\n\n" +
                       "All OffCrypt data has been permanently removed.\n" +
                       "The application will now close.",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 110),
                Size = new Size(380, 100),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var okButton = new CryptButtonC
            {
                Text = "EXIT APPLICATION",
                Size = new Size(160, 40),
                Location = new Point(130, 230),
                PrimaryColor = Color.FromArgb(39, 174, 96),
                DialogResult = DialogResult.OK
            };

            successForm.Controls.AddRange(new Control[] { iconLabel, messageLabel, okButton });
            successForm.ShowDialog(this);
        }

        #region ECDH and INI Settings Management

        /// <summary>
        /// Alustaa sekä ECDH- että INI-asetukset sovelluksen käynnistyessä
        /// </summary>
        private void InitializeECDHAndINISettings()
        {
            try
            {
                // Ensin lataa INI-asetukset (tai luo oletukset)
                InitializeINISettings();

                // Sitten alusta ECDH nykyisen asetuksen mukaan
                InitializeECDHSettings();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ECDH/INI initialization failed: {ex.Message}");
                // Käytä oletusasetuksia
                _currentEcdhMode = ECDHMode.StaticP256;
                _modernCrypto = new PasswordSecure(_currentEcdhMode);
            }
        }

        /// <summary>
        /// Alustaa ECDH-asetukset
        /// </summary>
        private void InitializeECDHSettings()
        {
            try
            {
                // Lataa tallennettu asetus tai käytä oletusta
                _currentEcdhMode = LoadECDHSetting();

                // Alusta moderni kryptoluokka valitulla moodilla
                _modernCrypto = new PasswordSecure(_currentEcdhMode);

                System.Diagnostics.Debug.WriteLine($"ECDH initialized: {_currentEcdhMode}");
            }
            catch (Exception ex)
            {
                // Jos virhe, käytä oletusta
                _currentEcdhMode = ECDHMode.StaticP256;
                _modernCrypto = new PasswordSecure(_currentEcdhMode);

                System.Diagnostics.Debug.WriteLine($"ECDH init failed, using default: {ex.Message}");
            }
        }

        /// <summary>
        /// ECDHCombobx selection changed event handler
        /// </summary>
        private void ECDHCombobx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var selectedMode = GetSelectedECDHMode();

                if (selectedMode != _currentEcdhMode)
                {
                    // Näytä vahvistusviesti käyttäjälle
                    var result = ShowECDHModeChangeConfirmation(selectedMode);

                    if (result == DialogResult.Yes)
                    {
                        // Vaihda uuteen moodiin
                        ChangeECDHMode(selectedMode);
                    }
                    else
                    {
                        // Palauta vanha arvo
                        SetECDHComboBoxValue(_currentEcdhMode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ECDH mode change failed: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Palauta vanha arvo
                SetECDHComboBoxValue(_currentEcdhMode);
            }
        }

        /// <summary>
        /// Alustaa INI-asetukset sovelluksen käynnistyessä
        /// </summary>
        private void InitializeINISettings()
        {
            try
            {
                if (!INIManager.INIFileExists())
                {
                    // Ensimmäinen käynnistys - luo oletusasetukset
                    SettingsManager.InitializeDefaultSettings(this);

                    MessageBox.Show(
                        "Welcome to OffCrypt!\n\n" +
                        "Default settings have been loaded.\n" +
                        "Configuration saved to:\n" +
                        INIManager.GetINIFilePath(),
                        "First Run - Default Settings",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    // Lataa olemassa olevat asetukset
                    SettingsManager.LoadSettingsFromINI(this);
                    System.Diagnostics.Debug.WriteLine("INI settings loaded successfully");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load settings: {ex.Message}\n\n" +
                    "Using default settings instead.",
                    "Settings Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                SettingsManager.LoadDefaultSettings(this);
            }
        }

        /// <summary>
        /// Save INI button click handler
        /// </summary>
        private void Saveinibtn_Click(object sender, EventArgs e)
        {
            try
            {
                SettingsManager.SaveSettingsToINI(this);

                string filePath = INIManager.GetINIFilePath();
                long fileSize = INIManager.GetINIFileSize();

                MessageBox.Show(
                    $"✅ Settings saved successfully!\n\n" +
                    $"📁 File: {Path.GetFileName(filePath)}\n" +
                    $"📊 Size: {fileSize} bytes\n" +
                    $"📍 Location:\n{Path.GetDirectoryName(filePath)}",
                    "Settings Saved",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Failed to save settings:\n\n{ex.Message}",
                    "Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load INI button click handler
        /// </summary>
        private void Loadinibtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!INIManager.INIFileExists())
                {
                    var result = MessageBox.Show(
                        "No settings file found!\n\n" +
                        "Would you like to create default settings?",
                        "No Settings File",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        SettingsManager.InitializeDefaultSettings(this);
                        MessageBox.Show(
                            "Default settings created and loaded!",
                            "Settings Created",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    return;
                }

                SettingsManager.LoadSettingsFromINI(this);

                string fileInfo = SettingsManager.GetINIFileInfo();

                MessageBox.Show(
                    $"✅ Settings loaded successfully!\n\n" +
                    $"📋 File Information:\n{fileInfo}",
                    "Settings Loaded",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Failed to load settings:\n\n{ex.Message}\n\n" +
                    "Using current settings instead.",
                    "Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Palauttaa nykyisen ECDH-moodin (tarvitaan SettingsManager:lle)
        /// </summary>
        public ECDHMode GetCurrentECDHMode()
        {
            return _currentEcdhMode;
        }

        /// <summary>
        /// Vaihtaa ECDH-moodin ja päivittää kryptoluokan
        /// </summary>
        private void ChangeECDHMode(ECDHMode newMode)
        {
            try
            {
                // Dispose vanha kryptoluokka
                _modernCrypto?.Dispose();

                // Päivitä tila
                _currentEcdhMode = newMode;

                // Luo uusi kryptoluokka uudella moodilla
                _modernCrypto = new PasswordSecure(_currentEcdhMode);

                // Tallenna asetus
                SaveECDHSetting(_currentEcdhMode);

                // Näytä onnistumisviesti
                ShowECDHModeChangeSuccess(newMode);

                System.Diagnostics.Debug.WriteLine($"ECDH mode changed to: {newMode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ECDH mode change error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Hakee valitun ECDH-moodin ComboBoxista
        /// </summary>
        private ECDHMode GetSelectedECDHMode()
        {
            return ECDHCombobx.SelectedIndex switch
            {
                0 => ECDHMode.StaticP256,  // "Static Key (P-256)"
                1 => ECDHMode.StaticP384,  // "Static Key (P-384)"  
                2 => ECDHMode.StaticP521,  // "Static Key (P-521)"
                3 => ECDHMode.Ephemeral,   // "Ephemeral Keys"
                _ => ECDHMode.StaticP256
            };
        }

        /// <summary>
        /// Asettaa ComboBoxin oikeaan arvoon
        /// </summary>
        private void SetECDHComboBoxValue(ECDHMode mode)
        {
            ECDHCombobx.SelectedIndex = mode switch
            {
                ECDHMode.StaticP256 => 0,  // "Static Key (P-256)"
                ECDHMode.StaticP384 => 1,  // "Static Key (P-384)"
                ECDHMode.StaticP521 => 2,  // "Static Key (P-521)"
                ECDHMode.Ephemeral => 3,   // "Ephemeral Keys"
                _ => 0
            };
        }

        /// <summary>
        /// Lataa ECDH-asetuksen rekisteristä
        /// </summary>
        private ECDHMode LoadECDHSetting()
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\OffCrypt"))
                {
                    if (key?.GetValue("ECDHMode") is string modeString &&
                        Enum.TryParse<ECDHMode>(modeString, out ECDHMode savedMode))
                    {
                        return savedMode;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load ECDH setting: {ex.Message}");
            }

            return ECDHMode.StaticP256; // Oletus
        }

        /// <summary>
        /// Tallentaa ECDH-asetuksen rekisteriin
        /// </summary>
        private void SaveECDHSetting(ECDHMode mode)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\OffCrypt"))
                {
                    key.SetValue("ECDHMode", mode.ToString());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save ECDH setting: {ex.Message}");
            }
        }

        /// <summary>
        /// Näyttää vahvistusdialogin ECDH-moodin vaihdolle
        /// </summary>
        private DialogResult ShowECDHModeChangeConfirmation(ECDHMode newMode)
        {
            var modeName = StaticKeyManager.GetCurveDisplayName(newMode);
            var compatibilityWarning = newMode == ECDHMode.Ephemeral ?
                "\n\n⚠️ WARNING: Ephemeral mode will make previously encrypted messages unrecoverable!" :
                "\n\n✅ This mode maintains compatibility with previously encrypted messages.";

            return MessageBox.Show(
                $"Change ECDH encryption mode to:\n{modeName}?{compatibilityWarning}\n\n" +
                "This will affect all new encrypted messages.",
                "Confirm ECDH Mode Change",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
        }

        /// <summary>
        /// Näyttää onnistumisdialogin ECDH-moodin vaihdon jälkeen
        /// </summary>
        private void ShowECDHModeChangeSuccess(ECDHMode newMode)
        {
            var modeName = StaticKeyManager.GetCurveDisplayName(newMode);
            var hasStaticKey = StaticKeyManager.HasStaticKey(newMode);
            var keyStatus = newMode == ECDHMode.Ephemeral ?
                "New keys generated for each message" :
                hasStaticKey ? "Using existing static key" : "New static key created";

            MessageBox.Show(
                $"✅ ECDH mode changed successfully!\n\n" +
                $"🔑 Mode: {modeName}\n" +
                $"📊 Status: {keyStatus}\n\n" +
                $"All new encrypted messages will use this configuration.",
                "ECDH Mode Updated",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #endregion

        /// <summary>
        /// Siivoa resurssit kun Form suljetaan
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                // Siivoa ContactManager-resurssit turvallisesti
                _contactManager?.Dispose();

                // Siivoa X25519Manager (sisältää herkät avaimet)
                _x25519Manager?.Dispose();

                // Siivoa salasana muistista
                if (_currentPassword != null)
                {
                    Array.Clear(_currentPassword, 0, _currentPassword.Length);
                    _currentPassword = null;
                }

                ApplicationLogger.LogInfo("Form1", "Form resources disposed successfully");
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError("Form1", $"Error during cleanup: {ex.Message}");
            }

            base.OnFormClosing(e);
        }

        /// <summary>
        /// Laskee fingerprint vastaanottajan X25519 julkisesta avaimesta ContactManager-integraatiota varten
        /// </summary>
        private string? CalculateRecipientFingerprint(string? armoredPublicKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(armoredPublicKey))
                    return null;

                // Dekoodaa X25519 PUBLIC KEY armor samalla tavalla kuin ContactManager.cs
                byte[] spkiDer = ArmorUtil.Decode(armoredPublicKey, "PUBLIC KEY");

                // Käytä samaa HashUtil.Fingerprint16 funktiota kuin X25519Manager ja ContactManager
                byte[] fingerprintBytes = HashUtil.Fingerprint16(spkiDer);
                return Convert.ToHexString(fingerprintBytes);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError("Form1", $"Failed to calculate recipient fingerprint: {ex.Message}");
                return null;
            }
        }

        private void cryptButtonc4_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void loyalSubTabBar1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
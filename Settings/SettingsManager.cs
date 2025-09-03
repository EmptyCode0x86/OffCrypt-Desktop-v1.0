using System;
using System.Windows.Forms;

namespace OffCrypt
{
    /// <summary>
    /// Hallinnoi OffCrypt-sovelluksen UI-asetusten tallentamista ja lataamista INI-tiedostoon
    /// Käsittelee Settings-tabin kaikkien komponenttien tiloja
    /// </summary>
    public static class SettingsManager
    {
        /// <summary>
        /// Tallentaa kaikki Settings-tabin asetukset INI-tiedostoon
        /// </summary>
        public static void SaveSettingsToINI(Form1 form1)
        {
            try
            {
                // Advanced Security - ECDH
                var ecdhMode = form1.GetCurrentECDHMode();
                INIManager.WriteValue("AdvancedSecurity", "ECDHMode", ecdhMode.ToString());
                INIManager.WriteInt("AdvancedSecurity", "ECDHModeIndex", form1.ECDHCombobx.SelectedIndex);

                // Disappearing Messages - Password mode
                SaveDisappearingPasswordSettings(form1);

                // Disappearing Messages - RSA mode  
                SaveDisappearingRSASettings(form1);

                // General info
                INIManager.WriteValue("General", "LastSaveDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                INIManager.WriteValue("General", "Version", "1.0");

                System.Diagnostics.Debug.WriteLine($"Settings saved to: {INIManager.GetINIFilePath()}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Save settings error: {ex.Message}");
                throw new InvalidOperationException($"Settings save failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lataa kaikki asetukset INI-tiedostosta ja asettaa ne UI:hin
        /// </summary>
        public static void LoadSettingsFromINI(Form1 form1)
        {
            try
            {
                if (!INIManager.INIFileExists())
                {
                    // Jos INI-tiedostoa ei ole, luo oletusasetukset
                    LoadDefaultSettings(form1);
                    return;
                }

                // Advanced Security - ECDH
                LoadECDHSettings(form1);

                // Disappearing Messages - Password mode
                LoadDisappearingPasswordSettings(form1);

                // Disappearing Messages - RSA mode
                LoadDisappearingRSASettings(form1);

                System.Diagnostics.Debug.WriteLine($"Settings loaded from: {INIManager.GetINIFilePath()}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load settings error: {ex.Message}");
                
                // Jos lataus epäonnistui, käytä oletusasetuksia
                LoadDefaultSettings(form1);
                throw new InvalidOperationException($"Settings load failed, using defaults: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lataa oletusasetukset UI:hin
        /// </summary>
        public static void LoadDefaultSettings(Form1 form1)
        {
            try
            {
                // ECDH oletuksena Static Key (P-256)
                form1.ECDHCombobx.SelectedIndex = 0;

                // Disappearing Messages - Password mode oletukset
                form1.ToggleDispPass.Checked = false;
                form1.DispPassTxtbox.Text = "3";
                form1.HourPassChck.Checked = true;
                form1.DayPassCheck.Checked = false;
                form1.WeekPassChck.Checked = false;
                form1.MonthPassChck.Checked = false;

                // Disappearing Messages - RSA mode oletukset
                form1.ToggleDispX255.Checked = false;
                form1.DispX255txtbox.Text = "3";
                form1.HourX255Chck.Checked = true;
                form1.DayX255Chck.Checked = false;
                form1.WeekX255Chck.Checked = false;
                form1.MonthX255Chck.Checked = false;

                System.Diagnostics.Debug.WriteLine("Default settings loaded");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load default settings error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Luo oletusasetusten INI-tiedoston ja lataa asetukset
        /// </summary>
        public static void InitializeDefaultSettings(Form1 form1)
        {
            try
            {
                // Luo oletusasetusten INI-tiedosto
                INIManager.CreateDefaultINI();

                // Lataa oletusasetukset UI:hin
                LoadDefaultSettings(form1);

                System.Diagnostics.Debug.WriteLine("Default settings initialized and INI file created");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Initialize default settings error: {ex.Message}");
                // Edes oletusasetukset UI:hin
                LoadDefaultSettings(form1);
                throw;
            }
        }

        /// <summary>
        /// Tallentaa Password-moodin disappearing message -asetukset
        /// </summary>
        private static void SaveDisappearingPasswordSettings(Form1 form1)
        {
            INIManager.WriteBool("DisappearingMessages_Password", "Enabled", form1.ToggleDispPass.Checked);
            INIManager.WriteValue("DisappearingMessages_Password", "TimeValue", form1.DispPassTxtbox.Text);
            
            INIManager.WriteBool("DisappearingMessages_Password", "HourChecked", form1.HourPassChck.Checked);
            INIManager.WriteBool("DisappearingMessages_Password", "DayChecked", form1.DayPassCheck.Checked);
            INIManager.WriteBool("DisappearingMessages_Password", "WeekChecked", form1.WeekPassChck.Checked);
            INIManager.WriteBool("DisappearingMessages_Password", "MonthChecked", form1.MonthPassChck.Checked);

            // Määritä TimeUnit aktiivisen checkboxin mukaan
            string timeUnit = "Hour"; // Oletus
            if (form1.HourPassChck.Checked) timeUnit = "Hour";
            else if (form1.DayPassCheck.Checked) timeUnit = "Day";
            else if (form1.WeekPassChck.Checked) timeUnit = "Week";
            else if (form1.MonthPassChck.Checked) timeUnit = "Month";
            
            INIManager.WriteValue("DisappearingMessages_Password", "TimeUnit", timeUnit);
        }

        /// <summary>
        /// Tallentaa RSA-moodin disappearing message -asetukset
        /// </summary>
        private static void SaveDisappearingRSASettings(Form1 form1)
        {
            INIManager.WriteBool("DisappearingMessages_RSA", "Enabled", form1.ToggleDispX255.Checked);
            INIManager.WriteValue("DisappearingMessages_RSA", "TimeValue", form1.DispX255txtbox.Text);
            
            INIManager.WriteBool("DisappearingMessages_RSA", "HourChecked", form1.HourX255Chck.Checked);
            INIManager.WriteBool("DisappearingMessages_RSA", "DayChecked", form1.DayX255Chck.Checked);
            INIManager.WriteBool("DisappearingMessages_RSA", "WeekChecked", form1.WeekX255Chck.Checked);
            INIManager.WriteBool("DisappearingMessages_RSA", "MonthChecked", form1.MonthX255Chck.Checked);

            // Määritä TimeUnit aktiivisen checkboxin mukaan
            string timeUnit = "Hour"; // Oletus
            if (form1.HourX255Chck.Checked) timeUnit = "Hour";
            else if (form1.DayX255Chck.Checked) timeUnit = "Day";
            else if (form1.WeekX255Chck.Checked) timeUnit = "Week";
            else if (form1.MonthX255Chck.Checked) timeUnit = "Month";
            
            INIManager.WriteValue("DisappearingMessages_RSA", "TimeUnit", timeUnit);
        }

        /// <summary>
        /// Lataa ECDH-asetukset
        /// </summary>
        private static void LoadECDHSettings(Form1 form1)
        {
            string ecdhMode = INIManager.ReadValue("AdvancedSecurity", "ECDHMode", "StaticP256");
            int ecdhIndex = INIManager.ReadInt("AdvancedSecurity", "ECDHModeIndex", 0);

            // Varmista että index on validi
            if (ecdhIndex >= 0 && ecdhIndex < form1.ECDHCombobx.Items.Count)
            {
                form1.ECDHCombobx.SelectedIndex = ecdhIndex;
            }
            else
            {
                form1.ECDHCombobx.SelectedIndex = 0; // Oletus
            }
        }

        /// <summary>
        /// Lataa Password-moodin disappearing message -asetukset
        /// </summary>
        private static void LoadDisappearingPasswordSettings(Form1 form1)
        {
            form1.ToggleDispPass.Checked = INIManager.ReadBool("DisappearingMessages_Password", "Enabled", false);
            form1.DispPassTxtbox.Text = INIManager.ReadValue("DisappearingMessages_Password", "TimeValue", "3");
            
            form1.HourPassChck.Checked = INIManager.ReadBool("DisappearingMessages_Password", "HourChecked", true);
            form1.DayPassCheck.Checked = INIManager.ReadBool("DisappearingMessages_Password", "DayChecked", false);
            form1.WeekPassChck.Checked = INIManager.ReadBool("DisappearingMessages_Password", "WeekChecked", false);
            form1.MonthPassChck.Checked = INIManager.ReadBool("DisappearingMessages_Password", "MonthChecked", false);
        }

        /// <summary>
        /// Lataa RSA-moodin disappearing message -asetukset
        /// </summary>
        private static void LoadDisappearingRSASettings(Form1 form1)
        {
            form1.ToggleDispX255.Checked = INIManager.ReadBool("DisappearingMessages_RSA", "Enabled", false);
            form1.DispX255txtbox.Text = INIManager.ReadValue("DisappearingMessages_RSA", "TimeValue", "3");
            
            form1.HourX255Chck.Checked = INIManager.ReadBool("DisappearingMessages_RSA", "HourChecked", true);
            form1.DayX255Chck.Checked = INIManager.ReadBool("DisappearingMessages_RSA", "DayChecked", false);
            form1.WeekX255Chck.Checked = INIManager.ReadBool("DisappearingMessages_RSA", "WeekChecked", false);
            form1.MonthX255Chck.Checked = INIManager.ReadBool("DisappearingMessages_RSA", "MonthChecked", false);
        }

        /// <summary>
        /// Palauttaa tiedot INI-tiedostosta
        /// </summary>
        public static string GetINIFileInfo()
        {
            try
            {
                if (!INIManager.INIFileExists())
                    return "INI file does not exist";

                string filePath = INIManager.GetINIFilePath();
                long fileSize = INIManager.GetINIFileSize();
                string lastSave = INIManager.ReadValue("General", "LastSaveDate", "Unknown");
                string version = INIManager.ReadValue("General", "Version", "Unknown");

                return $"File: {filePath}\n" +
                       $"Size: {fileSize} bytes\n" +
                       $"Last saved: {lastSave}\n" +
                       $"Version: {version}";
            }
            catch (Exception ex)
            {
                return $"Error reading INI info: {ex.Message}";
            }
        }
    }
}
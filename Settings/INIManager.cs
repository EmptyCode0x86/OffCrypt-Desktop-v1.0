using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OffCrypt
{
    public static class INIManager
    {
        private const string INI_FILENAME = "OffCrypt_Settings.ini";
        
        public static string GetINIFilePath()
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(userProfile, INI_FILENAME);
        }

        public static bool INIFileExists()
        {
            return File.Exists(GetINIFilePath());
        }

        public static string ReadValue(string section, string key, string defaultValue = "")
        {
            try
            {
                string filePath = GetINIFilePath();
                
                if (!File.Exists(filePath))
                    return defaultValue;

                var lines = File.ReadAllLines(filePath, Encoding.UTF8);
                bool inCorrectSection = false;
                string targetSection = $"[{section}]";

                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();
                    
                    if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                        continue;

                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                    {
                        inCorrectSection = trimmedLine.Equals(targetSection, StringComparison.OrdinalIgnoreCase);
                        continue;
                    }

                    if (inCorrectSection && trimmedLine.Contains("="))
                    {
                        var parts = trimmedLine.Split('=', 2);
                        if (parts.Length == 2 && parts[0].Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            return parts[1].Trim();
                        }
                    }
                }

                return defaultValue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"INI read error: {ex.Message}");
                return defaultValue;
            }
        }

        public static bool ReadBool(string section, string key, bool defaultValue = false)
        {
            string value = ReadValue(section, key, defaultValue.ToString());
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        public static int ReadInt(string section, string key, int defaultValue = 0)
        {
            string value = ReadValue(section, key, defaultValue.ToString());
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        public static void WriteValue(string section, string key, string value)
        {
            try
            {
                string filePath = GetINIFilePath();
                var iniData = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

                if (File.Exists(filePath))
                {
                    LoadINIData(filePath, iniData);
                }

                if (!iniData.ContainsKey(section))
                    iniData[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                iniData[section][key] = value;

                SaveINIData(filePath, iniData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"INI write error: {ex.Message}");
                throw new InvalidOperationException($"Failed to write INI file: {ex.Message}", ex);
            }
        }

        public static void WriteBool(string section, string key, bool value)
        {
            WriteValue(section, key, value.ToString().ToLower());
        }

        public static void WriteInt(string section, string key, int value)
        {
            WriteValue(section, key, value.ToString());
        }

        public static void DeleteINIFile()
        {
            try
            {
                string filePath = GetINIFilePath();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"INI delete error: {ex.Message}");
            }
        }

        public static void CreateDefaultINI()
        {
            try
            {
                string filePath = GetINIFilePath();
                if (File.Exists(filePath))
                    File.Delete(filePath);

                var defaultContent = new StringBuilder();
                defaultContent.AppendLine("; OffCrypt Settings Configuration File");
                defaultContent.AppendLine("; Auto-generated on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                defaultContent.AppendLine();
                
                defaultContent.AppendLine("[AdvancedSecurity]");
                defaultContent.AppendLine("ECDHMode=StaticP256");
                defaultContent.AppendLine("ECDHModeIndex=0");
                defaultContent.AppendLine();
                
                defaultContent.AppendLine("[DisappearingMessages_Password]");
                defaultContent.AppendLine("Enabled=false");
                defaultContent.AppendLine("TimeValue=3");
                defaultContent.AppendLine("TimeUnit=Hour");
                defaultContent.AppendLine("HourChecked=true");
                defaultContent.AppendLine("DayChecked=false");
                defaultContent.AppendLine("WeekChecked=false");
                defaultContent.AppendLine("MonthChecked=false");
                defaultContent.AppendLine();
                
                defaultContent.AppendLine("[DisappearingMessages_RSA]");
                defaultContent.AppendLine("Enabled=false");
                defaultContent.AppendLine("TimeValue=3");
                defaultContent.AppendLine("TimeUnit=Hour");
                defaultContent.AppendLine("HourChecked=true");
                defaultContent.AppendLine("DayChecked=false");
                defaultContent.AppendLine("WeekChecked=false");
                defaultContent.AppendLine("MonthChecked=false");
                defaultContent.AppendLine();
                
                defaultContent.AppendLine("[General]");
                defaultContent.AppendLine("LastSaveDate=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                defaultContent.AppendLine("Version=1.0");

                File.WriteAllText(filePath, defaultContent.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Create default INI error: {ex.Message}");
                throw;
            }
        }

        private static void LoadINIData(string filePath, Dictionary<string, Dictionary<string, string>> iniData)
        {
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            string currentSection = "";

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                    continue;

                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    if (!iniData.ContainsKey(currentSection))
                        iniData[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    continue;
                }

                if (!string.IsNullOrEmpty(currentSection) && trimmedLine.Contains("="))
                {
                    var parts = trimmedLine.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                        iniData[currentSection][key] = value;
                    }
                }
            }
        }

        private static void SaveINIData(string filePath, Dictionary<string, Dictionary<string, string>> iniData)
        {
            var content = new StringBuilder();
            content.AppendLine("; OffCrypt Settings Configuration File");
            content.AppendLine("; Last saved: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            content.AppendLine();

            foreach (var section in iniData)
            {
                content.AppendLine($"[{section.Key}]");

                foreach (var keyValue in section.Value)
                {
                    content.AppendLine($"{keyValue.Key}={keyValue.Value}");
                }

                content.AppendLine();
            }

            File.WriteAllText(filePath, content.ToString(), Encoding.UTF8);
        }

        public static long GetINIFileSize()
        {
            try
            {
                string filePath = GetINIFilePath();
                return File.Exists(filePath) ? new FileInfo(filePath).Length : -1;
            }
            catch
            {
                return -1;
            }
        }
    }
}
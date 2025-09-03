using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OffCrypt
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            
            // Check if identity exists
            if (!HasExistingIdentity())
            {
                // Show identity registration first
                using (var identityForm = new IdentityRegisterForm())
                {
                    var result = identityForm.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        // User cancelled, exit application
                        return;
                    }
                }
            }
            
            // Open main application
            Application.Run(new Form1());
        }
        
        private static bool HasExistingIdentity()
        {
            try
            {
                if (!System.IO.Directory.Exists(KeyringManager.KeyringDir))
                    return false;

                var pubFiles = System.IO.Directory.GetFiles(KeyringManager.KeyringDir, "*.ocpub");
                return pubFiles.Length > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}

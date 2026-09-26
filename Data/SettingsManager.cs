using System.IO;
using System.Text.Json;

namespace TechVault.UI.Data
{
    public static class SettingsManager
    {
        private static readonly string _settingsFilePath = "appsettings.json";

        public static bool UseMockData(string pageKey)
        {
            try
            {
                if (!File.Exists(_settingsFilePath)) return false;

                var json = File.ReadAllText(_settingsFilePath);
                using var document = JsonDocument.Parse(json);

                // Look for the specific page setting inside the MockDataSettings object
                if (document.RootElement.TryGetProperty("MockDataSettings", out var mockSettings))
                {
                    if (mockSettings.TryGetProperty(pageKey, out var pageSetting))
                    {
                        return pageSetting.GetBoolean();
                    }
                }

                // Fallback for older global setting just in case it's still lying around
                if (document.RootElement.TryGetProperty("UseMockData", out var globalSetting))
                {
                    return globalSetting.GetBoolean();
                }

                return false;
            }
            catch
            {
                // Fallback to live data on error or if the key isn't found
                return false;
            }
        }
    }
}
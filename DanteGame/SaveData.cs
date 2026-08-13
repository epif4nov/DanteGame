using System.IO;
using System.Text.Json;

namespace DanteGame
{
    public static class SaveData
    {
        private static string SavePath = "bestscore.json";

        public static void SaveBestScore(int bestScore)
        {
            try
            {
                string json = JsonSerializer.Serialize(bestScore);
                File.WriteAllText(SavePath, json);
            }
            catch { /* игнорируем ошибки сохранения */ }
        }

        public static int LoadBestScore()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    string json = File.ReadAllText(SavePath);
                    return JsonSerializer.Deserialize<int>(json);
                }
            }
            catch { /* игнорируем ошибки загрузки */ }
            return 0;
        }
    }
}
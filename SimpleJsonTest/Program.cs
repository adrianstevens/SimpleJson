using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace SimpleJsonTest
{
    public struct CalibrationPoint
    {
        /// <summary>
        /// Creates a CalibrationPoint instance
        /// </summary>
        /// <param name="rawX">The raw touchscreen X value</param>
        /// <param name="screenX">The equivalent screen X coordinate for the raw X value</param>
        /// <param name="rawY">The raw touchscreen Y value</param>
        /// <param name="screenY">The equivalent screen Y coordinate for the raw Y value</param>
        public CalibrationPoint(int rawX, int screenX, int rawY, int screenY)
        {
            ScreenX = screenX;
            ScreenY = screenY;
            RawX = rawX;
            RawY = rawY;
        }

        /// <summary>
        /// The equivalent screen X coordinate for the raw X value
        /// </summary>
        public int ScreenX { get; set; }
        /// <summary>
        /// The equivalent screen Y coordinate for the raw Y value
        /// </summary>
        public int ScreenY { get; set; }
        /// <summary>
        /// The raw touchscreen X value
        /// </summary>
        public int RawX { get; set; }
        /// <summary>
        /// The raw touchscreen Y value
        /// </summary>
        public int RawY { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"({RawX}->{ScreenX}, {RawY}->{ScreenY})";
        }
    }

    internal class MenuContainer
    {
        public MenuItem[] Menu { get; set; }
    }

    internal class MenuItem
    {
        public string Text { get; set; }
        public string Id { get; set; }
        public int Value { get; set; }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            var resourceData = LoadResource("menu.json");

            var menuJson = new string(System.Text.Encoding.UTF8.GetChars(resourceData));

            DeserializeTypeSafe(menuJson);
            DeserializeAsHashtable(menuJson);
        }

        private static void DeserializeTypeSafe(string menuJson)
        {
            string testJsonItem = "{\"ScreenX\":290,\"ScreenY\":210,\"RawX\":3341,\"RawY\":3353}";
            var point = SimpleJsonSerializer.JsonSerializer.Deserialize<CalibrationPoint>(testJsonItem);

            string testJsonArray = "[{\"ScreenX\":30,\"ScreenY\":30,\"RawX\":522,\"RawY\":514},{\"ScreenX\":290,\"ScreenY\":210,\"RawX\":3341,\"RawY\":3353}]";

            var points = SimpleJsonSerializer.JsonSerializer.Deserialize<CalibrationPoint[]>(testJsonArray);

            SimpleJsonSerializer.JsonSerializer.Deserialize<MenuContainer>(menuJson);
        }

        private static void DeserializeAsHashtable(string menuJson)
        {
            var menuData = SimpleJsonSerializer.JsonSerializer.DeserializeString(menuJson) as Hashtable;

            if (menuData["menu"] == null)
            {
                throw new ArgumentException("JSON root must contain a 'menu' item");
            }

            Console.WriteLine($"Root element is {menuData["menu"]}");

            var items = (ArrayList)menuData["menu"];

            foreach (Hashtable item in items)
            {
                Console.WriteLine($"Found {item["text"]}");
            }

            Console.WriteLine($"{ClockService.GetTime()}");
        }

        private static byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"SimpleJsonTest.{filename}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}
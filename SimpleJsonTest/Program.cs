using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace SimpleJsonTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            var resourceData = LoadResource("menu.json");

            var menuJson = new string(System.Text.Encoding.UTF8.GetChars(resourceData));
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
        }

        static byte[] LoadResource(string filename)
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
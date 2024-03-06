using BlockPuzzleCore;
using System.Reflection;

namespace ArrayAndListTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, SimpleJson! Arrays and Lists");

            LoadPuzzles();
        }

        static IEnumerable<Puzzle> LoadPuzzles()
        {
            byte[] resourceData = LoadResource("puzzles.json");
            var puzzleJson = new string(System.Text.Encoding.UTF8.GetChars(resourceData));

            var puzzlesArray = SimpleJsonSerializer.JsonSerializer.Deserialize<Puzzle[]>(puzzleJson);

            var puzzles = SimpleJsonSerializer.JsonSerializer.Deserialize<List<Puzzle>>(puzzleJson);

            return puzzles;
        }

        static byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"ArrayAndListTest.{filename}";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using var ms = new MemoryStream();

            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
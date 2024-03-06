using BlockPuzzleCore;
using System.Reflection;

namespace ArrayAndListTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, SimpleJson! Arrays and Lists");

            SerializeEnum();

            SerializePuzzle();

            LoadPuzzles();
        }

        static void SerializeEnum()
        {
            var serializer = new SimpleJsonSerializer.JsonSerializer();

            var value = PieceType.Horizontal2;

            var enumJson = serializer.Serialize(value);
            Console.WriteLine(enumJson);
        }

        static void SerializePuzzle()
        {
            var serializer = new SimpleJsonSerializer.JsonSerializer();

            var puzzle = new Puzzle();
            puzzle.AddPiece(0, 0, PieceType.Horizontal2);
            puzzle.AddPiece(0, 2, PieceType.Vertical2);
            puzzle.AddPiece(2, 0, PieceType.Horizontal3);
            puzzle.AddPiece(2, 3, PieceType.Vertical3);
            puzzle.AddPiece(4, 0, PieceType.Solve);

            var puzzleJson = serializer.Serialize(puzzle);

            Console.WriteLine(puzzleJson);
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
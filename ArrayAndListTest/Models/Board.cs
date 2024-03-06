using System.Text;

namespace BlockPuzzleCore;

public class Board
{
    public byte[,] BoardData { get; private set; }

    public int Width => 6;
    public int Height => 6;

    public Board()
    {
        BoardData = new byte[Width, Height];//0 is clear, 1 is a horizontal block, 2 is a vertical block
    }

    public bool IsOccupied(int x, int y)
    {
        return BoardData[x, y] > 0;
    }

    public void SetPiece(int x, int y, byte b)
    {
        BoardData[x, y] = b;
    }

    public string GetHash()
    {
        StringBuilder hash = new();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                hash.Append(BoardData[x, y]);
            }
        }

        return $"{hash}";
    }

    public bool IsLocationFree(PuzzlePiece piece)
    {
        return IsLocationFree(piece.X, piece.Y, piece.PieceType);
    }

    public bool IsLocationFree(int x, int y, PieceType pieceType)
    {
        int length = 2;
        bool isHorizontal = true;

        if (pieceType == PieceType.Horizontal3 || pieceType == PieceType.Vertical3)
        {
            length = 3;
        }

        if (pieceType == PieceType.Vertical2 || pieceType == PieceType.Vertical3)
        {
            isHorizontal = false;
        }

        if (isHorizontal)
        {   //make sure we're on the BoardData
            if (x + length > Width)
            {
                return false;
            }

            if (x < 0)
            {
                return false;
            }

            for (int i = 0; i < length; i++)
            {
                if (BoardData[x + i, y] > 0)
                {
                    return false;
                }
            }
        }
        else
        {   //make sure we're on the BoardData
            if (y + length > Height)
            {
                return false;
            }

            if (y < 0)
            {
                return false;
            }

            for (int i = 0; i < length; i++)
            {
                if (BoardData[x, y + i] > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Clear()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                BoardData[x, y] = 0;
            }
        }
    }
}
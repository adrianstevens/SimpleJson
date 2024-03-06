using System;
using System.Collections.Generic;

namespace BlockPuzzleCore;

public class Puzzle
{
    public List<PuzzlePiece> Pieces { get; set; }
    public Board Board { get; set; }

    public int MinMoves { get; set; } = -1;
    public int NumBlocks => Pieces.Count;

    readonly Random rand = new();

    public Puzzle()
    {
        Pieces = [];
        Board = new Board();
    }

    public Puzzle Clone()
    {
        var puzzle = new Puzzle();

        foreach (PuzzlePiece piece in Pieces)
        {
            puzzle.AddPiece(piece.X, piece.Y, piece.PieceType);
        }

        return puzzle;
    }

    public void UpdateBoard()
    {
        PuzzlePiece p;

        for (int i = 0; i < Pieces.Count; i++)
        {
            p = Pieces[i];

            bool isHorizontal = true;

            if (p.PieceType == PieceType.Vertical2 || p.PieceType == PieceType.Vertical3)
            {
                isHorizontal = false;
            }

            for (int hor = 0; hor < p.Width; hor++)
            {
                for (int vert = 0; vert < p.Height; vert++)
                {
                    Board.SetPiece(p.X + hor, p.Y + vert, (byte)(isHorizontal ? 1 : 2));
                }
            }
        }
    }

    public bool AddPiece(int x, int y, PieceType type)
    {
        PuzzlePiece piece = new PuzzlePiece(x, y, type);
        byte orientation = 1; //horizontal

        if (type == PieceType.Vertical2 || type == PieceType.Vertical3)
        {
            orientation = 2;
        }

        for (int hor = 0; hor < piece.Width; hor++)
        {
            for (int vert = 0; vert < piece.Height; vert++)
            {
                Board.SetPiece(x + hor, y + vert, orientation);
            }
        }

        Pieces.Add(piece);

        return true;
    }

    public void MovePiece(int xFrom, int yFrom, int xTo, int yTo)
    {
        PuzzlePiece? p = Find(xFrom, yFrom);

        if (p == null)
        {
            return;
        }

        byte orientation = 1; //horizontal

        if (p.PieceType == PieceType.Vertical2 || p.PieceType == PieceType.Vertical3)
        {
            orientation = 2;
        }

        for (int hor = 0; hor < p.Width; hor++)
        {
            for (int vert = 0; vert < p.Height; vert++)
            {
                Board.SetPiece(xFrom + hor, yFrom + vert, 0);
            }
        }

        p.MovePiece(xTo, yTo);

        for (int hor = 0; hor < p.Width; hor++)
        {
            for (int vert = 0; vert < p.Height; vert++)
            {
                Board.SetPiece(xTo + hor, yTo + vert, orientation);
            }
        }
    }

    public PuzzlePiece? Find(int x, int y)
    {
        foreach (PuzzlePiece piece in Pieces)
        {
            if (piece.X == x && piece.Y == y)
            {
                return piece;
            }
        }

        return null;
    }

    public bool IsPuzzleSolved()
    {
        foreach (PuzzlePiece piece in Pieces)
        {
            if (piece.IsSolved)
            {
                return true;
            }
        }

        return false;
    }

    public bool CreateRandomPuzzle(int numPieces)
    {
        Pieces = [];
        Board.Clear();

        int count = 1;

        //Solve on the far left for now ... we can  randomize later
        AddPiece(0, 2, PieceType.Solve);

        int iX, iY;
        PieceType piece;

        int iLoopCount = 0;

        while (count < numPieces)
        {
            piece = (PieceType)rand.Next((int)PieceType.Solve);
            iX = rand.Next(0, 6);
            iY = rand.Next(0, 6);

            //quick hack for better puzzles
            if (iY == 2 &&
                (piece == PieceType.Horizontal2 || piece == PieceType.Horizontal3))
            {
                continue;
            }

            if (Board.IsLocationFree(iX, iY, piece))
            {
                if (AddPiece(iX, iY, piece))
                {
                    count++;
                }
            }

            iLoopCount++;

            if (iLoopCount > 1000)
            {
                break;
            }
        }

        if (count < numPieces)
        {
            return false;
        }

        return true;
    }

    public string GetHash()
    {
        return Board.GetHash();
    }
}
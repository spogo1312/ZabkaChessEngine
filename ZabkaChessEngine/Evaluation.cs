using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZabkaChessEngine
{
    public class Evaluation
    {
        

        private static readonly int[] PieceValues = {
        0, 100, 320, 330, 500, 900, 20000 // Empty, Pawn, Knight, Bishop, Rook, Queen, King
        };

        private static readonly int[,] WhitePawnTable = new int[8, 8]
        {
            { 0,  0,  0,  0,  0,  0,  0,  0 },
            { 50, 50, 50, 50, 50, 50, 50, 50 },
            { 10, 10, 20, 30, 30, 20, 10, 10 },
            { 5,  5, 10, 25, 25, 10,  5,  5 },
            { 0,  0,  0, 20, 20,  0,  0,  0 },
            { 5, -5,-10,  0,  0,-10, -5,  5 },
            { 5, 10, 10,-20,-20, 10, 10,  5 },
            { 0,  0,  0,  0,  0,  0,  0,  0 }
        };

        private static readonly int[,] BlackPawnTable = new int[8, 8]
        {
            { 0,  0,  0,  0,  0,  0,  0,  0 },
            { 5, 10, 10,-20,-20, 10, 10,  5 },
            { 5, -5,-10,  0,  0,-10, -5,  5 },
            { 0,  0,  0, 20, 20,  0,  0,  0 },
            { 5,  5, 10, 25, 25, 10,  5,  5 },
            { 10, 10, 20, 30, 30, 20, 10, 10 },
            { 50, 50, 50, 50, 50, 50, 50, 50 },
            { 0,  0,  0,  0,  0,  0,  0,  0 }
        };

        private static readonly int[,] WhiteKnightTable = new int[8, 8]
        {
            { -50, -40, -30, -30, -30, -30, -40, -50 },
            { -40, -20,   0,   0,   0,   0, -20, -40 },
            { -30,   0,  10,  15,  15,  10,   0, -30 },
            { -30,   5,  15,  20,  20,  15,   5, -30 },
            { -30,   0,  15,  20,  20,  15,   0, -30 },
            { -30,   5,  10,  15,  15,  10,   5, -30 },
            { -40, -20,   0,   5,   5,   0, -20, -40 },
            { -50, -40, -30, -30, -30, -30, -40, -50 }
        };

        private static readonly int[,] BlackKnightTable = new int[8, 8]
        {
            { -50, -40, -30, -30, -30, -30, -40, -50 },
            { -40, -20,   0,   5,   5,   0, -20, -40 },
            { -30,   5,  10,  15,  15,  10,   5, -30 },
            { -30,   0,  15,  20,  20,  15,   0, -30 },
            { -30,   5,  15,  20,  20,  15,   5, -30 },
            { -30,   0,  10,  15,  15,  10,   0, -30 },
            { -40, -20,   0,   0,   0,   0, -20, -40 },
            { -50, -40, -30, -30, -30, -30, -40, -50 }
        };
        private static readonly int[,] WhiteBishopTable = new int[8, 8]
        {
            { -20, -10, -10, -10, -10, -10, -10, -20 },
            { -10,   0,   0,   0,   0,   0,   0, -10 },
            { -10,   0,   5,  10,  10,   5,   0, -10 },
            { -10,   5,   5,  10,  10,   5,   5, -10 },
            { -10,   0,  10,  10,  10,  10,   0, -10 },
            { -10,  10,  10,  10,  10,  10,  10, -10 },
            { -10,   5,   0,   0,   0,   0,   5, -10 },
            { -20, -10, -10, -10, -10, -10, -10, -20 }
        };

        private static readonly int[,] BlackBishopTable = new int[8, 8]
        {
            { -20, -10, -10, -10, -10, -10, -10, -20 },
            { -10,   5,   0,   0,   0,   0,   5, -10 },
            { -10,  10,  10,  10,  10,  10,  10, -10 },
            { -10,   0,  10,  10,  10,  10,   0, -10 },
            { -10,   5,   5,  10,  10,   5,   5, -10 },
            { -10,   0,   5,  10,  10,   5,   0, -10 },
            { -10,   0,   0,   0,   0,   0,   0, -10 },
            { -20, -10, -10, -10, -10, -10, -10, -20 }
        };
        private static readonly int[,] WhiteRookTable = new int[8, 8]
        {
            { 0,  0,  0,  0,  0,  0,  0,  0 },
            { 5, 10, 10, 10, 10, 10, 10,  5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { 0,  0,  0,  5,  5,  0,  0,  0 }
        };

        private static readonly int[,] BlackRookTable = new int[8, 8]
        {
            { 0,  0,  0,  5,  5,  0,  0,  0 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { 5, 10, 10, 10, 10, 10, 10,  5 },
            { 0,  0,  0,  0,  0,  0,  0,  0 }
        };
        private static readonly int[,] WhiteQueenTable = new int[8, 8]
        {
            { -20, -10, -10,  -5,  -5, -10, -10, -20 },
            { -10,   0,   0,   0,   0,   0,   0, -10 },
            { -10,   0,   5,   5,   5,   5,   0, -10 },
            { -5,   0,   5,   5,   5,   5,   0,  -5 },
            { 0,   0,   5,   5,   5,   5,   0,  -5 },
            { -10,   5,   5,   5,   5,   5,   0, -10 },
            { -10,   0,   5,   0,   0,   0,   0, -10 },
            { -20, -10, -10,  -5,  -5, -10, -10, -20 }
        };

        private static readonly int[,] BlackQueenTable = new int[8, 8]
        {
            { -20, -10, -10,  -5,  -5, -10, -10, -20 },
            { -10,   0,   5,   0,   0,   0,   0, -10 },
            { -10,   5,   5,   5,   5,   5,   0, -10 },
            { 0,   0,   5,   5,   5,   5,   0,  -5 },
            { -5,   0,   5,   5,   5,   5,   0,  -5 },
            { -10,   0,   5,   5,   5,   5,   0, -10 },
            { -10,   0,   0,   0,   0,   0,   0, -10 },
            { -20, -10, -10,  -5,  -5, -10, -10, -20 }
        };
        private static readonly int[,] WhiteKingMiddleGameTable = new int[8, 8]
        {
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -20, -30, -30, -40, -40, -30, -30, -20 },
            { -10, -20, -20, -20, -20, -20, -20, -10 },
            {  20,  20,   0,   0,   0,   0,  20,  20 },
            {  20,  30,  10,   0,   0,  10,  30,  20 }
        };

        private static readonly int[,] BlackKingMiddleGameTable = new int[8, 8]
        {
            {  20,  30,  10,   0,   0,  10,  30,  20 },
            {  20,  20,   0,   0,   0,   0,  20,  20 },
            { -10, -20, -20, -20, -20, -20, -20, -10 },
            { -20, -30, -30, -40, -40, -30, -30, -20 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 }
        };


        private readonly MoveGenerator moveGenerator = new MoveGenerator();

        public int Evaluate(Board board)
        {
            int materialScore = MaterialCount(board);
            int pieceSquareScore = PieceSquareScore(board);
            int mobilityScore = MobilityScore(board);
            int kingSafetyScore = KingSafetyScore(board);

            return materialScore + pieceSquareScore + mobilityScore + kingSafetyScore;
        }

        private int MaterialCount(Board board)
        {
            int score = 0;
            var squares = board.Squares;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = squares[row, col];
                    score += (int)piece.Color * PieceValues[(int)piece.Type];
                }
            }
            return score;
        }

        private int PieceSquareScore(Board board)
        {
            int score = 0;
            var squares = board.Squares;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = squares[row, col];
                    if (piece.Color == PieceColor.White)
                    {
                        score += GetPieceSquareValue(piece, row, col, true);
                    }
                    else if (piece.Color == PieceColor.Black)
                    {
                        score -= GetPieceSquareValue(piece, row, col, false);
                    }
                }
            }
            return score;
        }

        private int GetPieceSquareValue(Piece piece, int row, int col, bool isWhite)
        {
            switch (piece.Type)
            {
                case PieceType.Pawn:
                    return isWhite ? WhitePawnTable[row, col] : BlackPawnTable[row, col];
                case PieceType.Knight:
                    return isWhite ? WhiteKnightTable[row, col] : BlackKnightTable[row, col];
                case PieceType.Bishop:
                    return isWhite ? WhiteBishopTable[row, col] : BlackBishopTable[row, col];
                case PieceType.Rook:
                    return isWhite ? WhiteRookTable[row, col] : BlackRookTable[row, col];
                case PieceType.Queen:
                    return isWhite ? WhiteQueenTable[row, col] : BlackQueenTable[row, col];
                case PieceType.King:
                    return isWhite ? WhiteKingMiddleGameTable[row, col] : BlackKingMiddleGameTable[row, col];
                default:
                    return 0;
            }
        }


        private int MobilityScore(Board board)
        {
            int whiteMoves = moveGenerator.GenerateAllMoves(board, true).Count;
            int blackMoves = moveGenerator.GenerateAllMoves(board, false).Count;
            return whiteMoves - blackMoves;
        }

        private int KingSafetyScore(Board board)
        {
            int score = 0;
            score += EvaluateKingSafety(board, PieceColor.White);
            score -= EvaluateKingSafety(board, PieceColor.Black);
            return score;
        }

        private int EvaluateKingSafety(Board board, PieceColor color)
        {
            int score = 0;
            var kingPosition = FindKingPosition(board, color);

            if (kingPosition == null)
            {
                // King is captured, return a large penalty
                return color == PieceColor.White ? int.MinValue : int.MaxValue;
            }

            var kingRow = kingPosition.Item1;
            var kingCol = kingPosition.Item2;

            // Evaluate pawn shield
            score += EvaluatePawnShield(board, kingRow, kingCol, color);

            // Evaluate open files
            score += EvaluateOpenFiles(board, kingRow, kingCol);

            // Evaluate proximity of enemy pieces
            score += EvaluateEnemyProximity(board, kingRow, kingCol, color);

            return score;
        }

        private Tuple<int, int>? FindKingPosition(Board board, PieceColor color)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board.Squares[row, col].Type == PieceType.King && board.Squares[row, col].Color == color)
                    {
                        return new Tuple<int, int>(row, col);
                    }
                }
            }
            return null; // Return null if the king is not found
        }


        private int EvaluatePawnShield(Board board, int kingRow, int kingCol, PieceColor color)
        {
            int score = 0;
            int direction = color == PieceColor.White ? -1 : 1;

            // Check pawns in front of the king
            for (int col = kingCol - 1; col <= kingCol + 1; col++)
            {
                if (col >= 0 && col < 8)
                {
                    int row = kingRow + direction;
                    if (row >= 0 && row < 8 && board.Squares[row, col].Type == PieceType.Pawn && board.Squares[row, col].Color == color)
                    {
                        score += 20; // Adjust value as needed
                    }
                }
            }
            return score;
        }

        private int EvaluateOpenFiles(Board board, int kingRow, int kingCol)
        {
            int score = 0;

            // Check the file the king is on
            bool openFile = true;
            for (int row = 0; row < 8; row++)
            {
                if (board.Squares[row, kingCol].Type != PieceType.Empty && board.Squares[row, kingCol].Type != PieceType.King)
                {
                    openFile = false;
                    break;
                }
            }
            if (openFile)
            {
                score -= 30; // Penalty for being on an open file
            }
            return score;
        }

        private int EvaluateEnemyProximity(Board board, int kingRow, int kingCol, PieceColor color)
        {
            int score = 0;
            PieceColor enemyColor = color == PieceColor.White ? PieceColor.Black : PieceColor.White;

            // Check for enemy pieces near the king
            for (int row = kingRow - 2; row <= kingRow + 2; row++)
            {
                for (int col = kingCol - 2; col <= kingCol + 2; col++)
                {
                    if (row >= 0 && row < 8 && col >= 0 && col < 8)
                    {
                        Piece piece = board.Squares[row, col];
                        if (piece.Color == enemyColor)
                        {
                            score -= PieceValues[(int)piece.Type] / 10; // Penalty based on enemy piece value
                        }
                    }
                }
            }
            return score;
        }

    }

}

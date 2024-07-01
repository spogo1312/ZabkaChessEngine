using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZabkaChessEngine
{
    public struct Evaluation
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

        public Evaluation()
        {
        }

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
                        switch (piece.Type)
                        {
                            case PieceType.Pawn:
                                score += WhitePawnTable[row, col];
                                break;
                            case PieceType.Knight:
                                score += WhiteKnightTable[row, col];
                                break;
                            case PieceType.Bishop:
                                score += WhiteBishopTable[row, col];
                                break;
                            case PieceType.Rook:
                                score += WhiteRookTable[row, col];
                                break;
                            case PieceType.Queen:
                                score += WhiteQueenTable[row, col];
                                break;
                            case PieceType.King:
                                score += WhiteKingMiddleGameTable[row, col];
                                break;
                        }
                    }
                    else if (piece.Color == PieceColor.Black)
                    {
                        switch (piece.Type)
                        {
                            case PieceType.Pawn:
                                score -= BlackPawnTable[row, col];
                                break;
                            case PieceType.Knight:
                                score -= BlackKnightTable[row, col];
                                break;
                            case PieceType.Bishop:
                                score -= BlackBishopTable[row, col];
                                break;
                            case PieceType.Rook:
                                score -= BlackRookTable[row, col];
                                break;
                            case PieceType.Queen:
                                score -= BlackQueenTable[row, col];
                                break;
                            case PieceType.King:
                                score -= BlackKingMiddleGameTable[row, col];
                                break;
                        }
                    }
                }
            }
            return score;
        }


        private int MobilityScore(Board board)
        {
            int whiteMoves = moveGenerator.GenerateAllMoves(board, true).Count;
            int blackMoves = moveGenerator.GenerateAllMoves(board, false).Count;
            return whiteMoves - blackMoves;
        }

        private int KingSafetyScore(Board board)
        {
            // Placeholder for a more complex king safety evaluation
            // Implement king safety logic here
            return 0;
        }
    }

}

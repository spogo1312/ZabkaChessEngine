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
        0, 100, 300, 300, 500, 900, 10000 // Empty, Pawn, Knight, Bishop, Rook, Queen, King
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
            // This method currently doesn't do anything meaningful.
            // Add piece-square table logic here if needed.
            int score = 0;
            // Example for pawns (you need to define PawnTable):
            // int[,] PawnTable = { ... };
            // for (int row = 0; row < 8; row++)
            // {
            //     for (int col = 0; col < 8; col++)
            //     {
            //         Piece piece = board.Squares[row, col];
            //         if (piece.Type == PieceType.Pawn)
            //         {
            //             score += (int)piece.Color * PawnTable[row, col];
            //         }
            //     }
            // }
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

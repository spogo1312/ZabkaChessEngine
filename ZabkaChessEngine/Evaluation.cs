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
            0, 100, 300, 300, 500, 900, 0 // Empty, Pawn, Knight, Bishop, Rook, Queen, King
        };

       

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
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = board.Squares[row, col];
                    score += (int)piece.Color * PieceValues[(int)piece.Type];
                }
            }
            return score;
        }

        private int PieceSquareScore(Board board)
        {
            int score = 0;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = board.Squares[row, col];
                    if (piece.Type == PieceType.Pawn)
                    {
                        //score += (int)piece.Color * PawnTable[row, col];
                    }
                    // Add other piece tables similarly
                }
            }
            return score;
        }

        private int MobilityScore(Board board)
        {
            // Placeholder for a more complex mobility evaluation
            int whiteMoves = new MoveGenerator().GenerateAllMoves(board, true).Count;
            int blackMoves = new MoveGenerator().GenerateAllMoves(board, false).Count;
            return whiteMoves - blackMoves;
        }

        private int KingSafetyScore(Board board)
        {
            // Placeholder for a more complex king safety evaluation
            int score = 0;
            // Implement king safety logic here
            return score;
        }
    }
}

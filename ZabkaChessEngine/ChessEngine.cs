using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZabkaChessEngine
{
    public class ChessEngine
    {
        private readonly Evaluation evaluation = new Evaluation();
        private readonly MoveValidator moveValidator = new MoveValidator();

        public Move SelectBestMove(Board board)
        {
            int bestScore = board.IsWhiteTurn ? int.MinValue : int.MaxValue;
            Move bestMove = null;

            MoveGenerator moveGenerator = new MoveGenerator();
            List<Move> allMoves = moveGenerator.GenerateAllMoves(board, board.IsWhiteTurn);


            foreach (Move move in allMoves)
            {
                if (moveValidator.IsMoveLegal(board, move, board.IsWhiteTurn)) 
                {
                    Board boardCopy = CopyBoard(board);
                    moveValidator.ApplyMove(boardCopy, move);

                    int score = Minimax(boardCopy, 2, int.MinValue, int.MaxValue, !board.IsWhiteTurn);

                    if ((board.IsWhiteTurn && score > bestScore) || (!board.IsWhiteTurn && score < bestScore))
                    {
                        bestScore = score;
                        bestMove = move;
                    }
                }
               
            }
            return bestMove;
        }

        private int Minimax(Board board, int depth, int alpha, int beta, bool isMaximizingPlayer)
        {
            if (depth == 0)
            {
                return evaluation.Evaluate(board);
            }

            MoveGenerator moveGenerator = new MoveGenerator();
            List<Move> allMoves = moveGenerator.GenerateAllMoves(board, isMaximizingPlayer);

            if (isMaximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (Move move in allMoves)
                {
                    Board boardCopy = CopyBoard(board);
                    moveValidator.ApplyMove(boardCopy, move);
                    int eval = Minimax(boardCopy, depth - 1, alpha, beta, false);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (Move move in allMoves)
                {
                    Board boardCopy = CopyBoard(board);
                    moveValidator.ApplyMove(boardCopy, move);
                    int eval = Minimax(boardCopy, depth - 1, alpha, beta, true);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                return minEval;
            }
        }

        private Board CopyBoard(Board board)
        {
            Board newBoard = new Board();
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    newBoard.Squares[row, col] = new Piece(board.Squares[row, col].Type, board.Squares[row, col].Color);
                }
            }
            newBoard.EnPassantTarget = board.EnPassantTarget;
            newBoard.IsWhiteTurn = board.IsWhiteTurn;
            newBoard.WhiteKingSideCastling = board.WhiteKingSideCastling;
            newBoard.WhiteQueenSideCastling = board.WhiteQueenSideCastling;
            newBoard.BlackKingSideCastling = board.BlackKingSideCastling;
            newBoard.BlackQueenSideCastling = board.BlackQueenSideCastling;
            return newBoard;
        }
    }

}

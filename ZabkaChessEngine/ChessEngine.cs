using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZabkaChessEngine
{
    public class ChessEngine
    {
        private readonly Evaluation evaluation = new Evaluation();
        private readonly MoveValidator moveValidator = new MoveValidator();
        private readonly MoveGenerator moveGenerator = new MoveGenerator();
        Stopwatch stopwatch = new Stopwatch();
        long nodes = 0;
        int MaxDepth = 10;

        public Move SelectBestMove(Board board, int timeLimit)
        {
            Move bestMove = Move.NoMove;

            if (timeLimit <= 2000)
            {

                List<Move> allMoves = moveGenerator.GenerateAllMoves(board, board.IsWhiteTurn);
                int bestScore = board.IsWhiteTurn ? int.MinValue : int.MaxValue;
                foreach (Move move in allMoves)
                {
                    if (moveValidator.IsMoveLegal(board, move, board.IsWhiteTurn))
                    {
                        Board boardCopy = CopyBoard(board);
                        moveValidator.ApplyMove(boardCopy, move);

                        int score = Minimax(boardCopy, 3, int.MinValue, int.MaxValue, !board.IsWhiteTurn); // Reduced depth to 3

                        if ((board.IsWhiteTurn && score > bestScore) || (!board.IsWhiteTurn && score < bestScore))
                        {
                            bestScore = score;
                            bestMove = move;
                        }
                    }
                }
            }
            else 
            {

                for (int depth = 1; depth <= MaxDepth; depth++)
                {
                    Move move = SearchWithTimeLimit(board, depth, timeLimit);
                    if (!move.Equals(Move.NoMove))
                        bestMove = move;
                    else
                        break; // Time limit reached
                }
            }
           
            
            Console.WriteLine("Nodes searched:" + nodes);
            Console.WriteLine("Move Time:" + stopwatch.ElapsedMilliseconds + "ms");
            return bestMove;
        }
        private Move SearchWithTimeLimit(Board board, int depth, int timeLimit)
        {
            stopwatch = Stopwatch.StartNew();
            Move bestMove = Move.NoMove;
            int bestScore = board.IsWhiteTurn ? int.MinValue : int.MaxValue;

            List<Move> allMoves = moveGenerator.GenerateAllMoves(board, board.IsWhiteTurn);

            foreach (Move move in allMoves)
            {
                if (stopwatch.ElapsedMilliseconds > timeLimit)
                    return Move.NoMove; // Time limit reached

                if (moveValidator.IsMoveLegal(board, move, board.IsWhiteTurn))
                {
                    Board boardCopy = CopyBoard(board);
                    moveValidator.ApplyMove(boardCopy, move);

                    int score = Minimax(boardCopy, depth - 1, int.MinValue, int.MaxValue, !board.IsWhiteTurn);

                    if ((board.IsWhiteTurn && score > bestScore) || (!board.IsWhiteTurn && score < bestScore))
                    {
                        bestScore = score;
                        bestMove = move;
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine("evaluation:" + bestScore);
            return bestMove;
        }

        private int Minimax(Board board, int depth, int alpha, int beta, bool isMaximizingPlayer)
        {
            nodes++;

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
            Board newBoard = new Board
            {
                EnPassantTarget = board.EnPassantTarget,
                IsWhiteTurn = board.IsWhiteTurn,
                WhiteKingSideCastling = board.WhiteKingSideCastling,
                WhiteQueenSideCastling = board.WhiteQueenSideCastling,
                BlackKingSideCastling = board.BlackKingSideCastling,
                BlackQueenSideCastling = board.BlackQueenSideCastling
            };

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    newBoard.Squares[row, col] = board.Squares[row, col];
                }
            }

            return newBoard;
        }
    }

}

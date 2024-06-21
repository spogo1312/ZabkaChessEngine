using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZabkaChessEngine
{
    public class Perft
    {
        private MoveGenerator moveGenerator;
        private MoveValidator moveValidator;

        public Perft()
        {
            moveGenerator = new MoveGenerator();
            moveValidator = new MoveValidator();
        }

        public void PerformPerft(Board board, int depth)
        {
            Dictionary<string, long> moveCounts = new Dictionary<string, long>();
            long totalNodes = 0;

            List<Move> moves = moveGenerator.GenerateAllMoves(board, board.IsWhiteTurn);
            foreach (Move move in moves)
            {
                if (moveValidator.IsMoveLegal(board, move, board.IsWhiteTurn))
                {
                    Board boardCopy = CopyBoard(board);
                    moveValidator.ApplyMove(boardCopy, move);
                    boardCopy.IsWhiteTurn = !boardCopy.IsWhiteTurn;

                    long childNodes = PerformPerftRecursive(boardCopy, depth - 1);
                    totalNodes += childNodes;

                    string moveString = MoveToString(move);
                    if (!moveCounts.ContainsKey(moveString))
                    {
                        moveCounts[moveString] = 0;
                    }
                    moveCounts[moveString] += childNodes;
                }
            }

            Console.WriteLine($"Total: {totalNodes}");
            foreach (var moveCount in moveCounts)
            {
                Console.WriteLine($"{moveCount.Key} - {moveCount.Value}");
            }
        }

        private long PerformPerftRecursive(Board board, int depth)
        {
            if (depth == 0)
            {
                return 1;
            }

            long nodes = 0;
            List<Move> moves = moveGenerator.GenerateAllMoves(board, board.IsWhiteTurn);

            foreach (Move move in moves)
            {
                if (moveValidator.IsMoveLegal(board, move, board.IsWhiteTurn))
                {
                    Board boardCopy = CopyBoard(board);
                    moveValidator.ApplyMove(boardCopy, move);
                    boardCopy.IsWhiteTurn = !boardCopy.IsWhiteTurn;

                    nodes += PerformPerftRecursive(boardCopy, depth - 1);
                }
            }

            return nodes;
        }

        private string MoveToString(Move move)
        {
            string from = $"{(char)(move.FromY + 'a')}{8 - move.FromX}";
            string to = $"{(char)(move.ToY + 'a')}{8 - move.ToX}";
            string promotion = move.Promotion != PieceType.Empty ? GetPromotionString(move.Promotion) : "";
            return from + to + promotion;
        }

        private string GetPromotionString(PieceType promotion)
        {
            return promotion switch
            {
                PieceType.Queen => "q",
                PieceType.Rook => "r",
                PieceType.Bishop => "b",
                PieceType.Knight => "n",
                _ => ""
            };
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
            return newBoard;
        }
    }

}

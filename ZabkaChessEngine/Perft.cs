using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
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
            foreach (var moveCount in moveCounts.OrderBy(mc => mc.Key))
            {
                Console.WriteLine($"{moveCount.Key} - {moveCount.Value}");
            }
            stopwatch.Stop();
            Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
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
        public void ValidateIntermediateState()
        {
            // Initial board setup from FEN
            Board board = new Board();
            board.SetPositionFromFEN("8/8/K2p4/1Pp4r/1R3p1k/8/4P1P1/8 w - c6 0 2");

            // Perform perft 2 to see initial counts
            PerformPerft(board, 2);
            DisplayBoard(board);

            // Manually apply the move b5c6
            Move move = new Move(3, 1, 2, 2);
            moveValidator.ApplyMove(board, move);
            DisplayBoard(board);

            // Perform perft 1 to see counts after the move
            PerformPerft(board, 1);

            // Check the intermediate state
            DisplayBoard(board);
        }

        public void DisplayBoard(Board board)
        {
            board.PrintBoard();
        }
    }

}

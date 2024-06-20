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

        public long PerformPerft(Board board, int depth)
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
                    nodes += PerformPerft(boardCopy, depth - 1);
                }
            }

            return nodes;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZabkaChessEngine
{
    public class Move
    {
        public int FromX { get; set; }
        public int FromY { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }

        public Move(int fromX, int fromY, int toX, int toY)
        {
            FromX = fromX;
            FromY = fromY;
            ToX = toX;
            ToY = toY;
        }
    }

    public class MoveGenerator
    {
        private static readonly Random random = new Random();

        public List<Move> GenerateAllMoves(Board board, bool isWhiteTurn)
        {
            List<Move> allPossibleMoves = new List<Move>();

            for (int fromX = 0; fromX < 8; fromX++)
            {
                for (int fromY = 0; fromY < 8; fromY++)
                {
                    Piece piece = board.Squares[fromX, fromY];
                    if (piece.Type != PieceType.Empty && piece.Color == (isWhiteTurn ? PieceColor.White : PieceColor.Black))
                    {
                        List<Move> pieceMoves = GeneratePieceMoves(board, piece, fromX, fromY);
                        allPossibleMoves.AddRange(pieceMoves);
                    }
                }
            }

            return allPossibleMoves;
        }

        public List<Move> GeneratePieceMoves(Board board, Piece piece, int fromX, int fromY)
        {
            List<Move> moves = new List<Move>();

            switch (piece.Type)
            {
                case PieceType.Pawn:
                    moves.AddRange(GeneratePawnMoves(board, piece, fromX, fromY));
                    break;
                case PieceType.Rook:
                    moves.AddRange(GenerateRookMoves(board, piece, fromX, fromY));
                    break;
                case PieceType.Knight:
                    moves.AddRange(GenerateKnightMoves(board, piece, fromX, fromY));
                    break;
                case PieceType.Bishop:
                    moves.AddRange(GenerateBishopMoves(board, piece, fromX, fromY));
                    break;
                case PieceType.Queen:
                    moves.AddRange(GenerateQueenMoves(board, piece, fromX, fromY));
                    break;
                case PieceType.King:
                    moves.AddRange(GenerateKingMoves(board, piece, fromX, fromY));
                    break;
            }

            return moves;
        }

        private List<Move> GeneratePawnMoves(Board board, Piece piece, int fromX, int fromY)
        {
            List<Move> moves = new List<Move>();
            int direction = piece.Color == PieceColor.White ? -1 : 1;
            int startRow = piece.Color == PieceColor.White ? 6 : 1;
            int promotionRow = piece.Color == PieceColor.White ? 0 : 7;

            // Move forward
            if (IsInBounds(fromX + direction, fromY) && board.Squares[fromX + direction, fromY].Type == PieceType.Empty)
            {
                moves.Add(new Move(fromX, fromY, fromX + direction, fromY));
                // Double move from start position
                if (fromX == startRow && IsInBounds(fromX + 2 * direction, fromY) && board.Squares[fromX + 2 * direction, fromY].Type == PieceType.Empty)
                {
                    moves.Add(new Move(fromX, fromY, fromX + 2 * direction, fromY));
                }
            }

            // Capture diagonally
            if (IsInBounds(fromX + direction, fromY - 1) && board.Squares[fromX + direction, fromY - 1].Color == (piece.Color == PieceColor.White ? PieceColor.Black : PieceColor.White))
            {
                moves.Add(new Move(fromX, fromY, fromX + direction, fromY - 1));
            }
            if (IsInBounds(fromX + direction, fromY + 1) && board.Squares[fromX + direction, fromY + 1].Color == (piece.Color == PieceColor.White ? PieceColor.Black : PieceColor.White))
            {
                moves.Add(new Move(fromX, fromY, fromX + direction, fromY + 1));
            }

            // En passant
            if (IsInBounds(fromX, fromY - 1) && board.Squares[fromX, fromY - 1].Type == PieceType.Pawn && board.Squares[fromX, fromY - 1].Color != piece.Color)
            {
                if (piece.Color == PieceColor.White && fromX == 3 && board.Squares[fromX - 1, fromY - 1].Type == PieceType.Empty)
                {
                    moves.Add(new Move(fromX, fromY, fromX - 1, fromY - 1));
                }
                if (piece.Color == PieceColor.Black && fromX == 4 && board.Squares[fromX + 1, fromY - 1].Type == PieceType.Empty)
                {
                    moves.Add(new Move(fromX, fromY, fromX + 1, fromY - 1));
                }
            }
            if (IsInBounds(fromX, fromY + 1) && board.Squares[fromX, fromY + 1].Type == PieceType.Pawn && board.Squares[fromX, fromY + 1].Color != piece.Color)
            {
                if (piece.Color == PieceColor.White && fromX == 3 && board.Squares[fromX - 1, fromY + 1].Type == PieceType.Empty)
                {
                    moves.Add(new Move(fromX, fromY, fromX - 1, fromY + 1));
                }
                if (piece.Color == PieceColor.Black && fromX == 4 && board.Squares[fromX + 1, fromY + 1].Type == PieceType.Empty)
                {
                    moves.Add(new Move(fromX, fromY, fromX + 1, fromY + 1));
                }
            }

            // Handle promotion (simplified, typically you'll want to handle the promotion choice)
            moves.RemoveAll(move => move.ToX == promotionRow && piece.Type == PieceType.Pawn);

            return moves;
        }

        private List<Move> GenerateRookMoves(Board board, Piece piece, int fromX, int fromY)
        {
            List<Move> moves = new List<Move>();
            int[] directions = { -1, 1 };

            foreach (int direction in directions)
            {
                for (int i = fromX + direction; i >= 0 && i < 8; i += direction)
                {
                    if (board.Squares[i, fromY].Type == PieceType.Empty)
                    {
                        moves.Add(new Move(fromX, fromY, i, fromY));
                    }
                    else
                    {
                        if (board.Squares[i, fromY].Color != piece.Color)
                        {
                            moves.Add(new Move(fromX, fromY, i, fromY));
                        }
                        break;
                    }
                }
                for (int j = fromY + direction; j >= 0 && j < 8; j += direction)
                {
                    if (board.Squares[fromX, j].Type == PieceType.Empty)
                    {
                        moves.Add(new Move(fromX, fromY, fromX, j));
                    }
                    else
                    {
                        if (board.Squares[fromX, j].Color != piece.Color)
                        {
                            moves.Add(new Move(fromX, fromY, fromX, j));
                        }
                        break;
                    }
                }
            }

            return moves;
        }

        private List<Move> GenerateKnightMoves(Board board, Piece piece, int fromX, int fromY)
        {
            List<Move> moves = new List<Move>();
            int[,] knightMoves = { { -2, -1 }, { -1, -2 }, { -2, 1 }, { -1, 2 }, { 1, -2 }, { 2, -1 }, { 1, 2 }, { 2, 1 } };

            for (int i = 0; i < knightMoves.GetLength(0); i++)
            {
                int toX = fromX + knightMoves[i, 0];
                int toY = fromY + knightMoves[i, 1];

                if (toX >= 0 && toX < 8 && toY >= 0 && toY < 8)
                {
                    if (board.Squares[toX, toY].Type == PieceType.Empty || board.Squares[toX, toY].Color != piece.Color)
                    {
                        moves.Add(new Move(fromX, fromY, toX, toY));
                    }
                }
            }

            return moves;
        }

        private List<Move> GenerateBishopMoves(Board board, Piece piece, int fromX, int fromY)
        {
            List<Move> moves = new List<Move>();
            int[] directions = { -1, 1 };

            foreach (int directionX in directions)
            {
                foreach (int directionY in directions)
                {
                    for (int i = fromX + directionX, j = fromY + directionY; i >= 0 && i < 8 && j >= 0 && j < 8; i += directionX, j += directionY)
                    {
                        if (board.Squares[i, j].Type == PieceType.Empty)
                        {
                            moves.Add(new Move(fromX, fromY, i, j));
                        }
                        else
                        {
                            if (board.Squares[i, j].Color != piece.Color)
                            {
                                moves.Add(new Move(fromX, fromY, i, j));
                            }
                            break;
                        }
                    }
                }
            }

            return moves;
        }

        private List<Move> GenerateQueenMoves(Board board, Piece piece, int fromX, int fromY)
        {
            List<Move> moves = new List<Move>();
            moves.AddRange(GenerateRookMoves(board, piece, fromX, fromY));
            moves.AddRange(GenerateBishopMoves(board, piece, fromX, fromY));
            return moves;
        }

        private List<Move> GenerateKingMoves(Board board, Piece piece, int fromX, int fromY)
        {
            List<Move> moves = new List<Move>();
            int[] directions = { -1, 0, 1 };

            foreach (int directionX in directions)
            {
                foreach (int directionY in directions)
                {
                    if (directionX == 0 && directionY == 0) continue;

                    int toX = fromX + directionX;
                    int toY = fromY + directionY;

                    if (toX >= 0 && toX < 8 && toY >= 0 && toY < 8)
                    {
                        if (board.Squares[toX, toY].Type == PieceType.Empty || board.Squares[toX, toY].Color != piece.Color)
                        {
                            moves.Add(new Move(fromX, fromY, toX, toY));
                        }
                    }
                }
            }

            return moves;
        }
        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }
    }

    public class MoveValidator
    {
        public Move LastMove { get; set; }
        public bool IsMoveLegal(Board board, Move move, bool isWhiteTurn)
        {
            // Ensure the move follows the movement rules of the piece
            if (!IsPieceMoveValid(board, move, board.EnPassantTarget))
            {
                return false;
            }

            // Copy the board and apply the move
            Board boardCopy = CopyBoard(board);
            ApplyMove(boardCopy, move);

            // Check if the king is in check after the move
            return !IsKingInCheck(boardCopy, isWhiteTurn);
        }

        private bool IsPieceMoveValid(Board board, Move move, (int x, int y)? enPassantTarget)
        {
            Piece piece = board.Squares[move.FromX, move.FromY];
            switch (piece.Type)
            {
                case PieceType.Pawn:
                    return IsValidPawnMove(board, move, piece, enPassantTarget);
                case PieceType.Rook:
                    return IsValidRookMove(board, move, piece);
                case PieceType.Knight:
                    return IsValidKnightMove(board, move, piece);
                case PieceType.Bishop:
                    return IsValidBishopMove(board, move, piece);
                case PieceType.Queen:
                    return IsValidQueenMove(board, move, piece);
                case PieceType.King:
                    return IsValidKingMove(board, move, piece);
                default:
                    return false;
            }
        }

        private bool IsValidPawnMove(Board board, Move move, Piece piece, (int x, int y)? enPassantTarget)
        {
            int direction = piece.Color == PieceColor.White ? -1 : 1;
            int startRow = piece.Color == PieceColor.White ? 6 : 1;

            // Single move forward
            if (move.ToX == move.FromX + direction && move.ToY == move.FromY && board.Squares[move.ToX, move.ToY].Type == PieceType.Empty)
            {
                return true;
            }

            // Double move forward from start position
            if (move.FromX == startRow && move.ToX == move.FromX + 2 * direction && move.ToY == move.FromY && board.Squares[move.ToX, move.ToY].Type == PieceType.Empty && board.Squares[move.FromX + direction, move.FromY].Type == PieceType.Empty)
            {
                return true;
            }

            // Capturing move
            if (move.ToX == move.FromX + direction && Math.Abs(move.ToY - move.FromY) == 1)
            {
                if (board.Squares[move.ToX, move.ToY].Color != piece.Color && board.Squares[move.ToX, move.ToY].Type != PieceType.Empty)
                {
                    return true;
                }
                // En passant
                if (enPassantTarget.HasValue && enPassantTarget.Value.x == move.ToX && enPassantTarget.Value.y == move.ToY)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsValidRookMove(Board board, Move move, Piece piece)
        {
            if (move.FromX != move.ToX && move.FromY != move.ToY)
            {
                return false;
            }

            int xDirection = move.ToX > move.FromX ? 1 : (move.ToX < move.FromX ? -1 : 0);
            int yDirection = move.ToY > move.FromY ? 1 : (move.ToY < move.FromY ? -1 : 0);

            int x = move.FromX + xDirection;
            int y = move.FromY + yDirection;

            while (x != move.ToX || y != move.ToY)
            {
                if (board.Squares[x, y].Type != PieceType.Empty)
                {
                    return false;
                }
                x += xDirection;
                y += yDirection;
            }

            if (board.Squares[move.ToX, move.ToY].Color == piece.Color)
            {
                return false;
            }

            return true;
        }

        private bool IsValidKnightMove(Board board, Move move, Piece piece)
        {
            int xDiff = Math.Abs(move.ToX - move.FromX);
            int yDiff = Math.Abs(move.ToY - move.FromY);

            if ((xDiff == 2 && yDiff == 1) || (xDiff == 1 && yDiff == 2))
            {
                if (board.Squares[move.ToX, move.ToY].Color != piece.Color)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsValidBishopMove(Board board, Move move, Piece piece)
        {
            if (Math.Abs(move.ToX - move.FromX) != Math.Abs(move.ToY - move.FromY))
            {
                return false;
            }

            int xDirection = move.ToX > move.FromX ? 1 : -1;
            int yDirection = move.ToY > move.FromY ? 1 : -1;

            int x = move.FromX + xDirection;
            int y = move.FromY + yDirection;

            while (x != move.ToX || y != move.ToY)
            {
                if (board.Squares[x, y].Type != PieceType.Empty)
                {
                    return false;
                }
                x += xDirection;
                y += yDirection;
            }

            if (board.Squares[move.ToX, move.ToY].Color == piece.Color)
            {
                return false;
            }

            return true;
        }

        private bool IsValidQueenMove(Board board, Move move, Piece piece)
        {
            return IsValidRookMove(board, move, piece) || IsValidBishopMove(board, move, piece);
        }

        private bool IsValidKingMove(Board board, Move move, Piece piece)
        {
            int xDiff = Math.Abs(move.ToX - move.FromX);
            int yDiff = Math.Abs(move.ToY - move.FromY);

            if (xDiff <= 1 && yDiff <= 1)
            {
                if (board.Squares[move.ToX, move.ToY].Color != piece.Color)
                {
                    return true;
                }
            }

            return false;
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
            return newBoard;
        }

        private void ApplyMove(Board board, Move move)
        {
            board.Squares[move.ToX, move.ToY] = board.Squares[move.FromX, move.FromY];
            board.Squares[move.FromX, move.FromY] = new Piece(PieceType.Empty, PieceColor.None);

            // Handle en passant capture
            if (board.Squares[move.ToX, move.ToY].Type == PieceType.Pawn && move.ToY != move.FromY && board.Squares[move.ToX, move.ToY].Type == PieceType.Empty)
            {
                int direction = board.Squares[move.ToX, move.ToY].Color == PieceColor.White ? 1 : -1;
                board.Squares[move.ToX - direction, move.ToY] = new Piece(PieceType.Empty, PieceColor.None);
            }

            // Update en passant target
            if (board.Squares[move.ToX, move.ToY].Type == PieceType.Pawn && Math.Abs(move.ToX - move.FromX) == 2)
            {
                board.EnPassantTarget = ((move.FromX + move.ToX) / 2, move.ToY);
            }
            else
            {
                board.EnPassantTarget = null;
            }
        }

        private bool IsKingInCheck(Board board, bool isWhiteTurn)
        {
            // Find the king's position
            int kingX = -1, kingY = -1;
            PieceColor kingColor = isWhiteTurn ? PieceColor.White : PieceColor.Black;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board.Squares[row, col].Type == PieceType.King && board.Squares[row, col].Color == kingColor)
                    {
                        kingX = row;
                        kingY = col;
                        break;
                    }
                }
            }

            // Check if any opposing piece can capture the king
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = board.Squares[row, col];
                    if (piece.Type != PieceType.Empty && piece.Color != kingColor)
                    {
                        List<Move> opponentMoves = new MoveGenerator().GeneratePieceMoves(board, piece, row, col);
                        foreach (Move move in opponentMoves)
                        {
                            if (move.ToX == kingX && move.ToY == kingY)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }




}

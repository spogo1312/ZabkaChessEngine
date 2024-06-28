using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        public PieceType Promotion { get; set; }
        public bool IsCastling { get; set; } // Added castling flag

        public Move(int fromX, int fromY, int toX, int toY, PieceType promotion = PieceType.Empty, bool isCastling = false)
        {
            FromX = fromX;
            FromY = fromY;
            ToX = toX;
            ToY = toY;
            Promotion = promotion;
            IsCastling = isCastling; // Initialize castling flag
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

        public List<Move> GeneratePawnMoves(Board board, Piece piece, int fromX, int fromY)
        {
            List<Move> moves = new List<Move>();
            int direction = piece.Color == PieceColor.White ? -1 : 1;
            int startRow = piece.Color == PieceColor.White ? 6 : 1;
            int promotionRow = piece.Color == PieceColor.White ? 0 : 7;

            // Move forward
            if (IsInBounds(fromX + direction, fromY) && board.Squares[fromX + direction, fromY].Type == PieceType.Empty)
            {
                if (fromX + direction == promotionRow)
                {
                    // Add all promotion options
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY, PieceType.Queen));
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY, PieceType.Rook));
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY, PieceType.Bishop));
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY, PieceType.Knight));
                }
                else
                {
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY));
                    if (fromX == startRow && IsInBounds(fromX + 2 * direction, fromY) && board.Squares[fromX + 2 * direction, fromY].Type == PieceType.Empty)
                    {
                        moves.Add(new Move(fromX, fromY, fromX + 2 * direction, fromY));
                    }
                }
            }

            // Capture diagonally
            if (IsInBounds(fromX + direction, fromY - 1) && board.Squares[fromX + direction, fromY - 1].Color == (piece.Color == PieceColor.White ? PieceColor.Black : PieceColor.White))
            {
                if (fromX + direction == promotionRow)
                {
                    // Add all promotion options
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY - 1, PieceType.Queen));
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY - 1, PieceType.Rook));
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY - 1, PieceType.Bishop));
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY - 1, PieceType.Knight));
                }
                else
                {
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY - 1));
                }
            }
            if (IsInBounds(fromX + direction, fromY + 1) && board.Squares[fromX + direction, fromY + 1].Color == (piece.Color == PieceColor.White ? PieceColor.Black : PieceColor.White))
            {
                if (fromX + direction == promotionRow)
                {
                    // Add all promotion options
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY + 1, PieceType.Queen));
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY + 1, PieceType.Rook));
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY + 1, PieceType.Bishop));
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY + 1, PieceType.Knight));
                }
                else
                {
                    moves.Add(new Move(fromX, fromY, fromX + direction, fromY + 1));
                }
            }

            // En passant
            if (board.EnPassantTarget.HasValue)
            {
                if (fromX == (piece.Color == PieceColor.White ? 3 : 4))
                {
                    if (board.EnPassantTarget.Value == (fromX + direction, fromY - 1))
                    {
                        moves.Add(new Move(fromX, fromY, fromX + direction, fromY - 1));
                    }
                    if (board.EnPassantTarget.Value == (fromX + direction, fromY + 1))
                    {
                        moves.Add(new Move(fromX, fromY, fromX + direction, fromY + 1));
                    }
                }
            }



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

            // Castling moves
            if (piece.Color == PieceColor.White)
            {
                if (board.WhiteKingSideCastling && board.Squares[7, 5].Type == PieceType.Empty && board.Squares[7, 6].Type == PieceType.Empty)
                {
                    moves.Add(new Move(fromX, fromY, 7, 6, PieceType.Empty, true));
                }
                if (board.WhiteQueenSideCastling && board.Squares[7, 1].Type == PieceType.Empty && board.Squares[7, 2].Type == PieceType.Empty && board.Squares[7, 3].Type == PieceType.Empty)
                {
                    moves.Add(new Move(fromX, fromY, 7, 2, PieceType.Empty, true));
                }
            }
            else
            {
                if (board.BlackKingSideCastling && board.Squares[0, 5].Type == PieceType.Empty && board.Squares[0, 6].Type == PieceType.Empty)
                {
                    moves.Add(new Move(fromX, fromY, 0, 6, PieceType.Empty, true));
                }
                if (board.BlackQueenSideCastling && board.Squares[0, 1].Type == PieceType.Empty && board.Squares[0, 2].Type == PieceType.Empty && board.Squares[0, 3].Type == PieceType.Empty)
                {
                    moves.Add(new Move(fromX, fromY, 0, 2, PieceType.Empty, true));
                }
            }

            return moves;
        }
        public List<Move> GeneratePawnAttacks(Board board, Piece piece, int fromX, int fromY)
        {
            List<Move> attacks = new List<Move>();
            int direction = piece.Color == PieceColor.White ? -1 : 1;

            // Check diagonal attacks
            if (IsInBounds(fromX + direction, fromY - 1))
            {
                attacks.Add(new Move(fromX, fromY, fromX + direction, fromY - 1));
            }
            if (IsInBounds(fromX + direction, fromY + 1))
            {
                attacks.Add(new Move(fromX, fromY, fromX + direction, fromY + 1));
            }

            return attacks;
        }
        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }
    }

    public class MoveValidator
    {
        public Move LastMove { get; set; }
        public (int x, int y)? enPassantTarget;
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
            int promotionRow = piece.Color == PieceColor.White ? 0 : 7;

            // Single move forward
            if (move.ToX == move.FromX + direction && move.ToY == move.FromY && board.Squares[move.ToX, move.ToY].Type == PieceType.Empty && move.ToX != promotionRow)
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
                    // Temporarily apply the en passant move and check for king safety
                    Board boardCopy = CopyBoard(board);
                    ApplyMove(boardCopy, move);

                    // Remove the pawn captured en passant
                    boardCopy.Squares[move.FromX, move.ToY] = new Piece(PieceType.Empty, PieceColor.None);

                    if (!IsKingInCheck(boardCopy, piece.Color == PieceColor.White))
                    {
                        return true;
                    }
                }
                
            }
            // Promotion move
            if (move.ToX == promotionRow && move.Promotion != PieceType.Empty)
            {
                return true;
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

            if (move.IsCastling)
            {
                // Check if the squares between the king and rook are empty and not under attack
                if (piece.Color == PieceColor.White)
                {
                    if (move.ToY == 6 && board.WhiteKingSideCastling && board.Squares[7,7].Type == PieceType.Rook && board.Squares[7, 7].Color == PieceColor.White)
                    {
                        return !IsSquareUnderAttack(board, 7, 4, PieceColor.Black) &&
                               !IsSquareUnderAttack(board, 7, 5, PieceColor.Black) &&
                               !IsSquareUnderAttack(board, 7, 6, PieceColor.Black);
                    }
                    if (move.ToY == 2 && board.WhiteQueenSideCastling)
                    {
                        return !IsSquareUnderAttack(board, 7, 4, PieceColor.Black) &&
                               !IsSquareUnderAttack(board, 7, 3, PieceColor.Black) &&
                               !IsSquareUnderAttack(board, 7, 2, PieceColor.Black);
                    }
                }
                else
                {
                    if (move.ToY == 6 && board.BlackKingSideCastling)
                    {
                        return !IsSquareUnderAttack(board, 0, 4, PieceColor.White) &&
                               !IsSquareUnderAttack(board, 0, 5, PieceColor.White) &&
                               !IsSquareUnderAttack(board, 0, 6, PieceColor.White);
                    }
                    if (move.ToY == 2 && board.BlackQueenSideCastling)
                    {
                        return !IsSquareUnderAttack(board, 0, 4, PieceColor.White) &&
                               !IsSquareUnderAttack(board, 0, 3, PieceColor.White) &&
                               !IsSquareUnderAttack(board, 0, 2, PieceColor.White);
                    }
                }
                return false;
            }

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

        public void ApplyMove(Board board, Move move)
        {
            Piece movingPiece = board.Squares[move.FromX, move.FromY];

            if (move.IsCastling)
            {
                if (movingPiece.Color == PieceColor.White)
                {
                    if (move.ToY == 6) // White king-side castling
                    {
                        board.Squares[7, 5] = new Piece(PieceType.Rook, PieceColor.White);
                        board.Squares[7, 7] = new Piece(PieceType.Empty, PieceColor.None);
                    }
                    else if (move.ToY == 2) // White queen-side castling
                    {
                        board.Squares[7, 3] = new Piece(PieceType.Rook, PieceColor.White);
                        board.Squares[7, 0] = new Piece(PieceType.Empty, PieceColor.None);
                    }
                    board.WhiteKingSideCastling = false;
                    board.WhiteQueenSideCastling = false;
                }
                else
                {
                    if (move.ToY == 6) // Black king-side castling
                    {
                        board.Squares[0, 5] = new Piece(PieceType.Rook, PieceColor.Black);
                        board.Squares[0, 7] = new Piece(PieceType.Empty, PieceColor.None);
                    }
                    else if (move.ToY == 2) // Black queen-side castling
                    {
                        board.Squares[0, 3] = new Piece(PieceType.Rook, PieceColor.Black);
                        board.Squares[0, 0] = new Piece(PieceType.Empty, PieceColor.None);
                    }
                    board.BlackKingSideCastling = false;
                    board.BlackQueenSideCastling = false;
                }
            }

            // Handle en passant capture
            if (movingPiece.Type == PieceType.Pawn && move.ToY != move.FromY && board.Squares[move.ToX, move.ToY].Type == PieceType.Empty)
            {
                int direction = movingPiece.Color == PieceColor.White ? 1 : -1;
                board.Squares[move.ToX - direction, move.ToY] = new Piece(PieceType.Empty, PieceColor.None);
                board.Squares[move.FromX, move.ToY] = new Piece(PieceType.Empty, PieceColor.None);
            }

            // Update en passant target
            if (movingPiece.Type == PieceType.Pawn && Math.Abs(move.ToX - move.FromX) == 2)
            {
                board.EnPassantTarget = ((move.FromX + move.ToX) / 2, move.FromY);
            }
            else
            {
                board.EnPassantTarget = null;
            }

            // Update public en passant target variable
            enPassantTarget = board.EnPassantTarget;

            // Move the piece
            if (move.Promotion != PieceType.Empty)
            {
                board.Squares[move.ToX, move.ToY] = new Piece(move.Promotion, movingPiece.Color);
            }
            else
            {
                board.Squares[move.ToX, move.ToY] = movingPiece;
            }
            board.Squares[move.FromX, move.FromY] = new Piece(PieceType.Empty, PieceColor.None);



            // Reset castling rights if king or rook moves
            if (movingPiece.Type == PieceType.King)
            {
                if (movingPiece.Color == PieceColor.White)
                {
                    board.WhiteKingSideCastling = false;
                    board.WhiteQueenSideCastling = false;
                }
                else
                {
                    board.BlackKingSideCastling = false;
                    board.BlackQueenSideCastling = false;
                }
            }

            //White Castling Rights
            if ((move.FromX == 7 && move.FromY == 0) || (move.ToX == 7 && move.ToY == 0))
            {
                board.WhiteQueenSideCastling = false;
            }
            if ((move.FromX == 7 && move.FromY == 7) || (move.ToX == 7 && move.ToY == 7))
            {
                board.WhiteKingSideCastling = false;
            }
            //Black Castling Rights
            if ((move.FromX == 0 && move.FromY == 0) || (move.ToX == 0 && move.ToY == 0))
            {
                board.BlackQueenSideCastling = false;
            }
            if ((move.FromX == 0 && move.FromY == 7) || (move.ToX == 0 && move.ToY == 7))
            {
                board.BlackKingSideCastling = false;
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
        private bool IsSquareUnderAttack(Board board, int x, int y, PieceColor attackingColor)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Piece piece = board.Squares[row, col];
                    if (piece.Color == attackingColor)
                    {
                        List<Move> opponentMoves;
                        if (piece.Type == PieceType.Pawn)
                        {
                            opponentMoves = new MoveGenerator().GeneratePawnAttacks(board, piece, row, col);
                        }
                        else
                        {
                            opponentMoves = new MoveGenerator().GeneratePieceMoves(board, piece, row, col);
                        }

                        foreach (Move move in opponentMoves)
                        {
                            if (move.ToX == x && move.ToY == y)
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

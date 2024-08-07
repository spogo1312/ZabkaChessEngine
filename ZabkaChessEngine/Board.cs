﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZabkaChessEngine
{
    public enum PieceType
    {
        Empty,
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    public enum PieceColor
    {
        None = 0,
        White = 1,
        Black = -1
    }

    public struct Piece
    {
        public PieceType Type { get; set; }
        public PieceColor Color { get; set; }

        public Piece(PieceType type = PieceType.Empty, PieceColor color = PieceColor.None)
        {
            Type = type;
            Color = color;
        }
    }

    public class Board
    {
        public Piece[,] Squares { get; set; }
        public (int x, int y)? EnPassantTarget { get; set; }
        public bool IsWhiteTurn { get; set; }
        public bool WhiteKingSideCastling { get; set; }
        public bool WhiteQueenSideCastling { get; set; }
        public bool BlackKingSideCastling { get; set; }
        public bool BlackQueenSideCastling { get; set; }

        public Board()
        {
            Squares = new Piece[8, 8];
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            // Set up pawns
            for (int i = 0; i < 8; i++)
            {
                Squares[1, i] = new Piece(PieceType.Pawn, PieceColor.Black);
                Squares[6, i] = new Piece(PieceType.Pawn, PieceColor.White);
            }

            // Set up black pieces
            Squares[0, 0] = Squares[0, 7] = new Piece(PieceType.Rook, PieceColor.Black);
            Squares[0, 1] = Squares[0, 6] = new Piece(PieceType.Knight, PieceColor.Black);
            Squares[0, 2] = Squares[0, 5] = new Piece(PieceType.Bishop, PieceColor.Black);
            Squares[0, 3] = new Piece(PieceType.Queen, PieceColor.Black);
            Squares[0, 4] = new Piece(PieceType.King, PieceColor.Black);

            // Set up white pieces
            Squares[7, 0] = Squares[7, 7] = new Piece(PieceType.Rook, PieceColor.White);
            Squares[7, 1] = Squares[7, 6] = new Piece(PieceType.Knight, PieceColor.White);
            Squares[7, 2] = Squares[7, 5] = new Piece(PieceType.Bishop, PieceColor.White);
            Squares[7, 3] = new Piece(PieceType.Queen, PieceColor.White);
            Squares[7, 4] = new Piece(PieceType.King, PieceColor.White);

            // Initialize other properties
            EnPassantTarget = null;
            IsWhiteTurn = true;
            WhiteKingSideCastling = true;
            WhiteQueenSideCastling = true;
            BlackKingSideCastling = true;
            BlackQueenSideCastling = true;
        }

        public void SetPositionFromFEN(string fen)
        {
            // Split the FEN string into its components
            string[] parts = fen.Split(' ');
            if (parts.Length < 6)
            {
                throw new ArgumentException("Invalid FEN string.");
            }

            // Clear the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Squares[row, col] = new Piece(PieceType.Empty, PieceColor.None);
                }
            }

            // Parse the board layout
            string[] rows = parts[0].Split('/');
            for (int row = 0; row < 8; row++)
            {
                int col = 0;
                foreach (char c in rows[row])
                {
                    if (char.IsDigit(c))
                    {
                        col += c - '0';
                    }
                    else
                    {
                        PieceColor color = char.IsUpper(c) ? PieceColor.White : PieceColor.Black;
                        PieceType type = c switch
                        {
                            'p' or 'P' => PieceType.Pawn,
                            'r' or 'R' => PieceType.Rook,
                            'n' or 'N' => PieceType.Knight,
                            'b' or 'B' => PieceType.Bishop,
                            'q' or 'Q' => PieceType.Queen,
                            'k' or 'K' => PieceType.King,
                            _ => throw new ArgumentException("Invalid FEN string.")
                        };
                        Squares[row, col] = new Piece(type, color);
                        col++;
                    }
                }
            }

            // Parse side to move
            IsWhiteTurn = parts[1] == "w";

            // Parse castling availability
            string castlingRights = parts[2];
            WhiteKingSideCastling = castlingRights.Contains('K');
            WhiteQueenSideCastling = castlingRights.Contains('Q');
            BlackKingSideCastling = castlingRights.Contains('k');
            BlackQueenSideCastling = castlingRights.Contains('q');

            // Parse en passant target square
            if (parts[3] != "-")
            {
                int enPassantX = 8 - int.Parse(parts[3][1].ToString());
                int enPassantY = parts[3][0] - 'a';
                EnPassantTarget = (enPassantX, enPassantY);
            }
            else
            {
                EnPassantTarget = null;
            }
        }

        public void PrintBoard()
        {
            Console.WriteLine("   a b c d e f g h");
            Console.WriteLine();

            for (int row = 0; row < 8; row++)
            {
                Console.Write(8 - row + "  ");
                for (int col = 0; col < 8; col++)
                {
                    Console.Write(GetPieceSymbol(Squares[row, col]) + " ");
                }
                Console.WriteLine(" " + (8 - row));
            }

            Console.WriteLine();
            Console.WriteLine("   a b c d e f g h");
            Console.WriteLine(IsWhiteTurn ? "white turn" : "black turn");
        }

        private char GetPieceSymbol(Piece piece)
        {
            char symbol = piece.Type switch
            {
                PieceType.Pawn => 'P',
                PieceType.Knight => 'N',
                PieceType.Bishop => 'B',
                PieceType.Rook => 'R',
                PieceType.Queen => 'Q',
                PieceType.King => 'K',
                _ => '.',
            };

            return piece.Color == PieceColor.White ? char.ToUpper(symbol) : char.ToLower(symbol);
        }
    }
}

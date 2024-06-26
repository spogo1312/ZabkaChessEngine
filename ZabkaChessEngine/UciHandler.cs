using System;
using System.Collections.Generic;
using ZabkaChessEngine;

namespace ZabkaChessEngine 
{
    public class UciHandler
    {
        private Board board;
        private MoveGenerator moveGenerator;
        private MoveValidator moveValidator;
        private Perft perft;
        private static readonly Random random = new Random();
        private bool isBotWhite;
        private bool isWhiteTurn;
        private bool manualPlayMode;

        public UciHandler(bool isBotWhite)
        {
            board = new Board();
            moveGenerator = new MoveGenerator();
            moveValidator = new MoveValidator();
            perft = new Perft();
            this.isBotWhite = isBotWhite;
            this.isWhiteTurn = true;  // White always starts in chess
            this.manualPlayMode = false;
        }

        public void Start()
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (input == "uci")
                {
                    Console.WriteLine("id name Zabka");
                    Console.WriteLine("id author JakPod");
                    Console.WriteLine("uciok");
                }
                else if (input == "isready")
                {
                    Console.WriteLine("readyok");
                }
                else if (input == "ucinewgame")
                {
                    board = new Board();
                    isWhiteTurn = true;
                }
                else if (input.StartsWith("position"))
                {
                    if (input.Contains("startpos"))
                    {
                        board = new Board();
                        isWhiteTurn = true;  // Reset to white's turn
                    }
                    else if (input.Contains("fen"))
                    {
                        string fen = input.Substring(input.IndexOf("fen") + 4);
                        board.SetPositionFromFEN(fen);
                        // Reset the turn based on the FEN string
                        isWhiteTurn = fen.Contains(" w ");
                    }
                    // Apply moves from position command
                    if (input.Contains("moves"))
                    {
                        string[] parts = input.Split(' ');
                        for (int i = 3; i < parts.Length; i++)
                        {
                            ApplyMove(parts[i], false);
                        }
                    }
                }
                else if (input.StartsWith("go"))
                {
                    MakeBotMove();
                }
                else if (input == "quit")
                {
                    break;
                }
                else if (input == "playmanual")
                {
                    manualPlayMode = true;
                    Console.WriteLine("Manual play mode activated. You can now make moves for both sides.");
                }
                else if (input == "playbotvsbot")
                {
                    manualPlayMode = false;
                    PlayBotVsBot();
                }
                else if (input.StartsWith("move"))
                {
                    string move = input.Substring(5);
                    if (ApplyMove(move, !manualPlayMode))
                    {
                        if (IsBotTurn() && !manualPlayMode)
                        {
                            MakeBotMove();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Illegal move. Try again.");
                    }
                }
                else if (input == "display")
                {
                    Console.WriteLine("Current board state:");
                    board.PrintBoard();
                }
                else if (input.StartsWith("perft"))
                {
                    string[] parts = input.Split(' ');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int depth))
                    {
                        perft.PerformPerft(board, depth);
                    }
                    else
                    {
                        Console.WriteLine("Invalid perft command. Usage: perft <depth>");
                    }
                }
                else if (input == "validate")
                {
                    perft.ValidateIntermediateState();
                }
            }
        }

        private bool ApplyMove(string move, bool checkTurn)
        {
            int fromY = move[0] - 'a';
            int fromX = 8 - (move[1] - '0');
            int toY = move[2] - 'a';
            int toX = 8 - (move[3] - '0');
            PieceType promotionPiece = PieceType.Empty;
            bool isCastling = false;

            if (move.Length == 5)
            {
                switch (move[4])
                {
                    case 'q':
                        promotionPiece = PieceType.Queen;
                        break;
                    case 'r':
                        promotionPiece = PieceType.Rook;
                        break;
                    case 'b':
                        promotionPiece = PieceType.Bishop;
                        break;
                    case 'n':
                        promotionPiece = PieceType.Knight;
                        break;
                }
            }
            if (board.Squares[fromX, fromY].Type == PieceType.King && Math.Abs(toY - fromY) == 2)
            {
                isCastling = true;
            }

            Move userMove = new Move(fromX, fromY, toX, toY, promotionPiece, isCastling);

            bool isWhiteMove = board.Squares[fromX, fromY].Color == PieceColor.White;
            if (checkTurn && isWhiteMove != isWhiteTurn)
            {
                return false;
            }

            if (moveValidator.IsMoveLegal(board, userMove, isWhiteTurn))
            {
                moveValidator.ApplyMove(board, userMove);

                isWhiteTurn = !isWhiteTurn;  // Switch turn
                board.IsWhiteTurn = isWhiteTurn;

                //DEBUG Moves

                //Console.WriteLine("Current board state:");
                //board.PrintBoard();

                //DEBUG Moves

                return true;
                //// Handle en passant capture
                //if (board.Squares[fromX, fromY].Type == PieceType.Pawn && toX == fromX + (isWhiteTurn ? -1 : 1) && toY != fromY && board.Squares[toX, toY].Type == PieceType.Empty)
                //{
                //    board.Squares[fromX, toY] = new Piece(PieceType.Empty, PieceColor.None);
                //}

                //board.Squares[toX, toY] = board.Squares[fromX, fromY];
                //if (userMove.Promotion != PieceType.Empty) 
                //{
                //    board.Squares[toX, toY].Type = userMove.Promotion;
                //}
                //board.Squares[fromX, fromY] = new Piece(PieceType.Empty, PieceColor.None);
                //board.EnPassantTarget = moveValidator.enPassantTarget;
                //moveValidator.LastMove = userMove;

                //isWhiteTurn = !isWhiteTurn;  // Switch turn
                //board.IsWhiteTurn = isWhiteTurn;

                
                //return true;

            }
            return false;
        }

        private bool IsBotTurn()
        {
            return isBotWhite == isWhiteTurn;
        }

        private void MakeBotMove()
        {
            List<Move> allMoves = moveGenerator.GenerateAllMoves(board, isBotWhite);
            List<Move> legalMoves = new List<Move>();

            foreach (Move move in allMoves)
            {
                if (moveValidator.IsMoveLegal(board, move, isBotWhite))
                {
                    legalMoves.Add(move);
                }
            }

            if (legalMoves.Count > 0)
            {
                Move move = legalMoves[random.Next(legalMoves.Count)];
                char promotionPiece;
                switch (move.Promotion)
                {
                    case PieceType.Queen:
                        promotionPiece = 'q';
                        break;
                    case PieceType.Rook:
                        promotionPiece = 'r';
                        break;
                    case PieceType.Bishop:
                        promotionPiece = 'b';
                        break;
                    case PieceType.Knight:
                        promotionPiece = 'n';
                        break;
                    default:
                        promotionPiece = ' ';
                        break;
                }

                string moveString;
                if (promotionPiece == ' ')
                {
                    moveString = $"{(char)(move.FromY + 'a')}{8 - move.FromX}{(char)(move.ToY + 'a')}{8 - move.ToX}";
                }
                else
                {
                    moveString = $"{(char)(move.FromY + 'a')}{8 - move.FromX}{(char)(move.ToY + 'a')}{8 - move.ToX}{promotionPiece}";
                }

                if (move.IsCastling)
                {
                    if (move.ToY == 6)
                    {
                        moveString = isBotWhite ? "e1g1" : "e8g8"; // King-side castling
                    }
                    else if (move.ToY == 2)
                    {
                        moveString = isBotWhite ? "e1c1" : "e8c8"; // Queen-side castling
                    }
                }

                Console.WriteLine($"bestmove {moveString}");

                ApplyMove(moveString, false);
            }
            else
            {
                Console.WriteLine("bestmove (none)");
            }
        }
        private void PlayBotVsBot()
        {
            while (true)
            {
                if (MakeBotMoveWithCheck())
                {
                    break;
                }
            }
        }

        private bool MakeBotMoveWithCheck()
        {
            List<Move> allMoves = moveGenerator.GenerateAllMoves(board, isWhiteTurn);
            List<Move> legalMoves = new List<Move>();

            foreach (Move move in allMoves)
            {
                if (moveValidator.IsMoveLegal(board, move, isWhiteTurn))
                {
                    legalMoves.Add(move);
                }
            }

            if (legalMoves.Count > 0)
            {
                Move move = legalMoves[random.Next(legalMoves.Count)];
                char promotionPiece;
                switch (move.Promotion)
                {
                    case PieceType.Queen:
                        promotionPiece = 'q';
                        break;
                    case PieceType.Rook:
                        promotionPiece = 'r';
                        break;
                    case PieceType.Bishop:
                        promotionPiece = 'b';
                        break;
                    case PieceType.Knight:
                        promotionPiece = 'n';
                        break;
                    default:
                        promotionPiece = ' ';
                        break;
                }

                string moveString;
                if (promotionPiece == ' ')
                {
                    moveString = $"{(char)(move.FromY + 'a')}{8 - move.FromX}{(char)(move.ToY + 'a')}{8 - move.ToX}";
                }
                else
                {
                    moveString = $"{(char)(move.FromY + 'a')}{8 - move.FromX}{(char)(move.ToY + 'a')}{8 - move.ToX}{promotionPiece}";
                }

                if (move.IsCastling)
                {
                    if (move.ToY == 6)
                    {
                        moveString = isWhiteTurn ? "e1g1" : "e8g8"; // King-side castling
                    }
                    else if (move.ToY == 2)
                    {
                        moveString = isWhiteTurn ? "e1c1" : "e8c8"; // Queen-side castling
                    }
                }

                Console.WriteLine($"bestmove {moveString}");

                ApplyMove(moveString, false);
                isWhiteTurn = !isWhiteTurn;  // Switch turn

                return false;  // Continue game
            }
            else
            {
                Console.WriteLine(isWhiteTurn ? "Black wins" : "White wins");
                return true;  // End game
            }
        }
    }
}


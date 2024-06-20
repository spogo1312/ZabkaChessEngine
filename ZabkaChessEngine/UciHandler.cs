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
                    int depth = int.Parse(input.Split(' ')[1]);
                    long nodes = perft.PerformPerft(board, depth);
                    Console.WriteLine($"Perft result at depth {depth}: {nodes}");
                }
            }
        }

        private bool ApplyMove(string move, bool checkTurn)
        {
            int fromY = move[0] - 'a';
            int fromX = 8 - (move[1] - '0');
            int toY = move[2] - 'a';
            int toX = 8 - (move[3] - '0');

            Move userMove = new Move(fromX, fromY, toX, toY);

            bool isWhiteMove = board.Squares[fromX, fromY].Color == PieceColor.White;
            if (checkTurn && isWhiteMove != isWhiteTurn)
            {
                return false;
            }

            if (moveValidator.IsMoveLegal(board, userMove, isWhiteTurn))
            {
                // Handle en passant capture
                if (board.Squares[fromX, fromY].Type == PieceType.Pawn && toX == fromX + (isWhiteTurn ? -1 : 1) && toY != fromY && board.Squares[toX, toY].Type == PieceType.Empty)
                {
                    board.Squares[fromX, toY] = new Piece(PieceType.Empty, PieceColor.None);
                }

                board.Squares[toX, toY] = board.Squares[fromX, fromY];
                board.Squares[fromX, fromY] = new Piece(PieceType.Empty, PieceColor.None);
                board.EnPassantTarget = moveValidator.enPassantTarget;
                moveValidator.LastMove = userMove;

                Console.WriteLine("Current board state:");
                board.PrintBoard();

                isWhiteTurn = !isWhiteTurn;  // Switch turn
                return true;
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
                string moveString = $"{(char)(move.FromY + 'a')}{8 - move.FromX}{(char)(move.ToY + 'a')}{8 - move.ToX}";
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
                string moveString = $"{(char)(move.FromY + 'a')}{8 - move.FromX}{(char)(move.ToY + 'a')}{8 - move.ToX}";
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


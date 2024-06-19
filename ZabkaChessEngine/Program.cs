using ZabkaChessEngine;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter bot color (white/black): ");
        string color = Console.ReadLine();
        bool isBotWhite = color.ToLower() == "white";

        UciHandler uciHandler = new UciHandler(isBotWhite);
        uciHandler.Start();
        //FEN for testinf wrap around pawn capture
        //rnbqkbnr/1ppppppp/8/p7/7P/8/PPPPPPP1/RNBQKBNR w KQkq a6 0 2

        //FEN to enpessaunt
        //rnbqkbnr/ppp1p1pp/5p2/3pP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3

    }
}

// See https://aka.ms/new-console-template for more information
using ChessMinMax;
using System.Text.RegularExpressions;

var displayStrat = new ConsoleDisplay();
var validInputFormat = new Regex(@"[a-h]\d,[a-h]\d[QNBR]?");
var board = new Board();
var beforeComputerMoved = board;
bool done=false;
while (!done)
{
    string? input = "";
    bool validInput = false;
    while (!validInput)
    {
        Console.Clear();
        Console.WriteLine(board.GetDisplayString(displayStrat));
        Console.WriteLine(board.Serialize());
        Console.WriteLine("enter move as source,dest; letter first. Or 'r' for new game, or 'q' for quit");
        Console.Write(">");
        input = Console.ReadLine();
        if (input == "r")
        {
            board = new Board();
        }
        else if(input == "q")
        {
            done = true;
            validInput = true;
        }
        else if ((input??"not_s").StartsWith('s'))
        {
            Debug.ScaffoldTestCase(input!.TrimStart('s'), beforeComputerMoved.GetPacked());
        }
        else if(input == null || !validInputFormat.IsMatch(input))
        {
            DisplayError("Invalid format, must be <sourceCol><sourceRow>,<destCol><destRow>, e.g. a4,b5");
        }
        else
        {
            validInput = true;
        }
    }
    if (!done)
    {
        if (board.EnactMove(ParseInput(input!)))
        {
            Console.Clear();
            Console.WriteLine(board.GetDisplayString(displayStrat));
            Console.WriteLine(board.Serialize());
            await Task.Delay(750);
            beforeComputerMoved = board.Clone();
            board.PlayComputerMove();
        }
        else
        {
            DisplayError("Illegal move " + input);
        }
    }

}

Move ParseInput(string input)
{
    var sourceStr = input.Split(",")[0];
    var destStr = input.Split(",")[1];
    //TODO: castle, promote choice
    return new Move
    {
        SourceCol = sourceStr[0] - 'a',
        SourceRow = 8 - (sourceStr[1] - '0'),
        TargetCol = destStr[0] - 'a',
        TargetRow = 8 - (destStr[1] - '0')
    };
}
void DisplayError(string error)
{
    Console.WriteLine(error);
    Console.WriteLine("press any key to continue");
    Console.ReadKey();
}

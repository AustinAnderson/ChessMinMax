// See https://aka.ms/new-console-template for more information
using ChessMinMax;

Console.WriteLine("Hello, World!");
var board = new Board();
var str = board.GetDisplayString(new ConsoleDisplay());
//str = board.GetDisplayString(new AsciiDisplay());
Console.WriteLine(str);


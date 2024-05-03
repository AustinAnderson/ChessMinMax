using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    public class Debug
    {
        public static string GetDebugBitString(int state, string template) =>
            GetDebugBitString((uint)state, template);
        public static string GetDebugBitString(uint state, string template)
        {
            var assembled = "";
            var bits = Convert.ToString(state, 2).PadLeft(32, '0');
            int iBits = 0;
            int iTemplate = 0;
            while (iBits<bits.Length && iTemplate < template.Length)
            {
                if (template[iTemplate] == '0')
                {
                    assembled += bits[iBits];
                    iBits++;
                }
                else
                {
                    assembled += ' ';
                }
                iTemplate++;
            }
            return assembled;
        }
        private static FileInfo? testFile = null;
        private static bool failed = false;
        private static void FindTestFile()
        {
            if (testFile != null)
            {
                return;
            }
            if (failed)
            {
                Console.WriteLine("Test Scaffolding disabled due to previous error");
                return;
            }
            var current = new DirectoryInfo(Directory.GetCurrentDirectory());
            string? testProjPath = null;
            failed = false;
            while (testProjPath == null && !failed && current != null)
            {
                testProjPath = Directory.EnumerateDirectories(current.FullName, "UnitTests").FirstOrDefault();
                var baseFolder = Directory.EnumerateDirectories(current.FullName, ".git").FirstOrDefault();
                if(baseFolder != null)
                {
                    failed = true;
                }
                current = current.Parent;
            }
            if (failed)
            {
                Console.WriteLine("couldn't scaffold test, unable to find unit test folder");
                return;
            }
            var file = new FileInfo(Path.Combine(testProjPath!, "TestComputerMove.cs"));
            if (!file.Exists)
            {
                Console.WriteLine("couldn't scaffold test, path not found");
                Console.WriteLine(file.FullName);
                return;
            }
            testFile = file;
        }
        public static void ScaffoldTestCase(string name, PackedBoardState boardState)
        {
            if(testFile == null)
            {
                FindTestFile();
            }
            if (failed)
            {
                return;
            }
            var end = Environment.NewLine+"}"+Environment.NewLine;
            var contents = File.ReadAllText(testFile!.FullName).TrimEnd().TrimEnd('}');
            var boardTab = "            ";
            var testTemplate = 
@"
    [TestMethod]
    public void _TestName_()
    {
        var state = PackedBoardState.Pack([
            _Board_
        ]);
        var move = GameMinMax.RunAlgo(state, true, 2);
        Assert.IsNotNull(move);
        var assertionTuple = ((move.SourceRow, move.SourceCol), (move.TargetRow, move.TargetCol));
        Assert.AreEqual(((0,0),(0,0)), assertionTuple);
    }
";
            /*
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [pb,__,Qw,__,__,Nb,pb,__],//2
                [__,pb,__,pb,__,Bb,__,pb],//3
                [__,__,__,pw,__,Bw,__,__],//4
                [__,__,__,__,pw,Nw,__,__],//5
                [pw,pw,pw,__,__,pw,pw,pw],//6
                [__,__,Kw,Rw,__,Bw,__,Rw],//7
                //0  1  2  3  4  5  6  7
            */
            StringBuilder boardCode = new StringBuilder();
            for(int r = 0; r < 8; r++)
            {
                boardCode.Append('[');
                for(int c = 0; c< 8; c++)
                {
                    if (boardState[r,c].Type == PieceType.Empty)
                    {
                        boardCode.Append("__");
                    }
                    else
                    {
                        boardCode.Append(boardState[r, c].Type switch {
                            PieceType.King   => 'K',
                            PieceType.Queen  => 'Q',
                            PieceType.Bishop => 'B',
                            PieceType.Knight => 'N',
                            PieceType.Rook   => 'R',
                            PieceType.Pawn   => 'p',
                            _ =>'_'
                        });
                        boardCode.Append(boardState[r, c].Black ? 'b' : 'w');
                    }
                    if(c != 7)
                    {
                        boardCode.Append(',');
                    }
                }
                boardCode.Append("],//").Append(r).AppendLine().Append(boardTab);
            }
            boardCode.Append("//0");
            for(int c=1; c< 8; c++)
            {
                boardCode.Append("  ").Append(c);
            }

            var newContent = testTemplate.Replace("_TestName_", name).Replace("_Board_", boardCode.ToString()) + end;
            File.WriteAllText(testFile.FullName, contents + newContent);
        }
    }
}

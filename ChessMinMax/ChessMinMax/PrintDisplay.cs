using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    public abstract class PrintDisplay
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
        public string Get(Piece p, bool blackSquare) => PieceMap[(int)p.Type - 1][PackPieceIndex(p.Black, blackSquare)];
        public string GetBlank(bool blackSquare)
        {
            if (blackSquare) return BlankBlack;
            return BlankWhite;
        }
        public abstract int CellWidth { get; }
        protected abstract string[][] PieceMap { get; }
        protected abstract string BlankBlack { get; }
        protected abstract string BlankWhite { get; }
        protected int PackPieceIndex(bool blackPiece, bool onBlack)
            => (blackPiece, onBlack) switch
            {
                (true, true) => 0,
                (true, false) => 1,
                (false, true) => 2,
                (false, false) => 3
            };

        public static string DebugStrLong(ulong val)
        {
            unchecked
            {
                return PrintDisplay.DebugStrLong((long)val);
            }
        }
        public static string DebugStrLong(long val)
        {
            return string.Join("_", Convert.ToString(val, 2).PadLeft(64, '0')
                .Chunk(4)
                .Select(x => string.Join("", x)));
        }
    }
    public class AsciiDisplay : PrintDisplay
    {
        protected override string[][] PieceMap { get; } = [
            //bb,bw,wb,ww
            ["bK\u2588|","bK |","wK\u2588|","wK |"],//king
            ["bQ\u2588|","bQ |","wQ\u2588|","wQ |"],//queen
            ["bR\u2588|","bR |","wR\u2588|","wR |"],//rook
            ["bB\u2588|","bB |","wB\u2588|","wB |"],//bishop
            ["bN\u2588|","bN |","wN\u2588|","wN |"],//knigt
            ["bp\u2588|","bp |","wp\u2588|","wp |"]//pawn
        ];
        protected override string BlankBlack => "\u2588\u2588\u2588|";
        protected override string BlankWhite => "   |";
        public override int CellWidth => 4;
    }
    public class ConsoleDisplay : PrintDisplay
    {
        public ConsoleDisplay(string K = "K", string Q = "Q", string R = "R", string B = "B", string N = "N", string p = "p")
        {
            PieceMap = [
                //bb,bw,wb,ww
                [$"{bb} {K} {rst}",$"{bw} {K} {rst}",$"{wb} {K} {rst}",$"{ww} {K} {rst}"],//king
                [$"{bb} {Q} {rst}",$"{bw} {Q} {rst}",$"{wb} {Q} {rst}",$"{ww} {Q} {rst}"],//queen
                [$"{bb} {R} {rst}",$"{bw} {R} {rst}",$"{wb} {R} {rst}",$"{ww} {R} {rst}"],//rook
                [$"{bb} {B} {rst}",$"{bw} {B} {rst}",$"{wb} {B} {rst}",$"{ww} {B} {rst}"],//bishop
                [$"{bb} {N} {rst}",$"{bw} {N} {rst}",$"{wb} {N} {rst}",$"{ww} {N} {rst}"],//knight
                [$"{bb} {p} {rst}",$"{bw} {p} {rst}",$"{wb} {p} {rst}",$"{ww} {p} {rst}"]//pawn
            ];

        }
        private const string rst = "\x1b[0m";
        //piece,square
        private const string bb = "\x1b[1;30;40m";
        private const string bw = "\x1b[30;47m";
        private const string wb = "\x1b[37;40m";
        private const string ww = "\x1b[1;30;47m";
        protected override string[][] PieceMap { get; }         
        protected override string BlankBlack => bb + "   " + rst;
        protected override string BlankWhite => ww + "   " + rst;
        public override int CellWidth => 3;
    }
    



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    //default of piece means 0 for PieceType, which is Empty
    public struct Piece
    {
        public Piece(bool black, PieceType type) 
        { 
            Black = black;
            Type = type;
        }
        public bool Black { get; }
        public PieceType Type { get; }
        public static readonly Piece Empty = new Piece(false, PieceType.Empty);
        public override string ToString() => $"({(Black ? "B" : "W")},{Type})";
        public static bool operator ==(Piece left, Piece right) => (left.Black == right.Black) && left.Type == right.Type;
        public static bool operator !=(Piece left, Piece right) => left != right;
    }
}

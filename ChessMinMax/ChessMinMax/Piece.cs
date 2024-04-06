using System;
using System.Collections.Generic;
using System.Linq;
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
        public override string ToString() => $"({(Black ? "B" : "W")},{Type})";
    }
}

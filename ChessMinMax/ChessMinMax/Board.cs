using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    public class Board
    {
        private bool isWhitePlayer;
        public Board(bool isWhitePlayer=true)
        {
            this.isWhitePlayer = isWhitePlayer;
        }
        private static readonly Piece
            b_Pa = new(true, PieceType.Pawn),   b_Pb = new(true, PieceType.Pawn),
            b_Pc = new(true, PieceType.Pawn),   b_Pd = new(true, PieceType.Pawn),
            b_Pe = new(true, PieceType.Pawn),   b_Pf = new(true, PieceType.Pawn),
            b_Pg = new(true, PieceType.Pawn),   b_Ph = new(true, PieceType.Pawn),
            b_Rl = new(true, PieceType.Rook),   b_Rr = new(true, PieceType.Rook),
            b_Nl = new(true, PieceType.Knight), b_Nr = new(true, PieceType.Knight),
            b_Bl = new(true, PieceType.Bishop), b_Br = new(true, PieceType.Bishop),
            b_Q_ = new(true, PieceType.Queen),  b_K_ = new(true, PieceType.King);
        private static readonly Piece
            w_Pa = new(false, PieceType.Pawn),   w_Pb = new(false, PieceType.Pawn),
            w_Pc = new(false, PieceType.Pawn),   w_Pd = new(false, PieceType.Pawn),
            w_Pe = new(false, PieceType.Pawn),   w_Pf = new(false, PieceType.Pawn),
            w_Pg = new(false, PieceType.Pawn),   w_Ph = new(false, PieceType.Pawn),
            w_Rl = new(false, PieceType.Rook),   w_Rr = new(false, PieceType.Rook),
            w_Nl = new(false, PieceType.Knight), w_Nr = new(false, PieceType.Knight),
            w_Bl = new(false, PieceType.Bishop), w_Br = new(false, PieceType.Bishop),
            w_Q_ = new(false, PieceType.Queen),  w_K_ = new(false, PieceType.King);

        private Piece?[][] board = [
            [b_Rl,b_Nl,b_Bl,b_Q_,b_K_,b_Br,b_Nr,b_Rr],
            [b_Ph,b_Pg,b_Pf,b_Pe,b_Pd,b_Pc,b_Pb,b_Pa],
            [null,null,null,null,null,null,null,null],
            [null,null,null,null,null,null,null,null],
            [null,null,null,null,null,null,null,null],
            [null,null,null,null,null,null,null,null],
            [w_Ph,w_Pg,w_Pf,w_Pe,w_Pd,w_Pc,w_Pb,w_Pa],
            [w_Rl,w_Nl,w_Bl,w_Q_,w_K_,w_Br,w_Nr,w_Rr],
        ];
        public string GetDisplayString(PrintDisplay displayMap)
        {
            StringBuilder builder = new StringBuilder();
            int rowNum = 8;
            int r = 0;
            foreach(var row in board)
            {
                int ci = 0;
                builder.Append(rowNum+" ");
                foreach(var cell in row)
                {
                    bool isBlack = PackedBoardState.SquareIsBlack(r, ci);
                    string c = displayMap.GetBlank(isBlack);
                    if(cell != null)
                    {
                        c = displayMap.Get(cell.Value, isBlack);
                    }
                    builder.Append(c);
                    ci++;
                }
                builder.AppendLine("");
                r++;
                rowNum--;
            }
            var length = displayMap.CellWidth;

            builder.Append("  ");
            var toPlace = "abcdefgh";
            for (int i = 0; i < 8; i++) 
            {
                for (int j = 0; j < length; j++)
                {
                    if (j == length / 2)
                    {
                        builder.Append(toPlace[i]);
                    }
                    else
                    {
                        builder.Append(' ');
                    }
                }
            }
            return builder.ToString();
        }
        public void Play(string notation)
        {
            
        }

    }
}

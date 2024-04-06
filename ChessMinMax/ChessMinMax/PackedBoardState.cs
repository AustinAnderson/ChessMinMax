using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    public struct PackedBoardState
    {
        //typeEnum value
        //|.    is black
        //||,  /
        //vvv v
        //000 0 
        //so each long can encode 16 squares, or 1/4th of the board
        /*
         * ___________________
        ____0|_|_|_|_||_|_|_|_|
        ____1|_|_|_|_||_|_|_|_|
        ____2|_|_|_|_||_|_|_|_|
        ____3|_|_|_|_||_|_|_|_|
        ======================|
        __0_4|_|_|_|_||_|_|_|_|
        __1_5|_|_|_|_||_|_|_|_|
        __2_6|_|_|_|_||_|_|_|_|
        __3_7|_|_|_|_||_|_|_|_|
              0|1|2|3||4|5|6|7|
                      | | | | |
                      |0|1|2|3|
        */


        private long topLeft;
        private long topRight;
        private long bottomLeft;
        private long bottomRight;
        public Piece this[int row, int col]
        {
            get
            {
                return (row, col) switch
                {
                    { row: > 3, col: > 3 } => GetPieceInQuad(bottomRight, row - 4, col - 4),
                    { row: > 3, col: < 4 } => GetPieceInQuad(bottomLeft, row - 4, col),
                    { row: < 4, col: > 3 } => GetPieceInQuad(topRight, row, col - 4),
                    { row: < 4, col: < 4 } => GetPieceInQuad(topLeft, row, col)
                };
            }
            set
            {
                var bits = ((ulong)value.Type) << 1 | (value.Black ? 1UL : 0);
                var _ = (row, col) switch
                {
                    { row: > 3, col: > 3 } => SetPiece(bits, ref bottomRight, row - 4, col - 4),
                    { row: > 3, col: < 4 } => SetPiece(bits, ref bottomLeft, row - 4, col),
                    { row: < 4, col: > 3 } => SetPiece(bits, ref topRight, row, col - 4),
                    { row: < 4, col: < 4 } => SetPiece(bits, ref topLeft, row, col)
                };
            }
        }
        public static bool SquareIsBlack(int row, int col) => ((row ^ col) & 1) == 1;

        private const long FourMsbMask = 0b1111L << 60;
        // 0,0  0,1  0,2  0,3  1,0  1,1  1,2  1,3  2,0  2,1  2,2  2,3  3,0  3,1  3,2  3,3
        //   0    1    2    3    4    5    6    7    8    9    A    B    C    D    E    F
        //0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000
        private Piece GetPieceInQuad(long quadrant,int row, int col)
        {
            //flatten the row,col by row*4+col to get ndx of group of 4,
            //mul by 4 to get bit shift count,
            //shift quadrant up by that much
            //then rotate by 3 to make is black the sign bit, and the 3 LSBs encode the pieceType,
            //meaning casting to int lops off sign bit and allows casting straight to enum
            unchecked
            {
                var val = BitOperations.RotateLeft((ulong)(FourMsbMask & (quadrant << (((row << 2) + col) << 2))), 3);
                return new Piece(((long)val) < 0, (PieceType)((int)val));
            }
        }
        private int SetPiece(ulong pieceBits,ref long quadrant, int row, int col)
        {
            unchecked
            {
                var shift = (15-((row << 2) + col)) << 2;
                quadrant = (long)(((ulong)quadrant & (~(0b1111UL << shift))) | (pieceBits << shift));
                return 0;//unused return to use the lovely pattern match switch to invoke this ref method for side effects
            }
        }

    }
}


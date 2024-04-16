using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    public class PackedBoardState
    {
        private struct State
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


            public long topLeft;
            public long topRight;
            public long bottomLeft;
            public long bottomRight;
            public  int metaData;//stores en passant and castle info, see getters and setters below
        }
        private State state;
        public PackedBoardState Clone() => new(){ state = this.state };
        public Piece this[int row, int col]
        {
            get
            {
                return (row, col) switch
                {
                    { row: > 3, col: > 3 } => GetPieceInQuad(state.bottomRight, row - 4, col - 4),
                    { row: > 3, col: < 4 } => GetPieceInQuad(state.bottomLeft, row - 4, col),
                    { row: < 4, col: > 3 } => GetPieceInQuad(state.topRight, row, col - 4),
                    { row: < 4, col: < 4 } => GetPieceInQuad(state.topLeft, row, col)
                };
            }
            set
            {
                var bits = ((ulong)value.Type) << 1 | (value.Black ? 1UL : 0);
                var _ = (row, col) switch
                {
                    { row: > 3, col: > 3 } => SetPiece(bits, ref state.bottomRight, row - 4, col - 4),
                    { row: > 3, col: < 4 } => SetPiece(bits, ref state.bottomLeft, row - 4, col),
                    { row: < 4, col: > 3 } => SetPiece(bits, ref state.topRight, row, col - 4),
                    { row: < 4, col: < 4 } => SetPiece(bits, ref state.topLeft, row, col)
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
        /// <summary>
        /// moves and returns the piece that was captured, or empty
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public Piece Move(Move move)
        {
            var captured = Piece.Empty;
            var piece = this[move.SourceRow, move.SourceCol];
            this.SetPawnDoubleAdvancedLastTurn(move.SourceCol, piece.Black, false);
            

            if (move.TakesEnPassant)
            {
                this[move.SourceRow, move.SourceCol] = Piece.Empty;
                this[move.TargetRow, move.TargetCol] = piece;

                //for en passant the piece starts on the same row as the one we're capturing,
                //so captured is column we moved to and row we started on wether white or black
                captured = this[move.SourceRow, move.TargetCol];
                this[move.SourceRow, move.TargetCol] = Piece.Empty;
            }
            else if (move.CastlesKingSide)
            {
                this.SetRightRookMovedForPlayer(piece.Black);
                this[move.SourceRow, 5] = new Piece(piece.Black,PieceType.Rook);
                this[move.SourceRow, 6] = piece;
                this[move.SourceRow, 7] = Piece.Empty;
            }
            else if (move.CastlesKingSide)
            {
                this.SetLeftRookMovedForPlayer(piece.Black);
                this[move.SourceRow, 0] = Piece.Empty;
                this[move.SourceRow, 2] = piece;
                this[move.SourceRow, 3] = new Piece(piece.Black,PieceType.Rook);
            }
            else
            {
                captured = this[move.TargetRow, move.TargetCol];
                if (move.DoubleAdvancesPawn)
                {
                    this.SetPawnDoubleAdvancedLastTurn(move.SourceCol, piece.Black, true);
                }
                this[move.SourceRow, move.SourceCol] = new Piece(false, PieceType.Empty);
                if(move.PromotesToQueen)  piece = new Piece(piece.Black,PieceType.Queen);
                if(move.PromotesToBishop) piece = new Piece(piece.Black,PieceType.Bishop);
                if(move.PromotesToKnight) piece = new Piece(piece.Black,PieceType.Knight);
                if(move.PromotesToRook)   piece = new Piece(piece.Black,PieceType.Rook);
                this[move.TargetRow, move.TargetCol] = piece;
            }
            if(piece.Type == PieceType.King)
            {
                this.SetKingMovedForPlayer(piece.Black);
            }
            return captured;
        }
        // metadata int bit pack scheme
        //   rRook has moved
        //    |
        // king has moved     h-a pawn (reversed order, a on right)
        //  | |            double advanced
        //  | |             on previous turn
        //lRook has moved      /                 
        //| | |  pad  ,_,_,_,_/_,_,_,    
        //| | |   |   h g f e d c b a    same as white half
        //v v v   v   v v v v v v v v    with lower 16 bits
        //0 0 0 00000 0 0 0 0 0 0 0 0    0000000000000000
        // white half                      black half
        public bool LeftRookHasMovedForPlayer(bool isBlack) => (state.metaData >> (isBlack ? 15 : 31)) == 1;
        public void SetLeftRookMovedForPlayer(bool isBlack) => state.metaData |= 1 << (isBlack ? 15 : 31);

        public bool KingHasMovedForPlayer(bool isBlack) => ((state.metaData >> (isBlack ? 14 : 30)) & 1) == 1;
        public void SetKingMovedForPlayer(bool isBlack) => state.metaData |= 1 << (isBlack ? 14 : 30);

        public bool RightRookHasMovedForPlayer(bool isBlack) => ((state.metaData >> (isBlack ? 13 : 29)) & 1) == 1;
        public void SetRightRookMovedForPlayer(bool isBlack) => state.metaData |= 1 << (isBlack ? 13 : 29);

        //h g f e d c b a
        //0 0 0 0 0 0 0 0
        //7 6 5 4 3 2 1 0
        public bool PawnDoubleAdvancedLastTurn(int col, bool isBlack)
            => (state.metaData >> (col + (isBlack ? 0 : 16)) & 1) == 1;
        public void SetPawnDoubleAdvancedLastTurn(int col, bool isBlack, bool value)
        {
            var shift = col + (isBlack ? 0 : 16);
            if (value)
            {
                state.metaData |= (1 << shift);
            }
            else
            {
                state.metaData &= ~(1 << shift);
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    public interface IConstPackedBoardState
    {
        Piece this[int row,int col] { get; }
        PackedBoardState Clone();
        /// <summary>
        /// returns the captured piece, which might be empty if it was a normal move
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        Piece Move(IConstMove move);
        bool QueenCastleUnavailableForPlayer(bool isBlack);
        bool KingCastleUnavailableForPlayer(bool isBlack);
        (int r, int c) GetKingCoords(bool isBlack);
        bool PawnDoubleAdvancedLastTurn(int col, bool isBlack);
    }
    public class PackedBoardState: IConstPackedBoardState
    {
        public static PackedBoardState Pack(Piece?[][] EightByEightBoard)
        {
            if (EightByEightBoard.Length != 8 || EightByEightBoard.Any(c => c.Length != 8))
            {
                throw new ArgumentOutOfRangeException($"{nameof(EightByEightBoard)} must be 8 arrays of length 8");
            }
            var packed = new PackedBoardState();
            for(int r = 0; r < 8; r++)
            {
                for(int c = 0; c < 8; c++)
                {
                    packed[r, c] = EightByEightBoard[r][c] ?? Piece.Empty;
                }
            }
            return packed;
        }
        public string DebugPrintMetadata() => PrintDisplay.GetDebugBitString(
            state.metaData,
            "0 0  000 000  0 0 0 0 0 0 0 0   0 0  000 000  0 0 0 0 0 0 0 0"
        );
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
            public uint metaData;//stores en passant and castle info, see getters and setters below
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
                if (value.Type == PieceType.King)
                {
                    SetKingCoords(value.Black, (uint)row, (uint)col);
                }
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
        /// move remains unmodified
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public Piece Move(IConstMove move)
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
                this.SetKingCastleUnavailableForPlayer(piece.Black);
                this[move.SourceRow, 5] = new Piece(piece.Black,PieceType.Rook);
                this[move.SourceRow, 6] = piece;
                this[move.SourceRow, 7] = Piece.Empty;
            }
            else if (move.CastlesQueenSide)
            {
                this.SetQueenCastleUnavailableForPlayer(piece.Black);
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
                this.SetQueenCastleUnavailableForPlayer(piece.Black);
                this.SetKingCastleUnavailableForPlayer(piece.Black);
                this.SetKingCoords(piece.Black,(uint)move.TargetRow,(uint)move.TargetCol);
            }
            if(piece.Type == PieceType.Rook)
            {
                if(move.SourceCol == 0)
                {
                    this.SetQueenCastleUnavailableForPlayer(piece.Black);
                }
                if(move.SourceCol == 7)
                {
                    this.SetKingCastleUnavailableForPlayer(piece.Black);
                }
            }
            return captured;
        }

        // metadata int bit pack scheme
        //  
        //can king castle
        //  |         h-a pawn (reversed order, a on right)
        //  |                 double advanced
        //  |                on previous turn
        //can queen castle       /                 
        //| | king pos  ,_,_,_,_/_,_,_,    
        //| |  row col  h g f e d c b a    same as white half
        //v v   v   v   v v v v v v v v    with lower 16 bits
        //0 0  000 000  0 0 0 0 0 0 0 0    0000000000000000
        public bool QueenCastleUnavailableForPlayer(bool isBlack) => (state.metaData >> (isBlack ? 15 : 31)) == 1;
        private void SetQueenCastleUnavailableForPlayer(bool isBlack) => state.metaData |= 1U << (isBlack ? 15 : 31);

        public bool KingCastleUnavailableForPlayer(bool isBlack) => ((state.metaData >> (isBlack ? 14 : 30)) & 1) == 1;
        private void SetKingCastleUnavailableForPlayer(bool isBlack) => state.metaData |= 1U << (isBlack ? 14 : 30);

        public (int r, int c) GetKingCoords(bool isBlack)
            =>(
                (int)(state.metaData >> (isBlack ? 11 : 27)) & 0b0111,
                (int)(state.metaData >> (isBlack ?  8 : 24)) & 0b0111
            );
        private const uint BlackRowColReset = 0b1111_1111_1111_1111__1100_0000_1111_1111;
        private const uint WhiteRowColReset = 0b1100_0000_1111_1111__1111_1111_1111_1111;
        private void SetKingCoords(bool isBlack, uint row, uint col)
        {
            if (isBlack)
            {
                state.metaData = (state.metaData & BlackRowColReset) | (row <<11) | (col<<8);
            }
            else
            {
                state.metaData = (state.metaData & WhiteRowColReset) | (row <<27) | (col<<24);
            }
        }


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
                state.metaData |= (1U << shift);
            }
            else
            {
                state.metaData &= ~(1U << shift);
            }
        }
        public string Serialize()
        {
            var trueEmpty = new Piece(true, PieceType.Empty);
            var falseEmpty = new Piece(false, PieceType.Empty);
            //always at least 32 empty squares because start with that many,
            //empty doesn't use the color bit, so distribute metadata int into that
            int currentShift = 31;
            var copy = Clone();
            for(int r=0; r < 8; r++)
            {
                for(int c=0; c < 8; c++)
                {
                    if (copy[r,c].Type == PieceType.Empty && currentShift >= 0)
                    {
                        var newEmpty = falseEmpty;
                        if(((copy.state.metaData >> currentShift) & 1) == 1)
                        {
                            newEmpty = trueEmpty;
                        }
                        copy[r,c] = newEmpty;
                        currentShift--;
                    }
                }
            }
            return Convert.ToBase64String(new[] {
                copy.state.topLeft,
                copy.state.topRight,
                copy.state.bottomLeft,
                copy.state.bottomRight
            }.SelectMany(BitConverter.GetBytes).ToArray())
            .TrimEnd('=')//always 1 pad equals at end
            .Replace('+', '_')
            .Replace('/', '-');
        }
        public static PackedBoardState Deserialize(string serialized)
        {
            var board = new PackedBoardState();
            var bytes = Convert.FromBase64String(serialized.Replace('-', '/').Replace('_', '+') + '=')
                               .Chunk(sizeof(long)).ToArray();
            board.state.topLeft = BitConverter.ToInt64(bytes[0], 0);
            board.state.topRight = BitConverter.ToInt64(bytes[1], 0);
            board.state.bottomLeft = BitConverter.ToInt64(bytes[2], 0);
            board.state.bottomRight = BitConverter.ToInt64(bytes[3], 0);
            int shiftIndex = 31;
            for(int r = 0; r < 8; r++)
            {
                for(int c = 0; c < 8; c++)
                {
                    if (board[r,c].Type == PieceType.Empty && shiftIndex < 32)
                    {
                        if (board[r, c].Black)
                        {
                            board.state.metaData |= (1U << shiftIndex);
                            board[r, c] = Piece.Empty;
                        }
                        shiftIndex--;
                    }
                }
            }
            return board;
        }
        public override string ToString()
        {
            var typeLetterMap = new Dictionary<PieceType, char>
            {
                {PieceType.Empty,  '_' },
                {PieceType.King,   'K' },
                {PieceType.Queen,  'Q' },
                {PieceType.Bishop, 'B' },
                {PieceType.Knight, 'N' },
                {PieceType.Rook,   'R' },
                {PieceType.Pawn,   'p' },
            };
            string disp = "\r\n";
            for(int r = 0; r<8; r++)
            {
                for(int c = 0; c<8; c++)
                {
                    disp += "|";
                    var piece = this[r, c];
                    disp += typeLetterMap[piece.Type];
                    if (piece.Type == PieceType.Empty)
                    {
                        disp += "_";
                    }
                    else
                    {
                        disp += piece.Black ? "b" : "w";
                    }

                }
                disp += $" {r}\r\n";
            }
            disp += " ";
            for(int c = 0; c < 8; c++)
            {
                disp += $" {c} ";
            }
            return disp;
        }
    }
}


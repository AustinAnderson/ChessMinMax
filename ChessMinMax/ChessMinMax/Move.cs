using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    public interface IConstMove
    {
        public int SourceCol { get; }
        public int SourceRow  { get; }
        public int TargetCol  { get; }
        public int TargetRow  { get; }
        public bool Checks { get; }
        public bool CheckMates { get; }
        public bool StaleMates { get; }
        public bool TakesEnPassant { get; }
        public bool DoubleAdvancesPawn { get; }
        public bool PromotesToKnight { get; }
        public bool PromotesToBishop { get; }
        public bool PromotesToRook { get; }
        public bool PromotesToQueen { get; }
        public bool CastlesKingSide { get; }
        public bool CastlesQueenSide { get; }
        public int Score { get; }

    }
    public class Move:IConstMove
    {
        private const int ScoreResetMask = 0b000000000_1_1__1_1_1_1_1_1__1_1_1__111_111_111_111;
        private const int SourceColMask =  0b000000000_0_0__0_0_0_0_0_0__0_0_0__000_000_000_111;
        private const int SourceRowMask =  0b000000000_0_0__0_0_0_0_0_0__0_0_0__000_000_111_000;
        private const int TargetColMask =  0b000000000_0_0__0_0_0_0_0_0__0_0_0__000_111_000_000;
        private const int TargetRowMask =  0b000000000_0_0__0_0_0_0_0_0__0_0_0__111_000_000_000;
        //                                   dstR dstC 
        //                      stale  cmate   |   | srcR srcC
        //                            \  |check|   |   |   |
        //   score                     | | |   |   |   |   |
        //    v                        v v v   v   v   v   v
        // 000000000 0 0  0 0 0 0 0 0  0 0 0  000 000 000 000
        //           | |  | | | | | |
        //       promotes Q N B R | \
        //           | |          |  takesEnPassant
        // castles   Q K          doubleAdvancesPawn
        private int state;
        public int SourceCol { 
            get => state & SourceColMask; 
            set => state = (state & ~SourceColMask) | value; 
        }
        public int SourceRow { 
            get => (state & SourceRowMask) >> 3; 
            set => state = (state & ~SourceRowMask) | (value << 3); 
        }
        public int TargetCol { 
            get => (state & TargetColMask) >> 6; 
            set => state = (state & ~TargetColMask) | (value << 6); 
        }
        public int TargetRow { 
            get => (state & TargetRowMask) >> 9; 
            set => state = (state & ~TargetRowMask) | (value << 9); 
        }
        public bool Checks
        {
            get => ((state >> 12) & 1) == 1;
            set => state = (state & (~(1 << 12))) | (value ? 1 << 12 : 0);
        }
        public bool CheckMates
        {
            get => ((state >> 13) & 1) == 1;
            set => state = (state & (~(1 << 13))) | (value ? 1 << 13 : 0);
        }
        public bool StaleMates
        {
            get => ((state >> 14) & 1) == 1;
            set => state = (state & (~(1 << 14))) | (value ? 1 << 14 : 0);
        }
        public bool TakesEnPassant
        {
            get => ((state >> 15) & 1) == 1;
            set => state = (state & (~(1 << 15))) | (value ? 1 << 15 : 0);
        }
        public bool DoubleAdvancesPawn
        {
            get => ((state >> 16) & 1) == 1;
            set => state = (state & (~(1 << 16))) | (value ? 1 << 16 : 0);
        }
        public bool PromotesToKnight
        {
            get => ((state >> 17) & 1) == 1;
            set => state = (state & (~(1 << 17))) | (value ? 1 << 17 : 0);
        }
        public bool PromotesToBishop
        {
            get => ((state >> 18) & 1) == 1;
            set => state = (state & (~(1 << 18))) | (value ? 1 << 18 : 0);
        }
        public bool PromotesToRook
        {
            get => ((state >> 19) & 1) == 1;
            set => state = (state & (~(1 << 19))) | (value ? 1 << 19 : 0);
        }
        public bool PromotesToQueen
        {
            get => ((state >> 20) & 1) == 1;
            set => state = (state & (~(1 << 20))) | (value ? 1 << 20 : 0);
        }
        public bool CastlesKingSide
        {
            get => ((state >> 21) & 1) == 1;
            set => state = (state & (~(1 << 21))) | (value ? 1 << 21 : 0);
        }
        public bool CastlesQueenSide
        {
            get => ((state >> 22) & 1) == 1;
            set => state = (state & (~(1 << 22))) | (value ? 1 << 22 : 0);
        }
        /// <summary>
        /// 0-511
        /// </summary>
        public int Score { 
            get => state >> 23; 
            set => state = (state & ScoreResetMask) | (value << 23); 
        }
        public const int MAX_SCORE = (~ScoreResetMask)>>23;

        private static readonly Move EmptyMove = new Move();
        public static bool TryCreateMove(int fromRow,int fromCol,int toRow,int toCol, out Move move)
        {
            move = EmptyMove;
            if (toRow < 0 || toRow > 7 || toCol < 0 || toCol > 7)
            {
                return false;
            }
            move = new Move
            {
                SourceCol = fromCol,
                SourceRow = fromRow,
                TargetCol = toCol,
                TargetRow = toRow
            };
            return true;
        }

        public Move Clone() => new() { state = this.state };
        public override string ToString() 
        {
            var str = $"({SourceRow},{SourceCol}) => ({TargetRow},{TargetCol}) ";
            if (PromotesToBishop) str += "p => B";
            if (PromotesToKnight) str += "p => N";
            if (PromotesToQueen) str += "p => Q";
            if (PromotesToRook) str += "p => R";
            return str;
        }
        public string GetDebugBitString() => Debug.GetDebugBitString(
            this.state, 
            "000000000 0 0  0 0 0 0 0 0  0 0 0  000 000 000 000"
        );
        
    }
}

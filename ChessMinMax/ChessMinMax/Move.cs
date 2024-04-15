using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    public class Move
    {
        private const int ScoreResetMask = 0b00000000000_1_1_1_1_1_1__1_1_1__111_111_111_111;
        private const int SourceColMask =  0b00000000000_0_0_0_0_0_0__0_0_0__000_000_000_111;
        private const int SourceRowMask =  0b00000000000_0_0_0_0_0_0__0_0_0__000_000_111_000;
        private const int TargetColMask =  0b00000000000_0_0_0_0_0_0__0_0_0__000_111_000_000;
        private const int TargetRowMask =  0b00000000000_0_0_0_0_0_0__0_0_0__111_000_000_000;
        //                                dstR dstC 
        //                   stale  cmate   |   | srcR srcC
        //                         \  |check|   |   |   |
        //      score               | | |   |   |   |   |
        //        v                 v v v   v   v   v   v
        // 00000000000 0 0 0 0 0 0  0 0 0  000 000 000 000
        //             | | | | | |
        //    promotes Q N B R | \
        //                     |  takesEnPassant
        //                     doubleAdvancesPawn
        private int state;
        public int SourceCol { 
            get => state & SourceColMask; 
            set => state = (state & ~SourceColMask) | value; 
        }
        public int SourceRow { 
            get => (state & SourceRowMask) >> 3; 
            set => state = (state & ~SourceColMask) | (value << 3); 
        }
        public int TargetCol { 
            get => (state & TargetRowMask) >> 6; 
            set => state = (state & ~TargetColMask) | (value << 6); 
        }
        public int TargetRow { 
            get => (state & TargetColMask) >> 9; 
            set => state = (state & ~TargetColMask) | (value << 9); 
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
        public int Score { 
            get => state >> 21; 
            set => state = (state & ScoreResetMask) | (value << 21); 
        }

        private static readonly Move EmptyMove = new Move();
        public static bool TryCreateMove(int fromRow,int fromCol,int toRow,int toCol, out Move move)
        {
            move = EmptyMove;
            if (fromRow > 8 || fromCol < 0 || toRow > 8 || toCol < 0)
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

        public Move Clone() => new Move { state = state };
    }
}

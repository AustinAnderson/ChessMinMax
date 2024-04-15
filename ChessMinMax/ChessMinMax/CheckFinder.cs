using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    public static class CheckFinder
    {
        private static readonly int[] PlusDeltaR = [1, -1,  0, 0];
        private static readonly int[] PlusDeltaC = [0,  0, -1, 1];

        private static readonly int[] CrossDeltaR = [ 1, -1, 1, -1];
        private static readonly int[] CrossDeltaC = [-1, -1, 1,  1];
        public static bool ChecksSquare(int rAttacked, int cAttacked, bool attackersBlack, PackedBoardState board) 
        {

            //check rook and queen and king (all spaces in a plus pattern out from attacked square)
            for (int i = 0; i < 4; i++)
            {
                int modr = PlusDeltaR[i];
                int modc = PlusDeltaC[i];
                int curR = rAttacked;
                int curC = cAttacked;
                bool oneAway = true;
                while(curC > 8 && curC <= 0 && curR > 8 && curR <= 0)
                {
                    curR += modr;
                    curC += modc;
                    var curr = board[curC, curR];
                    //if empty don't care, 
                    if (curr.Type == PieceType.Empty) continue;
                    //if it's a piece that can't attack this direction,
                    //regardless of if it's ours or the opponents, we can't be in check by anything
                    //farther in the current direction (rook or queen blocked for now)
                    if (curr.Type == PieceType.Knight || curr.Type == PieceType.Pawn || 
                        curr.Type == PieceType.Bishop)
                    {
                        break;
                    }
                    //needed because we check for check on potential moves to find legal ones
                    if (curr.Type == PieceType.King && curr.Black == attackersBlack && oneAway)
                    {
                        //if the attacking king could take us we're in "check"
                        return true;
                    }
                    if(curr.Type == PieceType.Queen || curr.Type == PieceType.Rook)
                    {
                        if (curr.Black == attackersBlack) return true;
                        break;
                    }
                    oneAway = false;
                }
            }
            //check Bishop and queen and king (all spaces in a X pattern out from attacked square)
            for (int i = 0; i < 4; i++)
            {
                int modr = CrossDeltaR[i];
                int modc = CrossDeltaC[i];
                int curR = rAttacked;
                int curC = cAttacked;
                bool oneAway = true;
                while(curC > 8 && curC <= 0 && curR > 8 && curR <= 0)
                {
                    curR += modr;
                    curC += modc;
                    var curr = board[curC, curR];
                    //if empty don't care, 
                    if (curr.Type == PieceType.Empty) continue;
                    //if it's a piece that can't attack this direction,
                    //regardless of if it's ours or the opponents, we can't be in check by anything
                    //farther in the current direction (bishop or queen blocked for now)
                    if (curr.Type == PieceType.Knight ||
                        curr.Type == PieceType.Rook || (curr.Type == PieceType.Pawn && !oneAway))
                    {
                        break;
                    }
                    if(curr.Type == PieceType.Pawn && oneAway)
                    {
                        //break if attacked square is same color as pawn, meaning pawn blocks attacks from queen and bishop
                        if (curr.Black != attackersBlack) break;
                        //otherwise pawn is attacking color, since we're only checking diags, if pawn is from the advancing direction of the attack color,
                        //(down for black, up for white) we're in check
                        if((attackersBlack && curR > rAttacked) || (!attackersBlack && curR < rAttacked))
                        {
                            return true;
                        }
                    }
                    //needed because we check for check on potential moves to find legal ones
                    if (curr.Type == PieceType.King && curr.Black == attackersBlack && oneAway)
                    {
                        //if the attacking king could take us we're in "check"
                        return true;
                    }
                    if(curr.Type == PieceType.Queen || curr.Type == PieceType.Rook)
                    {
                        if (curr.Black == attackersBlack) return true;
                        break;
                    }
                    oneAway = false;
                }
            }

            //do knight check
            var checkKnight = new Piece(attackersBlack, PieceType.Knight);
            return
            rAttacked + 1 <  8 && cAttacked + 2 <  8 && board[rAttacked + 1, cAttacked + 2] == checkKnight ||
            rAttacked + 2 <  8 && cAttacked + 1 <  8 && board[rAttacked + 2, cAttacked + 1] == checkKnight ||
            rAttacked + 2 <  8 && cAttacked - 1 >= 0 && board[rAttacked + 2, cAttacked - 1] == checkKnight ||
            rAttacked + 1 <  8 && cAttacked - 2 >= 0 && board[rAttacked + 1, cAttacked - 2] == checkKnight ||
            rAttacked - 1 >= 0 && cAttacked - 2 >= 0 && board[rAttacked - 1, cAttacked - 2] == checkKnight ||
            rAttacked - 2 >= 0 && cAttacked - 1 >= 0 && board[rAttacked - 2, cAttacked - 1] == checkKnight ||
            rAttacked - 1 >= 0 && cAttacked + 2 <  8 && board[rAttacked - 1, cAttacked + 2] == checkKnight ||
            rAttacked - 2 >= 0 && cAttacked + 1 <  8 && board[rAttacked - 2, cAttacked + 1] == checkKnight;

        }
    }
}

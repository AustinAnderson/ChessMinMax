using Coords = System.ValueTuple<int, int>;

namespace ChessMinMax
{
    public static class AttackLogic
    {
        private static readonly (int,int)[] PlusDelta =  [(1, 0), (-1, 0), (0,-1), ( 0, 1)];
        private static readonly (int,int)[] CrossDelta = [(1,-1), (-1,-1), (1, 1), (-1, 1)];
        public static List<Coords> ThreatensSquare(int rAttacked, int cAttacked, bool attackersBlack, IConstPackedBoardState board) 
        {
            var coords = new List<Coords>();

            //check rook and queen and king (all spaces in a plus pattern out from attacked square)
            for (int i = 0; i < 4; i++)
            {
                var (modr,modc) = PlusDelta[i];
                int curR = rAttacked;
                int curC = cAttacked;
                int loopCount = 0;
                while(curC < 8 && curC >= 0 && curR < 8 && curR >= 0)
                {
                    curR += modr;
                    curC += modc;
                    loopCount++;
                    bool oneAway = loopCount == 1;
                    var curr = board[curR, curC];
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
                        coords.Add((curR, curC));
                        break;
                    }
                    else if(curr.Type == PieceType.Queen || curr.Type == PieceType.Rook)
                    {
                        if (curr.Black == attackersBlack)
                        {
                            coords.Add((curR, curC));
                        }
                        break;
                    }
                }
            }
            //check Bishop and queen and king (all spaces in a X pattern out from attacked square)
            for (int i = 0; i < 4; i++)
            {
                var (modr,modc) = CrossDelta[i];
                int curR = rAttacked;
                int curC = cAttacked;
                int loopCount = 0;
                while(curC < 8 && curC >= 0 && curR < 8 && curR >= 0)
                {
                    curR += modr;
                    curC += modc;
                    loopCount++;
                    bool oneAway = loopCount == 1;
                    var curr = board[curR, curC];
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
                        if((attackersBlack && curR < rAttacked) || (!attackersBlack && curR > rAttacked))
                        {
                            coords.Add((curR,curC));
                            break;
                        }
                    }
                    //needed because we check for check on potential moves to find legal ones
                    if (curr.Type == PieceType.King && curr.Black == attackersBlack && oneAway)
                    {
                        //if the attacking king could take us we're in "check"
                        coords.Add((curR,curC));
                        break;
                    }
                    if(curr.Type == PieceType.Queen || curr.Type == PieceType.Bishop)
                    {
                        if (curr.Black == attackersBlack)
                        {
                            coords.Add((curR,curC));
                        }
                        break;
                    }
                }
            }

            //do knight check
            CheckKnight(rAttacked + 1, cAttacked + 2, attackersBlack, board, coords);
            CheckKnight(rAttacked + 2, cAttacked + 1, attackersBlack, board, coords);
            CheckKnight(rAttacked + 2, cAttacked - 1, attackersBlack, board, coords);
            CheckKnight(rAttacked + 1, cAttacked - 2, attackersBlack, board, coords);
            CheckKnight(rAttacked - 1, cAttacked - 2, attackersBlack, board, coords);
            CheckKnight(rAttacked - 2, cAttacked - 1, attackersBlack, board, coords);
            CheckKnight(rAttacked - 1, cAttacked + 2, attackersBlack, board, coords);
            CheckKnight(rAttacked - 2, cAttacked + 1, attackersBlack, board, coords);
            return coords;
        }
        private static void CheckKnight(int rAttackingKnight, int cAttackingKnight, bool isBlack, IConstPackedBoardState state, List<Coords> coords)
        {
            if (rAttackingKnight < 8 && cAttackingKnight < 8 && rAttackingKnight >= 0 && cAttackingKnight >= 0 &&
                state[rAttackingKnight, cAttackingKnight].Type==PieceType.Knight &&
                state[rAttackingKnight, cAttackingKnight].Black==isBlack
            )
            {
                coords.Add((rAttackingKnight, cAttackingKnight));
            }
        }
    }
}

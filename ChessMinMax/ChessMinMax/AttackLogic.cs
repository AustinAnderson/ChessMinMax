namespace ChessMinMax
{
    public static class AttackLogic
    {
        private static readonly (int,int)[] PlusDelta =  [(1, 0), (-1, 0), (0,-1), ( 0, 1)];
        private static readonly (int,int)[] CrossDelta = [(1,-1), (-1,-1), (1, 1), (-1, 1)];
        public static List<(int r, int c)> ThreatensSquare(int rAttacked, int cAttacked, bool attackersBlack, IConstPackedBoardState board) 
        {
            var coords = new List<(int r,int c)>();

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
        private static void CheckKnight(int rAttackingKnight, int cAttackingKnight, bool isBlack, IConstPackedBoardState state, List<(int r,int c)> coords)
        {
            if (rAttackingKnight < 8 && cAttackingKnight < 8 && rAttackingKnight >= 0 && cAttackingKnight >= 0 &&
                state[rAttackingKnight, cAttackingKnight].Type==PieceType.Knight &&
                state[rAttackingKnight, cAttackingKnight].Black==isBlack
            )
            {
                coords.Add((rAttackingKnight, cAttackingKnight));
            }
        }
        public static Dictionary<(int rPinned,int cPinned),(int rAttacker,int cAttacker)> GetPinnedToKing(bool kingIsBlack, IConstPackedBoardState board)
        {
            var (rAttacked, cAttacked) = board.GetKingCoords(kingIsBlack);
            var coords = new Dictionary<(int rPinned, int cPinned), (int rAttacker, int cAttacker)>();
            foreach (var (limitedPiece, direction) in new[] { 
                (PieceType.Rook, PlusDelta), (PieceType.Bishop, CrossDelta) 
            })
            {
                //check rook/bishop and queen (all spaces in a plus/cross pattern out from king's square)
                for (int i = 0; i < 4; i++)
                {
                    var (modr, modc) = direction[i];
                    int curR = rAttacked;
                    int curC = cAttacked;
                    int potentialR = -1;
                    int potentialC = -1;
                    while (curC < 8 && curC >= 0 && curR < 8 && curR >= 0)
                    {
                        curR += modr;
                        curC += modc;
                        var curr = board[curR, curC];
                        //if empty don't care, if we're in check from a piece, it's not pinning
                        if (curr.Type == PieceType.Empty) continue;
                        //first piece we run into is same color
                        if (potentialR == -1)
                        {
                            if (curr.Black == kingIsBlack)
                            {
                                potentialR = curR;
                                potentialC = curC;
                            }
                            else//can't be a pin if not ours
                            {
                                break;
                            }
                        }
                        //we already ran into our piece, so either a pin and done with dir or non threatening piece still done with dir
                        else{

                            if (curr.Black != kingIsBlack && (curr.Type == limitedPiece || curr.Type == PieceType.Queen))
                            {
                                coords.Add((potentialR, potentialC), (curR, curC));
                            }
                            break;
                        }
                    }
                }
            }
            return coords;
        }
    }
}

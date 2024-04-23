using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    public class MoveScorer
    {
        private static readonly Dictionary<PieceType, int> pieceValues = new Dictionary<PieceType, int> {
            { PieceType.King, 1000 },
            { PieceType.Queen, 90 },
            { PieceType.Rook, 50 },
            { PieceType.Bishop, 35 },
            { PieceType.Knight, 30 },
            { PieceType.Pawn, 10 },
            { PieceType.Empty, 0 }
        };
        public static IEnumerable<Move> ScoreMoves(IEnumerable<Move> moves, IConstPackedBoardState boardState)
        {
            bool unknownPlayerColor=true;
            bool playerIsBlack = false;
            foreach (var move in moves)
            {
                if (unknownPlayerColor)
                {
                    playerIsBlack = boardState[move.SourceRow, move.SourceCol].Black;
                    unknownPlayerColor = true;
                }
                SetCheckOrMates(move, boardState);
                if (move.CheckMates)
                {
                    move.Score = Move.MAX_SCORE;
                }
                else
                {
                    var boardCopy = boardState.Clone();
                    var captured = boardCopy.Move(move);
                    var score = pieceValues[captured.Type];//includes 0 for empty
                    if (move.Checks)
                    {
                        score += 32;
                    }
                    move.Score = score;
                    //stale = enact move and opponent has no moves, kinda pricey, and rare, maybe skip?
                    //fork = good (TODO: avg of forked pieces?, but won't that come out naturally by way of next min max iteration?)
                    //threaten piece, also wonder if next min max will catch that?
                }

                yield return move;
            }
        }
        private static void SetCheckOrMates(Move move, IConstPackedBoardState board) 
        {
            var boardCopy = board.Clone();
            var piece = boardCopy[move.SourceRow, move.SourceCol];
            boardCopy.Move(move);
            //find opposing king, search left right up down and diags for opposing piece,
            //stopping that direction if same color piece in the way,
            //then check the knight squares away from the king for an opposing knight.

            int rOppKing = 0;
            int cOppKing = 0;
            bool done = false;
            for (; rOppKing < 8 && !done; rOppKing++)
            {
                for(; cOppKing < 8 && !done; cOppKing++)
                {
                    if (boardCopy[rOppKing, cOppKing] == new Piece(!piece.Black, PieceType.King))
                    {
                        done = true;
                    }
                }
            }
            //could probably do with just count of checks, and use move for coords if only one
            var checkers = CheckFinder.ChecksSquare(rOppKing, cOppKing, piece.Black, boardCopy);
            if(checkers.Count>0)
            {
                move.Checks = true;
                move.CheckMates =
                    //king can't take the checking piece or move out of check, i.e. no legal moves
                    MoveFinder.GetKingMoves(rOppKing, cOppKing, !piece.Black, boardCopy).Count == 0
                    //and can't get out of check by taking piece next turn
                    && (
                        //so more than one checker or
                        checkers.Count > 1 ||
                        //no pieces can immediately take the one putting us in check
                        CheckFinder.ChecksSquare(checkers[0].Item1, checkers[0].Item2, piece.Black, boardCopy).Count == 0
                    );
                    
            } 
        }
    }
}

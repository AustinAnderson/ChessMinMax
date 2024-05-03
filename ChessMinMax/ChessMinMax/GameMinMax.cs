using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    public class TreeNode
    {
        public bool blacksTurn;
        public List<TreeNode> EachMove = new List<TreeNode>();
        public PackedBoardState Board;
        public Move MoveToGetHere;
        public int score;
        public override string ToString()=> $"toHere: {MoveToGetHere} {MoveToGetHere.Score}=>{score}, {EachMove.Count} children";
        public void FillChildren(int depthCounter, HashSet<(int r,int c)> checkers)
        {
            if (depthCounter < 1) { return; }

            var nextCheck=new HashSet<(int r,int c)>();
            foreach(var move in MoveScorer.ScoreMoves(MoveFinder.GetMovesForPlayer(blacksTurn, Board, checkers), Board))
            {
                if (move.Checks) 
                {
                    nextCheck.Add((move.TargetRow, move.TargetCol));
                }
                var board = Board.Clone();
                board.Move(move);
                EachMove.Add(new TreeNode
                {
                    blacksTurn = !blacksTurn,
                    MoveToGetHere = move,
                    Board = board
                });
            }
            //TODO: tweak iteration order for performance
            foreach(var node in EachMove)
            {
                node.FillChildren(blacksTurn ? depthCounter : depthCounter - 1,nextCheck);
            }
        }
        public void SetScore(bool lookForMin)
        {
            int max = int.MinValue;
            if(EachMove.Count < 1)
            {
                score = MoveToGetHere.Score;
                return;
            }
            foreach(var node in EachMove)
            {
                node.SetScore(!lookForMin);
                if(node.score > max)
                {
                    max = node.score;
                }
            }
            if (max != int.MinValue)
            {
                if (lookForMin)
                {
                    max *= -1;
                }
                score += max;
            }
        }
    }
    public class GameMinMax
    {
        public static Move? RunAlgo(PackedBoardState boardState, bool blacksTurn, int maxDepth)
        {
            var root = new TreeNode
            {
                blacksTurn = blacksTurn,
                Board = boardState,
                MoveToGetHere = new Move()
            };
            var (rKing, cKing) = boardState.GetKingCoords(blacksTurn);
            root.FillChildren(maxDepth,new HashSet<(int,int)>(AttackLogic.ThreatensSquare(rKing,cKing,!blacksTurn,boardState)));
            if (root.EachMove.Count == 0)
            {
                return null;
            }
            root.SetScore(lookForMin: true);
            Move move = root.EachMove[0].MoveToGetHere;
            int max = 0;
            foreach(var node in root.EachMove)
            {
                if (node.score > max) 
                { 
                    max = node.score;
                    move = node.MoveToGetHere;
                }
            }
            return move;
        }
    }
}

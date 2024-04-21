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
        public void FillChildren(int depthCounter)
        {
            if (depthCounter < 1) { return; }
            foreach(var move in MoveScorer.ScoreMoves(MoveFinder.GetMovesForPlayer(blacksTurn, Board), Board))
            {
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
                node.FillChildren(blacksTurn? depthCounter: depthCounter-1);
            }
        }
        public void SetScore(bool lookForMin)
        {
            int minMax = int.MinValue;
            if (lookForMin) minMax = int.MaxValue;
            foreach(var node in EachMove)
            {
                node.SetScore(!lookForMin);
                if(lookForMin && node.score < minMax)
                {
                    minMax = node.score;
                }
                else if (!lookForMin && node.score > minMax)
                {
                    minMax = node.score;
                }
            }
            if (minMax != int.MinValue && minMax != int.MaxValue)
            {
                score = minMax;
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
            root.FillChildren(maxDepth);
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

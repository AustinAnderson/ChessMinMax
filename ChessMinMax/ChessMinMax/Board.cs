using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    public class Board
    {
        private PackedBoardState board;
        private bool isBlackPlayer;
        public Board(bool isBlackPlayer=false)
        {
            this.isBlackPlayer = isBlackPlayer;
            board = PackedBoardState.Pack([
                [Rb,Nb,Bb,Qb,Kb,Bb,Nb,Rb],
                [pb,pb,pb,pb,pb,pb,pb,pb],
                [__,__,__,__,__,__,__,__],
                [__,__,__,__,__,__,__,__],
                [__,__,__,__,__,__,__,__],
                [__,__,__,__,__,__,__,__],
                [pw,pw,pw,pw,pw,pw,pw,pw],
                [Rw,Nw,Bw,Qw,Kw,Bw,Nw,Rw],
            ]);
        }
        private static readonly Piece? __ = null;
        private static readonly Piece
            pb = new(true, PieceType.Pawn),
            Rb = new(true, PieceType.Rook),
            Nb = new(true, PieceType.Knight),
            Bb = new(true, PieceType.Bishop),
            Qb = new(true, PieceType.Queen),  Kb = new(true, PieceType.King);
        private static readonly Piece
            pw = new(false, PieceType.Pawn),
            Rw = new(false, PieceType.Rook),
            Nw = new(false, PieceType.Knight),
            Bw = new(false, PieceType.Bishop),
            Qw = new(false, PieceType.Queen),  Kw = new(false, PieceType.King);

        public string GetDisplayString(PrintDisplay displayMap)
        {
            StringBuilder builder = new StringBuilder();
            int rowNum = 8;
            for(int r = 0; r < 8; r++)
            {
                builder.Append(rowNum+" ");
                for(int ci = 0; ci < 8; ci++)
                {
                    bool isBlack = PackedBoardState.SquareIsBlack(r, ci);
                    string c = displayMap.GetBlank(isBlack);
                    if(board[r,ci].Type != PieceType.Empty)
                    {
                        c = displayMap.Get(board[r,ci], isBlack);
                    }
                    builder.Append(c);
                }
                builder.AppendLine("");
                rowNum--;
            }
            var length = displayMap.CellWidth;

            builder.Append("  ");
            var toPlace = "abcdefgh";
            for (int i = 0; i < 8; i++) 
            {
                for (int j = 0; j < length; j++)
                {
                    if (j == length / 2)
                    {
                        builder.Append(toPlace[i]);
                    }
                    else
                    {
                        builder.Append(' ');
                    }
                }
            }
            return builder.ToString();
        }
        public bool EnactMove(Move move)
        {
            var (rKing, cKing) = board.GetKingCoords(isBlackPlayer);
            var moves = MoveFinder.GetMovesForPlayer(blackPlayer:isBlackPlayer,board,
                new HashSet<(int,int)>(AttackLogic.ThreatensSquare(rKing, cKing, attackersBlack:!isBlackPlayer, board))
            );
            var selected = moves.FirstOrDefault(x =>
                (x.SourceRow, x.SourceCol, x.TargetRow, x.TargetCol) ==
                (move.SourceRow, move.SourceCol, move.TargetRow, move.TargetCol)
            );
            if(selected != null)
            {
                board.Move(selected);
                return true;
            }
            return false;
        }
        public Move PlayComputerMove()
        {
            var move = GameMinMax.RunAlgo(board, blacksTurn: !isBlackPlayer, 2);
            if (move == null) throw new ArgumentException("move was null");
            board.Move(move);
            return move;
        }
        public string Serialize()
        {
            return board.Serialize();
        }
    }
}

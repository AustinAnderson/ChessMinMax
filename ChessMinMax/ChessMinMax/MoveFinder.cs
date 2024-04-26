using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    public class MoveFinder
    {
        public static IEnumerable<Move> GetMovesForPlayer(bool blackPlayer, IConstPackedBoardState boardState)
        {
            for (int row = 0; row< 8; row++) 
            { 
                for(int col = 0; col < 8; col++)
                {
                    if (boardState[row, col].Black == blackPlayer)
                    {
                        foreach(var move in GetLegalMoves(row, col, boardState))
                        {
                            yield return move;
                        }
                    }
                }
            }
        }
        public static IEnumerable<Move> GetLegalMoves(int row,int col, IConstPackedBoardState board)
        {
            var piece = board[row, col];
            return piece.Type switch
            {
                PieceType.Empty => new List<Move>(),
                PieceType.King => GetKingMoves(row, col, piece.Black, board),
                PieceType.Queen => GetRookQueenBishopMoves(row,col, piece.Black, board,
                    [(-1, -1), (-1, 0), (-1, 1), (0, 1), (1, 1), (1, 0), (1, -1), (0, -1)]
                ),
                PieceType.Bishop => GetRookQueenBishopMoves(row,col, piece.Black, board,
                    [(-1, -1), (-1, 1), (1, 1), (1, -1)]
                ),
                PieceType.Rook => GetRookQueenBishopMoves(row,col, piece.Black, board,
                    [(-1, 0), (0, 1), (1, 0), (0, -1)]
                ),
                PieceType.Knight => GetKnightMoves(row, col, piece.Black, board),
                PieceType.Pawn => GetPawnMoves(row,col, piece.Black, board),
                _ => throw new NotImplementedException($"moves of {piece.Type}")
            };
        }
        /*
         * ___________________
        ___0|_|_|_|_||_|_|_|_|
        ___1|_|_|_|_||_|_|_|_|
        ___2|_|_|_|_||_|_|_|_|
        ___3|_|_|_|_||_|_|_|_|
        =====================|
        ___4|_|_|_|_||_|_|_|_|
        ___5|_|_|_|_||_|_|_|_|
        ___6|_|_|_|_||_|_|_|_|
        ___7|_|_|_|_||_|_|_|_|
             0|1|2|3||4|5|6|7|
        */
        public static List<Move> GetPawnMoves(int pRow, int pCol, bool isBlack, IConstPackedBoardState board)
        {
            var moves = new List<Move>();
            var (advanceDir, startRow, opJumpRow, endRow) = isBlack switch
            {
                false => (-1, 6, 3, 0),
                true => (1, 1, 4, 7)
            };


            if (Move.TryCreateMove(pRow, pCol, pRow + advanceDir, pCol, out Move normalAdvance)
                && board[normalAdvance.TargetRow, normalAdvance.TargetCol].Type == PieceType.Empty
            )
            {
                moves.Add(normalAdvance);
            }

            if (Move.TryCreateMove(pRow, pCol, pRow + advanceDir, pCol - 1, out Move attackLeft))
            {
                var piece = board[attackLeft.TargetRow, attackLeft.TargetCol];
                //target is an opposing piece, can move there
                if (piece.Type != PieceType.Empty && piece.Black != isBlack)
                {
                    moves.Add(attackLeft);
                }
            }
            if (Move.TryCreateMove(pRow, pCol, pRow + advanceDir, pCol + 1, out Move attackRight))
            {
                var piece = board[attackRight.TargetRow, attackRight.TargetCol];
                //target is an opposing piece, can move there
                if (piece.Type != PieceType.Empty && piece.Black != isBlack)
                {
                    moves.Add(attackRight);
                }
            }
            for (int i = moves.Count - 1; i >= 0; i--)
            {
                var move = moves[i];
                //don't have to check black or white because it's impossible for a pawn to move backwards,
                //so e.i. white pawn can never be on row 7
                if (move.TargetRow == endRow)
                {
                    var knight = move.Clone();
                    knight.PromotesToKnight = true;
                    moves.Add(knight);

                    var bishop = move.Clone();
                    bishop.PromotesToBishop = true;
                    moves.Add(bishop);

                    var rook = move.Clone();
                    rook.PromotesToRook = true;
                    moves.Add(rook);

                    move.PromotesToQueen = true;
                }
            }
            if (
                Move.TryCreateMove(
                    pRow, pCol,
                    pRow + (advanceDir * 2), pCol,
                    out Move doubleAdvance
                )
                && pRow == startRow
                && board[doubleAdvance.SourceRow, doubleAdvance.TargetCol].Type == PieceType.Empty
            )
            {
                doubleAdvance.DoubleAdvancesPawn = true;
                moves.Add(doubleAdvance);
            }
            //check enpassant
            if (pRow == opJumpRow)
            {
                if (pCol > 0)
                {
                    var passantCheck = board[pRow, pCol - 1];
                    if (passantCheck.Black != isBlack && passantCheck.Type == PieceType.Pawn
                        && board.PawnDoubleAdvancedLastTurn(pCol - 1, passantCheck.Black)
                    )
                    {
                        moves.Add(new Move
                        {
                            SourceRow = pRow,
                            SourceCol = pCol,
                            TargetRow = pRow + advanceDir,
                            TargetCol = pCol - 1,
                            TakesEnPassant = true
                        });
                    }
                }
                if (pCol < 7)
                {
                    var passantCheck = board[pRow, pCol + 1];
                    if (passantCheck.Black != isBlack && passantCheck.Type == PieceType.Pawn
                        && board.PawnDoubleAdvancedLastTurn(pCol + 1, passantCheck.Black)
                    )
                    {
                        moves.Add(new Move
                        {
                            SourceRow = pRow,
                            SourceCol = pCol,
                            TargetRow = pRow + advanceDir,
                            TargetCol = pCol + 1,
                            TakesEnPassant = true
                        });
                    }
                }
            }
            return moves;
        }
        public static List<Move> GetRookQueenBishopMoves(int nRow, int nCol, bool isBlack, 
            IConstPackedBoardState board, (int, int)[] directions
        )
        {
            List<Move> moves = new();
            foreach (var (modR, modC) in directions)
            {
                bool doneWithDirection = false;
                int dstR = nRow + modR;
                int dstC = nCol + modC;
                while (!doneWithDirection)
                {
                    if(!Move.TryCreateMove(nRow, nCol, dstR, dstC, out Move tentative))
                    {
                        break;
                    }
                    var target = board[tentative.TargetRow, tentative.TargetCol];
                    //empty , same color, res
                    // 0    ,      1    , done with direction/break;
                    // 0    ,      0    , add move, break;
                    // 1    ,      -    , add move
                    if(target.Type == PieceType.Empty)
                    {
                        moves.Add(tentative);
                    }
                    else
                    {
                        if(target.Black != isBlack)
                        {
                            moves.Add(tentative);
                        }
                        doneWithDirection = true;
                    }
                    dstR += modR;
                    dstC += modC;
                }
            }
            return moves;
        }
        public static List<Move> GetKnightMoves(int nRow, int nCol, bool isBlack, IConstPackedBoardState board)
        {
            List<Move> moves = new();
            var directions = new[] { (-1, -2), (-2, -1), (-2, 1), (-1, 2), (1, 2), (2, 1), (2, -1), (1, -2) };
            foreach (var (modR, modC) in directions)
            {
                if (Move.TryCreateMove(nRow, nCol, nRow + modR, nCol + modC, out Move tentative) && board[tentative.TargetRow, tentative.TargetCol].Black != isBlack)
                {
                    moves.Add(tentative);
                }
            }
            return moves;
        }
        public static List<Move> GetKingMoves(int kRow, int kCol,bool isBlack, IConstPackedBoardState board)
        {
            List<Move> moves = new();
            var directions = new[] { (-1, -1), (-1, 0), (-1, 1), (0, 1), (1, 1), (1, 0), (1, -1), (0, -1) };
            foreach(var (modR,modC) in directions)
            {
                
                if (!Move.TryCreateMove(kRow, kCol, kRow+modR, kCol+modC, out Move tentative)
                    || board[tentative.TargetRow, tentative.TargetCol].Black == isBlack
                )
                {
                    continue;
                }
                var boardCopy = board.Clone();//make a copy on each iteration to enact the move and revert by throwing the copy away
                boardCopy.Move(tentative);
                //check if the opposing pieces create a check on the tentative king move, can't move into check
                if(AttackLogic.ThreatensSquare(tentative.TargetRow, tentative.TargetCol, attackersBlack: !isBlack, boardCopy).Count==0)
                {
                    moves.Add(tentative);
                }
            }
            if(!board.KingHasMovedForPlayer(isBlack) && !board.LeftRookHasMovedForPlayer(isBlack))
            {
                if (
                    board[kRow, 1].Type == PieceType.Empty &&
                    board[kRow, 2].Type == PieceType.Empty &&
                    board[kRow, 3].Type == PieceType.Empty &&
                    AttackLogic.ThreatensSquare(kRow, kCol, !isBlack, board).Count==0 &&
                    AttackLogic.ThreatensSquare(kRow, 2, !isBlack, board).Count==0 &&
                    AttackLogic.ThreatensSquare(kRow, 3, !isBlack, board).Count==0
                )
                {
                    moves.Add(new Move { CastlesQueenSide = true, SourceRow = kRow, SourceCol = kCol });
                }
            }
            if(!board.KingHasMovedForPlayer(isBlack) && !board.RightRookHasMovedForPlayer(isBlack))
            {
                if (
                    board[kRow, 5].Type == PieceType.Empty &&
                    board[kRow, 6].Type == PieceType.Empty &&
                    AttackLogic.ThreatensSquare(kRow, kCol, !isBlack, board).Count==0 &&
                    AttackLogic.ThreatensSquare(kRow, 5, !isBlack, board).Count==0 &&
                    AttackLogic.ThreatensSquare(kRow, 6, !isBlack, board).Count==0
                )
                {
                    moves.Add(new Move { CastlesKingSide = true, SourceRow = kRow, SourceCol = kCol });
                }
            }
            return moves;
        }
        
    }
}

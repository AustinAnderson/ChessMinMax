using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChessMinMax
{
    public class MoveFinder
    {
        //TODO: move to move scorer
        private static readonly Dictionary<PieceType, int> pieceValues = new Dictionary<PieceType, int> {
            { PieceType.King, 1000 },
            { PieceType.Queen, 90 },
            { PieceType.Rook, 50 },
            { PieceType.Bishop, 35 },
            { PieceType.Knight, 30 },
            { PieceType.Pawn, 10 },
        };

        public IEnumerable<Move> GetMovesForPlayer(bool blackPlayer, PackedBoardState boardState)
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
        public IEnumerable<Move> GetLegalMoves(int row,int col, PackedBoardState board)
        {
            var piece = board[row, col];
            var moves = piece.Type switch
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
            //TODO:move to move scorer
            foreach(var move in moves)
            {
                SetCheckOrMates(move, board);
                yield return move;//iterate list only once, logic in foreach conceptually is going here
            }
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
        private List<Move> GetPawnMoves(int pRow, int pCol, bool isBlack, PackedBoardState board)
        {
            var moves = new List<Move>();

            //                                                             black vals, white vals
            foreach(var (advanceDir, startRow, opJumpRow, endRow) in new[] {(1,1,4,7),(-1,6,3,0)})
            {
                
                if (Move.TryCreateMove(pRow, pCol, pRow + advanceDir, pCol, out Move normalAdvance)
                    && board[normalAdvance.TargetRow,normalAdvance.TargetCol].Type == PieceType.Empty
                )
                {
                    moves.Add(normalAdvance);
                }
                
                if (Move.TryCreateMove(pRow, pCol, pRow + advanceDir, pCol - 1, out Move attackLeft))
                {
                    var piece = board[attackLeft.TargetRow,attackLeft.TargetCol];
                    //target is an opposing piece, can move there
                    if(piece.Type != PieceType.Empty && piece.Black != isBlack)
                    {
                        moves.Add(attackLeft);
                    }
                }
                if (Move.TryCreateMove(pRow, pCol, pRow + advanceDir, pCol + 1, out Move attackRight))
                {
                    var piece = board[attackRight.TargetRow,attackRight.TargetCol];
                    //target is an opposing piece, can move there
                    if(piece.Type != PieceType.Empty && piece.Black != isBlack)
                    {
                        moves.Add(attackRight);
                    }
                }
                foreach(var move in moves)
                {
                    //don't have to check black or white because it's impossible for a pawn to move backwards,
                    //so e.i. white pawn can never be on row 7
                    if(move.TargetRow == endRow)
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
                if(
                    Move.TryCreateMove(
                        pRow, pCol,
                        pRow + (advanceDir * 2), pCol,
                        out Move doubleAdvance
                    ) 
                    && pRow == startRow 
                    && board[doubleAdvance.SourceRow,doubleAdvance.TargetCol].Type == PieceType.Empty
                )
                {
                    doubleAdvance.DoubleAdvancesPawn = true;
                    moves.Add(doubleAdvance);
                }
                //check enpassant
                if(pRow == opJumpRow)
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
            }
            return moves;
        }
        private List<Move> GetRookQueenBishopMoves(int nRow, int nCol, bool isBlack, 
            PackedBoardState board, (int, int)[] directions
        )
        {
            List<Move> moves = new();
            foreach (var (modR, modC) in directions)
            {
                bool directionDone = false;
                while (!directionDone)
                {
                    if(!Move.TryCreateMove(nRow, nCol, nRow + modR, nCol + modC, out Move tentative))
                    {
                        break;
                    }
                    if (board[tentative.TargetRow, tentative.TargetCol].Black == isBlack)
                    {
                        directionDone = true;
                    }
                    else
                    {
                        moves.Add(tentative);
                    }
                }
            }
            return moves;
        }
        private List<Move> GetKnightMoves(int nRow, int nCol, bool isBlack, PackedBoardState board)
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
        private List<Move> GetKingMoves(int kRow, int kCol,bool isBlack, PackedBoardState board)
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
                var boardCopy = board;//make a copy on each iteration to enact the move and revert by throwing the copy away
                boardCopy.Move(tentative);
                //check if the opposing pieces create a check on the tentative king move, can't move into check
                if(!CheckFinder.ChecksSquare(tentative.TargetRow, tentative.TargetCol, attackersBlack: !isBlack, boardCopy))
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
                    !CheckFinder.ChecksSquare(kRow, kCol, !isBlack, board) &&
                    !CheckFinder.ChecksSquare(kRow, 2, !isBlack, board) &&
                    !CheckFinder.ChecksSquare(kRow, 3, !isBlack, board)
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
                    !CheckFinder.ChecksSquare(kRow, kCol, !isBlack, board) &&
                    !CheckFinder.ChecksSquare(kRow, 5, !isBlack, board) &&
                    !CheckFinder.ChecksSquare(kRow, 6, !isBlack, board)
                )
                {
                    moves.Add(new Move { CastlesKingSide = true, SourceRow = kRow, SourceCol = kCol });
                }
            }
            return moves;
        }

        //TODO: move to move scorer
        private void SetCheckOrMates(Move move, PackedBoardState board) 
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
            if(CheckFinder.ChecksSquare(rOppKing,cOppKing,piece.Black, boardCopy))
            {
                move.Checks = true;
                move.CheckMates = 
                    //king can't take the checking piece or move out of check, i.e. no legal moves
                    GetKingMoves(rOppKing, cOppKing, !piece.Black, boardCopy).Count == 0 &&
                    //nothing can take the square putting us in check
                    CheckFinder.ChecksSquare(move.TargetRow, move.TargetCol, piece.Black, boardCopy);
            } 

        }
    }
}

using ChessMinMax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class TestMoveFinder
    {
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



        [TestMethod]
        public void TestQueen()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [pb,__,Qw,__,__,Nb,pb,__],//2
                [__,pb,__,pb,__,Bb,__,pb],//3
                [__,__,__,pw,__,Bw,__,__],//4
                [__,__,__,__,pw,Nw,__,__],//5
                [pw,pw,pw,__,__,pw,pw,pw],//6
                [__,__,Kw,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var moves = MoveFinder.GetLegalMoves(2, 2, state).ToList();
            Assert.AreEqual(16, moves.Count);
        }
        [TestMethod]
        public void TestRook()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [pb,__,Qw,__,__,Nb,pb,__],//2
                [__,pb,__,pb,__,Bb,__,pb],//3
                [__,__,__,__,__,Bw,__,__],//4
                [__,__,__,__,pw,Nw,__,__],//5
                [pw,pw,pw,__,__,pw,pw,pw],//6
                [__,__,Kw,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var moves = MoveFinder.GetLegalMoves(7, 3, state).ToList();
            Assert.AreEqual(5, moves.Count);
        }
        [TestMethod]
        public void TestPawn()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,pw,__,pb,pb,__,__],//1
                [pb,__,Qw,__,__,Nb,pb,__],//2
                [__,pb,__,pb,__,Bb,__,pb],//3
                [__,__,__,__,__,Bw,__,__],//4
                [__,__,__,__,pw,Nw,__,__],//5
                [pw,pw,__,__,__,pw,pw,pw],//6
                [__,__,Kw,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var moves = MoveFinder.GetLegalMoves(1, 2, state).ToList();
            Move[] expected = [
                new Move {SourceRow = 1, SourceCol = 2, TargetRow = 0, TargetCol = 1, PromotesToQueen = true},
                new Move {SourceRow = 1, SourceCol = 2, TargetRow = 0, TargetCol = 1, PromotesToRook = true},
                new Move {SourceRow = 1, SourceCol = 2, TargetRow = 0, TargetCol = 1, PromotesToBishop = true},
                new Move {SourceRow = 1, SourceCol = 2, TargetRow = 0, TargetCol = 1, PromotesToKnight = true},
                new Move {SourceRow = 1, SourceCol = 2, TargetRow = 0, TargetCol = 2, PromotesToQueen = true},
                new Move {SourceRow = 1, SourceCol = 2, TargetRow = 0, TargetCol = 2, PromotesToRook = true},
                new Move {SourceRow = 1, SourceCol = 2, TargetRow = 0, TargetCol = 2, PromotesToBishop = true},
                new Move {SourceRow = 1, SourceCol = 2, TargetRow = 0, TargetCol = 2, PromotesToKnight = true},
                new Move {SourceRow = 1, SourceCol = 2, TargetRow = 0, TargetCol = 3, PromotesToQueen = true},
                new Move {SourceRow = 1, SourceCol = 2, TargetRow = 0, TargetCol = 3, PromotesToRook = true},
                new Move {SourceRow = 1, SourceCol = 2, TargetRow = 0, TargetCol = 3, PromotesToBishop = true},
                new Move {SourceRow = 1, SourceCol = 2, TargetRow = 0, TargetCol = 3, PromotesToKnight = true},
            ];
            AssertUtils.AssertSameMoveList(expected, moves);
        }
        [TestMethod]
        public void TestBlockedPawn()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [pb,__,Qb,__,__,Nb,pb,__],//2
                [__,pb,pw,pb,__,Bb,__,pb],//3
                [__,__,__,__,__,Bw,__,__],//4
                [__,__,__,__,pw,Nw,__,__],//5
                [pw,pw,__,__,__,pw,pw,pw],//6
                [__,__,Kw,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var moves = MoveFinder.GetLegalMoves(3, 2, state).ToList();
            Assert.AreEqual(0, moves.Count);
        }
        [TestMethod]
        public void TestEnPassant()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,pw,__,pb,pb,__,__],//1
                [pb,__,Qw,__,__,Nb,pb,__],//2
                [__,pb,__,pb,__,Bb,__,pb],//3
                [__,pw,pb,__,__,Bw,__,__],//4
                [__,__,__,__,pw,Nw,__,__],//5
                [pw,__,__,__,__,pw,pw,pw],//6
                [__,__,Kw,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            state.SetPawnDoubleAdvancedLastTurn(1, isBlack: false, true);
            var moves = MoveFinder.GetLegalMoves(4, 2, state).ToList();
            Move[] expected = [
                new Move{ SourceRow = 4, SourceCol = 2, TargetRow = 5, TargetCol = 2},
                new Move{ SourceRow = 4, SourceCol = 2, TargetRow = 5, TargetCol = 1, TakesEnPassant = true}
            ];
            AssertUtils.AssertSameMoveList(expected, moves);
        }
        [TestMethod]
        public void NoMoveKing()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [pb,__,Qw,__,__,Nb,pb,__],//2
                [__,__,__,__,__,Bb,__,pb],//3
                [__,__,__,__,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [__,__,__,pb,__,__,Rb,__],//6
                [__,__,Kw,pw,__,__,__,__],//7
              //  0  1  2  3  4  5  6  7
            ]);

        }
        [TestMethod]
        public void NoMovePawn()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [pb,__,Qw,__,__,Nb,pb,__],//2
                [__,__,__,__,__,Bb,__,pb],//3
                [__,__,__,__,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [__,__,__,pb,__,__,Rb,__],//6
                [__,__,Kw,pw,__,__,__,__],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var moves = MoveFinder.GetLegalMoves(7, 2, state).ToList().Count();
            Assert.AreEqual(0, moves);

        }
    }
}

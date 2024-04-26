using ChessMinMax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class TestMoveScorer
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
        public void TestFindCheckMateQueen()
        {
            var state = PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,pb,__,pb,__,__],//1
                [__,__,Qw,__,pw,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,__,Rw,__,__,Bw,__,__],//4
                [__,__,__,__,pw,Nw,__,__],//5
                [__,__,__,__,__,pw,pw,pw],//6
                [__,__,Kw,__,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var res = MoveScorer.ScoreMoves(new[] {new Move
            {
                SourceRow = 2, SourceCol = 2,
                TargetRow = 1, TargetCol = 3
            } }, state).Single();
            Assert.AreEqual((true, (2, 2), (1, 3)), (res.CheckMates, (res.SourceRow,res.SourceCol), (res.TargetRow,res.TargetCol)));

        }
        [TestMethod]
        public void TestFindCheckMate()
        {
            var state = PackedBoardState.Pack([
                [Kb,__,__,__,__,__,__,__],//0
                [__,Rw,Qw,__,__,__,__,__],//1
                [__,__,__,__,__,__,__,__],//2
                [__,__,__,__,__,__,__,__],//3
                [__,__,__,__,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [__,__,__,__,__,__,__,__],//6
                [__,__,Kw,__,__,__,__,__],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var res = MoveScorer.ScoreMoves(new[] {new Move
            {
                SourceRow = 1, SourceCol = 1,
                TargetRow = 0, TargetCol = 1
            } }, state).Single();
            Assert.AreEqual((true, (1, 1), (0, 1)), (res.CheckMates, (res.SourceRow,res.SourceCol), (res.TargetRow,res.TargetCol)));
        }
        [TestMethod]
        public void TestFindNotCheckMateKingCaptureOut()
        {
            var state = PackedBoardState.Pack([
                [Kb,__,__,__,__,__,__,__],//0
                [__,Rw,Rw,__,__,__,__,__],//1
                [__,__,__,__,__,__,__,__],//2
                [__,__,__,__,__,__,__,__],//3
                [__,__,__,__,__,Nb,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [__,__,__,__,__,__,__,__],//6
                [__,__,Kw,__,__,__,__,__],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var res = MoveScorer.ScoreMoves(new[] {new Move
            {
                SourceRow = 1, SourceCol = 1,
                TargetRow = 0, TargetCol = 1
            } }, state).Single();
            Assert.AreEqual((false, (1, 1), (0, 1)), (res.CheckMates, (res.SourceRow,res.SourceCol), (res.TargetRow,res.TargetCol)));

        }
        [TestMethod]
        public void TestFindNotCheckMateOtherCaptureOut()
        {
            var state = PackedBoardState.Pack([
                [Kb,__,__,__,__,Rb,__,__],//0
                [__,Rw,Qw,__,__,__,__,__],//1
                [__,__,__,__,__,__,__,__],//2
                [__,__,__,__,__,__,__,__],//3
                [__,__,__,__,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [__,__,__,__,__,__,__,__],//6
                [__,__,Kw,__,__,__,__,__],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var res = MoveScorer.ScoreMoves(new[] {new Move
            {
                SourceRow = 1, SourceCol = 1,
                TargetRow = 0, TargetCol = 1
            } }, state).Single();
            Assert.AreEqual((false, (1, 1), (0, 1)), (res.CheckMates, (res.SourceRow,res.SourceCol), (res.TargetRow,res.TargetCol)));

        }
        [TestMethod]
        public void TestFindCheckMateWithCapture()
        {
            //moving the rook up is check mate, and taking it with opposing rook
            //is still check mate because of bishop
            var state = PackedBoardState.Pack([
                [Kb,__,__,__,__,Rb,__,__],//0
                [__,Rw,Qw,__,__,__,__,__],//1
                [__,__,Bw,__,__,__,__,__],//2
                [__,__,__,__,__,__,__,__],//3
                [__,__,__,__,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [__,__,__,__,__,__,__,__],//6
                [__,__,Kw,__,__,__,__,__],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var res = MoveScorer.ScoreMoves(new[] {new Move
            {
                SourceRow = 1, SourceCol = 1,
                TargetRow = 0, TargetCol = 1
            } }, state).Single();
            Assert.AreEqual((true, (1, 1), (0, 1)), (res.CheckMates, (res.SourceRow,res.SourceCol), (res.TargetRow,res.TargetCol)));
        }

    }
}

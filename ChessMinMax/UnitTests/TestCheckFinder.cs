using ChessMinMax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class TestCheckFinder
    {
        private static readonly Piece? __ = null;
        private static readonly Piece
            pb = new(true, PieceType.Pawn),
            Rb = new(true, PieceType.Rook),
            Nb = new(true, PieceType.Knight),
            Bb = new(true, PieceType.Bishop),
            Qb = new(true, PieceType.Queen),  
            Kb = new(true, PieceType.King);
        private static readonly Piece
            pw = new(false, PieceType.Pawn),
            Rw = new(false, PieceType.Rook),
            Nw = new(false, PieceType.Knight),
            Bw = new(false, PieceType.Bishop),
            Qw = new(false, PieceType.Queen),  
            Kw = new(false, PieceType.King);
        [TestMethod]
        public void DiagUpRight()
        {
            Assert.IsTrue(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,Bb,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,__,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }
        [TestMethod]
        public void DiagUpRightBlocked()
        {
            Assert.IsFalse(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,__,pb,__,__],//1
                [__,__,__,__,__,Bb,pb,__],//2
                [__,__,__,__,pw,__,__,pb],//3
                [__,__,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }
        [TestMethod]
        public void Up()
        {
            Assert.IsTrue(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,Rb,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,__,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }
        [TestMethod]
        public void UpBlocked()
        {
            Assert.IsFalse(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,Rb,pb,pb,__,__],//1
                [__,__,__,pw,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,__,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }
        [TestMethod]
        public void DiagUpLeft()
        {
            Assert.IsTrue(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [Qb,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,pb,__,__,__,__,pb],//3
                [__,__,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }
        [TestMethod]
        public void DiagUpLeftBlocked()
        {
            Assert.IsFalse(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [Qb,__,__,__,pb,pb,__,__],//1
                [__,Nw,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,__,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }
        [TestMethod]
        public void Left()
        {
            Assert.IsTrue(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [Qb,__,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }
        [TestMethod]
        public void LeftBlocked()
        {
            Assert.IsFalse(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [Qb,Bw,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }
        [TestMethod]
        public void DiagDownLeft()
        {
            Assert.IsTrue(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Bb,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }
        [TestMethod]
        public void Down()
        {
            Assert.IsTrue(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Nb,pw,__,__,__,pw,pw],//6
                [Bb,__,__,Qb,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }
        [TestMethod]
        public void DownBlocked()
        {
            Assert.IsFalse(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Nb,pw,pw,__,__,pw,pw],//6
                [Bb,__,__,Qb,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }

        [TestMethod]
        public void DiagDownRight()
        {
            Assert.IsTrue(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Nb,pw,pw,__,__,pw,pw],//6
                [Bb,__,__,Qb,__,Bw,Bb,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }
        [TestMethod]
        public void DiagDownRightBlocked()
        {
            Assert.IsFalse(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Nb,pw,pw,__,Rw,pw,pw],//6
                [Bb,__,__,Qb,__,Bw,Bb,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }
        [TestMethod]
        public void Right()
        {
            Assert.IsTrue(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,Rb,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Nb,pw,pw,__,__,pw,pw],//6
                [Bb,__,__,Qb,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }
        [TestMethod]
        public void RightBlocked()
        {
            Assert.IsFalse(CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,Rw,__,Rb,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Nb,pw,pw,__,__,pw,pw],//6
                [Bb,__,__,Qb,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])));
        }
    }
}

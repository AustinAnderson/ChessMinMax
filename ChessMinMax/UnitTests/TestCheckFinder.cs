using ChessMinMax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            Assert.AreEqual((2,5),
                CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,Bb,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,__,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Single());
        }
        [TestMethod]
        public void DiagUpRightBlocked()
        {
            Assert.AreEqual(0,
                CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,__,pb,__,__],//1
                [__,__,__,__,__,Bb,pb,__],//2
                [__,__,__,__,pw,__,__,pb],//3
                [__,__,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Count);
        }
        [TestMethod]
        public void Up()
        {
            Assert.AreEqual((1,3),
                CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,Rb,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,__,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Single());
        }
        [TestMethod]
        public void UpBlocked()
        {
            Assert.AreEqual(0,CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,Rb,pb,pb,__,__],//1
                [__,__,__,pw,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,__,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Count);
        }
        [TestMethod]
        public void DiagUpLeft()
        {
            Assert.AreEqual((1,0),
                CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [Qb,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,pb,__,__,__,__,pb],//3
                [__,__,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Single());
        }
        [TestMethod]
        public void DiagUpLeftBlocked()
        {
            Assert.AreEqual(0,CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [Qb,__,__,__,pb,pb,__,__],//1
                [__,Nw,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,__,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Count);
        }
        [TestMethod]
        public void Left()
        {
            Assert.AreEqual((4,0),
                CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [Qb,__,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Single());
        }
        [TestMethod]
        public void LeftBlocked()
        {
            Assert.AreEqual(0,CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [Qb,Bw,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,pw,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Count);
        }
        [TestMethod]
        public void DiagDownLeft()
        {
            Assert.AreEqual((6,1),
                CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Bb,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Single());
        }
        public void DiagDownLeftBlocked()
        {
            Assert.AreEqual(0,
                CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,__,__,__,__],//4
                [__,__,pw,__,__,__,__,__],//5
                [pw,Bb,pw,__,__,__,pw,pw],//6
                [__,__,__,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Count);
        }
        [TestMethod]
        public void Down()
        {
            Assert.AreEqual((7,3),
                CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Nb,pw,__,__,__,pw,pw],//6
                [Bb,__,__,Qb,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Single());
        }
        [TestMethod]
        public void DownBlocked()
        {
            Assert.AreEqual(0,CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Nb,pw,pw,__,__,pw,pw],//6
                [Bb,__,__,Qb,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Count);
        }

        [TestMethod]
        public void DiagDownRight()
        {
            Assert.AreEqual((7,6),
                CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Nb,pw,pw,__,__,pw,pw],//6
                [Bb,__,__,Qb,__,Bw,Bb,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Single());
        }
        [TestMethod]
        public void DiagDownRightBlocked()
        {
            Assert.AreEqual(0,CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Nb,pw,pw,__,Rw,pw,pw],//6
                [Bb,__,__,Qb,__,Bw,Bb,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Count);
        }
        [TestMethod]
        public void Right()
        {
            Assert.AreEqual((4,4),
                CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,Rb,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Nb,pw,pw,__,__,pw,pw],//6
                [Bb,__,__,Qb,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Single());
        }
        [TestMethod]
        public void RightBlocked()
        {
            Assert.AreEqual(0,CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [__,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,Rw,__,Rb,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Nb,pw,pw,__,__,pw,pw],//6
                [Bb,__,__,Qb,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Count);
        }
        [TestMethod]
        public void Multiple()
        {
            var actual = CheckFinder.ChecksSquare(4, 3, true, PackedBoardState.Pack([
                [__,__,__,Kb,__,Bb,__,Rb],//0
                [Qb,__,__,__,pb,pb,__,__],//1
                [__,__,__,Rb,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [__,Bw,__,Kw,__,__,__,__],//4
                [__,__,__,__,__,__,__,__],//5
                [pw,Nb,pw,pw,__,__,pw,pw],//6
                [Bb,__,__,__,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ])).Order().ToArray();
            var expected = (new[] { (1, 0), (2, 3) }).Order().ToArray();
            Assert.AreEqual(expected.Length, actual.Length);
            
            Assert.AreEqual((expected[0], expected[1]), (actual[0], expected[1]));
        }
    }
}

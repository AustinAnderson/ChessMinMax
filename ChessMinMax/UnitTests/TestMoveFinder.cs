using ChessMinMax;

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
            var moves = TestGetLegalMoves(2, 2, state);
            Assert.AreEqual(16, moves.Count);
        }
        [TestMethod]
        public void TestOtherKingInCheck()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [pb,__,Qw,__,__,Nb,pb,__],//2
                [__,pb,__,__,__,Bb,__,pb],//3
                [__,__,__,__,__,Bw,__,__],//4
                [__,__,__,__,pw,Nw,__,__],//5
                [pw,pw,pw,__,__,pw,pw,pw],//6
                [__,__,Kw,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var moves = TestGetLegalMoves(0, 3, state);
            Assert.AreEqual(0, moves.Count);
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
            var moves = TestGetLegalMoves(7, 3, state);
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
            var moves = TestGetLegalMoves(1, 2, state);
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
        public void TestWhitePawnDouble()
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
            var moves = TestGetLegalMoves(6, 1, state);
            Move[] expected = [
                new Move {SourceRow = 6, SourceCol = 1, TargetRow = 5, TargetCol = 1 },
                new Move {SourceRow = 6, SourceCol = 1, TargetRow = 4, TargetCol = 1, DoubleAdvancesPawn = true }
            ];
            AssertUtils.AssertSameMoveList(expected, moves);
        }
        [TestMethod]
        public void TestBlackPawnDouble()
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
            var moves = TestGetLegalMoves(1, 4, state);
            Move[] expected = [
                new Move {SourceRow = 1, SourceCol = 4, TargetRow = 2, TargetCol = 4 },
                new Move {SourceRow = 1, SourceCol = 4, TargetRow = 3, TargetCol = 4, DoubleAdvancesPawn = true }
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
            var moves = TestGetLegalMoves(3, 2, state);
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
            var moves = TestGetLegalMoves(4, 2, state);
            Move[] expected = [
                new Move{ SourceRow = 4, SourceCol = 2, TargetRow = 5, TargetCol = 2},
                new Move{ SourceRow = 4, SourceCol = 2, TargetRow = 5, TargetCol = 1, TakesEnPassant = true}
            ];
            AssertUtils.AssertSameMoveList(expected, moves);
        }
        [TestMethod]
        public void NoMoveCheckProtectedPawn()
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
            var moves = TestGetLegalMoves(7, 2, state);
            Assert.AreEqual(0, moves.Count);

        }
        [TestMethod]
        public void TestNoMovePinPawnTakeLeft()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [pb,__,__,__,__,__,__,__],//2
                [__,pb,__,__,__,__,Bb,__],//3
                [__,__,__,__,__,__,__,__],//4
                [__,__,pb,__,__,__,__,__],//5
                [__,__,__,pw,__,pw,pw,pw],//6
                [__,__,Kw,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var moves = TestGetLegalMoves(6, 3, state);
            Assert.AreEqual(0, moves.Count);
        }
        [TestMethod]
        public void TestNoMovePinPawnTakeRight()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [pb,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [Qb,__,__,__,__,__,__,__],//4
                [__,__,__,pb,__,Nw,__,__],//5
                [__,__,pw,__,__,pw,pw,pw],//6
                [__,__,__,Kw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var moves = TestGetLegalMoves(6, 2, state);
            Assert.AreEqual(0, moves.Count);
        }
        [TestMethod]
        public void TestNoMovePinRook()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [pb,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [Qb,__,__,__,__,__,__,__],//4
                [__,__,__,pb,__,Nw,__,__],//5
                [__,__,Rw,__,__,pw,pw,pw],//6
                [__,__,__,Kw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var moves = TestGetLegalMoves(6, 2, state);
            Assert.AreEqual(0, moves.Count);
        }
        [TestMethod]
        public void TestNoMovePinRookFromTwo()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [pb,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [Qb,__,__,__,__,__,__,__],//4
                [__,Bb,__,pb,__,Nw,__,__],//5
                [__,__,Rw,__,__,pw,pw,pw],//6
                [__,__,__,Kw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var moves = TestGetLegalMoves(6, 2, state);
            Assert.AreEqual(0, moves.Count);
        }
        [TestMethod]
        public void TestOneMovePinnedBishop()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [pb,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [Qb,__,__,__,__,__,__,__],//4
                [__,__,__,__,__,Nw,__,__],//5
                [__,__,Bw,__,__,pw,pw,pw],//6
                [__,__,__,Kw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var moves = TestGetLegalMoves(6, 2, state);
            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(
                ((6,2),(4,0)), 
                ((moves[0].SourceRow, moves[0].SourceCol), (moves[0].TargetRow, moves[0].TargetCol))
            );
        }
        [TestMethod]
        public void TestOneMoveDoublePinnedBishop()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [pb,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [Qb,__,__,__,__,__,__,__],//4
                [__,Bb,__,__,__,Nw,__,__],//5
                [__,__,Bw,__,__,pw,pw,pw],//6
                [__,__,__,Kw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var moves = TestGetLegalMoves(6, 2, state);
            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(
                ((6,2),(5,1)), 
                ((moves[0].SourceRow, moves[0].SourceCol), (moves[0].TargetRow, moves[0].TargetCol))
            );
        }
        [TestMethod]
        public void TestFalsePinBishop()
        {
            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,__,__,pb,pb,__,__],//1
                [pb,__,__,__,__,__,pb,__],//2
                [__,__,__,__,__,__,__,pb],//3
                [Qb,__,__,__,__,__,__,__],//4
                [__,pb,__,__,__,Nw,__,__],//5
                [__,__,Bw,__,__,pw,pw,pw],//6
                [__,__,__,Kw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            var moves = TestGetLegalMoves(6, 2, state);
            Assert.AreEqual(6, moves.Count);
        }
        private static List<Move> TestGetLegalMoves(int row, int col, IConstPackedBoardState state)
        {
            bool black = state[row, col].Black;
            return MoveFinder.GetLegalMoves(row, col, state, AttackLogic.GetPinnedToKing(black, state)).ToList();
        }
    }
}

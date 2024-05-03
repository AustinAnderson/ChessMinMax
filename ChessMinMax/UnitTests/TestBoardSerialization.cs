using ChessMinMax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class TestBoardSerialization
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
        public void TestRoundTrip()
        {

            var state = PackedBoardState.Pack([
                [__,Rb,__,Kb,__,Bb,__,Rb],//0
                [__,__,pw,__,pb,pb,__,__],//1
                [pb,__,Qw,__,__,Nb,pb,__],//2
                [__,pb,__,pb,__,Bb,__,pb],//3
                [__,pw,__,__,__,Bw,__,__],//4
                [__,__,__,__,pw,Nw,__,__],//5
                [pw,__,__,__,__,pw,pw,pw],//6
                [__,__,Kw,Rw,__,Bw,__,Rw],//7
              //  0  1  2  3  4  5  6  7
            ]);
            state.SetPawnDoubleAdvancedLastTurn(1,false,true);
            state.Move(new Move { SourceRow = 7, SourceCol = 2, TargetRow = 7, TargetCol = 1 });
            var serialized = state.Serialize();
            var copy = PackedBoardState.Deserialize(serialized);
            Assert.AreEqual(state.ToString(), copy.ToString());
            Assert.AreEqual(true, copy.QueenCastleUnavailableForPlayer(isBlack: false), "white can't castle queenside");
            Assert.AreEqual(true, copy.KingCastleUnavailableForPlayer(isBlack: false), "white can't castle kingside");
            Assert.AreEqual(true, copy.PawnDoubleAdvancedLastTurn(1, false), "white b pawn doubled advanced last turn");
        }
    }
}

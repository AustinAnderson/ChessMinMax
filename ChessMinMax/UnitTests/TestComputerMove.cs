using ChessMinMax;

namespace UnitTests;
[TestClass]
public class TestComputerMove
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
    public void Test()
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
        var (rBKing,cBKing) = state.GetKingCoords(true);
        var checks = AttackLogic.ThreatensSquare(rBKing, cBKing, false, state);
        GameMinMax.RunAlgo(state, true, 2);

    }

    [TestMethod]
    public void TestTakesFreeBishop()
    {
        var state = PackedBoardState.Pack([
            [Rb,Nb,Bb,Qb,Kb,Bb,Nb,Rb],//0
            [pb,pb,pb,__,pb,__,pb,pb],//1
            [__,__,__,pb,__,pb,__,__],//2
            [__,__,__,__,__,__,Bw,__],//3
            [__,__,__,__,pw,__,__,__],//4
            [__,__,__,pw,__,__,__,__],//5
            [pw,pw,pw,__,__,pw,pw,pw],//6
            [Rw,Nw,__,Qw,Kw,Bw,Nw,Rw],//7
            //0  1  2  3  4  5  6  7
        ]);
        var move = GameMinMax.RunAlgo(state, true, 2);
        Assert.IsNotNull(move);
    }


    [TestMethod]
    public void MustTakeQueen()
    {
        var state = PackedBoardState.Pack([
            [Rb,__,Bb,Qb,Kb,Bb,Nb,Rb],//0
            [pb,pb,pb,Nb,pb,__,__,pb],//1
            [__,__,__,pb,__,pb,Qw,__],//2
            [__,__,__,__,__,__,Bw,__],//3
            [__,__,__,__,pw,__,__,__],//4
            [__,__,__,pw,__,__,__,__],//5
            [pw,pw,pw,__,__,pw,pw,pw],//6
            [Rw,Nw,__,__,Kw,Bw,Nw,Rw],//7
            //0  1  2  3  4  5  6  7
        ]);
        var move = GameMinMax.RunAlgo(state, true, 2);
        Assert.IsNotNull(move);
        var assertionTuple = ((move.SourceRow, move.SourceCol), (move.TargetRow, move.TargetCol));
        Assert.AreEqual(((1, 7), (2, 6)), assertionTuple);
    }

}

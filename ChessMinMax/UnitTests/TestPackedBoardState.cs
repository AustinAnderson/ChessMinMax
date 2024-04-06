using ChessMinMax;

namespace UnitTests
{
    [TestClass]
    public class TestPackedBoardState
    {

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
        [TestMethod]
        public void TestRoundTrip()
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j=0;j< 8; j++)
                {
                    for(PieceType p = PieceType.Empty; p <= PieceType.Pawn; p++)
                    {
                        foreach (var b in new[]{ true, false })
                        {
                            var expected = new Piece(b, p);
                            var state = new PackedBoardState();
                            state[i, j] = expected;
                            var actual = state[i, j];
                            var name = $"[{i},{j}]: Piece(isBlack:{b},PieceType.{p})";
                            Assert.AreEqual(expected.ToString(), actual.ToString(), name);
                        }
                    }
                }
            }
        }
    }
}
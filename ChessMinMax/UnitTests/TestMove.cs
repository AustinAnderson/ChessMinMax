using ChessMinMax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class TestMove
    {
        [TestMethod]
        public void TestSrcDstRoundTrip()
        {
            for(int sr = 0; sr < 8; sr++)
            {
                for(int sc=0; sc < 8; sc++)
                {
                    for(int dr=0; dr < 8; dr++)
                    {
                        for(int dc=0; dc < 8; dc++)
                        {
                            if ((0, 0, 0, 0) == (sr, sc, dr, dc)) continue;
                            var move = new Move();
                            move.SourceRow = sr;
                            move.SourceCol = sc;
                            move.TargetRow = dr;
                            move.TargetCol = dc;
                            Assert.AreEqual((sr, sc, dr, dc), (move.SourceRow, move.SourceCol, move.TargetRow, move.TargetCol),
                                $"({sr},{sc})=>({dr},{dc})"
                            );
                        }
                    }
                }
            }
        }
    }
}

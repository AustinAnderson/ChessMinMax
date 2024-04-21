using ChessMinMax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    internal class AssertUtils
    {
        public static void AssertSameMoveList(IEnumerable<Move> expected, IEnumerable<Move> actual)
        {
            var actList = expected.OrderBy(x => x.GetDebugBitString()).ToList();
            var expectList = actual.OrderBy(x => x.GetDebugBitString()).ToList();
            Assert.AreEqual(actList.Count, expectList.Count, "lists are different lengths");
            for(int i = 0; i < actList.Count; i++)
            {
                Assert.AreEqual(actList[i].GetDebugBitString(), expectList[i].GetDebugBitString(),
                    $"list[{i}], {actList[i]} vs {expectList[i]}"
                );
            }
        }
    }
}

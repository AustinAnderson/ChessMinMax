﻿using ChessMinMax;
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
        public void TestFindCheckMate()
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
        }
        [TestMethod]
        public void TestFindCheckMateDoubleCheck()
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
        }

    }
}

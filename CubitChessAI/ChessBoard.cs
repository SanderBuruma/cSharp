using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubitChessAI
{
    class ChessBoard
    {
        public int[,] ChessPieces { get; set; }
        public bool[] KingMoved { get; set; }
        public bool WhiteToMove { get; set; }

        /**<summary>Initiate the board. No argument means start a new board. </summary>*/
        public ChessBoard()
        {
            ChessPieces = new int[8, 8] {
                {(int)Pieces.rookB, (int)Pieces.kghtB, (int)Pieces.bshpB, (int)Pieces.queeB, (int)Pieces.kingB, (int)Pieces.bshpB, (int)Pieces.kghtB, (int)Pieces.rookB},
                {(int)Pieces.pawnB, (int)Pieces.pawnB, (int)Pieces.pawnB, (int)Pieces.pawnB, (int)Pieces.pawnB, (int)Pieces.pawnB, (int)Pieces.pawnB, (int)Pieces.pawnB},
                {(int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty},
                {(int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty},
                {(int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty},
                {(int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty, (int)Pieces.empty},
                {(int)Pieces.rookW, (int)Pieces.kghtW, (int)Pieces.bshpW, (int)Pieces.queeW, (int)Pieces.kingW, (int)Pieces.bshpW, (int)Pieces.kghtW, (int)Pieces.rookW},
                {(int)Pieces.pawnW, (int)Pieces.pawnW, (int)Pieces.pawnW, (int)Pieces.pawnW, (int)Pieces.pawnW, (int)Pieces.pawnW, (int)Pieces.pawnW, (int)Pieces.pawnW},
            };
            KingMoved = new bool[2]{ false, false };
            WhiteToMove = true;
        }
    }
}

/**<summary>The pieces of the chessboard, enumerated as numbers to make them easy to interpret by the Brain</summary>*/
enum Pieces
{
    empty = 0,

    //white
    pawnW = 1,
    kghtW = 2,
    bshpW = 4,
    rookW = 8,
    queeW = 16,
    kingW = 128,

    //black
    pawnB = -1,
    kghtB = -2,
    bshpB = -4,
    rookB = -8,
    queeB = -16,
    kingB = -128,
}
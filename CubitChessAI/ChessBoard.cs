using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubitChessAI
{
    class ChessBoard
    {
        public Pieces[,] ChessPieces { get; set; }
        public bool[] KingMoved { get; set; }
        public bool WhiteToMove { get; set; }

        /**<summary>Initiate the board. No argument means start a new board. </summary>*/
        public ChessBoard()
        {
            ChessPieces = new Pieces[8, 8] {
                {Pieces.rookB, Pieces.kghtB, Pieces.bshpB, Pieces.queeB, Pieces.kingB, Pieces.bshpB, Pieces.kghtB, Pieces.rookB},
                {Pieces.pawnB, Pieces.pawnB, Pieces.pawnB, Pieces.pawnB, Pieces.pawnB, Pieces.pawnB, Pieces.pawnB, Pieces.pawnB},
                {Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty},
                {Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty},
                {Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty},
                {Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty, Pieces.empty},
                {Pieces.rookW, Pieces.kghtW, Pieces.bshpW, Pieces.queeW, Pieces.kingW, Pieces.bshpW, Pieces.kghtW, Pieces.rookW},
                {Pieces.pawnW, Pieces.pawnW, Pieces.pawnW, Pieces.pawnW, Pieces.pawnW, Pieces.pawnW, Pieces.pawnW, Pieces.pawnW},
            };
            /**<summary>first value is for white, second for black</summary>*/
            KingMoved = new bool[2]{ false, false };
            WhiteToMove = true;
        }

        public IEnumerable<ChessMove> GetOptions()
        {
            IEnumerable<ChessMove> chessMoves = Enumerable.Empty<ChessMove>();

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (WhiteToMove && ChessPieces[x,y] > Pieces.empty)
                    {
                        if (ChessPieces[x, y] == Pieces.pawnW)
                        {
                            chessMoves.Concat<ChessMove>(MovePawn(true,x,y));
                        } else

                        if (ChessPieces[x, y] == Pieces.kghtW)
                        {

                        } else

                        if (ChessPieces[x, y] == Pieces.bshpW)
                        {

                        } else

                        if (ChessPieces[x, y] == Pieces.rookW)
                        {

                        } else

                        if (ChessPieces[x, y] == Pieces.queeW)
                        {

                        } else

                        if (ChessPieces[x, y] == Pieces.kingW)
                        {

                        }
                    }
                    else if (!WhiteToMove && ChessPieces[x, y] < Pieces.empty)
                    {

                    }
                }
            }

            return chessMoves;
        }

        # region pieces moving

        private IEnumerable<ChessMove> MovePawn(bool whiteToMove, int x, int y)
        {
            IEnumerable<ChessMove> chessMoves = Enumerable.Empty<ChessMove>();

            return chessMoves;
        }

        #endregion
    }

    class ChessMovePart
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Pieces Piece { get; set; }

        public ChessMovePart(int x, int y, Pieces piece)
        {
            X = x;
            Y = y;
            Piece = piece;
        }
    }

    class ChessMove
    {
        public IEnumerable<ChessMovePart> Move { get; set; }
        ChessMove()
        {
            Move = Enumerable.Empty<ChessMovePart>();
        }

        public void AddPart(int x, int y, Pieces piece)
        {
            Move.Append<ChessMovePart>(new ChessMovePart(x, y, piece));
        }
    }
}

/**<summary>The pieces of the chessboard, represented as numbers to make them easy to interpret by the Brain</summary>*/
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
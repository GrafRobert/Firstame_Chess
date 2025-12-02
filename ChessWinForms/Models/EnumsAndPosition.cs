
namespace ChessWinForms.Models
{

    public enum PieceColor {
        White ,
        Black
    }

    public enum  PieceType
    {
        King,
        Queen,
        Rook,
        Bishop,
        Knight,
        Pawn
    }

    public class Position
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }
        
            public bool IsValid() => Row >= 0 && Row < 8 && Column >= 0 && Column < 8;

        public override string ToString() => $"{Row}, {Column}";

    }
}


    

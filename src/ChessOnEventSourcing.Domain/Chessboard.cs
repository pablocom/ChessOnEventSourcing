using System.Diagnostics.CodeAnalysis;
using ChessOnEventSourcing.Domain.Exceptions;
using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain;

public sealed class Chessboard : AggregateRoot
{
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? FinishedAt { get; private set; }
    public Colour? Winner { get; private set; }
    
    public IReadOnlyList<IReadOnlyPiece> KilledPieces => _killedPieces.AsReadOnly();

    private Colour _currentTurnColour = Colour.White;
    private readonly List<Piece> _killedPieces = new();
    private readonly Dictionary<Square, Piece> _pieces = new();

    private Chessboard(Guid id, DateTimeOffset createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
        InitializeAllPieces();
        AddEvent(new ChessboardCreated(Id, CreatedAt));
    }

    public static Chessboard Create(Guid id, DateTimeOffset createdAt) => new(id, createdAt);

    private void Finish()
    {
        FinishedAt = DateTimeOffset.Now;
        AddEvent(new ChessboardFinished(Id, FinishedAt.Value));
    }
    
    public static Chessboard From(ChessboardCreated created)
    {
        var chessboard = new Chessboard(created.AggregateId, created.CreatedAt)
        {
            Version = 1
        };

        return chessboard;
    }

    public void Move(Square origin, Square destination)
    {
        if (!_pieces.TryGetValue(origin, out var piece))
            throw new NoPieceFoundAtSquareException(origin);

        if (piece.Colour != _currentTurnColour)
            throw new InvalidMoveException("The piece at this square is from a different colour than the current turn");

        if (!piece.GetAvailableMoves(_pieces).Contains(destination))
            throw new InvalidMoveException("Illegal move");

        if (IsCheckAfterMovingPieceAt(origin))
            throw new InvalidMoveException("Illegal move. That move would check the king");

        if (_pieces.TryGetValue(destination, out var killedPiece))
        {
            _pieces.Remove(killedPiece.Square);
            _killedPieces.Add(killedPiece);
        }

        _pieces.Remove(origin);
        piece.MoveTo(destination);
        _pieces[piece.Square] = piece;
        AddEvent(new PieceMoved(Id, piece.Type, origin.Column.Value, origin.Row.Value, destination.Column.Value, destination.Row.Value));

        if (IsCheckMate())
        {
            Winner = _currentTurnColour;
            Finish();
        }
        else
        {
            SwitchTurn();
        }        
    }

    private bool IsCheckMate()
    {
        var opponentsKing = _pieces.Values.OfType<King>().First(x => x.Colour == _currentTurnColour.Opposite());

        if (!opponentsKing.IsBeingChecked(_pieces))
            return false;

        var opponentPieces = _pieces.Values.Where(p => p.Colour == _currentTurnColour.Opposite());
        foreach (var piece in opponentPieces)
        {
            var possibleMoves = piece.GetAvailableMoves(_pieces);

            foreach (var destination in possibleMoves)
            {
                if (MoveAvoidsCheck(piece, destination))
                    return false;
            }
        }

        return true;
    }

    private bool MoveAvoidsCheck(Piece piece, Square destination)
    {
        var boardCopy = new Dictionary<Square, Piece>(_pieces);
        boardCopy.Remove(piece.Square);
        
        if (boardCopy.TryGetValue(destination, out var targetPiece))
        {
            if (targetPiece.Colour == piece.Colour)
                return false;
            
            boardCopy.Remove(destination);
        }
        
        var movedPiece = piece.CloneWithSquare(destination);
        boardCopy.Add(movedPiece.Square, movedPiece);

        var king = boardCopy.Values.OfType<King>().First(p => p.Colour == _currentTurnColour.Opposite());

        var anyPieceStillCheckingTheKing = boardCopy.Values.Where(p => p.Colour == _currentTurnColour)
            .Any(p => p.GetAvailableMoves(boardCopy).Contains(king.Square));

        return !anyPieceStillCheckingTheKing;
    }

    private bool IsCheckAfterMovingPieceAt(Square origin)
    {
        var boardWithoutThePieceThatIsMoving = new Dictionary<Square, Piece>(_pieces);
        boardWithoutThePieceThatIsMoving.Remove(origin);

        var king = _pieces.Values.First(x => x.Colour == _currentTurnColour && x.Type is PieceType.King);
        var oppositeColourPieces = _pieces.Values.Where(x => x.Colour == _currentTurnColour.Opposite());
        
        foreach (var piece in oppositeColourPieces)
        {
            var availableMoves = piece.GetAvailableMoves(boardWithoutThePieceThatIsMoving);

            if (availableMoves.Contains(king.Square)) 
                return true;
        }

        return false;
    }

    private void SwitchTurn() => _currentTurnColour = _currentTurnColour.Opposite();
    
    public IReadOnlyPiece GetPieceAt(Square square)
    {
        if (TryGetPieceAt(square, out var piece))
            return piece;

        throw new InvalidOperationException($"No piece found at {square}");
    }

    private bool TryGetPieceAt(Square square, [NotNullWhen(true)] out Piece? piece) => _pieces.TryGetValue(square, out piece);

    private void InitializeAllPieces()
    {
        InitializeMajorPieces(Row.One, Colour.White);
        InitializePawns(Row.Two, Colour.White);

        InitializeMajorPieces(Row.Eight, Colour.Black);
        InitializePawns(Row.Seven, Colour.Black);
    }

    private void InitializeMajorPieces(Row row, Colour colour)
    {
        _pieces.Add(Square.At(Column.A, row), new Rook(Square.At(Column.A, row), colour));
        _pieces.Add(Square.At(Column.B, row), new Knight(Square.At(Column.B, row), colour));
        _pieces.Add(Square.At(Column.C, row), new Bishop(Square.At(Column.C, row), colour));
        _pieces.Add(Square.At(Column.D, row), new Queen(Square.At(Column.D, row), colour));
        _pieces.Add(Square.At(Column.E, row), new King(Square.At(Column.E, row), colour));
        _pieces.Add(Square.At(Column.F, row), new Bishop(Square.At(Column.F, row), colour));
        _pieces.Add(Square.At(Column.G, row), new Knight(Square.At(Column.G, row), colour));
        _pieces.Add(Square.At(Column.H, row), new Rook(Square.At(Column.H, row), colour));
    }

    private void InitializePawns(Row row, Colour colour)
    {
        foreach (var column in Column.All)
        {
            _pieces.Add(Square.At(column, row), new Pawn(Square.At(column, row), colour));
        }
    }
    
    public override void Apply(Event @event)
    {
        switch (@event)
        {
            case ChessboardFinished finished:
                Apply(finished);
                break;
            case PieceMoved pieceMoved:
                Apply(pieceMoved);
                break;
        }

        Version++;
    }

    private void Apply(ChessboardFinished finished)
    {
        FinishedAt = finished.FinishedAt;
    }

    private void Apply(PieceMoved pieceMoved)
    {
        var origin = Square.At(pieceMoved.OriginColumn, pieceMoved.OriginRow);
        var destination = Square.At(pieceMoved.DestinationColumn, pieceMoved.DestinationRow);

        Move(origin, destination);
    }
}

using ChessOnEventSourcing.Domain.Events;

namespace ChessOnEventSourcing.Domain;

public sealed class Chessboard : AggregateRoot
{
    public Guid CreatedBy { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? FinishedAt { get; private set; }

    private readonly Colour _currentTurn = Colour.White;

    private readonly Dictionary<Position, Piece> _pieces = new();

    public Chessboard(Guid id, Guid createdBy, DateTimeOffset createdAt)
    {
        Id = id;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        InitializePieces();

        AddEvent(new ChessboardCreated(Id, CreatedBy, CreatedAt));
    }

    private void InitializePieces()
    {
        InitializeMajorPieces(Row.One, Colour.White);
        InitializePawns(Row.Two, Colour.White);

        InitializeMajorPieces(Row.Eight, Colour.Black);
        InitializePawns(Row.Seven, Colour.Black);
    }

    private void InitializeMajorPieces(Row row, Colour colour)
    {
        _pieces.Add(Position.At(Column.A, row), new Rook(Position.At(Column.A, row), colour));
        _pieces.Add(Position.At(Column.B, row), new Knight(Position.At(Column.B, row), colour));
        _pieces.Add(Position.At(Column.C, row), new Bishop(Position.At(Column.C, row), colour));
        _pieces.Add(Position.At(Column.D, row), new Queen(Position.At(Column.D, row), colour));
        _pieces.Add(Position.At(Column.E, row), new King(Position.At(Column.E, row), colour));
        _pieces.Add(Position.At(Column.F, row), new Bishop(Position.At(Column.F, row), colour));
        _pieces.Add(Position.At(Column.G, row), new Knight(Position.At(Column.G, row), colour));
        _pieces.Add(Position.At(Column.H, row), new Rook(Position.At(Column.H, row), colour));
    }

    private void InitializePawns(Row row, Colour colour)
    {
        foreach (var column in Column.All)
        {
            var position = Position.At(column, row);
            _pieces.Add(position, new Pawn(position, colour));
        }
    }
    
    public void Finish()
    {
        FinishedAt = DateTimeOffset.Now;
        
        AddEvent(new ChessboardFinished(Id, FinishedAt.Value));
    }

    public void Move(Position origin, Position destination)
    {
        var piece = _pieces[origin];
        piece.MoveTo(destination);
        
        _pieces.Remove(origin);
        _pieces.Add(destination, piece);
        
        AddEvent(new PieceMoved(Id, piece.Type, origin.Column.Value, origin.Row.Value, destination.Column.Value, destination.Row.Value));
    }

    public static Chessboard From(ChessboardCreated created)
    {
        var chessboard = new Chessboard(created.AggregateId, created.CreatedBy, created.CreatedAt);
        
        chessboard.ClearEvents();
        chessboard.Version = 1;
        return chessboard;
    }

    public override void Apply(Event @event)
    {
        switch (@event)
        {
            case PieceMoved pieceMoved:
                Apply(pieceMoved);
                break;
            case ChessboardFinished finished:
                Apply(finished);
                break;
        }

        Version++;
    }

    private void Apply(PieceMoved pieceMoved)
    {
        var origin = Position.At(new(pieceMoved.OriginColumn), new(pieceMoved.OriginRow));
        var destination = Position.At(new(pieceMoved.DestinationColumn), new(pieceMoved.DestinationRow));
        
        var piece = _pieces[origin];
        piece.MoveTo(destination);
        
        _pieces.Remove(origin);
        _pieces.Add(destination, piece);
    }

    private void Apply(ChessboardFinished finished)
    {
        FinishedAt = finished.FinishedAt;
    }
}

public interface IChessboardRepository
{
    Task<Chessboard?> GetBy(Guid chessboardId);
    Task Save(Chessboard chessboard);
}

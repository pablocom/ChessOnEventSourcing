namespace ChessOnEventSourcing.Domain;

public sealed class Chessboard : AggregateRoot
{
    public Guid CreatedBy { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? FinishedAt { get; private set; }

    private readonly Colour _currentTurn = Colour.White;
    private readonly List<Piece> _pieces = new();

    public Chessboard(Guid id, Guid createdBy, DateTimeOffset createdAt)
    {
        Id = id;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        InitializePieces();

        AddEvent(new ChessboardCreated(Id, CreatedBy, CreatedAt));
    }

    public void Finish()
    {
        FinishedAt = DateTimeOffset.Now;
        
        AddEvent(new ChessboardFinished(Id, FinishedAt.Value));
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
        _pieces.Add(new Rook(new Position(Column.A, row), colour));
        _pieces.Add(new Knight(new Position(Column.B, row), colour));
        _pieces.Add(new Bishop(new Position(Column.C, row), colour));
        _pieces.Add(new Queen(new Position(Column.D, row), colour));
        _pieces.Add(new King(new Position(Column.E, row), colour));
        _pieces.Add(new Bishop(new Position(Column.F, row), colour));
        _pieces.Add(new Knight(new Position(Column.G, row), colour));
        _pieces.Add(new Rook(new Position(Column.H, row), colour));
    }

    private void InitializePawns(Row row, Colour colour)
    {
        foreach (var column in Column.All)
            _pieces.Add(new Pawn(new Position(column, row), colour));
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
        throw new NotImplementedException();
    }

    public void Move(Position from, Position to)
    {
        var piece = _pieces.First(x => from == x.Position);
        piece.MoveTo(to);

        AddEvent(new PieceMoved(Id, piece.Type, from.Column.Value, from.Row.Value, to.Column.Value, to.Row.Value));
    }
}

public interface IChessboardRepository
{
    Task<Chessboard?> GetBy(Guid chessboardId);
    Task Save(Chessboard chessboard);
}

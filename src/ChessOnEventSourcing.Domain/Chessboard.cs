using System.Diagnostics.CodeAnalysis;
using ChessOnEventSourcing.Domain.Exceptions;
using ChessOnEventSourcing.Domain.PieceMoveStrategies;
using ChessOnEventSourcing.Domain.Pieces;
using ChessOnEventSourcing.Domain.Services;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Domain;

public sealed class Chessboard : AggregateRoot
{
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? FinishedAt { get; private set; }
    public Colour? Winner { get; private set; }
    public Colour CurrentTurnColour { get; private set; } = Colour.White;
    public IReadOnlyList<Move> Moves => _moves.AsReadOnly();
    
    internal Dictionary<Square, Piece> Pieces { get; } = new();
    internal List<Piece> KilledPieces { get; } = new();
    
    private readonly List<Move> _moves = [];

    private Chessboard(Guid id, DateTimeOffset createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
        InitializeAllPieces();
        AddEvent(new ChessboardCreated(Id, CreatedAt));
    }

    public static Chessboard Create(Guid id, DateTimeOffset createdAt) => new(id, createdAt);
    
    public IReadOnlyList<IReadOnlyPiece> GetKilledPieces() => KilledPieces.AsReadOnly();

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

    public void MovePiece(Square origin, Square destination)
    {
        if (!Pieces.TryGetValue(origin, out var piece))
            throw new NoPieceFoundAtSquareException(origin);

        if (piece.Colour != CurrentTurnColour)
            throw new InvalidMoveException("The piece at this square is from a different colour than the current turn");
        
        var moveStrategy = PieceMoveStrategySelector.GetMoveStrategy(this, origin, destination);
        
        if (moveStrategy.IsValidMove())
            throw new InvalidMoveException("Illegal move");
        
        moveStrategy.Execute();
        
        _moves.Add(new Move(piece.Type, piece.Colour, origin, destination));
        AddEvent(new PieceMoved(Id, piece.Type, origin.Column.Value, origin.Row.Value, destination.Column.Value, destination.Row.Value));

        if (IsCheckMate())
        {
            Winner = CurrentTurnColour;
            Finish();
        }
        else
        {
            SwitchTurn();
        }        
    }

    private bool IsCheckMate()
    {
        if (!CheckFinder.IsCheckFrom(CurrentTurnColour, Pieces))
            return false;

        var opponentPieces = Pieces.Values.Where(p => p.Colour == CurrentTurnColour.Opposite());
        foreach (var piece in opponentPieces)
        {
            var possibleMoves = piece.GetAvailableMoves(Pieces);

            if (possibleMoves.Any(destination => MoveAvoidsCheck(piece, destination)))
                return false;
        }

        return true;
    }

    private bool MoveAvoidsCheck(Piece piece, Square destination)
    {
        var boardCopy = new Dictionary<Square, Piece>(Pieces);
        boardCopy.Remove(piece.Square);
        
        if (boardCopy.TryGetValue(destination, out var targetPiece))
        {
            if (targetPiece.Colour == piece.Colour)
                return false;
            
            boardCopy.Remove(destination);
        }
        
        var movedPiece = piece.CloneWithSquare(destination);
        boardCopy.Add(movedPiece.Square, movedPiece);
        
        return !CheckFinder.IsCheckFrom(CurrentTurnColour, boardCopy);
    }

    private void SwitchTurn() => CurrentTurnColour = CurrentTurnColour.Opposite();
    
    public IReadOnlyPiece GetPieceAt(Square square)
    {
        if (TryGetPieceAt(square, out var piece))
            return piece;

        throw new InvalidOperationException($"No piece found at {square}");
    }

    public bool TryGetPieceAt(Square square, [NotNullWhen(true)] out Piece? piece) => Pieces.TryGetValue(square, out piece);

    private void InitializeAllPieces()
    {
        InitializeMajorPieces(Row.One, Colour.White);
        InitializePawns(Row.Two, Colour.White);

        InitializeMajorPieces(Row.Eight, Colour.Black);
        InitializePawns(Row.Seven, Colour.Black);
    }

    private void InitializeMajorPieces(Row row, Colour colour)
    {
        Pieces.Add(Square.At(Column.A, row), new Rook(Square.At(Column.A, row), colour));
        Pieces.Add(Square.At(Column.B, row), new Knight(Square.At(Column.B, row), colour));
        Pieces.Add(Square.At(Column.C, row), new Bishop(Square.At(Column.C, row), colour));
        Pieces.Add(Square.At(Column.D, row), new Queen(Square.At(Column.D, row), colour));
        Pieces.Add(Square.At(Column.E, row), new King(Square.At(Column.E, row), colour));
        Pieces.Add(Square.At(Column.F, row), new Bishop(Square.At(Column.F, row), colour));
        Pieces.Add(Square.At(Column.G, row), new Knight(Square.At(Column.G, row), colour));
        Pieces.Add(Square.At(Column.H, row), new Rook(Square.At(Column.H, row), colour));
    }

    private void InitializePawns(Row row, Colour colour)
    {
        foreach (var column in Column.All)
        {
            Pieces.Add(Square.At(column, row), new Pawn(Square.At(column, row), colour));
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

        MovePiece(origin, destination);
    }
}

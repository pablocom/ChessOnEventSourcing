using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.Domain.ValueObjects;

namespace ChessOnEventSourcing.Application;

public sealed class ChessboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChessboardRepository _chessboards;

    public ChessboardService(IUnitOfWork unitOfWork, IChessboardRepository chessboards)
    {
        _unitOfWork = unitOfWork;
        _chessboards = chessboards;
    }

    public async Task CreateChessboard(Guid id)
    {
        try
        {
            await _unitOfWork.BeginTransaction();

            var chessboard = Chessboard.Create(id, Guid.NewGuid(), DateTimeOffset.Now);
        
            await _chessboards.Save(chessboard);
            await _unitOfWork.Commit();
        }
        catch (Exception)
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
    
    public async Task Finish(Guid id)
    {
        try
        {
            await _unitOfWork.BeginTransaction();

            var chessboard = await _chessboards.GetBy(id);
            chessboard!.Finish();
            await _chessboards.Save(chessboard);
            
            await _unitOfWork.Commit();
        }
        catch (Exception)
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task Move(Guid id)
    {
        try
        {
            await _unitOfWork.BeginTransaction();

            var chessboard = await _chessboards.GetBy(id);

            chessboard!.Move(origin: Square.At(Column.D, Row.Two), destination: Square.At(Column.D, Row.Four));
            
            await _chessboards.Save(chessboard);
            await _unitOfWork.Commit();
        }
        catch (Exception)
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

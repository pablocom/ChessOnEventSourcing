using ChessOnEventSourcing.Domain;

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

            var chessboard = new Chessboard(id, Guid.NewGuid(), DateTimeOffset.Now);
        
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

            chessboard!.Move(Position.At(Column.D, Row.Two), Position.At(Column.D, Row.Four));
            
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

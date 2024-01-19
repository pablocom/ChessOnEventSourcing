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

    public async Task CreateChessboard()
    {
        await _unitOfWork.BeginTransaction();

        var chessboard = new Chessboard(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now);
        
        await _chessboards.Save(chessboard);
        await _unitOfWork.Commit();
    }
}

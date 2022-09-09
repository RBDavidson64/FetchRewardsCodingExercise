using FetchRewardsApi.Interfaces;
using FetchRewardsApi.Repositories;

namespace FetchRewardsApi.Commands;

internal class SpendPointsCommand
    : ISpendPoints
{
    private readonly IPointRepository     _pointRepository;
    private readonly ILogger<SpendPointsCommand> _logger;
    public SpendPointsCommand(IPointRepository pointRepository, ILogger<SpendPointsCommand> logger)
    {
        _pointRepository = pointRepository;
        _logger     = logger;
    }
}
using FetchRewardsApi.Interfaces;
using FetchRewardsApi.Repositories;

namespace FetchRewardsApi.Commands;

internal class AddPointsCommand : IAddPoints
{
    private readonly IPointRepository   _pointRepository;
    private readonly ILogger<AddPointsCommand> _logger;

    public AddPointsCommand(IPointRepository pointRepository, ILogger<AddPointsCommand> logger)
    {
        _pointRepository = pointRepository;
        _logger     = logger;
    }
}
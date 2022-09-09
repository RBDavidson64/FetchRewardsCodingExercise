using FetchRewardsApi.Interfaces;
using FetchRewardsApi.Records;
using FetchRewardsApi.Repositories;

namespace FetchRewardsApi.Commands;

internal class GetBalancesCommand
    : IGetBalances
{
    private readonly IPointRepository     _pointRepository;
    public GetBalancesCommand(IPointRepository pointRepository)
    {
        _pointRepository = pointRepository;
    }

    public async Task<string> GetBalances()
    {
        await Task.Yield();
        return string.Empty;
        //IEnumerable<Balance> balances =  _pointRepository.GetBalancesCommand();

    }
}
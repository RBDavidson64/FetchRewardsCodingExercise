namespace FetchRewardsApi.Interfaces;

public interface IGetBalances
{
    Task<string> GetBalances();
}
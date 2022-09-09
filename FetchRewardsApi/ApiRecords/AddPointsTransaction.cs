namespace FetchRewardsApi.Records;

public record AddPointsTransaction(string Payer, int Points, DateTime TransactionDateTime);
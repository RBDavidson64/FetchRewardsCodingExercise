using System.Runtime.CompilerServices;
using FetchRewardsApi.Commands;
using FetchRewardsApi.Interfaces;
using FetchRewardsApi.Records;
using FetchRewardsApi.Repositories;

[assembly: InternalsVisibleTo("UnitTests")]

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IPointRepository, PointRepository>();
builder.Services.AddTransient<IAddPoints, AddPointsCommand>();
builder.Services.AddTransient<ISpendPoints, SpendPointsCommand>();
builder.Services.AddTransient<IGetBalances, GetBalancesCommand>();
var app = builder.Build();
app.MapPost("/add", (IAddPoints addPoints, AddPointsTransaction addPointsTransaction) => "Hello World!");
app.MapPost("/spend", (ISpendPoints spendPoints, SpendPointsTransaction spendPointsTransaction) => "Hello World!");
app.MapGet("/balance", (IGetBalances getBalances) => getBalances.GetBalances());
app.Run();

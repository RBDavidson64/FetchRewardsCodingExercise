using System.Runtime.CompilerServices;
using FetchRewardsApi.ApiRecords;
using FetchRewardsApi.Commands;
using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;
using FetchRewardsApi.Repositories;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("UnitTests")]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PointsContext>(dbContextOptionsBuilder => 
                                                 dbContextOptionsBuilder.UseInMemoryDatabase(nameof(PointsContext)));

builder.Services
        // Repositories
       .AddScoped<IAllocatedPointsRepository, AllocatedPointsRepository>()
       .AddScoped<IAvailablePointsRepository, AvailablePointsRepository>()
       .AddScoped<IPayerBalanceRepository, PayerBalanceRepository>()
       .AddScoped<IPointTransactionRepository, PointTransactionRepository>()
       .AddScoped<ISpentPointsRepository, SpentPointsRepository>()
        // public API commands
       .AddScoped<IAddPoints, AddPointsCommand>()
       .AddScoped<IGetBalances, GetBalancesCommand>()
       .AddScoped<ISpendPoints, SpendPointsCommand>()
        // internal api commands
       .AddScoped<IAdjustPayerPoints, AdjustPayerPointsCommand>()
       .AddScoped<IAllocateAvailablePoints, AllocateAvailablePointsCommand>()
       .AddScoped<IAllocatePoints, AllocatePointsCommand>()
       .AddScoped<ICreateAvailablePoints, CreateAvailablePointsCommand>()
       .AddScoped<ICreatePointTransaction, CreatePointTransactionCommand>()
       .AddScoped<ICreateSpentPoints, CreateSpentPointsCommand>()
        // other
       .AddTransient<ISpendPointsResults>(_=> new SpendPointsResults())
    ;

var app = builder.Build();
app.MapPost("/add",   async (IAddPoints   addPoints,   AddPointsTransaction   addPointsTransaction,   CancellationToken cancellationToken) => await addPoints.AddPoints(addPointsTransaction, cancellationToken).ConfigureAwait(false));
app.MapPost("/spend", async (ISpendPoints spendPoints, SpendPointsTransaction spendPointsTransaction, CancellationToken cancellationToken) => await spendPoints.SpendPoints(spendPointsTransaction, cancellationToken).ConfigureAwait(false));
app.MapGet("/balance", async ( IGetBalances getBalances, CancellationToken cancellationToken) => await getBalances.GetBalances(cancellationToken).ConfigureAwait(false));
app.Run();

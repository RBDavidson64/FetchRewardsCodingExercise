using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FetchRewardsApi.Repositories;

/// <summary>
/// A repository used to add new <see cref="SpentPoints"/> entity records.
/// </summary>
internal sealed class SpentPointsRepository
    : ISpentPointsRepository
{
    private readonly GenericRepository<SpentPoints> _repository;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> used to read, add, and update entity records.</param>
    public SpentPointsRepository(PointsContext context) => _repository = new GenericRepository<SpentPoints>(context);

    /// <inheritdoc />
    public void Add(SpentPoints                spentPoints) => _repository.Add(spentPoints);
}
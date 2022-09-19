using FetchRewardsApi.EntityModel;
using FetchRewardsApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FetchRewardsApi.Repositories;

/// <summary>
/// A repository used to add new <see cref="AllocatedPoints"/> entity records.
/// </summary>
internal sealed class AllocatedPointsRepository
    : IAllocatedPointsRepository
{
    private readonly GenericRepository<AllocatedPoints> _repository;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> used to read, add, and update entity records.</param>
    public AllocatedPointsRepository(PointsContext context) => _repository             = new GenericRepository<AllocatedPoints>(context);

    /// <inheritdoc />
    public void Add(AllocatedPoints                allocatedPoints) => _repository.Add(allocatedPoints);
}
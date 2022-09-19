using System.Text.Json;
using FetchRewardsApi.EntityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests.TestHelpers;

/// <summary>
/// Methods used by many tests in different test classes.
/// </summary>
internal static class HelperMethods
{
    /// <summary>
    ///     Creates a new <see cref="PointsContext" /> instance using an in memory <see cref="DbContext" />
    /// </summary>
    /// <param name="uniqueName">
    ///     If not null or whitespace, the name which is assigned to the in memory <see cref="DbContext" />
    ///     <value> If <paramref name="uniqueName" /> is null, a random name will be used. </value>
    ///     <remarks>
    ///         If <see cref="CreatePointsContext" /> is called multiple times and passed the same non-null, non-empty-space
    ///         name, then the same <see cref="PointsContext" /> will be returned each time, and will include any entity records
    ///         added or updated using that context.
    /// 
    ///         If different names are used when used when creating the <see cref="PointsContext" />, then each context is distinct;
    ///         meaning the returned <see cref="PointsContext" /> will have no entity records.
    ///     </remarks>
    /// </param>
    /// <returns>A <see cref="PointsContext" /> instance.</returns>
    internal static PointsContext CreatePointsContext(string? uniqueName = null)
    {
        uniqueName = string.IsNullOrWhiteSpace(uniqueName) ? Path.GetRandomFileName() : uniqueName;
        var dbContextOptionsBuilder = new DbContextOptionsBuilder();
        dbContextOptionsBuilder.UseInMemoryDatabase(uniqueName);
        var dbContextOptions = dbContextOptionsBuilder.Options;
        return new PointsContext(dbContextOptions);
    }

    /// <summary>
    ///     Extracts the value and status code inside an <see cref="IResult" />. The value is converted to type <typeparamref name="T" />
    /// </summary>
    /// <param name="result">The <see cref="IResult" /> whose value will be converted.</param>
    /// <typeparam name="T">The expected type of data contained in the <see cref="IResult" /> as Json</typeparam>
    /// <returns>A <see cref="Tuple"/> with the converted value and <see cref="int"/> status code.</returns>
    internal static async Task<(T? Value, int StatusCode)> GetResponseValue<T>(IResult result)
    {
        var mockHttpContext = new DefaultHttpContext
                              {
                                  // RequestServices needs to be set so the IResult implementation can log.
                                  RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider(),
                                  Response =
                                  {
                                      // The default response body is Stream.Null which throws away anything that is written to it.
                                      Body = new MemoryStream()
                                  }
                              };

        await result.ExecuteAsync(mockHttpContext);
        var statusCode = mockHttpContext.Response.StatusCode;
        // Reset MemoryStream to start so we can read the response.
        mockHttpContext.Response.Body.Position = 0;
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var value       = await JsonSerializer.DeserializeAsync<T>(mockHttpContext.Response.Body, jsonOptions).ConfigureAwait(false);
        return (value, statusCode);
    }

    /// <summary>
    ///     Extracts the status code in an <see cref="IResult" />
    /// </summary>
    /// <param name="result">The <see cref="IResult" /> to process.</param>
    /// <returns>The <see cref="int"/> status code</returns>
    internal static async Task<int> GetStatusCode(IResult result)
    {
        var mockHttpContext = new DefaultHttpContext
                              {
                                  // RequestServices needs to be set so the IResult implementation can log.
                                  RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider(),
                                  Response =
                                  {
                                      // The default response body is Stream.Null which throws away anything that is written to it.
                                      Body = new MemoryStream()
                                  }
                              };

        await result.ExecuteAsync(mockHttpContext);
        return mockHttpContext.Response.StatusCode;
    }

    /// <summary>
    ///     Validates that an <see cref="IResult" /> is an <see cref="OkResult" />
    /// </summary>
    /// <param name="result">The <see cref="IResult" /> to be validated.</param>
    public static async Task Validate_OkResult(IResult result)
    {
        var statusCode = await GetStatusCode(result).ConfigureAwait(false);
        Assert.Equal(StatusCodes.Status200OK, statusCode);
    }

    /// <summary>
    ///     Validates an <see cref="IResult" /> contains a <see cref="ProblemDetails" /> for an  <see cref="StatusCodes.Status422UnprocessableEntity" /> error with a specific message.
    /// </summary>
    /// <param name="result">The <see cref="IResult" /> to process</param>
    /// <param name="expectedErrorMessage">
    ///     The expected <see cref="string" /> error message.
    /// </param>
    public static async Task Validate_UnprocessableEntityObjectResult(IResult result, string expectedErrorMessage)
    {
        var (problemDetails, statusCode) = await GetResponseValue<ProblemDetails>(result).ConfigureAwait(false);
        Assert.NotNull(problemDetails);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, problemDetails.Status);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, statusCode);
        Assert.Equal(expectedErrorMessage,                     problemDetails.Detail);
    }

    /// <summary>
    ///     Validates that the <see cref="PointsContext.PayerBalances" /> has the correct records and <see cref="PayerBalance.Balance" />,
    ///     and that the sum of the related <see cref="AvailablePoints.UnallocatedPoints" /> equals the <see cref="PayerBalance.Balance"/>.
    /// </summary>
    /// <param name="context">The <see cref="PointsContext" /> to validate.</param>
    /// <param name="expectedPayerBalances">An dictionary with payer names as keys and expected balances as values.</param>
    public static void ValidatePayerBalances(PointsContext context, IDictionary<string, int> expectedPayerBalances)
    {
        Assert.Equal(expectedPayerBalances.Count, context.PayerBalances.Count());

        foreach (var (key, expectedBalance) in expectedPayerBalances)
        {
            var payerBalance = context.PayerBalances.SingleOrDefault(x => x.Payer == key);
            Assert.NotNull(payerBalance);
            Assert.Equal(expectedBalance, payerBalance.Balance);

            var availablePointsByPayer = context.AvailablePointsSet.Where(x => x.Payer == key).Sum(x => x.UnallocatedPoints);
            Assert.Equal(expectedBalance, availablePointsByPayer);

            var availablePointsByNavigation = payerBalance.AvailablePoints.Sum(x => x.UnallocatedPoints);
            Assert.Equal(expectedBalance, availablePointsByNavigation);
        }
    }
}
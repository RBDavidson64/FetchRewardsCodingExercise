namespace UnitTests.TestHelpers;

/// <summary>
/// A summary of how many records were processed and how many failed.
/// </summary>
/// <param name="RecordsProcessed">The total number of records processed</param>
/// <param name="FailureCount">The number of records which were rejected during processing.</param>
public record struct RecordsProcessedAndFailureCount(int RecordsProcessed, int FailureCount);
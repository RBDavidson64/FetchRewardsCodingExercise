using FetchRewardsApi.Interfaces;
using FetchRewardsApi.Records;

namespace FetchRewardsApi.Repositories;

public class PointRepository : IPointRepository
{
    // The original datetime stamped transactions which adjust the available points.
    // When spending, Use TryPeek/Dequeue to remove elements until you have enough to cover the spent points.  
    private PriorityQueue<AddPointsTransaction,DateTime> _transactions;
    
    // use another list to keep track of the original transaction and the spent points applied against it.
    
    // need a rolling total that accounts for -points in transactions.
    // As points are applied against transations the amount of applied points must be accounted for.
    //
    /*
     * their example
     *  ● { "payer": "DANNON", "points": 1000, "timestamp": "2020-11-02T14:00:00Z" }
     *  ● { "payer": "UNILEVER", "points": 200, "timestamp": "2020-10-31T11:00:00Z" }
     *  ● { "payer": "DANNON", "points": -200, "timestamp": "2020-10-31T15:00:00Z" }
     *  ● { "payer": "MILLER COORS", "points": 10000, "timestamp": "2020-11-01T14:00:00Z" }
     *  ● { "payer": "DANNON", "points": 300, "timestamp": "2020-10-31T10:00:00Z" }
     *
     * when sorted becomes
     *
     *  ● { "payer": "DANNON",          "points": 300,      "timestamp": "2020-10-31T10:00:00Z" }
     *  ● { "payer": "UNILEVER",        "points": 200,      "timestamp": "2020-10-31T11:00:00Z" }
     *  ● { "payer": "DANNON",          "points": -200,     "timestamp": "2020-10-31T15:00:00Z" }
     *  ● { "payer": "MILLER COORS",    "points": 10000,    "timestamp": "2020-11-01T14:00:00Z" }
     *  ● { "payer": "DANNON",          "points": 1000,     "timestamp": "2020-11-02T14:00:00Z" }
     *
     * when { "points": 5000 } comes through you must add up all the points to reach 5000 or more
     *
     *  ● { "payer": "DANNON",          "points": 300,      "timestamp": "2020-10-31T10:00:00Z" } = +300
     *  ● { "payer": "UNILEVER",        "points": 200,      "timestamp": "2020-10-31T11:00:00Z" } = +500
     *  ● { "payer": "DANNON",          "points": -200,     "timestamp": "2020-10-31T15:00:00Z" } = +300
     *  ● { "payer": "MILLER COORS",    "points": 10000,    "timestamp": "2020-11-01T14:00:00Z" } = +10300
     *
     * Then account for the fact that DANNON has a -200 transaction, and can only allow 100 points to be applied.
     * i.e. convet the list to something like
     *  ● { "payer": "DANNON",          "Available points": 100,      "timestamp": "2020-10-31T10:00:00Z" } = +100
     *  ● { "payer": "UNILEVER",        "Available points": 200,      "timestamp": "2020-10-31T11:00:00Z" } = +300
     *  ● { "payer": "MILLER COORS",    "Available points": 10000,    "timestamp": "2020-11-01T14:00:00Z" } = +10300
     *
     * then after applying the points to something like
     *  ● { "payer": "DANNON",          "Remaining points": 0,      "timestamp": "2020-10-31T10:00:00Z" } 
     *  ● { "payer": "UNILEVER",        "Remaining points": 0,      "timestamp": "2020-10-31T11:00:00Z" } 
     *  ● { "payer": "MILLER COORS",    "Remaining points": 5300,    "timestamp": "2020-11-01T14:00:00Z" }
     *
     * Note the the { "payer": "DANNON",          "points": 1000,     "timestamp": "2020-11-02T14:00:00Z" } transaction
     * was unaffected by this sequence.
     */
}
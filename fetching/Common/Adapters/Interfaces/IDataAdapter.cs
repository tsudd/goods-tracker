using GoodsTracker.DataCollector.Common.Trackers;
using GoodsTracker.DataCollector.Common.Trackers.Interfaces;

namespace GoodsTracker.DataCollector.Common.Adapters;
public interface IDataAdapter
{
    void SaveItems(IItemTracker tracker, IEnumerable<string> shopIds);
}
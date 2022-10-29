using Common.Trackers;
using Common.Trackers.Interfaces;

namespace Common.Adapters;
public interface IDataAdapter
{
    void SaveItems(IItemTracker tracker, IEnumerable<string> shopIds);
}
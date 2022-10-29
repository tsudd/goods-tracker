using Models;
using Models.Constants;

namespace Common.Mapers.Interfaces;
public interface IItemMapper
{
    const string CATEGORIES_SEPARATOR = ",";
    Item MapItemFields(Dictionary<ItemFields, string> fields);
}
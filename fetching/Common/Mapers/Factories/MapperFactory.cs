using Common.Mapers.Implementations;
using Common.Mapers.Interfaces;

namespace Common.Mapers.Factories;
public partial class MapperFactory
{
    protected static MapperFactory? _instance;
    private MapperFactory() { }

    public static MapperFactory GetInstance()
    {
        if (_instance is null)
        {
            _instance = new MapperFactory();
        }
        return _instance;
    }

    public IItemMapper CreateMapper(string mapper)
    {
        switch (mapper)
        {
            case nameof(BasicMapper):
                return new BasicMapper();
            default:
                throw new ArgumentException($"couldn't create {mapper}: no such mapper in the app.");
        }
    }

}
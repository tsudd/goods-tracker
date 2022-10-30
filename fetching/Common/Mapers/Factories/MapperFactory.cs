using Common.Mapers.Implementations;
using Common.Mapers.Interfaces;

namespace Common.Mapers.Factories;
public partial class MapperFactory
{
    protected static MapperFactory? _instance;
    private MapperFactory() { }

    protected static MapperFactory GetInstance()
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

    public static MapperFactory GetSpecifiedFactory(string typeName)
    {
        try
        {
            return (MapperFactory)(Type.GetType(typeName)?.GetMethod("GetInstance")?.Invoke(null, null)
            ?? throw new ArgumentException("No such tracker factory in the app"));
        }
        catch (ArgumentException ex)
        {
            throw ex;
        }
    }

}
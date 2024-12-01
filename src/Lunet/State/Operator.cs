namespace Lunet.State;

struct Operator
{
    public Func<long, long, long> integerFunc;
    public Func<double, double, double> floatFunc;
}
namespace Consolio
{
    public readonly record struct GameFactory(string Name, Func<Game> Factory);
}
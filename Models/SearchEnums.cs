namespace VideoHallen.Models;

[Flags]
public enum RentableFlag
{
    Out = 1,
    In = 2,
    Damaged = 4,
    Destroyed = 8,
    Reserved = 16,
    All = Out | In | Damaged | Destroyed | Reserved,
}
[Flags]
public enum RentableType
{
    Movie = 1,
    Game = 2,
    RentConsole = 4,
    All = Movie | Game | RentConsole,
}
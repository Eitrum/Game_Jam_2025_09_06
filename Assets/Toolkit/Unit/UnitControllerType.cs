namespace Toolkit.Unit {
    [System.Flags]
    public enum UnitControllerType {
        None,
        Player = 1,
        AI = 2,
        Local = 4,
        Network = 8,
        Opponent = 16,
        Ally = 32,
    }
}

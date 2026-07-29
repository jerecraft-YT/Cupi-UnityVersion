public enum DireccionesMovimientoNotas
{
    None = 5,
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
    Custom = 4
}

public enum CorrespondenciaTecla
{
    None = 10,
    One = 0,
    Two = 1,
    Three = 2,
    Four = 3,
    Five = 4,
    Six = 5,
    Seven = 6,
    Eight = 7,
    Nine = 8,
    Ten = 9
}

public enum ModoJuego
{
    None,
    Tile,
    Radial
}

public enum ModoInput
{
    None,
    Player,
    Bot,
    Custom
}

public enum ModoTime
{
    None,
    Global,
    Custom
}

public enum EstadoNota
{
    None,
    EnProceso,
    Presionada,
    Fallada,
    Procesada
}

public enum EstadoPuntuacion
{
    None,
    Fallaste,
    EnProceso,
    Pesimo,
    Malo,
    Bueno,
    Perfecto
}

public enum TipoNota
{
    None = 2,
    Normal = 0,
    Sostenida = 1
}
public enum ModoNota
{
    None = 2,
    Tile = 0,
    Radial = 1
}

public enum TipoObjetoPool
{
    None = 2,
    NotaNormalTile = 0,
    NotaSostenidaTile = 1,
    NotaNormalRadial = 3,
    NotaSostenidaRadial = 4,
}


// por si algun especialito agrega un valor en medio (osea yo :P)
public enum TileModePlayStyle
{
    None = 10,
    OneKey = 0,
    TwoKeys = 1,
    ThreeKeys = 2,
    FourKeys = 3,
    FiveKeys = 4,
    SixKeys = 5,
    SevenKeys = 6,
    EightKeys = 7,
    NineKeys = 8,
    TenKeys = 9
}

/*
[Flags]
//<< mueve los bits a un lado
//deben ser multiplos de 2 para seleccionar varios a la vez sin problemas
public enum Something
{
    None = 0 << 0,// 0000000
    SuddenDeath = 1 << 1,// 0000010 == 2
    Multiplier = 2 << 2,//0001000 == 4
    abc = 3 << 3,
    bca = 4 << 4,
    aaa = 5 << 5
}
*/
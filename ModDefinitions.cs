namespace LosDropV;

public record ModInfo(string Name, string Url, string FileName);

public static class ModDefinitions
{
    public static readonly ModInfo Menyoo = new(
        Name: "Menyoo PC",
        Url: "https://github.com/MAFINS/MenyooSP/releases/download/v1.0.0/MenyooRelease.rar",
        FileName: "MenyooRelease.rar"
    );

    public static readonly ModInfo SimpleZombies = new(
        Name: "Simple Zombies",
        Url: "https://files.gta5-mods.com/uploads/simple-zombies/a98c4a-ZombiesMod%20(1.0.2d).zip",
        FileName: "SimpleZombies.zip"
    );
}

# LosDropV ⚡

**LosDropV** is a high-performance, CLI-driven deployment tool for GTA V mods. Built for speed and portability, it allows users to install complex mods like Menyoo and Simple Zombies with a single command.

## 🚀 Features

- **Blazing Fast Deployment**: Optimized multi-threaded downloads and extraction.
- **Cool CLI Aesthetic**: Modern terminal interface with cyber-inspired visuals.
- **Zero Dependencies**: Self-contained executable, no .NET runtime required.
- **Portable**: Run from anywhere, no installation needed.

## 📦 Building

To build the portable version:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The executable will be located in `./bin/Release/net8.0/win-x64/publish/LosDropV.exe`.

## 🛠️ Usage

1. Launch `LosDropV.exe`.
2. The tool will automatically detect your GTA V installation.
3. Select the mod package you wish to deploy.
4. Enjoy your modded game!

---
*Powered by LosDropV Architecture*

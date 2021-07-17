# RPG-Patcher
An in-development TUI application using .NET Core for managing RPG Maker XP/VX/VX Ace projects and archives.

The application is more like a compilation of more tools into one application.


## External Resources
Uses a .NET Core port of [uuksu's RPGMakerDecrypter](https://github.com/uuksu/RPGMakerDecrypter/) which was compiled to a DLL, and [migueldeicaza's gui.cs](https://github.com/migueldeicaza/gui.cs) which was added via NuGet.

## Compilation
### External Resources
* [RPGMakerDecrypter](https://github.com/uuksu/RPGMakerDecrypter/)
  - Was converted to .NET Core 2.1 LTS
  - Converted project was named `RPGMakerDecrypter.Decrypter.Core`

* [RM2k2XP.NETCore](https://github.com/xubiod/RM2k2XP.NETCore)
  - Was forked from [uuksu's RM2k2XP](https://github.com/uuksu/RM2k2XP) to add a .NET Core 3.1 project
  - Uses `RM2k2XP.NETCore.Converters`

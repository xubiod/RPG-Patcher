# RPG-Patcher
An in-development TUI application using .NET Core for managing RPG Maker XP/VX/VX Ace projects and mods. 


## External Resources
Uses a .NET Core port of [uuksu's RPGMakerDecrypter](https://github.com/uuksu/RPGMakerDecrypter/) which was compiled to a DLL, and [migueldeicaza's gui.cs](https://github.com/migueldeicaza/gui.cs) which was added via NuGet.

## Compilation
### External Resources
* [RPGMakerDecrypter](https://github.com/uuksu/RPGMakerDecrypter/)
  - Was converted to .NET Core 2.1 LTS
  - Converted project was named `RPGMakerDecrypter.Decrypter.Core`
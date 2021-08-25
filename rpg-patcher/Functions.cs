using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using RM2k2XP.Converters;
using RM2k2XP.Converters.Formats;
using RPGMakerDecrypter.Decrypter;
using Terminal.Gui;
using TGAttribute = Terminal.Gui.Attribute;

namespace rpg_patcher
{
    internal static class Functions
    {
        public const string RndString = "01990849ca73d97c358aa63409043a93";

        public static class Checks
        {
            public class Installed
            {
                public static bool Rm2K2003 { get; protected set; }
                public static bool Rmxp { get; protected set; }
                public static bool Rmvx { get; protected set; }
                public static bool RmvxAce { get; protected set; }
                public static bool Rmmv { get; protected set; }
                public static bool Rmmz { get; protected set; }

                public static void Set(bool xp = false, bool vx = false, bool vxace = false, bool mv = false, bool mz = false, bool rm2K2003 = false)
                {
                    Rmxp = xp; Rmvx = vx; RmvxAce = vxace; Rmmv = mv; Rmmz = mz; Rm2K2003 = rm2K2003;
                }
            }

            // https://stackoverflow.com/questions/16379143/check-if-application-is-installed-in-registry
            private static bool CheckInstalled(string cName)
            {
                string displayName;

                string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);
                if (key != null)
                {
                    foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                    {
                        displayName = subkey.GetValue("DisplayName") as string;
                        if (displayName != null && displayName.Contains(cName))
                        {
                            return true;
                        }
                    }
                    key.Close();
                }

                registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
                key = Registry.LocalMachine.OpenSubKey(registryKey);
                if (key == null) return false;
                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(cName))
                    {
                        return true;
                    }
                }
                key.Close();
                return false;
            }

            public static void CheckForRpgMaker()
            {
                Installed.Set(CheckInstalled("RPG Maker XP"), CheckInstalled("RPG Maker VX"), CheckInstalled("RPG Maker VX Ace"), CheckInstalled("RPG Maker MV"), CheckInstalled("RPG Maker MZ"), CheckInstalled("RPG Maker 2000") || CheckInstalled("RPG Maker 2003"));
            }
        }

        public static class Operation
        {
            private static readonly Label ErrorLabel = new Label("") { Height = Dim.Fill(1), Width = Dim.Fill(1) };
            private static Button _quitError;

            public static void OperationCompleted()
            {
                Window completed = new Window("Operation complete.");
                Button quitComplete = new Button("Close", true)
                {
                    X = Pos.Center(),
                    Y = Pos.Bottom(completed) - Pos.Y(completed) - 3
                };

                completed.ColorScheme = new ColorScheme
                {
                    Normal = TGAttribute.Make(Color.Black, Color.BrightGreen),
                    Focus = TGAttribute.Make(Color.Black, Color.BrightGreen),
                    HotNormal = TGAttribute.Make(Color.Black, Color.BrightGreen),
                    HotFocus = TGAttribute.Make(Color.Black, Color.BrightGreen)
                };

                completed.Width = 25;
                completed.Height = 3;

                completed.X = Pos.Center();
                completed.Y = Pos.Center();

                quitComplete.Clicked += () => { StaticWindows.Main.Window.SetFocus(); Application.RequestStop(); };

                completed.Add(quitComplete);
                Application.Run(completed);
            }

            public static void ShowError(string issue)
            {
                Window error = new Window("Error!")
                {
                    ColorScheme = new ColorScheme
                    {
                        Normal = TGAttribute.Make(Color.White, Color.BrightRed),
                        Focus = TGAttribute.Make(Color.White, Color.BrightRed),
                        HotNormal = TGAttribute.Make(Color.White, Color.BrightRed),
                        HotFocus = TGAttribute.Make(Color.White, Color.BrightRed)
                    },

                    Width = 50,
                    Height = Dim.Percent(50),

                    X = Pos.Center(),
                    Y = Pos.Center()
                };

                _quitError = new Button("Close", true)
                {
                    X = Pos.Center(),
                    Y = Pos.Bottom(error) - Pos.Y(error) - 3
                };
                _quitError.Clicked += () => { error.Remove(ErrorLabel); Application.RequestStop(); };

                ErrorLabel.Text = Regex.Replace(issue, "(.{46})", "$1\n");
                ErrorLabel.X = 1;
                ErrorLabel.Y = 1;

                error.Add(ErrorLabel);
                error.Add(_quitError);

                Application.Run(error);
            }

            public static string VersionInstalled(RPGMakerVersion version)
            {
                const string installed = "(installed on system)";
                const string notInstalled = "(not installed on system)";
                bool result;

                switch (version)
                {
                    case RPGMakerVersion.Xp: { result = Checks.Installed.Rmxp; break; }
                    case RPGMakerVersion.Vx: { result = Checks.Installed.Rmvx; break; }
                    case RPGMakerVersion.VxAce: { result = Checks.Installed.RmvxAce; break; }
                    default: { result = false; break; }
                }

                return result ? installed : notInstalled;
            }

            public static string GetVersion(string input)
            {
                if (input == "") return null;

                RPGMakerVersion version = RGSSAD.GetVersion(input);

                switch (version)
                {
                    case RPGMakerVersion.Xp: return "RPG Maker XP";
                    case RPGMakerVersion.Vx: return "RPG Maker VX";
                    case RPGMakerVersion.VxAce: return "RPG Maker VX Ace";
                    default:
                        return null;
                }
            }

            public static void ExecuteIfProjectSelected(Action callback)
            {
                try {
                    if ((Program.ProjectPath ?? RndString) != RndString)
                    {
                        callback();
                    }
                } catch (Exception ex) {
                    Application.RequestStop();
                    ShowError(ex.Message);
                }
            }

            public static class RpgMaker2K {

                public static void Convert2KCharsets(string pathToCharsets, string output)
                {
                    RPGMaker2000CharsetConverter converter = new RPGMaker2000CharsetConverter();

                    List<string> allCharsets = Directory.EnumerateFiles(pathToCharsets).ToList();
                    List<RPGMakerXPCharset> results;
                    int i;
                    string outfile;

                    foreach (string filename in allCharsets)
                    {
                        results = converter.ToRPGMakerXpCharset(filename);
                        for (i = 0; i < results.Count; i++)
                        {
                            outfile = $"{output}\\{Path.GetFileNameWithoutExtension(filename)}_{i}";
                            results[i].Save(outfile);
                        }
                    }

                    OperationCompleted();
                }

                public static void Convert2KChipsets(string pathToChipsets, string output)
                {
                    RPGMaker2000ChipsetConverter converter = new RPGMaker2000ChipsetConverter();

                    List<string> allChipsets = Directory.EnumerateFiles(pathToChipsets).ToList();
                    RPGMakerXPTileset results;

                    foreach (string filename in allChipsets)
                    {
                        results = converter.ToRPGMakerXpTileset(filename);
                        results.SaveAll($"{Path.GetFileNameWithoutExtension(filename)}", $"{output}\\");
                    }

                    OperationCompleted();
                }
            }
        }

        public static class Project
        {
            public static void MakeProject(bool ignoreComplete = false)
            {
                try
                {
                    Operation.ExecuteIfProjectSelected(() =>
                    {
                        FileDialog.CreateSaveDialog("Project File", "Pick a directory.", new[] { Path.GetExtension(Program.ProjectPath) }, () =>
                        {
                            Misc.EnsurePathExists(FileDialog.SaveDialog.DirectoryPath.ToString());
                            ProjectGenerator.GenerateProject(RGSSAD.GetVersion(Program.ProjectPath), FileDialog.SaveDialog.DirectoryPath.ToString());

                            if (!ignoreComplete) Operation.OperationCompleted();
                        });
                    });
                }
                catch (Exception ex)
                {
                    Application.RequestStop(); Operation.ShowError(ex.Message);
                }
            }

            public static void MakeProjectWithSavePath(bool ignoreComplete = false)
            {
                try
                {
                    Operation.ExecuteIfProjectSelected(() => {
                        Misc.EnsurePathExists(FileDialog.SaveDialog.DirectoryPath.ToString());
                        ProjectGenerator.GenerateProject(RGSSAD.GetVersion(Program.ProjectPath), FileDialog.SaveDialog.DirectoryPath.ToString());

                        if (!ignoreComplete) Operation.OperationCompleted();
                    });
                }
                catch (Exception ex)
                {
                    Application.RequestStop(); Operation.ShowError(ex.Message);
                }
            }

            public static void MakeProjectIndeterministic(RPGMakerVersion type, bool ignoreComplete = false)
            {
                try
                {
                    FileDialog.CreateSaveDialog("Project File", "Pick a directory.", new[] { Path.GetExtension(Program.ProjectPath) }, () => {
                        Misc.EnsurePathExists(FileDialog.SaveDialog.DirectoryPath.ToString());
                        ProjectGenerator.GenerateProject(type, FileDialog.SaveDialog.DirectoryPath.ToString());

                        if (!ignoreComplete) Operation.OperationCompleted();
                    });
                }
                catch (Exception ex)
                {
                    Application.RequestStop(); Operation.ShowError(ex.Message);
                }
            }

            public static void MakeMvProject(bool ignoreComplete = false)
            {
                try
                {
                    FileDialog.CreateSaveDialog("Project File", "Pick a directory.", new[] { "Game.rpgproject" }, () => {
                        Misc.EnsurePathExists(FileDialog.SaveDialog.DirectoryPath.ToString());
                        File.WriteAllText(FileDialog.SaveDialog.DirectoryPath.ToString() + "\\Game.rpgproject", "RPGMV 1.6.2");

                        if (!ignoreComplete) Operation.OperationCompleted();
                    });
                }
                catch (Exception ex)
                {
                    Application.RequestStop(); Operation.ShowError(ex.Message);
                }
            }
        }

        public class Extract
        {
            public Label InfoLabel;
            public static int    Location;
            public static int    Max =          1;
            public Window AllFiles =     new Window("Archived files");
            public Button QuitAllFiles;
            public Button AllFilesNext;
            public Button AllFilesLast;
            public static List<string> Files =  new List<string>();

            public Extract() => Init();

            public Extract Init()
            {
                AllFiles.ColorScheme = Style.ArchivedList;

                AllFiles.Width = Dim.Percent(80);
                AllFiles.Height = Dim.Percent(80);

                AllFiles.X = Pos.Center();
                AllFiles.Y = Pos.Center();

                SetupElements();

                AllFiles.Add(InfoLabel);
                AllFiles.Add(AllFilesNext);
                AllFiles.Add(AllFilesLast);
                AllFiles.Add(QuitAllFiles);

                return this;
            }

            public void SetupElements()
            {
                InfoLabel = new Label
                {
                    X = 1,
                    Y = 0,
                    Width = Dim.Fill(1),
                    Height = Dim.Fill(3)
                };

                QuitAllFiles = new Button("Close", true)
                {
                    X = Pos.Center(),
                    Y = Pos.Bottom(AllFiles) - Pos.Y(AllFiles) - 3
                };
                QuitAllFiles.Clicked += () => { Location = 0; StaticWindows.Main.Window.SetFocus(); Application.RequestStop(); };

                AllFilesNext = new Button("Next", true)
                {
                    X = Pos.Center() + 7,
                    Y = Pos.Bottom(AllFiles) - Pos.Y(AllFiles) - 3
                };
                AllFilesNext.Clicked += () => { Location = (Location + 1) % Max; GetAllFiles(this); };

                AllFilesLast = new Button("Last", true)
                {
                    X = Pos.Center() - 18,
                    Y = Pos.Bottom(AllFiles) - Pos.Y(AllFiles) - 3
                };
                AllFilesLast.Clicked += () => { Location = (Location + Max - 1) % Max; GetAllFiles(this); };
            }

            public const int MaxNamesOnDisplay = 15;

            public static void GetAllFiles(Extract useExtractObj = null)
            {
                string path = Program.ProjectPath;
                string infoText = "";

                //WaitDialog.Width = 15;
                //WaitDialog.Height = 3;

                RPGMakerVersion version = RGSSAD.GetVersion(path);

                switch (version)
                {
                    case RPGMakerVersion.Xp:
                    case RPGMakerVersion.Vx:
                        {
                            RGSSADv1 encrypted = new RGSSADv1(path);

                            Max = (int)Math.Ceiling((double)encrypted.ArchivedFiles.Count / MaxNamesOnDisplay);
                            int size = 0;

                            foreach (ArchivedFile file in encrypted.ArchivedFiles)
                            {
                                size += file.Size;
                            }

                            infoText = "Amount of files: " + encrypted.ArchivedFiles.Count + " (" + Misc.FileSize(size) + " total)\n\n";

                            for (int q = Location * MaxNamesOnDisplay; q < Math.Min((Location + 1) * MaxNamesOnDisplay, encrypted.ArchivedFiles.Count); q++)
                            {
                                infoText += (encrypted.ArchivedFiles[q].Name).Substring(0, Math.Min(58 - Misc.FileSize(encrypted.ArchivedFiles[q].Size).Length, encrypted.ArchivedFiles[q].Name.Length)) + " (" + Misc.FileSize(encrypted.ArchivedFiles[q].Size) + ")" + "\n";
                            }

                            Misc.UpdateStatus("Displaying files " + (Location * MaxNamesOnDisplay + 1) + " thru " + Math.Min((Location + 1) * MaxNamesOnDisplay, encrypted.ArchivedFiles.Count) + " out of " + encrypted.ArchivedFiles.Count);

                            encrypted.Dispose();
                            break;
                        }
                    case RPGMakerVersion.VxAce:
                        {
                            RGSSADv3 encrypted = new RGSSADv3(path);

                            Max = (int)Math.Ceiling((double)encrypted.ArchivedFiles.Count / MaxNamesOnDisplay);
                            int size = 0;

                            foreach (ArchivedFile file in encrypted.ArchivedFiles)
                            {
                                size += file.Size;
                            }

                            infoText = "Amount of files: " + encrypted.ArchivedFiles.Count + " (" + Misc.FileSize(size) + " total)\n\n";

                            for (int q = Location * MaxNamesOnDisplay; q < Math.Min((Location + 1) * MaxNamesOnDisplay, encrypted.ArchivedFiles.Count); q++)
                            {
                                infoText += (encrypted.ArchivedFiles[q].Name).Substring(0, Math.Min(58 - Misc.FileSize(encrypted.ArchivedFiles[q].Size).Length, encrypted.ArchivedFiles[q].Name.Length)) + " (" + Misc.FileSize(encrypted.ArchivedFiles[q].Size) + ")" + "\n";
                            }

                            encrypted.Dispose();
                            break;
                        }
                }

                infoText += $"\n{Location + 1} / {Max}";

                if (useExtractObj == null)
                {
                    Extract extractWin = new Extract();

                    extractWin.InfoLabel.Text = infoText;

                    extractWin.AllFiles.ColorScheme = Style.ArchivedList;
                    Application.Run(extractWin.AllFiles);
                }
                else
                {
                    useExtractObj.InfoLabel.Text = infoText;
                }
            }

            public static void ExtractAllFiles(bool ignoreComplete = false)
            {
                string path = Program.ProjectPath;
                string where = FileDialog.SaveDialog.DirectoryPath.ToString();

                //WaitDialog.Width = 15;
                //WaitDialog.Height = 3;

                RPGMakerVersion version = RGSSAD.GetVersion(path);

                switch (version)
                {
                    case RPGMakerVersion.Xp:
                    case RPGMakerVersion.Vx:
                        {
                            RGSSADv1 encrypted = new RGSSADv1(path);
                            try
                            {
                                Misc.UpdateStatus("Extracting all files for a XP/VX archive...");
                                encrypted.ExtractAllFiles(where, Settings.Values.OverwriteFiles);
                            }
                            catch (IOException file)
                            {
                                Operation.ShowError(file.Message);
                            }
                            encrypted.Dispose();
                            break;
                        }
                    case RPGMakerVersion.VxAce:
                        {
                            RGSSADv3 encrypted = new RGSSADv3(path);
                            try
                            {
                                Misc.UpdateStatus("Extracting all files for a VX Ace archive...");
                                encrypted.ExtractAllFiles(where, Settings.Values.OverwriteFiles);
                            }
                            catch (IOException file)
                            {
                                Operation.ShowError(file.Message);
                            }
                            encrypted.Dispose();
                            break;
                        }
                }

                if (!ignoreComplete) Operation.OperationCompleted();
            }

            public static void CopyGameFiles(bool ignoreComplete = false)
            {
                // ignore these types: rgssad, rgss2a, rgss3a

                string path = Path.GetDirectoryName(Program.ProjectPath);
                if (!path.EndsWith('\\')) path += "\\";

                Files = Directory.GetFiles(path)
                    .Where(x => !x.Contains("rgssad"))
                    .Where(x => !x.Contains("rgss2a"))
                    .Where(x => !x.Contains("rgss3a"))
                    .ToList();

                FileDialog.CreateSaveDialog("Copy to...", "Pick a place", new[] { "rgssad", "rgss2a", "rgss3a" }, () => { });

                if (!Directory.Exists(FileDialog.SaveDialog.DirectoryPath.ToString() + "\\")) Directory.CreateDirectory(FileDialog.SaveDialog.DirectoryPath.ToString() + "\\");

                Misc.CopyAll(new DirectoryInfo(path), new DirectoryInfo(FileDialog.SaveDialog.DirectoryPath.ToString() + "\\"));

                if (!ignoreComplete) Operation.OperationCompleted();
            }

            public static void CopyGameFilesIndeterministic(bool ignoreComplete = false)
            {
                // ignore these types: rgssad, rgss2a, rgss3a

                string path = Path.GetDirectoryName(Program.ProjectPath);
                if (!path.EndsWith('\\')) path += "\\";

                Files = Directory.GetFiles(path)
                    .Where(x => !x.Contains("rgssad"))
                    .Where(x => !x.Contains("rgss2a"))
                    .Where(x => !x.Contains("rgss3a"))
                    .ToList();

                if (!Directory.Exists(FileDialog.SaveDialog.DirectoryPath.ToString() + "\\")) Directory.CreateDirectory(FileDialog.SaveDialog.DirectoryPath.ToString() + "\\");

                Misc.CopyAll(new DirectoryInfo(path), new DirectoryInfo(FileDialog.SaveDialog.DirectoryPath.ToString() + "\\"));

                if (!ignoreComplete) Operation.OperationCompleted();
            }

            public static void FindAndExtractFile(bool ignoreComplete = false)
            {
                string path = Program.ProjectPath;
                string where = FileDialog.SaveDialog.DirectoryPath.ToString();
                string file = new StaticWindows.ExportOneFile().GetFile();

                //WaitDialog.Width = 15;
                //WaitDialog.Height = 3;

                Misc.UpdateStatus("Extracting file: " + file);

                RPGMakerVersion version = RGSSAD.GetVersion(path);

                switch (version)
                {
                    case RPGMakerVersion.Xp:
                    case RPGMakerVersion.Vx:
                        {
                            RGSSADv1 encrypted = new RGSSADv1(path);
                            try
                            {
                                ArchivedFile result = encrypted.ArchivedFiles.FirstOrDefault(x => x.Name.Contains(file));

                                if ((result.Name ?? "TheresNoFileHere") != "TheresNoFileHere")
                                {
                                    encrypted.ExtractFile(result, where, Settings.Values.OverwriteFiles);
                                }

                                if (!ignoreComplete)
                                {
                                    Misc.UpdateStatus("Extracted file: " + file);
                                    Operation.OperationCompleted();
                                }
                            }
                            catch (IOException fileErr)
                            {
                                Operation.ShowError(fileErr.Message + "\n\n" + fileErr.Source);
                            }
                            encrypted.Dispose();
                            break;
                        }
                    case RPGMakerVersion.VxAce:
                        {
                            RGSSADv3 encrypted = new RGSSADv3(path);
                            try
                            {
                                ArchivedFile result = encrypted.ArchivedFiles.FirstOrDefault(x => x.Name.Contains(file));

                                if ((result.Name ?? "TheresNoFileHere") != "TheresNoFileHere")
                                {
                                    encrypted.ExtractFile(result, where, Settings.Values.OverwriteFiles);
                                }

                                if (!ignoreComplete)
                                {
                                    Misc.UpdateStatus("Extracted file: " + file);
                                    Operation.OperationCompleted();
                                }
                            }
                            catch (IOException fileErr)
                            {
                                Operation.ShowError(fileErr.Message + "\n\n" + fileErr.Source);
                            }
                            encrypted.Dispose();
                            break;
                        }
                }
            }
        }

        public static class FileDialog
        {
            public static OpenDialog OpenDialog = new OpenDialog("Not Supplied", "Not Supplied");
            public static SaveDialog SaveDialog = new SaveDialog("Not Supplied", "Not Supplied");

            public static void CreateOpenDialog(string title, string message, string[] allowedFileTypes, Action callback)
            {
                OpenDialog = new OpenDialog(title, message);
                OpenDialog.Id = "OpenFileDialog";

                OpenDialog.X = Pos.Center();
                OpenDialog.Y = Pos.Center();
                OpenDialog.ColorScheme = Colors.Dialog;

                OpenDialog.AllowedFileTypes = allowedFileTypes;

                var setdir = new Button("Set Dir...");
                setdir.Clicked += () => { try { OpenDialog.DirectoryPath = OpenDialog.DirectoryPath; } catch (Exception) { /* nothing */ } };
                OpenDialog.AddButton(setdir);

                (OpenDialog.Subviews.First().Subviews.FirstOrDefault(x => (x as Button ?? new Button("x")).Text == "Open") as Button).Clicked += callback;
                (OpenDialog.Subviews.First().Subviews.FirstOrDefault(x => (x as Button ?? new Button("x")).Text == "Cancel") as Button).Clicked += () => Application.Top.MostFocused.SetFocus();

                Application.Run(OpenDialog);
            }

            public static void CreateSaveDialog(string title, string message, string[] allowedFileTypes, Action callback)
            {
                SaveDialog = new SaveDialog(title, message);
                SaveDialog.Id = "SaveFileDialog";

                SaveDialog.X = Pos.Center();
                SaveDialog.Y = Pos.Center();
                SaveDialog.ColorScheme = Colors.Dialog;

                SaveDialog.AllowedFileTypes = allowedFileTypes;

                var setdir = new Button("Set Dir...");
                setdir.Clicked += () => { try { OpenDialog.DirectoryPath = OpenDialog.DirectoryPath; } catch (Exception) { /* nothing */ } };
                OpenDialog.AddButton(setdir);

                (SaveDialog.Subviews.First().Subviews.FirstOrDefault(x => (x as Button ?? new Button("x")).Text == "Save") as Button).Clicked += () => callback();

                (SaveDialog.Subviews.First().Subviews.FirstOrDefault(x => (x as Button ?? new Button("x")).Text == "Cancel") as Button).Clicked += () => Application.Top.MostFocused.SetFocus();

                Application.Run(SaveDialog);
            }
        }

        public static class Misc
        {
            public static string FileSize(int bytes)
            {
                switch (Settings.Values.BytePref)
                {
                    // kilobytes/megabytes
                    case 1:
                    {
                        if (bytes < 1000)
                        {
                            return bytes + " bytes";
                        }

                        if (bytes > 1000 && bytes < 1000000)
                        {
                            return (Math.Floor(bytes / 10.0) / 100) + " KB";
                        }

                        return (Math.Floor(bytes / 10000.0) / 100) + " MB";
                    }

                    // just bytes
                    case 2:
                        {
                            return string.Format("{0:#,###0}", bytes) + " bytes";
                        }

                    // kibibytes/mebibytes
                    case 0:
                    default:
                    {
                        if (bytes < 1024)
                        {
                            return bytes + " bytes";
                        }

                        if (bytes > 1024 && bytes < 1048576)
                        {
                            return (Math.Floor(bytes / 10.24) / 100) + " KiB";
                        }

                        return (Math.Floor(bytes / 10485.76) / 100) + " MiB";
                    }
                }
            }

            // from stackoverflow
            public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
            {
                try
                {
                    //check if the target directory exists
                    if (Directory.Exists(target.FullName) == false)
                    {
                        Directory.CreateDirectory(target.FullName);
                    }

                    //copy all the files into the new directory

                    foreach (FileInfo fi in source.GetFiles())
                    {
                        UpdateStatus("Copying file: " + fi.Name);
                        fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                    }

                    //copy all the sub directories using recursion

                    foreach (DirectoryInfo diSourceDir in source.GetDirectories())
                    {
                        UpdateStatus("Copying directory: " + diSourceDir.Name);
                        DirectoryInfo nextTargetDir = target.CreateSubdirectory(diSourceDir.Name);
                        CopyAll(diSourceDir, nextTargetDir);
                    }
                    //success here
                }
                catch (IOException ie)
                {
                    //handle it here

                    Operation.ShowError(ie.Message);
                }
            }

            public static void UpdateStatus(string newStatus)
            {
                (Application.Top.Subviews.FirstOrDefault(x => x.Id == "ProgressText") as Label).Text = newStatus;
                Application.Refresh();
            }

            public static void EnsurePathExists(string path)
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            }
        }
    }
}

﻿using RPGMakerDecrypter.Decrypter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Terminal.Gui;
using TGAttribute = Terminal.Gui.Attribute;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Net.Http;
using RM2k2XP.Converters;

namespace rpg_patcher
{
    static class Functions
    {
        public const string RNDString = "01990849ca73d97c358aa63409043a93";

        public static class Checks
        {
            public static bool IsUpToDate()
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "RPG Patcher-" + ThisAssembly.Git.Tag);
                using var response = client.GetAsync("https://api.github.com/repos/xubiod/RPG-Patcher/commits").Result;
                var json = response.Content.ReadAsStringAsync().Result;

                dynamic commits = JArray.Parse(json);
                string lastCommit = commits[0].sha;

                return lastCommit == ThisAssembly.Git.Sha;
            }

            // https://stackoverflow.com/questions/16379143/check-if-application-is-installed-in-registry
            static bool CheckInstalled(string c_name)
            {
                string displayName;

                string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);
                if (key != null)
                {
                    foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                    {
                        displayName = subkey.GetValue("DisplayName") as string;
                        if (displayName != null && displayName.Contains(c_name))
                        {
                            return true;
                        }
                    }
                    key.Close();
                }

                registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
                key = Registry.LocalMachine.OpenSubKey(registryKey);
                if (key != null)
                {
                    foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                    {
                        displayName = subkey.GetValue("DisplayName") as string;
                        if (displayName != null && displayName.Contains(c_name))
                        {
                            return true;
                        }
                    }
                    key.Close();
                }
                return false;
            }

            public static void CheckForRPGMaker()
            {
                var xp =    CheckInstalled("RPG Maker XP");
                var vx =    CheckInstalled("RPG Maker VX");
                var vxace = CheckInstalled("RPG Maker VX Ace");

                var fore = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;

                if (!xp)    Console.WriteLine("RPG Maker XP has not been detected in the registry");
                if (!vx)    Console.WriteLine("RPG Maker VX has not been detected in the registry");
                if (!vxace) Console.WriteLine("RPG Maker VX Ace has not been detected in the registry");

                Console.ForegroundColor = fore;
                if (!(xp && vx && vxace))
                {
                    Console.Write(
@"
This doesn't mean that you can't run or use RPG Patcher, but you won't be able to edit maps and scripts without
some external tools or the appropriate RPG Maker version.

Please do not pirate RPG Maker just for modding, fan-made patches, etc.

Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        public static class Operation
        {
            static Label errorLabel = new Label("") { Height = Dim.Fill(1), Width = Dim.Fill(1) };
            static Button quitError;

            public static void OperationCompleted()
            {
                Window completed = new Window("Operation complete.");
                Button quitComplete = new Button("Close", true)
                {
                    X = Pos.Center(),
                    Y = Pos.Bottom(completed) - Pos.Y(completed) - 3
                };

                completed.ColorScheme = new ColorScheme()
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

                quitComplete.Clicked += () => { StaticWindows.Main._window.SetFocus(); Application.RequestStop(); };

                completed.Add(quitComplete);
                Application.Run(completed);
            }

            public static void ShowError(string issue)
            {
                Window Error = new Window("Error!")
                {
                    ColorScheme = new ColorScheme()
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

                quitError = new Button("Close", true)
                {
                    X = Pos.Center(),
                    Y = Pos.Bottom(Error) - Pos.Y(Error) - 3
                };
                quitError.Clicked += () => { Error.Remove(errorLabel); Application.RequestStop(); };

                errorLabel.Text = Regex.Replace(issue, "(.{46})", "$1\n");
                errorLabel.X = 1;
                errorLabel.Y = 1;

                Error.Add(errorLabel);
                Error.Add(quitError);

                Application.Run(Error);
            }

            public static string GetVersion(string input)
            {
                RPGMakerVersion _version = RGSSAD.GetVersion(input);

                switch (_version)
                {
                    case RPGMakerVersion.Xp: return "RPG Maker XP";
                    case RPGMakerVersion.Vx: return "RPG Maker VX";
                    case RPGMakerVersion.VxAce: return "RPG Maker VX Ace";
                    case RPGMakerVersion.Invalid:
                    default:
                        return null;
                }
            }

            public static void ExecuteIfProjectSelected(Action callback)
            {
                try {
                    if ((Program.ProjectPath ?? RNDString) != RNDString)
                    {
                        callback();
                    }
                } catch (Exception ex) {
                    Application.RequestStop();
                    ShowError(ex.Message);
                }
            }

            public static class RPGMaker2k {

                public static void Convert2kCharsets(string pathToCharsets, string output)
                {
                    RPGMaker2000CharsetConverter converter = new RPGMaker2000CharsetConverter();

                    List<string> allCharsets = Directory.EnumerateFiles(pathToCharsets).ToList<string>();
                    List<RM2k2XP.Converters.Formats.RPGMakerXPCharset> results;
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

                    Operation.OperationCompleted();
                }

                public static void Convert2kChipsets(string pathToChipsets, string output)
                {
                    RPGMaker2000ChipsetConverter converter = new RPGMaker2000ChipsetConverter();

                    List<string> allChipsets = Directory.EnumerateFiles(pathToChipsets).ToList<string>();
                    RM2k2XP.Converters.Formats.RPGMakerXPTileset results;

                    foreach (string filename in allChipsets)
                    {
                        results = converter.ToRPGMakerXpTileset(filename);
                        results.SaveAll($"{Path.GetFileNameWithoutExtension(filename)}", $"{output}\\");
                    }

                    Operation.OperationCompleted();
                }
            }
        }

        public static class Project
        {
            public static void MakeProject(bool ignoreComplete = false)
            {
                try
                {
                    Functions.Operation.ExecuteIfProjectSelected(() =>
                    {
                        FileDialog.CreateSaveDialog("Project File", "Pick a directory.", new string[] { Path.GetExtension(Program.ProjectPath) }, () =>
                        {
                            Misc.EnsurePathExists(FileDialog._SaveDialog.DirectoryPath.ToString());
                            ProjectGenerator.GenerateProject(RGSSAD.GetVersion(Program.ProjectPath), FileDialog._SaveDialog.DirectoryPath.ToString());

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
                    Functions.Operation.ExecuteIfProjectSelected(() => {
                        Misc.EnsurePathExists(FileDialog._SaveDialog.DirectoryPath.ToString());
                        ProjectGenerator.GenerateProject(RGSSAD.GetVersion(Program.ProjectPath), FileDialog._SaveDialog.DirectoryPath.ToString());

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
                    FileDialog.CreateSaveDialog("Project File", "Pick a directory.", new string[] { Path.GetExtension(Program.ProjectPath) }, () => {
                        Misc.EnsurePathExists(FileDialog._SaveDialog.DirectoryPath.ToString());
                        ProjectGenerator.GenerateProject(type, FileDialog._SaveDialog.DirectoryPath.ToString());

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
            public Label infoLabel;
            public static int    location =     0;
            public static int    max =          1;
            public Window AllFiles =     new Window("Archived files");
            public Button quitAllFiles;
            public Button allFilesNext;
            public Button allFilesLast;
            public static List<string> files =  new List<string>();

            public Extract() => Init();

            public Extract Init()
            {
                AllFiles.ColorScheme = Program.ArchivedList;

                AllFiles.Width = Dim.Percent(80);
                AllFiles.Height = Dim.Percent(80);

                AllFiles.X = Pos.Center();
                AllFiles.Y = Pos.Center();

                SetupElements();

                AllFiles.Add(infoLabel);
                AllFiles.Add(allFilesNext);
                AllFiles.Add(allFilesLast);
                AllFiles.Add(quitAllFiles);

                return this;
            }

            public void SetupElements()
            {
                infoLabel = new Label()
                {
                    X = 1,
                    Y = 0,
                    Width = Dim.Fill(1),
                    Height = Dim.Fill(3)
                };

                quitAllFiles = new Button("Close", true)
                {
                    X = Pos.Center(),
                    Y = Pos.Bottom(AllFiles) - Pos.Y(AllFiles) - 3
                };
                quitAllFiles.Clicked += () => { location = 0; StaticWindows.Main._window.SetFocus(); Application.RequestStop(); };

                allFilesNext = new Button("Next", true)
                {
                    X = Pos.Center() + 7,
                    Y = Pos.Bottom(AllFiles) - Pos.Y(AllFiles) - 3
                };
                allFilesNext.Clicked += () => { location = (location + 1) % max; GetAllFiles(this); };

                allFilesLast = new Button("Last", true)
                {
                    X = Pos.Center() - 18,
                    Y = Pos.Bottom(AllFiles) - Pos.Y(AllFiles) - 3
                };
                allFilesLast.Clicked += () => { location = (location + max - 1) % max; GetAllFiles(this); };
            }

            public const int MaxNamesOnDisplay = 15;

            public static void GetAllFiles(Extract useExtractObj = null)
            {
                string path = Program.ProjectPath;
                string infoText = "";

                //WaitDialog.Width = 15;
                //WaitDialog.Height = 3;

                RPGMakerVersion _version = RGSSAD.GetVersion(path);

                switch (_version)
                {
                    case RPGMakerVersion.Xp:
                    case RPGMakerVersion.Vx:
                        {
                            RGSSADv1 encrypted = new RGSSADv1(path);

                            max = (int)Math.Ceiling((double)encrypted.ArchivedFiles.Count / MaxNamesOnDisplay);
                            int size = 0;

                            foreach (ArchivedFile file in encrypted.ArchivedFiles)
                            {
                                size += file.Size;
                            }

                            infoText = "Amount of files: " + encrypted.ArchivedFiles.Count + " (" + Misc.FileSize(size) + " total)\n\n";

                            for (int q = location * MaxNamesOnDisplay; q < Math.Min((location + 1) * MaxNamesOnDisplay, encrypted.ArchivedFiles.Count); q++)
                            {
                                infoText += (encrypted.ArchivedFiles[q].Name).Substring(0, Math.Min(58 - Misc.FileSize(encrypted.ArchivedFiles[q].Size).Length, encrypted.ArchivedFiles[q].Name.Length)) + " (" + Misc.FileSize(encrypted.ArchivedFiles[q].Size) + ")" + "\n";
                            }

                            Misc.UpdateStatus("Displaying files " + (location * MaxNamesOnDisplay + 1) + " thru " + Math.Min((location + 1) * MaxNamesOnDisplay, encrypted.ArchivedFiles.Count) + " out of " + encrypted.ArchivedFiles.Count.ToString());

                            encrypted.Dispose();
                            break;
                        }
                    case RPGMakerVersion.VxAce:
                        {
                            RGSSADv3 encrypted = new RGSSADv3(path);

                            max = (int)Math.Ceiling((double)encrypted.ArchivedFiles.Count / MaxNamesOnDisplay);
                            int size = 0;

                            foreach (ArchivedFile file in encrypted.ArchivedFiles)
                            {
                                size += file.Size;
                            }

                            infoText = "Amount of files: " + encrypted.ArchivedFiles.Count + " (" + Misc.FileSize(size) + " total)\n\n";

                            for (int q = location * MaxNamesOnDisplay; q < Math.Min((location + 1) * MaxNamesOnDisplay, encrypted.ArchivedFiles.Count); q++)
                            {
                                infoText += (encrypted.ArchivedFiles[q].Name).Substring(0, Math.Min(58 - Misc.FileSize(encrypted.ArchivedFiles[q].Size).Length, encrypted.ArchivedFiles[q].Name.Length)) + " (" + Misc.FileSize(encrypted.ArchivedFiles[q].Size) + ")" + "\n";
                            }

                            encrypted.Dispose();
                            break;
                        }
                    default: break;
                }

                infoText += $"\n{location + 1} / {max}";

                if (useExtractObj == null)
                {
                    Extract extractWin = new Extract();

                    extractWin.infoLabel.Text = infoText;

                    extractWin.AllFiles.ColorScheme = Program.ArchivedList;
                    Application.Run(extractWin.AllFiles);
                }
                else
                {
                    useExtractObj.infoLabel.Text = infoText;
                }
            }

            public static void ExtractAllFiles(bool ignoreComplete = false)
            {
                string path = Program.ProjectPath;
                string where = Functions.FileDialog._SaveDialog.DirectoryPath.ToString();

                //WaitDialog.Width = 15;
                //WaitDialog.Height = 3;

                RPGMakerVersion _version = RGSSAD.GetVersion(path);

                switch (_version)
                {
                    case RPGMakerVersion.Xp:
                    case RPGMakerVersion.Vx:
                        {
                            RGSSADv1 encrypted = new RGSSADv1(path);
                            try
                            {
                                Misc.UpdateStatus("Extracting all files for a XP/VX archive...");
                                encrypted.ExtractAllFiles(where, User.Default.OverwriteFiles);
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
                                encrypted.ExtractAllFiles(where, User.Default.OverwriteFiles);
                            }
                            catch (IOException file)
                            {
                                Operation.ShowError(file.Message);
                            }
                            encrypted.Dispose();
                            break;
                        }
                    default: break;
                }

                if (!ignoreComplete) Operation.OperationCompleted();
            }

            public static void CopyGameFiles(bool ignoreComplete = false)
            {
                // ignore these types: rgssad, rgss2a, rgss3a

                string path = Path.GetDirectoryName(Program.ProjectPath);
                if (!path.EndsWith('\\')) path += "\\";

                files = Directory.GetFiles(path)
                    .Where(x => !x.Contains("rgssad"))
                    .Where(x => !x.Contains("rgss2a"))
                    .Where(x => !x.Contains("rgss3a"))
                    .ToList();

                FileDialog.CreateSaveDialog("Copy to...", "Pick a place", new string[] { "rgssad", "rgss2a", "rgss3a" }, () => { return; });

                if (!Directory.Exists(FileDialog._SaveDialog.DirectoryPath.ToString() + "\\")) Directory.CreateDirectory(FileDialog._SaveDialog.DirectoryPath.ToString() + "\\");

                Misc.CopyAll(new DirectoryInfo(path), new DirectoryInfo(FileDialog._SaveDialog.DirectoryPath.ToString() + "\\"));

                if (!ignoreComplete) Operation.OperationCompleted();
            }

            public static void CopyGameFilesIndeterministic(bool ignoreComplete = false)
            {
                // ignore these types: rgssad, rgss2a, rgss3a

                string path = Path.GetDirectoryName(Program.ProjectPath);
                if (!path.EndsWith('\\')) path += "\\";

                files = Directory.GetFiles(path)
                    .Where(x => !x.Contains("rgssad"))
                    .Where(x => !x.Contains("rgss2a"))
                    .Where(x => !x.Contains("rgss3a"))
                    .ToList();

                if (!Directory.Exists(FileDialog._SaveDialog.DirectoryPath.ToString() + "\\")) Directory.CreateDirectory(FileDialog._SaveDialog.DirectoryPath.ToString() + "\\");

                Misc.CopyAll(new DirectoryInfo(path), new DirectoryInfo(FileDialog._SaveDialog.DirectoryPath.ToString() + "\\"));

                if (!ignoreComplete) Operation.OperationCompleted();
            }

            public static void FindAndExtractFile(bool ignoreComplete = false)
            {
                string path = Program.ProjectPath;
                string where = Functions.FileDialog._SaveDialog.DirectoryPath.ToString();
                string file = new StaticWindows.ExportOneFile().GetFile();

                //WaitDialog.Width = 15;
                //WaitDialog.Height = 3;

                Functions.Misc.UpdateStatus("Extracting file: " + file);

                RPGMakerVersion _version = RGSSAD.GetVersion(path);

                switch (_version)
                {
                    case RPGMakerVersion.Xp:
                    case RPGMakerVersion.Vx:
                        {
                            RGSSADv1 encrypted = new RGSSADv1(path);
                            try
                            {
                                ArchivedFile _result = encrypted.ArchivedFiles.FirstOrDefault(x => x.Name.Contains(file));

                                if ((_result.Name ?? "TheresNoFileHere") != "TheresNoFileHere")
                                {
                                    encrypted.ExtractFile(_result, where, User.Default.OverwriteFiles);
                                }

                                if (!ignoreComplete)
                                {
                                    Functions.Misc.UpdateStatus("Extracted file: " + file);
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
                                ArchivedFile _result = encrypted.ArchivedFiles.FirstOrDefault(x => x.Name.Contains(file));

                                if ((_result.Name ?? "TheresNoFileHere") != "TheresNoFileHere")
                                {
                                    encrypted.ExtractFile(_result, where, User.Default.OverwriteFiles);
                                }

                                if (!ignoreComplete)
                                {
                                    Functions.Misc.UpdateStatus("Extracted file: " + file);
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
                    default: break;
                }
            }
        }

        public static class FileDialog
        {
            public static OpenDialog _OpenDialog = new OpenDialog("Not Supplied", "Not Supplied");
            public static SaveDialog _SaveDialog = new SaveDialog("Not Supplied", "Not Supplied");

            public static void CreateOpenDialog(string title, string message, string[] allowed_file_types, Action callback)
            {
                _OpenDialog = new OpenDialog(title, message);
                _OpenDialog.Id = "OpenFileDialog";

                _OpenDialog.X = Pos.Center();
                _OpenDialog.Y = Pos.Center();
                _OpenDialog.ColorScheme = Colors.Dialog;

                _OpenDialog.AllowedFileTypes = allowed_file_types;

                var setdir = new Button("Set Dir...");
                setdir.Clicked += () => { try { _OpenDialog.DirectoryPath = _OpenDialog.DirectoryPath; } catch (Exception) { /* nothing */ } };
                _OpenDialog.AddButton(setdir);

                (_OpenDialog.Subviews.First().Subviews.FirstOrDefault(x => (x as Button ?? new Button("x")).Text == "Open") as Button).Clicked += callback;
                (_OpenDialog.Subviews.First().Subviews.FirstOrDefault(x => (x as Button ?? new Button("x")).Text == "Cancel") as Button).Clicked += () => Application.Top.MostFocused.SetFocus();

                Application.Run(_OpenDialog);
            }

            public static void CreateSaveDialog(string title, string message, string[] allowed_file_types, Action callback)
            {
                _SaveDialog = new SaveDialog(title, message);
                _SaveDialog.Id = "SaveFileDialog";

                _SaveDialog.X = Pos.Center();
                _SaveDialog.Y = Pos.Center();
                _SaveDialog.ColorScheme = Colors.Dialog;

                _SaveDialog.AllowedFileTypes = allowed_file_types;

                var setdir = new Button("Set Dir...");
                setdir.Clicked += () => { try { _OpenDialog.DirectoryPath = _OpenDialog.DirectoryPath; } catch (Exception) { /* nothing */ } };
                _OpenDialog.AddButton(setdir);

                (_SaveDialog.Subviews.First().Subviews.FirstOrDefault(x => (x as Button ?? new Button("x")).Text == "Save") as Button).Clicked += () => callback();

                (_SaveDialog.Subviews.First().Subviews.FirstOrDefault(x => (x as Button ?? new Button("x")).Text == "Cancel") as Button).Clicked += () => Application.Top.MostFocused.SetFocus();

                Application.Run(_SaveDialog);
            }
        }

        public static class Misc
        {
            public static string FileSize(int bytes)
            {
                switch (User.Default.BytePref)
                {
                    // kilobytes/megabytes
                    case 1:
                        {
                            if (bytes < 1000)
                            {
                                return bytes.ToString() + " bytes";
                            }
                            else if (bytes > 1000 && bytes < 1000000)
                            {
                                return (Math.Floor(bytes / 10.0) / 100).ToString() + " KB";
                            }
                            else
                            {
                                return (Math.Floor(bytes / 10000.0) / 100).ToString() + " MB";
                            }
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
                                return bytes.ToString() + " bytes";
                            }
                            else if (bytes > 1024 && bytes < 1048576)
                            {
                                return (Math.Floor(bytes / 10.24) / 100).ToString() + " KiB";
                            }
                            else
                            {
                                return (Math.Floor(bytes / 10485.76) / 100).ToString() + " MiB";
                            }
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

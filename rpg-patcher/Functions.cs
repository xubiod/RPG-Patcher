using RPGMakerDecrypter.Decrypter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Terminal.Gui;
using TGAttribute = Terminal.Gui.Attribute;

namespace rpg_patcher
{
    static class Functions
    {
        public static class Operation
        {
            static Window Error = new Window("Error!");
            static Label errorLabel = new Label("");
            static Button quitError = new Button("Close", true)
            {
                X = Pos.Center(),
                Y = Pos.Bottom(Error) - Pos.Y(Error) - 3,
                Clicked = () => { Error.Remove(errorLabel); Application.RequestStop(); }
            };

            public static Window Complete = new Window("Operation complete.");
            static Button quitComplete = new Button("Close", true)
            {
                X = Pos.Center(),
                Y = Pos.Bottom(Complete) - Pos.Y(Complete) - 3,
                Clicked = () => { Application.Top.SetFocus(StaticWindows.Main._window); Application.RequestStop(); }
            };

            public static void Init()
            {
                #region operation complete
                Complete.ColorScheme = new ColorScheme()
                {
                    Normal = TGAttribute.Make(Color.Black, Color.BrightGreen),
                    Focus = TGAttribute.Make(Color.Black, Color.BrightGreen),
                    HotNormal = TGAttribute.Make(Color.Black, Color.BrightGreen),
                    HotFocus = TGAttribute.Make(Color.Black, Color.BrightGreen)
                };

                Complete.Width = 25;
                Complete.Height = 3;

                Complete.X = Pos.Center();
                Complete.Y = Pos.Center();

                Complete.Add(quitComplete);
                #endregion

                #region file info
                Export.AllFiles.ColorScheme = Program.ArchivedList;

                Export.AllFiles.Width = Dim.Percent(80);
                Export.AllFiles.Height = Dim.Percent(80);

                Export.AllFiles.X = Pos.Center();
                Export.AllFiles.Y = Pos.Center();

                Export.AllFiles.Add(Export.infoLabel);
                Export.AllFiles.Add(Export.allFilesNext);
                Export.AllFiles.Add(Export.allFilesLast);
                Export.AllFiles.Add(Export.quitAllFiles);
                #endregion

                #region error 
                Error.ColorScheme = new ColorScheme()
                {
                    Normal = TGAttribute.Make(Color.White, Color.BrightRed),
                    Focus = TGAttribute.Make(Color.White, Color.BrightRed),
                    HotNormal = TGAttribute.Make(Color.White, Color.BrightRed),
                    HotFocus = TGAttribute.Make(Color.White, Color.BrightRed)
                };

                Error.Width = 50;
                Error.Height = Dim.Percent(50);

                Error.X = Pos.Center();
                Error.Y = Pos.Center();
                #endregion
            }

            public static void ShowError(string issue)
            {
                errorLabel.Text = Regex.Replace(issue, ".{48}", "$0\n");
                errorLabel.X = Pos.Center();
                errorLabel.Y = Pos.Center();

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
                    if ((Program.ProjectPath ?? "") != "")
                    {
                        callback();
                    }
                } catch (Exception ex) {
                    Application.RequestStop();
                    ShowError(ex.Message);
                }
            }
        }

        public static class Project
        {
            public static void MakeProject()
            {
                try
                {
                    if (Operation.GetVersion(Program.ProjectPath) != "")
                    {
                        FileDialog.CreateSaveDialog("Project File", "Pick a directory.", new string[] { Path.GetExtension(Program.ProjectPath) }, () => {
                            ProjectGenerator.GenerateProject(RGSSAD.GetVersion(Program.ProjectPath), FileDialog._SaveDialog.DirectoryPath.ToString());
                            Application.Run(Operation.Complete); 
                        });
                    }
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
                    if (Operation.GetVersion(Program.ProjectPath) != "")
                    {
                        ProjectGenerator.GenerateProject(RGSSAD.GetVersion(Program.ProjectPath), FileDialog._SaveDialog.DirectoryPath.ToString());
                        if (!ignoreComplete) Application.Run(Operation.Complete);
                    }
                }
                catch (Exception ex)
                {
                    Application.RequestStop(); Operation.ShowError(ex.Message);
                }
            }

            public static void MakeProjectIndeterministic(RPGMakerVersion type)
            {
                try
                {
                    if (Operation.GetVersion(Program.ProjectPath) != "")
                    {
                        FileDialog.CreateSaveDialog("Project File", "Pick a directory.", new string[] { Path.GetExtension(Program.ProjectPath) }, () => {
                            ProjectGenerator.GenerateProject(type, FileDialog._SaveDialog.DirectoryPath.ToString());
                            Application.Run(Operation.Complete);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Application.RequestStop(); Operation.ShowError(ex.Message);
                }
            }
        }

        public static class Export
        {
            public static Label  infoLabel =    new Label(1, 1, "a\na\na\na\na\na\na\na\na\na\na\na\na\na");
            static int    location =     0;
            static int    max =          1;
            public static Window AllFiles =     new Window("Archived files");
            public static Button quitAllFiles = new Button("Close", true)
            {
                X = Pos.Center(),
                Y = Pos.Bottom(AllFiles) - Pos.Y(AllFiles) - 3,
                Clicked = () => { location = 0; Application.Top.SetFocus(StaticWindows.Main._window); Application.RequestStop(); }
            };
            public static Button allFilesNext = new Button("Next", true)
            {
                X = Pos.Center() + 7,
                Y = Pos.Bottom(AllFiles) - Pos.Y(AllFiles) - 3,
                Clicked = () => { location = (location + 1) % max; GetAllFiles(); }
            };
            public static Button allFilesLast = new Button("Last", true)
            {
                X = Pos.Center() - 18,
                Y = Pos.Bottom(AllFiles) - Pos.Y(AllFiles) - 3,
                Clicked = () => { location = (location + max - 1) % max; GetAllFiles(); }
            };
            public static List<string> files =  new List<string>();

            public static void GetAllFiles()
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

                            max = (int)Math.Ceiling((double)encrypted.ArchivedFiles.Count / 10);
                            int size = 0;

                            foreach (ArchivedFile file in encrypted.ArchivedFiles)
                            {
                                size += file.Size;
                            }

                            infoText = "Amount of files: " + encrypted.ArchivedFiles.Count + " (" + Misc.FileSize(size) + " total)\n\n";

                            for (int q = location * 10; q < Math.Min((location + 1) * 10, encrypted.ArchivedFiles.Count); q++)
                            {
                                infoText += (encrypted.ArchivedFiles[q].Name).Substring(0, Math.Min(58 - Misc.FileSize(encrypted.ArchivedFiles[q].Size).Length, encrypted.ArchivedFiles[q].Name.Length)) + " (" + Misc.FileSize(encrypted.ArchivedFiles[q].Size) + ")" + "\n";
                            }

                            encrypted.Dispose();
                            break;
                        }
                    case RPGMakerVersion.VxAce:
                        {
                            RGSSADv3 encrypted = new RGSSADv3(path);

                            max = (int)Math.Ceiling((double)encrypted.ArchivedFiles.Count / 10);
                            int size = 0;

                            foreach (ArchivedFile file in encrypted.ArchivedFiles)
                            {
                                size += file.Size;
                            }

                            infoText = "Amount of files: " + encrypted.ArchivedFiles.Count + " (" + Misc.FileSize(size) + " total)\n\n";

                            for (int q = location * 10; q < Math.Min((location + 1) * 10, encrypted.ArchivedFiles.Count); q++)
                            {
                                infoText += (encrypted.ArchivedFiles[q].Name).Substring(0, Math.Min(58 - Misc.FileSize(encrypted.ArchivedFiles[q].Size).Length, encrypted.ArchivedFiles[q].Name.Length)) + " (" + Misc.FileSize(encrypted.ArchivedFiles[q].Size) + ")" + "\n";
                            }

                            encrypted.Dispose();
                            break;
                        }
                    default: break;
                }

                infoText += "\n" + (location + 1).ToString() + " / " + max.ToString();

                infoLabel.Text = infoText;

                AllFiles.ColorScheme = Program.ArchivedList;
                Application.Run(AllFiles);
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
                                encrypted.ExtractAllFiles(where, StaticWindows.Settings.OverwriteFiles);
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
                                encrypted.ExtractAllFiles(where, StaticWindows.Settings.OverwriteFiles);
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

                if (!ignoreComplete) Application.Run(Operation.Complete);
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

                FileDialog.CreateSaveDialog("Copy to...", "Pick a place", new string[] { "rgssad", "rgss2a", "rgss3a" }, null);

                if (!Directory.Exists(FileDialog._SaveDialog.DirectoryPath.ToString() + "\\")) Directory.CreateDirectory(FileDialog._SaveDialog.DirectoryPath.ToString() + "\\");

                Misc.CopyAll(new DirectoryInfo(path), new DirectoryInfo(FileDialog._SaveDialog.DirectoryPath.ToString() + "\\"));

                if (!ignoreComplete) Application.Run(Operation.Complete);
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

                if (!ignoreComplete) Application.Run(Operation.Complete);
            }

            public static void FindAndExportFile(bool ignoreComplete = false)
            {
                string path = Program.ProjectPath;
                string where = Functions.FileDialog._SaveDialog.DirectoryPath.ToString();
                string file = StaticWindows.ExportOneFile.GetFile();

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
                                ArchivedFile _result = encrypted.ArchivedFiles.FirstOrDefault(x => x.Name.Contains(file));

                                if ((_result.Name ?? "TheresNoFileHere") != "TheresNoFileHere")
                                {
                                    encrypted.ExtractFile(_result, where, StaticWindows.Settings.OverwriteFiles);
                                }

                                if (!ignoreComplete) Application.Run(Operation.Complete);
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
                                    encrypted.ExtractFile(_result, where, StaticWindows.Settings.OverwriteFiles);
                                }

                                if (!ignoreComplete) Application.Run(Operation.Complete);
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
                _OpenDialog.AddButton(new Button("Set Dir...") { Clicked = () => { try { _OpenDialog.DirectoryPath = _OpenDialog.DirectoryPath; } catch (Exception) { /* nothing */ } } });

                (_OpenDialog.Subviews.First().Subviews.FirstOrDefault(x => (x as Button ?? new Button("x")).Text == "Open") as Button).Clicked += callback;
                (_OpenDialog.Subviews.First().Subviews.FirstOrDefault(x => (x as Button ?? new Button("x")).Text == "Cancel") as Button).Clicked += () => Application.Top.SetFocus(Application.Top.MostFocused);

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
                _SaveDialog.AddButton(new Button("Set Dir...") { Clicked = () => { try { _OpenDialog.DirectoryPath = _OpenDialog.DirectoryPath; } catch (Exception) { /* nothing */ } } });

                (_SaveDialog.Subviews.First().Subviews.FirstOrDefault(x => (x as Button ?? new Button("x")).Text == "Save") as Button).Clicked += () => callback();

                (_SaveDialog.Subviews.First().Subviews.FirstOrDefault(x => (x as Button ?? new Button("x")).Text == "Cancel") as Button).Clicked += () => Application.Top.SetFocus(Application.Top.MostFocused);

                Application.Run(_SaveDialog);
            }
        }

        public static class Misc
        {
            public static string FileSize(int bytes)
            {
                switch (StaticWindows.Settings.BytePref)
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
                                return (Math.Floor(bytes / 10.24) / 100).ToString() + " KB";
                            }
                            else
                            {
                                return (Math.Floor(bytes / 10485.76) / 100).ToString() + " MB";
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
                        fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                    }


                    //copy all the sub directories using recursion

                    foreach (DirectoryInfo diSourceDir in source.GetDirectories())
                    {
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
        }
    }
}

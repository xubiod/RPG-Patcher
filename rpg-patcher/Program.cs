using System;
using System.IO;
using System.Linq;
using RPGMakerDecrypter.Decrypter;
using Terminal.Gui;

namespace rpg_patcher
{
    internal class Program
    {
        public static string ProjectPath = "";

        public static MenuBar MainMenuBar;

        private static void Preload()
        {
            Settings.Load("settings");
            if (Settings.Values.PersistentProject && File.Exists("project")) ProjectPath = File.ReadAllText("project");

            Functions.Checks.CheckForRpgMaker();
        }

        private static void Main()
        {
            Preload();

            // initalize terminal.gui
            Application.Init();

            //Application.Top.ColorScheme =

            Style.Theme(Settings.Values.Theme);

            // menubar
            MainMenuBar = new MenuBar(new[] {
                new MenuBarItem ("_File", new[] {
                    new MenuItem ("_About", "", () => {
                        StaticWindows.About about = new StaticWindows.About();
                        Application.Run(about.Window);
                    }),
                    new MenuItem ("_Load", "", () => {
                        Functions.FileDialog.CreateOpenDialog("Project", "Pick a project", new[] { "rgssad", "rgss2a", "rgss3a" }, () => UpdateElements());
                    }),
                    new MenuItem ("_Unload", "", () => {
                        ProjectPath = "";
                        File.Delete("project");
                        UpdateElements(true, true);
                    }),
                    new MenuItem ("_Settings", "", () => {
                        StaticWindows.Settings settings = new StaticWindows.Settings();
                        Application.Run(settings.Window);
                    }),
                    //new MenuItem ("_Focus on main window", "", () => {
                    //    //Application.Top.SetFocus(StaticWindows.Main._window);
                    //    StaticWindows.Main._window.SetFocus();
                    //}),
                    new MenuItem ("_Quit", "", () => {
                        Application.RequestStop();
                    })
                }),

                new MenuBarItem ("_Project", new[] {
                    new MenuItem ("Create _project files with loaded version", "", () => {
                        Functions.Project.MakeProject();
                    }),
                    null,
                    new MenuItem ("Create _XP project files", "", () => {
                        Functions.Project.MakeProjectIndeterministic(RPGMakerVersion.Xp);
                    }),
                    new MenuItem ("Create _VX project files", "", () => {
                        Functions.Project.MakeProjectIndeterministic(RPGMakerVersion.Vx);
                    }),
                    new MenuItem ("Create VX _Ace project files", "", () => {
                        Functions.Project.MakeProjectIndeterministic(RPGMakerVersion.VxAce);
                    }),
                    new MenuItem ("Create _MV project files", "", () => {
                        Functions.Project.MakeMvProject();
                    }),
                    new MenuItem ("Create M_Z project files", "", () => {
                        Functions.Project.MakeMzProject();
                    })
                }),

                new MenuBarItem ("_Extracting", new[] {
                    new MenuItem ("_Copy game files from project directory", "", () => {
                        Functions.Extract.CopyGameFiles();
                    }),
                    new MenuItem ("_List files in the archive", "", () => {
                        Functions.Extract.GetAllFiles();
                    }),
                    null,
                    new MenuItem ("Extract _single file", "", () => {
                        Functions.Operation.ExecuteIfProjectSelected(() => {
                            StaticWindows.ExportOneFile oneFile = new StaticWindows.ExportOneFile();
                            Application.Run(oneFile.Window);
                        });
                    }),
                    new MenuItem ("Extract _everything", "", () => {
                        Functions.FileDialog.CreateSaveDialog("Save to...", "Pick a place", new[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.ExtractAllFiles()));
                    }),
                    new MenuItem ("Extract everything + _project files", "", () => {
                        Functions.FileDialog.CreateSaveDialog("Save to...", "Pick a place", new[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.ExtractAllFiles(true)));
                        Functions.Project.MakeProjectWithSavePath();
                    }),
                    new MenuItem ("Extract everything + project files + _game", "", () => {
                        Functions.FileDialog.CreateSaveDialog("Save to...", "Pick a place", new[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.ExtractAllFiles(true)));
                        Functions.Project.MakeProjectWithSavePath(true);
                        Functions.Extract.CopyGameFilesIndeterministic();
                    }),
                }),

                new MenuBarItem ("_Modding", new[] {
                    new MenuItem ("_Working on it", "", () => {
                        Functions.Operation.ShowError("whoops");
                    })
                }),

                new MenuBarItem ("_Special", new[] {
                    new MenuItem ("Check for _RPG Maker installs in the registry", "", () => {
                        // $"RPG Maker XP installed: {(Functions.Checks.Installed.XP ? "Yes" : "No")}\nRPG Maker VX installed: {(Functions.Checks.Installed.VX ? "Yes" : "No")}\nRPG Maker VX Ace installed: {(Functions.Checks.Installed.VXAce ? "Yes" : "No")}"
                        Button closeDialog = new Button("Close", true);
                        closeDialog.Clicked += () => { StaticWindows.Main.Window.SetFocus(); Application.RequestStop(); };

                        Dialog installed = new Dialog("RPG Maker installs");
                        installed.AddButton(closeDialog); // \n\n

                        installed.Add(new Label("RPG Maker 2000/2003 installed*:\nRPG Maker XP installed:\nRPG Maker VX installed:\nRPG Maker VX Ace installed:\nRPG Maker MV installed*:\nRPG Maker MZ installed*:"));

                        Label installedVersions = new Label($"{(Functions.Checks.Installed.Rm2K2003 ? "Yes" : "No")}\n{(Functions.Checks.Installed.Rmxp ? "Yes" : "No")}\n{(Functions.Checks.Installed.Rmvx ? "Yes" : "No")}\n{(Functions.Checks.Installed.RmvxAce ? "Yes" : "No")}\n{(Functions.Checks.Installed.Rmmv ? "Yes" : "No")}\n{(Functions.Checks.Installed.Rmmz ? "Yes" : "No")}") { TextAlignment = TextAlignment.Right, X = Pos.Right(installed) - Pos.X(installed) - 6 };
                        installed.Add(installedVersions);
                        installed.Add(new Label("Installed or not, this program will not restrict use.\n\n*Functionality currently limited.") { Y = Pos.Bottom(installedVersions) + 1 });

                        installed.Width = Dim.Percent(50);
                        installed.Height = Dim.Percent(47);

                        Application.Run(installed);
                    }),
                    null,
                    new MenuItem ("RPG Maker 2000/03 Ch_arset -> XP format (entire folder)", "", () => {
                        string infile = "", outfile = "";

                        Functions.FileDialog.CreateOpenDialog("Navigate to folder of RPG Maker 2000/03 charsets.", "", new[] {"png"}, () => { infile = Functions.FileDialog.OpenDialog.FilePath.ToString(); });
                        Functions.FileDialog.CreateSaveDialog("Where to put the converted charsets?", "", new[] {"png"}, () => { outfile = Functions.FileDialog.SaveDialog.FilePath.ToString(); });

                        Functions.Operation.RpgMaker2K.Convert2KCharsets(Path.GetDirectoryName(infile), Path.GetDirectoryName(outfile));
                    }),
                    new MenuItem ("RPG Maker 2000/03 Ch_ipset -> XP tileset (entire folder)", "", () => {
                        string infile = "", outfile = "";

                        Functions.FileDialog.CreateOpenDialog("Navigate to folder of RPG Maker 2000/03 chipsets.", "", new[] {"png"}, () => { infile = Functions.FileDialog.OpenDialog.FilePath.ToString(); });
                        Functions.FileDialog.CreateSaveDialog("Where to put the converted tilesets?", "", new[] {"png"}, () => { outfile = Functions.FileDialog.SaveDialog.FilePath.ToString(); });

                        Functions.Operation.RpgMaker2K.Convert2KChipsets(Path.GetDirectoryName(infile), Path.GetDirectoryName(outfile));
                    })
                })
            });

            // add menu and main window
            Application.Top.Add(MainMenuBar, StaticWindows.Main.Window, new Label("Current operation status will be displayed here") { Id = "ProgressText", X = 1, Y = Pos.Bottom(StaticWindows.Main.Window) - 1 });

            // initalize static windows
            //StaticWindows.Create();
            StaticWindows.Main.Init();
            UpdateElements(true, true);

            // run it
            Application.Run();
        }

        public static void UpdateElements(bool forceNoOpen = false, bool forceNoProjectSave = false)
        {
            //( as Label).Text = "Project: " + StaticWindows.Open._window.FilePath;
            if (!forceNoOpen) ProjectPath = (Functions.FileDialog.OpenDialog.FilePath).ToString();
            if (Settings.Values.PersistentProject && !forceNoProjectSave && !String.IsNullOrWhiteSpace(ProjectPath)) File.WriteAllText("project", ProjectPath);

            string project = "No project loaded.";

            string version = "N/A";

            if (!String.IsNullOrEmpty(ProjectPath))
            {
                project = ProjectPath;
                string obtainedVersion = Functions.Operation.GetVersion(ProjectPath);
                version = obtainedVersion != null ? ($"{obtainedVersion} {Functions.Operation.VersionInstalled(RGSSAD.GetVersion(ProjectPath))}") : version;
            }

            (StaticWindows.Main.Window.Subviews.First().Subviews.FirstOrDefault(x => x.Id == "ProjectString") as Label).Text = $"Project: {project}\nVersion: {version}";

            //(StaticWindows.Main._window.Subviews.FirstOrDefault(x => x.Id == "ProjectString") as Label).Text = "Project: " + StaticWindows.Open._window.FilePath;
        }
    }
}

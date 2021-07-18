using RPGMakerDecrypter.Decrypter;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Terminal.Gui;

namespace rpg_patcher
{
    class Program
    {
        public static string ProjectPath = "";

        public static MenuBar mainMenuBar;

        private static void Preload()
        {
            User.Load("settings");
            if (User.Default.PersistentProject) ProjectPath = File.ReadAllText("project");

            Functions.Checks.CheckForRPGMaker();
        }

        static void Main(string[] args)
        {
            Preload();

            // initalize terminal.gui
            Application.Init();

            //Application.Top.ColorScheme =

            Style.Theme(User.Default.Theme);

            // menubar
            mainMenuBar = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_About", "", () => {
                        StaticWindows.About about = new StaticWindows.About();
                        Application.Run(about._window);
                    }),
                    new MenuItem ("_Load", "", () => {
                        Functions.FileDialog.CreateOpenDialog("Project", "Pick a project", new string[] { "rgssad", "rgss2a", "rgss3a" }, () => UpdateElements());
                    }),
                    new MenuItem ("_Settings", "", () => {
                        StaticWindows.Settings settings = new StaticWindows.Settings();
                        Application.Run(settings._window);
                    }),
                    //new MenuItem ("_Focus on main window", "", () => {
                    //    //Application.Top.SetFocus(StaticWindows.Main._window);
                    //    StaticWindows.Main._window.SetFocus();
                    //}),
                    new MenuItem ("_Quit", "", () => {
                        Application.RequestStop();
                    })
                }),

                new MenuBarItem ("_Project", new MenuItem [] {
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
                    new MenuItem ("_Create VX _Ace project files", "", () => {
                        Functions.Project.MakeProjectIndeterministic(RPGMakerVersion.VxAce);
                    }),
                    new MenuItem ("Create _MV project files", "", () => {
                        Functions.Project.MakeMVProject();
                    })
                }),

                new MenuBarItem ("_Extracting", new MenuItem [] {
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
                            Application.Run(oneFile._window);
                        });
                    }),
                    new MenuItem ("Extract _everything", "", () => {
                        Functions.FileDialog.CreateSaveDialog("Save to...", "Pick a place", new string[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.ExtractAllFiles()));
                    }),
                    new MenuItem ("Extract everything + _project files", "", () => {
                        Functions.FileDialog.CreateSaveDialog("Save to...", "Pick a place", new string[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.ExtractAllFiles(true)));
                        Functions.Project.MakeProjectWithSavePath();
                    }),
                    new MenuItem ("Extract everything + project files + _game", "", () => {
                        Functions.FileDialog.CreateSaveDialog("Save to...", "Pick a place", new string[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.ExtractAllFiles(true)));
                        Functions.Project.MakeProjectWithSavePath(true);
                        Functions.Extract.CopyGameFilesIndeterministic();
                    }),
                }),

                new MenuBarItem ("_Patching", new MenuItem [] {
                    new MenuItem ("_Working on it", "", () => {
                        Functions.Operation.ShowError("whoops");
                    })
                }),

                new MenuBarItem ("_Special", new MenuItem [] {
                    new MenuItem ("Check for _RPG Maker installs in the registry", "", () => {
                        // $"RPG Maker XP installed: {(Functions.Checks.Installed.XP ? "Yes" : "No")}\nRPG Maker VX installed: {(Functions.Checks.Installed.VX ? "Yes" : "No")}\nRPG Maker VX Ace installed: {(Functions.Checks.Installed.VXAce ? "Yes" : "No")}"
                        Button closeDialog = new Button("Close", true);
                        closeDialog.Clicked += () => { StaticWindows.Main._window.SetFocus(); Application.RequestStop(); };

                        Dialog installed = new Dialog("RPG Maker installs");
                        installed.AddButton(closeDialog);
                        installed.Add(new Label($"RPG Maker XP installed: {(Functions.Checks.Installed.XP ? "Yes" : "No")}\nRPG Maker VX installed: {(Functions.Checks.Installed.VX ? "Yes" : "No")}\nRPG Maker VX Ace installed: {(Functions.Checks.Installed.VXAce ? "Yes" : "No")}\n\nInstalled or not, this program\nwill not restrict use."));
                        installed.Width = Dim.Percent(30);
                        installed.Height = Dim.Percent(30);

                        Application.Run(installed);
                    }),
                    null,
                    new MenuItem ("RPG Maker 2000/03 Ch_arset -> XP format (entire folder)", "", () => {
                        string infile = "", outfile = "";

                        Functions.FileDialog.CreateOpenDialog("Navigate to folder of RPG Maker 2000/03 charsets.", "", new string[] {"png"}, () => { infile = Functions.FileDialog._OpenDialog.FilePath.ToString(); });
                        Functions.FileDialog.CreateSaveDialog("Where to put the converted charsets?", "", new string[] {"png"}, () => { outfile = Functions.FileDialog._SaveDialog.FilePath.ToString(); });

                        Functions.Operation.RPGMaker2k.Convert2kCharsets(Path.GetDirectoryName(infile), Path.GetDirectoryName(outfile));
                    }),
                    new MenuItem ("RPG Maker 2000/03 Ch_ipset -> XP tileset (entire folder)", "", () => {
                        string infile = "", outfile = "";

                        Functions.FileDialog.CreateOpenDialog("Navigate to folder of RPG Maker 2000/03 chipsets.", "", new string[] {"png"}, () => { infile = Functions.FileDialog._OpenDialog.FilePath.ToString(); });
                        Functions.FileDialog.CreateSaveDialog("Where to put the converted tilesets?", "", new string[] {"png"}, () => { outfile = Functions.FileDialog._SaveDialog.FilePath.ToString(); });

                        Functions.Operation.RPGMaker2k.Convert2kChipsets(Path.GetDirectoryName(infile), Path.GetDirectoryName(outfile));
                    })
                })
            });

            // add menu and main window
            Application.Top.Add(mainMenuBar, StaticWindows.Main._window, new Label("Current operation status will be displayed here") { Id = "ProgressText", X = 1, Y = Pos.Bottom(StaticWindows.Main._window) - 1 });

            // initalize static windows
            //StaticWindows.Create();
            StaticWindows.Main.Init();
            UpdateElements(true);

            // run it
            Application.Run();
        }

        public static void UpdateElements(bool forceNoOpen = false)
        {
            //( as Label).Text = "Project: " + StaticWindows.Open._window.FilePath;
            if (!forceNoOpen) ProjectPath = (Functions.FileDialog._OpenDialog.FilePath).ToString();
            if (User.Default.PersistentProject) File.WriteAllText("project", ProjectPath);

            (StaticWindows.Main._window.Subviews.First().Subviews.FirstOrDefault(x => x.Id == "ProjectString") as Label).Text = $"Project: {ProjectPath}\nVersion: {Functions.Operation.GetVersion(ProjectPath)} {Functions.Operation.VersionInstalled(RGSSAD.GetVersion(ProjectPath))}";

            //(StaticWindows.Main._window.Subviews.FirstOrDefault(x => x.Id == "ProjectString") as Label).Text = "Project: " + StaticWindows.Open._window.FilePath;
        }
    }
}

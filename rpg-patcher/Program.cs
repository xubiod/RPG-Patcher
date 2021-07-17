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

        static void Main(string[] args)
        {
            // update detector
            if (false)//!Functions.Checks.IsUpToDate())
            {
                var defaultFore = Console.ForegroundColor;
                Console.Write("This version of RPG Patcher is out of date\nPlease update it manually with .NET Core 3 from:\n");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("https://github.com/xubiod/RPG-Patcher");

                Console.ForegroundColor = defaultFore;
                Console.Write("\nPress any key to continue...");

                Console.ReadKey();
                Console.Clear();
            }

            Functions.Checks.CheckForRPGMaker();

            // initalize terminal.gui
            Application.Init();

            //Application.Top.ColorScheme =

            Style.Theme(User.Default.Theme);

            // menubar
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_About", "", () => {
                        StaticWindows.About about = new StaticWindows.About();
                        Application.Run(about._window);
                    }),
                    new MenuItem ("_Focus on main window", "", () => {
                        //Application.Top.SetFocus(StaticWindows.Main._window);
                        StaticWindows.Main._window.SetFocus();
                    }),
                    new MenuItem ("_Quit", "", () => {
                        Application.RequestStop();
                    })
                }),

                new MenuBarItem ("_Project", new MenuItem [] {
                    new MenuItem ("_Load (DO FIRST)", "", () => {
                        Functions.FileDialog.CreateOpenDialog("Project", "Pick a project", new string[] { "rgssad", "rgss2a", "rgss3a" }, Program.UpdateElements);
                    }),
                    new MenuItem ("_Create project files with loaded version", "", () => {
                        Functions.Project.MakeProject();
                    }),
                    new MenuItem ("_Create XP project files", "", () => {
                        Functions.Project.MakeProjectIndeterministic(RPGMakerVersion.Xp);
                    }),
                    new MenuItem ("_Create VX project files", "", () => {
                        Functions.Project.MakeProjectIndeterministic(RPGMakerVersion.Vx);
                    }),
                    new MenuItem ("_Create VX Ace project files", "", () => {
                        Functions.Project.MakeProjectIndeterministic(RPGMakerVersion.VxAce);
                    })
                }),

                new MenuBarItem ("_Extracting", new MenuItem [] {
                    new MenuItem ("_Load (DO FIRST)", "", () => {
                        Functions.FileDialog.CreateOpenDialog("Project", "Pick a project", new string[] { "rgssad", "rgss2a", "rgss3a" }, Program.UpdateElements);
                    }),
                    new MenuItem ("_Copy game files from project directory", "", () => {
                        Functions.Extract.CopyGameFiles();
                    }),
                    new MenuItem ("_List files in the archive", "", () => {
                        Functions.Extract.GetAllFiles();
                    }),
                    new MenuItem ("_Extract single file", "", () => {
                        Functions.Operation.ExecuteIfProjectSelected(() => {
                            StaticWindows.ExportOneFile oneFile = new StaticWindows.ExportOneFile();
                            Application.Run(oneFile._window);
                        });
                    }),
                    new MenuItem ("_Extract everything", "", () => {
                        Functions.FileDialog.CreateSaveDialog("Save to...", "Pick a place", new string[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.ExtractAllFiles()));
                    }),
                    new MenuItem ("_Extract everything + project files", "", () => {
                        Functions.FileDialog.CreateSaveDialog("Save to...", "Pick a place", new string[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.ExtractAllFiles(true)));
                        Functions.Project.MakeProjectWithSavePath();
                    }),
                    new MenuItem ("_Extract everything + project files + game", "", () => {
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
                    new MenuItem ("_RPG Maker 2000/03 Charset -> XP format (entire folder)", "", () => {
                        string infile = "", outfile = "";

                        Functions.FileDialog.CreateOpenDialog("Navigate to folder of RPG Maker 2000/03 charsets.", "", new string[] {"png"}, () => { infile = Functions.FileDialog._OpenDialog.FilePath.ToString(); });
                        Functions.FileDialog.CreateSaveDialog("Where to put the converted charsets?", "", new string[] {"png"}, () => { outfile = Functions.FileDialog._SaveDialog.FilePath.ToString(); });

                        Functions.Operation.RPGMaker2k.Convert2kCharsets(Path.GetDirectoryName(infile), Path.GetDirectoryName(outfile));
                    }),
                    new MenuItem ("_RPG Maker 2000/03 Chipset -> XP tileset (entire folder)", "", () => {
                        string infile = "", outfile = "";

                        Functions.FileDialog.CreateOpenDialog("Navigate to folder of RPG Maker 2000/03 chipsets.", "", new string[] {"png"}, () => { infile = Functions.FileDialog._OpenDialog.FilePath.ToString(); });
                        Functions.FileDialog.CreateSaveDialog("Where to put the converted tilesets?", "", new string[] {"png"}, () => { outfile = Functions.FileDialog._SaveDialog.FilePath.ToString(); });

                        Functions.Operation.RPGMaker2k.Convert2kChipsets(Path.GetDirectoryName(infile), Path.GetDirectoryName(outfile));
                    })
                }),

                new MenuBarItem ("_Settings", new MenuItem [] {
                    new MenuItem ("_Open settings window", "", () => {
                        StaticWindows.Settings settings = new StaticWindows.Settings();
                        Application.Run(settings._window);
                    })
                })
            });

            // add menu and main window
            Application.Top.Add(menu, StaticWindows.Main._window, new Label("Current operation status will be displayed here") { Id = "ProgressText", X = 1, Y = Pos.Bottom(StaticWindows.Main._window) - 1 });

            // initalize static windows
            //StaticWindows.Create();
            StaticWindows.Main.Init();

            // run it
            Application.Run();
        }

        public static void UpdateElements()
        {
            //( as Label).Text = "Project: " + StaticWindows.Open._window.FilePath;
            ProjectPath = (Functions.FileDialog._OpenDialog.FilePath).ToString();

            (StaticWindows.Main._window.Subviews.First().Subviews.FirstOrDefault(x => x.Id == "ProjectString") as Label).Text = "Project: " + ProjectPath + "\nVersion: " + Functions.Operation.GetVersion(ProjectPath);

            //(StaticWindows.Main._window.Subviews.FirstOrDefault(x => x.Id == "ProjectString") as Label).Text = "Project: " + StaticWindows.Open._window.FilePath;
        }
    }
}

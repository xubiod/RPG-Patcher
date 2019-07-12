﻿using RPGMakerDecrypter.Decrypter;
using System;
using System.Linq;
using Terminal.Gui;
using TGAttribute = Terminal.Gui.Attribute;

namespace rpg_patcher
{
    class Program
    {
        public static string ProjectPath = "";

        static void Main(string[] args)
        {
            // initalize terminal.gui
            Application.Init();

            //Application.Top.ColorScheme = 

            Mode(0);

            // menubar
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_About", "", () => {
                        Application.Run(StaticWindows.About._window);
                    }),
                    new MenuItem ("_Focus on main window", "", () => {
                        Application.Top.SetFocus(StaticWindows.Main._window);
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

                new MenuBarItem ("_Exporting", new MenuItem [] {
                    new MenuItem ("_Load (DO FIRST)", "", () => {
                        Functions.FileDialog.CreateOpenDialog("Project", "Pick a project", new string[] { "rgssad", "rgss2a", "rgss3a" }, Program.UpdateElements);
                    }),
                    new MenuItem ("_Copy game files from project directory", "", () => {
                        Functions.Export.CopyGameFiles();
                    }),
                    new MenuItem ("_List files in the archive", "", () => {
                        Functions.Export.GetAllFiles();
                    }),
                    new MenuItem ("_Export everything", "", () => {
                        Functions.FileDialog.CreateSaveDialog("Save to...", "Pick a place", new string[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Export.ExtractAllFiles()));
                    }),
                    new MenuItem ("_Export everything + project files", "", () => {
                        Functions.FileDialog.CreateSaveDialog("Save to...", "Pick a place", new string[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Export.ExtractAllFiles(true)));
                        Functions.Project.MakeProjectWithSavePath();
                    }),
                    new MenuItem ("_Export everything + project files + game", "", () => {
                        Functions.FileDialog.CreateSaveDialog("Save to...", "Pick a place", new string[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Export.ExtractAllFiles(true)));
                        Functions.Project.MakeProjectWithSavePath(true);
                        Functions.Export.CopyGameFilesIndeterministic();
                    }),
                }),

                new MenuBarItem ("_Settings", new MenuItem [] {
                    new MenuItem ("_Open settings window", "", () => {
                        Application.Run(StaticWindows.Settings._window);
                    })
                })
            });

            // add menu and main window
            Application.Top.Add(menu, StaticWindows.Main._window);

            // initalize static windows
            StaticWindows.Create();
            Functions.Operation.Init();

            // run it
            Application.Run();
        }

        public static void Mode(int mode)
        {
            switch (mode)
            {
                case 0:
                    {
                        Colors.Base = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Black),
                            HotNormal = TGAttribute.Make(Color.White, Color.DarkGray),
                            Focus = TGAttribute.Make(Color.White, Color.Blue),
                            HotFocus = TGAttribute.Make(Color.BrighCyan, Color.Blue)
                        };

                        Colors.Menu = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.Gray),
                            Focus = TGAttribute.Make(Color.White, Color.Blue),
                            HotNormal = TGAttribute.Make(Color.DarkGray, Color.Gray),
                            HotFocus = TGAttribute.Make(Color.BrighCyan, Color.Blue)
                        };

                        Colors.Dialog = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.White, Color.DarkGray),
                            Focus = TGAttribute.Make(Color.White, Color.Blue),
                            HotNormal = TGAttribute.Make(Color.Blue, Color.White),
                            HotFocus = TGAttribute.Make(Color.BrighCyan, Color.Blue)
                        };
                        break;
                    }

                case 1:
                    {
                        Colors.Base = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.Gray),
                            HotNormal = TGAttribute.Make(Color.White, Color.DarkGray),

                            Focus = TGAttribute.Make(Color.White, Color.Brown),
                            HotFocus = TGAttribute.Make(Color.BrightYellow, Color.Brown)
                        };

                        Colors.Menu = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.Gray),
                            Focus = TGAttribute.Make(Color.White, Color.Blue),
                            HotNormal = TGAttribute.Make(Color.DarkGray, Color.Gray),
                            HotFocus = TGAttribute.Make(Color.BrighCyan, Color.Blue)
                        };

                        Colors.Dialog = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.White),
                            Focus = TGAttribute.Make(Color.White, Color.Brown),
                            HotNormal = TGAttribute.Make(Color.Brown, Color.White),
                            HotFocus = TGAttribute.Make(Color.BrightYellow, Color.Brown)
                        };
                        break;
                    }

                default: { break; }
            }

            StaticWindows.RefreshColors();
            Application.Refresh();
        }

        public static void UpdateElements()
        {
            //( as Label).Text = "Project: " + StaticWindows.Open._window.FilePath;
            ProjectPath = (Functions.FileDialog._OpenDialog.DirectoryPath + "\\" + Functions.FileDialog._OpenDialog.FilePath).ToString();

            (StaticWindows.Main._window.Subviews.First().Subviews.FirstOrDefault(x => x.Id == "ProjectString") as Label).Text = "Project: " + ProjectPath + "\nVersion: " + Functions.Operation.GetVersion(ProjectPath);

            //(StaticWindows.Main._window.Subviews.FirstOrDefault(x => x.Id == "ProjectString") as Label).Text = "Project: " + StaticWindows.Open._window.FilePath;
        }
    }
}

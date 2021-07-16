using RPGMakerDecrypter.Decrypter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace rpg_patcher
{
    static class StaticWindows
    {
        public static void Create()
        {
            About.Init();
            Main.Init();
            Settings.Init();
            ExportOneFile.Init();
        }

        public static void RefreshColors()
        {
            About.RefreshColors();
            Main.RefreshColors();
            Settings.RefreshColors();
            ExportOneFile.RefreshColors();
        }

        public static class About
        {
            public static Window _window = new Window("About");

            public static Window Init()
            {
                quit.Clicked += () => { Main._window.SetFocus(); Application.RequestStop(); };

                _window.Add(content);
                _window.Width = content.Width + 4;
                _window.Height = content.Height + 5;

                _window.X = Pos.Center();
                _window.Y = Pos.Center();

                _window.Add(quit);

                // _window.ColorScheme = Application.Top.ColorScheme;

                return _window;
            }

            public static void RefreshColors()
            {
                _window.ColorScheme = Colors.Base;
            }

            static Label content = new Label("RPG Patcher\n\nDeveloped by xubiod 2019\n\nTerminal.GUI (GUI.cs) was developed by migueldeicaza\n\nRPGMakerDecrypter was developed by uuksu\nPort to .NET Core by xubiod")
            {
                X = Pos.Center(),
                Y = Pos.Center()
            };

            static Button quit = new Button("Close", true)
            {
                X = Pos.Center(),
                Y = Pos.Bottom(_window) - Pos.Y(_window) - 3
            };
        }

        public static class Main
        {
            public static Window _window = new Window("RPG Patcher");

            public static void Init()
            {
                _window.X = 0;
                _window.Y = 1;
                _window.Width = Dim.Fill();
                _window.Height = Dim.Fill();

                (_contents[0] as Button).Clicked += () => Functions.FileDialog.CreateOpenDialog("Project", "Pick a project", new string[] { "rgssad", "rgss2a", "rgss3a" }, Program.UpdateElements);
                (_contents[4] as Button).Clicked += () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.GetAllFiles());
                (_contents[6] as Button).Clicked += () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.CopyGameFiles());
                (_contents[8] as Button).Clicked += () => Functions.FileDialog.CreateSaveDialog("Project", "Pick a project", new string[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.ExtractAllFiles()));
                (_contents[10] as Button).Clicked += () => { Functions.Operation.ExecuteIfProjectSelected(() => Functions.Project.MakeProject()); };

                _window.Add(_contents.ToArray());
            }

            public static void RefreshColors()
            {
                _window.ColorScheme = Colors.Base;

                if (_window.Subviews.First().Subviews.Count != 0) _window.Subviews.First().Subviews.FirstOrDefault(x => x.Id == "ProjectString").ColorScheme = Program.HighlighedLabel;
            }

            static List<View> _contents = new List<View>()
            {
                new Button("Load", true)
                {
                    X = Pos.At(2),
                    Y = Pos.At(2)
                },
                new Label("Loads an RPG Maker project.")
                {
                    X = Pos.At(16),
                    Y = Pos.At(2)
                },
                new Label("Project: No project loaded.\nVersion: N/A")
                {
                    X = Pos.At(2),
                    Y = Pos.At(4),
                    Id = "ProjectString",
                    ColorScheme = Program.HighlighedLabel,
                    Width = 64
                },
                new Label("Here's some of the more common actions you can do:")
                {
                    X = Pos.At(2),
                    Y = Pos.At(7)
                },
                new Button("Show Info", true)
                {
                    X = Pos.At(3),
                    Y = Pos.At(9)
                },
                new Label("Lists infromation about the archive.")
                {
                    X = Pos.At(23),
                    Y = Pos.At(9)
                },
                new Button("Copy Files", true)
                {
                    X = Pos.At(3),
                    Y = Pos.At(11)
                },
                new Label("Copies game files. Use as backup or \"deeper\" modding.")
                {
                    X = Pos.At(23),
                    Y = Pos.At(11)
                },
                new Button("Extract All", true)
                {
                    X = Pos.At(3),
                    Y = Pos.At(13)
                },
                new Label("Extracts all encrypted files from the project.")
                {
                    X = Pos.At(23),
                    Y = Pos.At(13)
                },
                new Button("Make Project", true)
                {
                    X = Pos.At(3),
                    Y = Pos.At(15)
                },
                new Label("Makes a project file in the loaded project's version.")
                {
                    X = Pos.At(23),
                    Y = Pos.At(15)
                },
                new Label("You can do more or multiple actions at once via the menu bar. Press Alt\nand the beginning letter of the item to open it.")
                {
                    X = Pos.At(2),
                    Y = Pos.At(18)
                }
            };
        }

        public static class Settings
        {
            public static Window _window = new Window("Settings");

            public static Window Init()
            {
                (content[1] as RadioGroup).SelectedItemChanged += (RadioGroup.SelectedItemChangedArgs x) => { BytePref = x.SelectedItem; };
                (content[3] as RadioGroup).SelectedItemChanged += (RadioGroup.SelectedItemChangedArgs x) => { Program.Theme(x.SelectedItem); };
                (content[5] as RadioGroup).SelectedItemChanged += (RadioGroup.SelectedItemChangedArgs x) => { OverwriteFiles = x.SelectedItem == 0; };

                _window.Add(content);
                _window.Width = Dim.Percent(80);
                _window.Height = Dim.Percent(80);

                _window.X = Pos.Center();
                _window.Y = Pos.Center();

                quit.Clicked += () => { Main._window.SetFocus(); Application.RequestStop(); };

                _window.Add(quit);

                return _window;
            }

            public static void RefreshColors()
            {
                _window.ColorScheme = Colors.Base;
            }

            public static int BytePref = 0;
            public static bool OverwriteFiles = true;

            static View[] content = {
                new Label(1, 1, "Byte Representation"),

                new RadioGroup(2, 2, new NStack.ustring[] {"Kibibyte/Mebibyte (Windows Default)", "Kilobyte/Megabyte", "Only bytes"}, 0),

                new Label(1, 6, "Theme"),

                new RadioGroup(2, 7, new NStack.ustring[] {"Dark (Default)", "Light"}, 0),

                new Label(1, 10, "File Behaviour"),

                new RadioGroup(2, 11, new NStack.ustring[] {"Always Overwrite (Default)", "Do not overwrite"}, 0)
            };

            static Button quit = new Button("Close", true)
            {
                X = Pos.Center(),
                Y = Pos.Bottom(_window) - Pos.Y(_window) - 3
            };
        }

        public static class ExportOneFile
        {
            public static Window _window = new Window("Export One File");

            public static Window Init()
            {
                _window.Add(content);
                _window.Width = Dim.Sized(52);
                _window.Height = Dim.Sized(8);

                _window.X = Pos.Center();
                _window.Y = Pos.Center();

                save.Clicked += () => Functions.FileDialog.CreateSaveDialog("Place", "Pick a place", new string[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.FindAndExtractFile()));
                quit.Clicked += () => { Main._window.SetFocus(); Application.RequestStop(); };

                _window.Add(save, quit);

                return _window;
            }

            public static void RefreshColors()
            {
                _window.ColorScheme = Colors.Base;
            }

            public static string GetFile()
            {
                return (_window.Subviews.First().Subviews.FirstOrDefault(x => (x as TextField ?? new TextField("x")).Id == "FileToExport") as TextField).Text.ToString();
            }

            public static int BytePref = 0;
            public static bool OverwriteFiles = true;

            static View[] content = {
                new Label(1, 1, "File from Archive (list directory!)"),

                new TextField(1, 2, 48, "")
                {
                    Id = "FileToExport"
                }
            };

            static Button save = new Button("Export")
            {
                X = Pos.At(1),
                Y = Pos.At(4)
            };

            static Button quit = new Button("Close", true)
            {
                X = Pos.Center(),
                Y = Pos.Bottom(_window) - Pos.Y(_window) - 3
            };
        }
    }
}

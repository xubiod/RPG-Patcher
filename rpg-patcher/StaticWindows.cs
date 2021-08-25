using System.Collections.Generic;
using System.Linq;
using NStack;
using RPGMakerDecrypter.Decrypter;
using Terminal.Gui;

namespace rpg_patcher
{
    internal static class StaticWindows
    {
        public static class Globals
        {
            public static string ExportOneFileGet;
        }

        public class About
        {
            public About() => Init();

            public Window Window = new Window("About");

            public Window Init()
            {
                Window.X = Pos.Center();
                Window.Y = Pos.Center();

                SetupElements();

                Window.Width = _content.Width + 4;
                Window.Height = _content.Height + 5;

                Window.Add(_content);
                Window.Add(_quit);

                // _window.ColorScheme = Application.Top.ColorScheme;

                return Window;
            }

            private void SetupElements()
            {
                _content = new Label($"RPG Patcher (patch {ThisAssembly.Git.Commit})\n\nDeveloped by xubiod 2019-2021\n\nFor full external resource credit, please visit the repo page.\n{ThisAssembly.Git.RepositoryUrl}")
                {
                    X = Pos.Center(),
                    Y = Pos.Center()
                };

                _quit = new Button("Close", true)
                {
                    X = Pos.Center(),
                    Y = Pos.Bottom(Window) - Pos.Y(Window) - 3
                };

                _quit.Clicked += () => { Main.Window.SetFocus(); Application.RequestStop(); };
            }

            public void RefreshColors()
            {
                Window.ColorScheme = Colors.Base;
            }

            private Label _content;

            private Button _quit;
        }

        public static class Main
        {
            public static Window Window = new Window("RPG Patcher");

            public static Window Init()
            {
                Window.X = 0;
                Window.Y = 1;
                Window.Width = Dim.Fill();
                Window.Height = Dim.Fill();

                SetupElements();

                Window.Add(Contents.ToArray());

                return Window;
            }

            private static void SetupElements()
            {
                (Contents[0] as Button).Clicked += () => Functions.FileDialog.CreateOpenDialog("Project", "Pick a project", new[] { Constants.RpgMakerXpArchiveName, Constants.RpgMakerVxArchiveName, Constants.RpgMakerVxAceArchiveName }, () => Program.UpdateElements());
                (Contents[4] as Button).Clicked += () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.GetAllFiles());
                (Contents[6] as Button).Clicked += () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.CopyGameFiles());
                (Contents[8] as Button).Clicked += () => Functions.FileDialog.CreateSaveDialog("Project", "Pick a project", new[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.ExtractAllFiles()));
                (Contents[10] as Button).Clicked += () => { Functions.Operation.ExecuteIfProjectSelected(() => Functions.Project.MakeProject()); };
            }

            public static void RefreshColors()
            {
                Window.ColorScheme = Colors.Base;

                if (Window.Subviews.First().Subviews.Count != 0) Window.Subviews.First().Subviews.FirstOrDefault(x => x.Id == "ProjectString").ColorScheme = Style.HighlighedLabel;
            }

            private static readonly List<View> Contents = new List<View>
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
                    ColorScheme = Style.HighlighedLabel,
                    Width = Dim.Fill()
                },
                new Label("Here's some of the more common actions you can do:")
                {
                    X = Pos.At(2),
                    Y = Pos.At(7)
                },
                new Button("Show Info")
                {
                    X = Pos.At(3),
                    Y = Pos.At(9)
                },
                new Label("Lists infromation about the archive.")
                {
                    X = Pos.At(23),
                    Y = Pos.At(9)
                },
                new Button("Copy Files")
                {
                    X = Pos.At(3),
                    Y = Pos.At(11)
                },
                new Label("Copies game files. Use as backup or \"deeper\" modding.")
                {
                    X = Pos.At(23),
                    Y = Pos.At(11)
                },
                new Button("Extract All")
                {
                    X = Pos.At(3),
                    Y = Pos.At(13)
                },
                new Label("Extracts all encrypted files from the project.")
                {
                    X = Pos.At(23),
                    Y = Pos.At(13)
                },
                new Button("Make Project")
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

        public class Settings
        {
            public Window Window = new Window("Settings");

            public Settings() => Init();

            public Window Init()
            {
                Window.Width = Dim.Percent(50);
                Window.Height = Dim.Percent(80);

                Window.X = Pos.Center();
                Window.Y = Pos.Center();

                SetupElements();

                Window.Add(_scrollBarView);
                Window.Add(_quit);

                return Window;
            }

            private void SetupElements()
            {
                (Content[1] as RadioGroup).SelectedItem = rpg_patcher.Settings.Values.BytePref;
                (Content[3] as RadioGroup).SelectedItem = rpg_patcher.Settings.Values.OverwriteFiles ? 0 : 1;
                (Content[5] as RadioGroup).SelectedItem = rpg_patcher.Settings.Values.PersistentProject ? 1 : 0;
                (Content[7] as RadioGroup).SelectedItem = rpg_patcher.Settings.Values.Theme;

                (Content[1] as RadioGroup).SelectedItemChanged += x => { rpg_patcher.Settings.Values.BytePref = x.SelectedItem; };
                (Content[3] as RadioGroup).SelectedItemChanged += x => { rpg_patcher.Settings.Values.OverwriteFiles = x.SelectedItem == 0; };
                (Content[5] as RadioGroup).SelectedItemChanged += x => { rpg_patcher.Settings.Values.PersistentProject = x.SelectedItem == 1; };
                (Content[7] as RadioGroup).SelectedItemChanged += x => { Style.Theme(x.SelectedItem); RefreshColors(); };

                _quit = new Button("Close", true)
                {
                    X = Pos.Center(),
                    Y = Pos.Bottom(Window) - Pos.Y(Window) - 3
                };

                _quit.Clicked += () => { rpg_patcher.Settings.Save("settings"); Main.Window.SetFocus(); Application.RequestStop(); };

                _scrollBarView.X = 0;
                _scrollBarView.Y = 0;

                _scrollBarView.ContentSize = new Size(40, 30);

                _scrollBarView.Width = Dim.Fill();
                _scrollBarView.Height = Dim.Fill(3);

                _scrollBarView.Add(Content);
            }

            public void RefreshColors()
            {
                Window.ColorScheme = Colors.Base;
                Application.Refresh();
            }

            public int BytePref = 0;
            public bool OverwriteFiles = true;

            private static readonly View[] Content = {
                new Label(1, 0, "Byte Representation"),

                new RadioGroup(2, 1, new ustring[] {"Kibibyte/Mebibyte (Windows Default)", "Kilobyte/Megabyte", "Only bytes"}),

                new Label(1, 5, "File Behaviour"),

                new RadioGroup(2, 6, new ustring[] {"Always Overwrite (Default)", "Do not overwrite"}),

                new Label(1, 9, "Store last used project"),

                new RadioGroup(2, 10, new ustring[] {"No (Default)", "Yes"}),

                new Label(1, 13, "Theme"),

                new RadioGroup(2, 14, Style.ThemeNamesAsUstringArray())
            };

            private Button _quit;
            private readonly ScrollView _scrollBarView = new ScrollView();
        }

        public class ExportOneFile
        {
            public Window Window = new Window("Export One File");

            public ExportOneFile() => Init();
            public Window Init()
            {
                Window.Add(Content);
                Window.Width = Dim.Sized(52);
                Window.Height = Dim.Sized(8);

                Window.X = Pos.Center();
                Window.Y = Pos.Center();

                SetupElements();

                Window.Add(Save, _quit);

                return Window;
            }

            private void SetupElements()
            {
                Save.Clicked += () => Functions.FileDialog.CreateSaveDialog("Place", "Pick a place", new[] { "" }, () => Functions.Operation.ExecuteIfProjectSelected(() => Functions.Extract.FindAndExtractFile()));

                _quit = new Button("Close", true)
                {
                    X = Pos.Center(),
                    Y = Pos.Bottom(Window) - Pos.Y(Window) - 3
                };

                _quit.Clicked += () => { GetFile(); Main.Window.SetFocus(); Application.RequestStop(); };
            }

            public void RefreshColors()
            {
                Window.ColorScheme = Colors.Base;
            }

            public string GetFile()
            {
                Globals.ExportOneFileGet = (Window.Subviews.First().Subviews.FirstOrDefault(x => (x as TextField ?? new TextField("x")).Id == "FileToExport") as TextField).Text.ToString();
                return Globals.ExportOneFileGet;
            }

            public int BytePref = 0;
            public bool OverwriteFiles = true;

            private static readonly View[] Content = {
                new Label(1, 1, "File from Archive (list directory!)"),

                new TextField(1, 2, 48, "")
                {
                    Id = "FileToExport"
                }
            };

            private static readonly Button Save = new Button("Export")
            {
                X = Pos.At(1),
                Y = Pos.At(4)
            };

            private static Button _quit;
        }
    }
}

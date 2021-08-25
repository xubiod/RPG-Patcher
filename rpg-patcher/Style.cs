using System.Collections.Generic;
using System.Linq;
using NStack;
using Terminal.Gui;
using TGAttribute = Terminal.Gui.Attribute;

namespace rpg_patcher
{
    internal class Style
    {
        public static ColorScheme ArchivedList;
        public static ColorScheme HighlighedLabel;

        public const int ThemeDark = 0;
        public const int ThemeDarker = 1;
        public const int ThemeDarkerer = 2;
        public const int ThemeLight = 3;
        public const int ThemeLighter = 4;
        public const int ThemeOldBsod = 5;
        public const int ThemeTerminal = 6;
        public const int ThemeMonochrome = 7;
        public const int ThemeMonochromeDark = 8;

        public static readonly Dictionary<string, int> Themes = new Dictionary<string, int>
        {
            { "Dark", ThemeDark }, { "Dark+", ThemeDarker }, { "Darker", ThemeDarkerer }, { "Light", ThemeLight }, { "Lighter", ThemeLighter }, { "Old BSOD", ThemeOldBsod }, { "Terminal", ThemeTerminal },
            { "Monochrome Bright", ThemeMonochrome }, { "Monochrome Dark", ThemeMonochromeDark }
        };

        public static ustring[] ThemeNamesAsUstringArray()
        {
            ustring[] results = new ustring[Themes.Keys.Count];
            string[] tmp = Themes.Keys.ToArray();
            int i;

            for (i = 0; i < Themes.Count; i++)
            {
                results[i] = ustring.Make(tmp[i] + (i == 0 ? " (default)" : ""));
            }

            return results;
        }

        public static void Theme(string name)
        {
            Theme(Themes[name]);
        }

        public static void Theme(int mode)
        {
            switch (mode)
            {
                case ThemeDark:
                    {
                        Colors.Base = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Black),
                            Focus = TGAttribute.Make(Color.White, Color.Blue),
                            HotNormal = TGAttribute.Make(Color.White, Color.DarkGray),
                            HotFocus = TGAttribute.Make(Color.BrightCyan, Color.Blue)
                        };

                        Colors.Menu = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.Gray),
                            Focus = TGAttribute.Make(Color.White, Color.Blue),
                            HotNormal = TGAttribute.Make(Color.DarkGray, Color.Gray),
                            HotFocus = TGAttribute.Make(Color.BrightCyan, Color.Blue)
                        };

                        Colors.Dialog = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.White, Color.DarkGray),
                            Focus = TGAttribute.Make(Color.White, Color.Blue),
                            HotNormal = TGAttribute.Make(Color.Blue, Color.White),
                            HotFocus = TGAttribute.Make(Color.BrightCyan, Color.Blue)
                        };

                        ArchivedList = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Blue),
                            Focus = TGAttribute.Make(Color.White, Color.BrightBlue),
                            HotNormal = TGAttribute.Make(Color.White, Color.Blue),
                            HotFocus = TGAttribute.Make(Color.White, Color.BrightBlue)
                        };

                        HighlighedLabel = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.BrightYellow, Color.Black)
                        };
                        break;
                    }

                case ThemeDarker:
                    {
                        Colors.Base = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Black),
                            Focus = TGAttribute.Make(Color.White, Color.Blue),
                            HotNormal = TGAttribute.Make(Color.White, Color.DarkGray),
                            HotFocus = TGAttribute.Make(Color.BrightCyan, Color.Blue)
                        };

                        Colors.Menu = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.White, Color.DarkGray),
                            Focus = TGAttribute.Make(Color.White, Color.Blue),
                            HotNormal = TGAttribute.Make(Color.DarkGray, Color.Gray),
                            HotFocus = TGAttribute.Make(Color.BrightCyan, Color.Blue)
                        };

                        Colors.Dialog = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Black),
                            Focus = TGAttribute.Make(Color.White, Color.Blue),
                            HotNormal = TGAttribute.Make(Color.Blue, Color.White),
                            HotFocus = TGAttribute.Make(Color.BrightCyan, Color.Blue)
                        };

                        ArchivedList = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Blue),
                            Focus = TGAttribute.Make(Color.White, Color.BrightBlue),
                            HotNormal = TGAttribute.Make(Color.White, Color.Blue),
                            HotFocus = TGAttribute.Make(Color.White, Color.BrightBlue)
                        };

                        HighlighedLabel = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.BrightYellow, Color.Black)
                        };
                        break;
                    }

                case ThemeDarkerer:
                    {
                        Colors.Base = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Gray, Color.Black),
                            Focus = TGAttribute.Make(Color.Gray, Color.DarkGray),
                            HotNormal = TGAttribute.Make(Color.Gray, Color.DarkGray),
                            HotFocus = TGAttribute.Make(Color.DarkGray, Color.Black)
                        };

                        Colors.Menu = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Gray, Color.Black),
                            Focus = TGAttribute.Make(Color.Gray, Color.DarkGray),
                            HotNormal = TGAttribute.Make(Color.Gray, Color.DarkGray),
                            HotFocus = TGAttribute.Make(Color.DarkGray, Color.Black)
                        };

                        Colors.Dialog = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Gray, Color.Black),
                            Focus = TGAttribute.Make(Color.Gray, Color.DarkGray),
                            HotNormal = TGAttribute.Make(Color.Gray, Color.DarkGray),
                            HotFocus = TGAttribute.Make(Color.DarkGray, Color.Black)
                        };

                        ArchivedList = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Gray, Color.Black),
                            Focus = TGAttribute.Make(Color.Gray, Color.DarkGray),
                            HotNormal = TGAttribute.Make(Color.Gray, Color.DarkGray),
                            HotFocus = TGAttribute.Make(Color.DarkGray, Color.Black)
                        };

                        HighlighedLabel = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Blue, Color.Black)
                        };
                        break;
                    }

                case ThemeLight:
                    {
                        Colors.Base = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.Gray),
                            Focus = TGAttribute.Make(Color.White, Color.Brown),
                            HotNormal = TGAttribute.Make(Color.White, Color.DarkGray),
                            HotFocus = TGAttribute.Make(Color.BrightYellow, Color.Brown)
                        };

                        Colors.Menu = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.Gray),
                            Focus = TGAttribute.Make(Color.White, Color.Blue),
                            HotNormal = TGAttribute.Make(Color.DarkGray, Color.Gray),
                            HotFocus = TGAttribute.Make(Color.BrightCyan, Color.Blue)
                        };

                        Colors.Dialog = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.White),
                            Focus = TGAttribute.Make(Color.White, Color.Brown),
                            HotNormal = TGAttribute.Make(Color.Brown, Color.White),
                            HotFocus = TGAttribute.Make(Color.BrightYellow, Color.Brown)
                        };

                        ArchivedList = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Brown),
                            Focus = TGAttribute.Make(Color.Black, Color.BrightYellow),
                            HotNormal = TGAttribute.Make(Color.White, Color.Brown),
                            HotFocus = TGAttribute.Make(Color.Black, Color.BrightYellow)
                        };

                        HighlighedLabel = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.BrightBlue, Color.Gray)
                        };
                        break;
                    }

                case ThemeOldBsod:
                    {
                        Colors.Base = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Gray, Color.Blue),
                            Focus = TGAttribute.Make(Color.Blue, Color.Gray),
                            HotNormal = TGAttribute.Make(Color.Blue, Color.Gray),
                            HotFocus = TGAttribute.Make(Color.Gray, Color.Blue)
                        };

                        Colors.Menu = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Gray, Color.Blue),
                            Focus = TGAttribute.Make(Color.Blue, Color.Gray),
                            HotNormal = TGAttribute.Make(Color.Gray, Color.Blue),
                            HotFocus = TGAttribute.Make(Color.Blue, Color.Gray)
                        };

                        Colors.Dialog = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Gray, Color.Blue),
                            Focus = TGAttribute.Make(Color.Blue, Color.Gray),
                            HotNormal = TGAttribute.Make(Color.Gray, Color.Blue),
                            HotFocus = TGAttribute.Make(Color.Blue, Color.Gray)
                        };

                        ArchivedList = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Blue, Color.Gray),
                            Focus = TGAttribute.Make(Color.Gray, Color.Blue),
                            HotNormal = TGAttribute.Make(Color.Blue, Color.Gray),
                            HotFocus = TGAttribute.Make(Color.Gray, Color.Blue)
                        };

                        HighlighedLabel = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Blue)
                        };
                        break;
                    }

                case ThemeTerminal:
                    {
                        Colors.Base = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            Focus = TGAttribute.Make(Color.Black, Color.BrightGreen),
                            HotNormal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            HotFocus = TGAttribute.Make(Color.Black, Color.BrightGreen)
                        };

                        Colors.Menu = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            Focus = TGAttribute.Make(Color.Black, Color.BrightGreen),
                            HotNormal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            HotFocus = TGAttribute.Make(Color.Black, Color.BrightGreen)
                        };

                        Colors.Dialog = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            Focus = TGAttribute.Make(Color.Black, Color.BrightGreen),
                            HotNormal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            HotFocus = TGAttribute.Make(Color.Black, Color.BrightGreen)
                        };

                        ArchivedList = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            Focus = TGAttribute.Make(Color.Black, Color.BrightGreen),
                            HotNormal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            HotFocus = TGAttribute.Make(Color.Black, Color.BrightGreen)
                        };

                        HighlighedLabel = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.BrightYellow, Color.Black)
                        };
                        break;
                    }

                case ThemeLighter:
                    {
                        Colors.Base = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.White),
                            Focus = TGAttribute.Make(Color.Black, Color.BrightYellow),
                            HotNormal = TGAttribute.Make(Color.Black, Color.Gray),
                            HotFocus = TGAttribute.Make(Color.BrightYellow, Color.Black)
                        };

                        Colors.Menu = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.White),
                            Focus = TGAttribute.Make(Color.Black, Color.BrightYellow),
                            HotNormal = TGAttribute.Make(Color.Black, Color.Gray),
                            HotFocus = TGAttribute.Make(Color.BrightYellow, Color.Black)
                        };

                        Colors.Dialog = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.White),
                            Focus = TGAttribute.Make(Color.Black, Color.BrightYellow),
                            HotNormal = TGAttribute.Make(Color.Black, Color.Gray),
                            HotFocus = TGAttribute.Make(Color.BrightYellow, Color.Black)
                        };

                        ArchivedList = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.White),
                            Focus = TGAttribute.Make(Color.Black, Color.BrightYellow),
                            HotNormal = TGAttribute.Make(Color.Black, Color.Gray),
                            HotFocus = TGAttribute.Make(Color.BrightYellow, Color.Black)
                        };

                        HighlighedLabel = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.BrightRed, Color.White)
                        };
                        break;
                    }

                case ThemeMonochrome:
                    {
                        Colors.Base = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.White),
                            Focus = TGAttribute.Make(Color.White, Color.Black),
                            HotNormal = TGAttribute.Make(Color.Black, Color.White),
                            HotFocus = TGAttribute.Make(Color.White, Color.Black)
                        };

                        Colors.Menu = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.White),
                            Focus = TGAttribute.Make(Color.White, Color.Black),
                            HotNormal = TGAttribute.Make(Color.Black, Color.White),
                            HotFocus = TGAttribute.Make(Color.White, Color.Black)
                        };

                        Colors.Dialog = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.White),
                            Focus = TGAttribute.Make(Color.White, Color.Black),
                            HotNormal = TGAttribute.Make(Color.Black, Color.White),
                            HotFocus = TGAttribute.Make(Color.White, Color.Black)
                        };

                        ArchivedList = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.White),
                            Focus = TGAttribute.Make(Color.White, Color.Black),
                            HotNormal = TGAttribute.Make(Color.Black, Color.White),
                            HotFocus = TGAttribute.Make(Color.White, Color.Black)
                        };

                        HighlighedLabel = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.White)
                        };
                        break;
                    }

                case ThemeMonochromeDark:
                    {
                        Colors.Base = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Black),
                            Focus = TGAttribute.Make(Color.Black, Color.White),
                            HotNormal = TGAttribute.Make(Color.White, Color.Black),
                            HotFocus = TGAttribute.Make(Color.Black, Color.White),
                        };

                        Colors.Menu = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Black),
                            Focus = TGAttribute.Make(Color.Black, Color.White),
                            HotNormal = TGAttribute.Make(Color.White, Color.Black),
                            HotFocus = TGAttribute.Make(Color.Black, Color.White),
                        };

                        Colors.Dialog = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Black),
                            Focus = TGAttribute.Make(Color.Black, Color.White),
                            HotNormal = TGAttribute.Make(Color.White, Color.Black),
                            HotFocus = TGAttribute.Make(Color.Black, Color.White),
                        };

                        ArchivedList = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Black),
                            Focus = TGAttribute.Make(Color.Black, Color.White),
                            HotNormal = TGAttribute.Make(Color.White, Color.Black),
                            HotFocus = TGAttribute.Make(Color.Black, Color.White),
                        };

                        HighlighedLabel = new ColorScheme
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Black)
                        };
                        break;
                    }
            }

            //StaticWindows.RefreshColors();
            StaticWindows.Main.RefreshColors();
            if (Program.MainMenuBar != null) Program.MainMenuBar.ColorScheme = Colors.Menu;
            Settings.Values.Theme = mode;

            Application.Refresh();
        }
    }
}

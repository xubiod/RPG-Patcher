using NStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;
using TGAttribute = Terminal.Gui.Attribute;

namespace rpg_patcher
{
    class Style
    {
        public static ColorScheme ArchivedList;
        public static ColorScheme HighlighedLabel;

        public const int Theme_Dark = 0;
        public const int Theme_Light = 1;
        public const int Theme_OldBSOD = 2;
        public const int Theme_Terminal = 3;

        public static readonly Dictionary<string, int> Themes = new Dictionary<string, int>()
        {
            { "Dark", Theme_Dark }, { "Light", Theme_Light }, { "Old BSOD", Theme_OldBSOD }, { "Terminal", Theme_Terminal }
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
                case Theme_Dark:
                    {
                        Colors.Base = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Black),
                            HotNormal = TGAttribute.Make(Color.White, Color.DarkGray),
                            Focus = TGAttribute.Make(Color.White, Color.Blue),
                            HotFocus = TGAttribute.Make(Color.BrightCyan, Color.Blue)
                        };

                        Colors.Menu = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.Gray),
                            Focus = TGAttribute.Make(Color.White, Color.Blue),
                            HotNormal = TGAttribute.Make(Color.DarkGray, Color.Gray),
                            HotFocus = TGAttribute.Make(Color.BrightCyan, Color.Blue)
                        };

                        Colors.Dialog = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.White, Color.DarkGray),
                            Focus = TGAttribute.Make(Color.White, Color.Blue),
                            HotNormal = TGAttribute.Make(Color.Blue, Color.White),
                            HotFocus = TGAttribute.Make(Color.BrightCyan, Color.Blue)
                        };

                        ArchivedList = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Blue),
                            Focus = TGAttribute.Make(Color.White, Color.BrightBlue),
                            HotNormal = TGAttribute.Make(Color.White, Color.Blue),
                            HotFocus = TGAttribute.Make(Color.White, Color.BrightBlue)
                        };

                        HighlighedLabel = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.BrightYellow, Color.Black)
                        };
                        break;
                    }

                case Theme_Light:
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
                            HotFocus = TGAttribute.Make(Color.BrightCyan, Color.Blue)
                        };

                        Colors.Dialog = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.Black, Color.White),
                            Focus = TGAttribute.Make(Color.White, Color.Brown),
                            HotNormal = TGAttribute.Make(Color.Brown, Color.White),
                            HotFocus = TGAttribute.Make(Color.BrightYellow, Color.Brown)
                        };

                        ArchivedList = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Brown),
                            Focus = TGAttribute.Make(Color.Black, Color.BrightYellow),
                            HotNormal = TGAttribute.Make(Color.White, Color.Brown),
                            HotFocus = TGAttribute.Make(Color.Black, Color.BrightYellow)
                        };

                        HighlighedLabel = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.BrightBlue, Color.Gray)
                        };
                        break;
                    }

                case Theme_OldBSOD:
                    {
                        Colors.Base = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.Gray, Color.Blue),
                            Focus = TGAttribute.Make(Color.Blue, Color.Gray),
                            HotNormal = TGAttribute.Make(Color.Blue, Color.Gray),
                            HotFocus = TGAttribute.Make(Color.Gray, Color.Blue)
                        };

                        Colors.Menu = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.Gray, Color.Blue),
                            Focus = TGAttribute.Make(Color.Blue, Color.Gray),
                            HotNormal = TGAttribute.Make(Color.Gray, Color.Blue),
                            HotFocus = TGAttribute.Make(Color.Blue, Color.Gray)
                        };

                        Colors.Dialog = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.Gray, Color.Blue),
                            Focus = TGAttribute.Make(Color.Blue, Color.Gray),
                            HotNormal = TGAttribute.Make(Color.Gray, Color.Blue),
                            HotFocus = TGAttribute.Make(Color.Blue, Color.Gray)
                        };

                        ArchivedList = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.Blue, Color.Gray),
                            Focus = TGAttribute.Make(Color.Gray, Color.Blue),
                            HotNormal = TGAttribute.Make(Color.Blue, Color.Gray),
                            HotFocus = TGAttribute.Make(Color.Gray, Color.Blue)
                        };

                        HighlighedLabel = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.White, Color.Blue)
                        };
                        break;
                    }

                case Theme_Terminal:
                    {
                        Colors.Base = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            Focus = TGAttribute.Make(Color.Black, Color.BrightGreen),
                            HotNormal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            HotFocus = TGAttribute.Make(Color.Black, Color.BrightGreen)
                        };

                        Colors.Menu = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            Focus = TGAttribute.Make(Color.Black, Color.BrightGreen),
                            HotNormal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            HotFocus = TGAttribute.Make(Color.Black, Color.BrightGreen)
                        };

                        Colors.Dialog = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            Focus = TGAttribute.Make(Color.Black, Color.BrightGreen),
                            HotNormal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            HotFocus = TGAttribute.Make(Color.Black, Color.BrightGreen)
                        };

                        ArchivedList = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            Focus = TGAttribute.Make(Color.Black, Color.BrightGreen),
                            HotNormal = TGAttribute.Make(Color.BrightGreen, Color.Black),
                            HotFocus = TGAttribute.Make(Color.Black, Color.BrightGreen)
                        };

                        HighlighedLabel = new ColorScheme()
                        {
                            Normal = TGAttribute.Make(Color.BrightYellow, Color.Black)
                        };
                        break;
                    }

                default: { break; }
            }

            //StaticWindows.RefreshColors();
            StaticWindows.Main.RefreshColors();
            User.Default.Theme = mode;

            Task.Delay(50).ContinueWith(t => { Application.Refresh(); });
        }
    }
}

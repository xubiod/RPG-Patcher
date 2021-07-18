using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Terminal.Gui;
using System.Threading.Tasks;

namespace rpg_patcher
{
    public static class User
    {
        public static class Default
        {
            public static int BytePref;
            public static int Theme;
            public static bool OverwriteFiles = true;
        }

        public static void Save(string filename)
        {
            int index = 0;
            int size = sizeof(int) * 3;
            byte[] data = new byte[size];
            byte[] bufferIn;
            int[] allToBuffer = { Default.BytePref, Default.Theme, Convert.ToInt32(Default.OverwriteFiles) };

            for (int i = 0; i < allToBuffer.Length; i++)
            {
                bufferIn = BitConverter.GetBytes(allToBuffer[i]);
                Buffer.BlockCopy(bufferIn, 0, data, index, bufferIn.Length);
                index += bufferIn.Length;
            }

            File.WriteAllBytes(filename, data);
        }

        public static void Load(string filename)
        {
            int index = 0;
            try
            {
                byte[] data = File.ReadAllBytes(filename);
                byte[] bufferOut = new byte[sizeof(int)];

                Buffer.BlockCopy(data, index, bufferOut, 0, sizeof(int));
                Default.BytePref = BitConverter.ToInt32(bufferOut, 0);
                index += sizeof(int);

                Buffer.BlockCopy(data, index, bufferOut, 0, sizeof(int));
                Default.Theme = BitConverter.ToInt32(bufferOut, 0);
                index += sizeof(int);

                Buffer.BlockCopy(data, index, bufferOut, 0, sizeof(int));
                Default.OverwriteFiles = Convert.ToBoolean(BitConverter.ToInt32(bufferOut, 0));
            }
            catch (IOException ioe)
            {
                Save(filename);
                Load(filename);
                return;
            }

            return;
        }
    }
}

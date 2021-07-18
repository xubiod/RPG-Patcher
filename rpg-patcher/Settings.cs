using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Terminal.Gui;
using System.Threading.Tasks;

namespace rpg_patcher
{
    public static class Settings
    {
        public static class Values
        {
            public static int BytePref;
            public static int Theme;
            public static bool OverwriteFiles = true;
            public static bool PersistentProject = false;

            public static int Count = 4;
        }

        public static void Save(string filename)
        {
            int index = 0;
            int size = sizeof(int) * Values.Count;
            byte[] data = new byte[size];
            byte[] bufferIn;
            int[] allToBuffer = { Values.BytePref, Values.Theme, Convert.ToInt32(Values.OverwriteFiles), Convert.ToInt32(Values.PersistentProject) };

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
                Values.BytePref = BitConverter.ToInt32(bufferOut, 0);
                index += sizeof(int);

                Buffer.BlockCopy(data, index, bufferOut, 0, sizeof(int));
                Values.Theme = BitConverter.ToInt32(bufferOut, 0);
                index += sizeof(int);

                Buffer.BlockCopy(data, index, bufferOut, 0, sizeof(int));
                Values.OverwriteFiles = Convert.ToBoolean(BitConverter.ToInt32(bufferOut, 0));
                index += sizeof(int);

                Buffer.BlockCopy(data, index, bufferOut, 0, sizeof(int));
                Values.PersistentProject = Convert.ToBoolean(BitConverter.ToInt32(bufferOut, 0));
            }
            catch
            {
                Save(filename);
                Load(filename);
                return;
            }

            return;
        }
    }
}

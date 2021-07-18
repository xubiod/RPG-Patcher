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
            int size = sizeof(ushort) * Values.Count;
            byte[] data = new byte[size];
            byte[] bufferIn;
            ushort[] allToBuffer = { Convert.ToUInt16(Values.BytePref), Convert.ToUInt16(Values.Theme), Convert.ToUInt16(Values.OverwriteFiles), Convert.ToUInt16(Values.PersistentProject) };

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
                byte[] bufferOut = new byte[sizeof(ushort)];

                Buffer.BlockCopy(data, index, bufferOut, 0, sizeof(ushort));
                Values.BytePref = BitConverter.ToUInt16(bufferOut, 0);
                index += sizeof(ushort);

                Buffer.BlockCopy(data, index, bufferOut, 0, sizeof(ushort));
                Values.Theme = BitConverter.ToUInt16(bufferOut, 0);
                index += sizeof(ushort);

                Buffer.BlockCopy(data, index, bufferOut, 0, sizeof(ushort));
                Values.OverwriteFiles = Convert.ToBoolean(BitConverter.ToUInt16(bufferOut, 0));
                index += sizeof(ushort);

                Buffer.BlockCopy(data, index, bufferOut, 0, sizeof(ushort));
                Values.PersistentProject = Convert.ToBoolean(BitConverter.ToUInt16(bufferOut, 0));
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

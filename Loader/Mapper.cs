using System.Runtime.InteropServices;

namespace Loader
{
    class Mapper
    {
        [DllImport("Mapper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Map([In, Out][MarshalAs(UnmanagedType.LPArray)] byte[] data, int pid);
    }
}

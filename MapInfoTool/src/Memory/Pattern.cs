using System;

namespace MapInfoTool.Memory
{
    public sealed class Pattern
    {
        private readonly string _bytes, _mask;

        public Pattern(string bytes, string mask)
        {
            _bytes = bytes;
            _mask = mask;
        }

        public unsafe IntPtr Get(int offset = 0)
        {
            MODULEINFO module;

            Win32Native.GetModuleInformation(Win32Native.GetCurrentProcess(), Win32Native.GetModuleHandle(null), out module, (uint)sizeof(MODULEINFO));

            var address = module.lpBaseOfDll.ToInt64();

            var end = address + module.SizeOfImage;

            for (; address < end; address++)
            {
                if (BCompare((byte*)(address), _bytes.ToCharArray(), _mask.ToCharArray()))
                {
                    return new IntPtr(address + offset);
                }
            }

            return IntPtr.Zero;
        }

        private unsafe bool BCompare(byte* pData, char[] bMask, char[] szMask)
        {
            for (int i = 0; i < bMask.Length; i++)
            {
                if (szMask[i] == 'x' && pData[i] != bMask[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}

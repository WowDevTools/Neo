using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Neo.IO
{
    internal static unsafe class Extensions
    {
        public static IEnumerable<byte> HexToBytes(this string str)
        {
            for (var i = 0; i < str.Length; i += 2)
            {
                var cl = str[i + 1];
                var ch = str[i];
                var cnl = cl - '0';
                var cnh = ch - '0';
                if (cnl < 0 || cnl > 9)
                {
                    cnl = (cl - 'a') + 10;
                    if (cnl < 10 || cnl > 15)
                        cnl = (cl - 'A') + 10;
                }
                if (cnh < 0 || cnh > 9)
                {
                    cnh = (ch - 'a') + 10;
                    if (cnh < 10 || cnh > 15)
                        cnh = (ch - 'A') + 10;
                }
                yield return (byte)((cnh << 4) | cnl);
            }
        }

	    [Obsolete("Requires the use of memcpy", true)]
	    public static void ReadToPointer(this BinaryReader br, IntPtr dest, int size)
        {
            var bytes = br.ReadBytes(size);
	        fixed (byte* b = bytes)
	        {
		        UnsafeNativeMethods.CopyMemory((byte*)dest.ToPointer(), b, size);
	        }
        }

        public static uint ReadUInt32Be(this BinaryReader br)
        {
            var be = br.ReadUInt32();
            return (be >> 24) | (((be >> 16) & 0xFF) << 8) | (((be >> 8) & 0xFF) << 16) | ((be & 0xFF) << 24);
        }

        public static string ReadCString(this BinaryReader reader)
        {
            byte num;
            List<byte> temp = new List<byte>();
	        while ((num = reader.ReadByte()) != 0 && reader.BaseStream.Position != reader.BaseStream.Length)
	        {
		        temp.Add(num);
	        }

            return Encoding.UTF8.GetString(temp.ToArray());
        }

	    [Obsolete("Requires the use of memcpy", true)]
        public static T Read<T>(this BinaryReader br) where T : struct
        {
            if (SizeCache<T>.TypeRequiresMarshal)
            {
                throw new ArgumentException(
                    "Cannot read a generic structure type that requires marshaling support. Read the structure out manually.");
            }

            // OPTIMIZATION!
            var ret = new T();
            fixed (byte* b = br.ReadBytes(SizeCache<T>.Size))
            {
                var tPtr = (byte*)SizeCache<T>.GetUnsafePtr(ref ret);
                UnsafeNativeMethods.CopyMemory(tPtr, b, SizeCache<T>.Size);
            }
            return ret;
        }

	    [Obsolete("Requires the use of memcpy", true)]
	    public static void Write<T>(this BinaryWriter bw, T value) where T : struct
        {
            if (SizeCache<T>.TypeRequiresMarshal)
            {
                throw new ArgumentException(
                    "Cannot write a generic structure type that requires marshaling support. Write the structure out manually.");
            }

            // fastest way to copy?
            var buf = new byte[SizeCache<T>.Size];

            var valData = (byte*)SizeCache<T>.GetUnsafePtr(ref value);
	        fixed (byte* pB = buf)
	        {
		        UnsafeNativeMethods.CopyMemory(pB, valData, SizeCache<T>.Size);
	        }

            bw.Write(buf);
        }

	    [Obsolete("Requires the use of memcpy", true)]
	    public static T[] ReadArray<T>(this BinaryReader br, int count) where T : struct
        {
            if (count == 0)
                return new T[0];

            if (SizeCache<T>.TypeRequiresMarshal)
                throw new ArgumentException(
                    "Cannot read a generic structure type that requires marshaling support. Read the structure out manually.");

            // NOTE: this may be safer to just call Read<T> each iteration to avoid possibilities of moved memory, etc.
            // For now, we'll see if this works.
            var ret = new T[count];
            fixed (byte* pB = br.ReadBytes(SizeCache<T>.Size * count))
            {
                var genericPtr = (byte*)SizeCache<T>.GetUnsafePtr(ref ret[0]);
                UnsafeNativeMethods.CopyMemory(genericPtr, pB, SizeCache<T>.Size * count);
            }
            return ret;
        }

	    [Obsolete("Requires the use of memcpy", true)]
	    public static void WriteArray<T>(this BinaryWriter writer, T[] values) where T : struct
        {
            if (values.Length == 0)
                return;

            if (SizeCache<T>.TypeRequiresMarshal)
                {
	                throw new ArgumentException(
                    "Cannot write a generic structure type that requires marshaling support. Write the structure out manually.");
                }

            var buf = new byte[SizeCache<T>.Size * values.Length];
            var valData = (byte*) SizeCache<T>.GetUnsafePtr(ref values[0]);

	        fixed (byte* ptr = buf)
	        {
		        UnsafeNativeMethods.CopyMemory(ptr, valData, buf.Length);
	        }

            writer.Write(buf, 0, buf.Length);
        }
    }
}

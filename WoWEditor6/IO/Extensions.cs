using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace WoWEditor6.IO
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

        public static void ReadToPointer(this BinaryReader br, IntPtr dest, int size)
        {
            var bytes = br.ReadBytes(size);
            fixed(byte* b = bytes)
            {
                UnsafeNativeMethods.CopyMemory((byte*) dest.ToPointer(), b, size);
            }
        }

        public static uint ReadUInt32Be(this BinaryReader br)
        {
            var be = br.ReadUInt32();
            return (be >> 24) | (((be >> 16) & 0xFF) << 8) | (((be >> 8) & 0xFF) << 16) | ((be & 0xFF) << 24);
        }

        public static T Read<T>(this BinaryReader br) where T : struct
        {
            if (SizeCache<T>.TypeRequiresMarshal)
                throw new ArgumentException(
                    "Cannot read a generic structure type that requires marshaling support. Read the structure out manually.");

            // OPTIMIZATION!
            var ret = new T();
            var bytes = br.ReadBytes(SizeCache<T>.Size);
            if (bytes.Length != SizeCache<T>.Size)
                throw new InvalidOperationException("Could not read enough bytes from the underlying stream");
            fixed (byte* b = bytes)
            {
                var tPtr = (byte*)SizeCache<T>.GetUnsafePtr(ref ret);
                UnsafeNativeMethods.CopyMemory(tPtr, b, SizeCache<T>.Size);
            }
            return ret;
        }

        public static string ReadWString(this BinaryReader br)
        {
            var chars = "";
            do
            {
                var numAvailable = (br.BaseStream.Length - br.BaseStream.Position) / 2;
                if (numAvailable == 0)
                    throw new EndOfStreamException();

                numAvailable = Math.Min(numAvailable, 30);

                var chunk = br.ReadArray<ushort>((int)numAvailable);
                var isDone = false;
                for (var i = 0; i < numAvailable; ++i)
                {
                    var c = (char)chunk[i];
                    if (c == 0)
                    {
                        isDone = true;
                        break;
                    }
                    chars += c;
                }

                if (isDone)
                    break;

            } while (true);

            return chars;
        }

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

        public static void ReadToArray<T>(this BinaryReader br, T[] data) where T : struct
        {
            if (SizeCache<T>.TypeRequiresMarshal)
                throw new ArgumentException(
                    "Cannot read a generic structure type that requires marshaling support. Read the structure out manually.");

            // NOTE: this may be safer to just call Read<T> each iteration to avoid possibilities of moved memory, etc.
            // For now, we'll see if this works.
            fixed (byte* pB = br.ReadBytes(SizeCache<T>.Size * data.Length))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    var tPtr = (byte*)SizeCache<T>.GetUnsafePtr(ref data[i]);
                    UnsafeNativeMethods.CopyMemory(tPtr, &pB[i * SizeCache<T>.Size], SizeCache<T>.Size);
                }
            }
        }

        public static void Write<T>(this BinaryWriter bw, T value) where T : struct
        {
            if (SizeCache<T>.TypeRequiresMarshal)
                throw new ArgumentException(
                    "Cannot write a generic structure type that requires marshaling support. Write the structure out manually.");

            // fastest way to copy?
            var buf = new byte[SizeCache<T>.Size];

            var valData = (byte*)SizeCache<T>.GetUnsafePtr(ref value);

            fixed (byte* pB = buf)
            {
                UnsafeNativeMethods.CopyMemory(pB, valData, SizeCache<T>.Size);
            }

            bw.Write(buf);
        }

        public static void WriteArray<T>(this BinaryWriter writer, T[] values) where T : struct
        {
	        if (values.Length == 0)
		        return;

			if (SizeCache<T>.TypeRequiresMarshal)
				throw new ArgumentException(
					"Cannot write a generic structure type that requires marshaling support. Write the structure out manually.");

	        var buf = new byte[SizeCache<T>.Size * values.Length];
	        var valData = (byte*) SizeCache<T>.GetUnsafePtr(ref values[0]);

	        fixed (byte* ptr = buf)
		        UnsafeNativeMethods.CopyMemory(ptr, valData, buf.Length);

	        writer.Write(buf, 0, buf.Length);
        }
    }
}

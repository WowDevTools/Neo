using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
// ReSharper disable StaticMemberInGenericType

namespace WoWEditor6.IO
{
    /// <summary>
    /// Provided by Apoc, see:
    /// https://gist.github.com/ApocDev/bd3947020ef89c868f6d
    /// </summary>
    public static class SizeCache<T> where T : struct
    {
        /// <summary> The size of the Type </summary>
        public static readonly int Size;

        /// <summary> True if this type requires the Marshaler to map variables. (No direct pointer dereferencing) </summary>
        public static readonly bool TypeRequiresMarshal;

        internal static readonly GetUnsafePtrDelegate GetUnsafePtr;

        static SizeCache()
        {
            // Booleans = 1 char.
            Type realType;
            if (typeof(T) == typeof(bool))
            {
                Size = 1;
                realType = typeof(T);
            }
            else if (typeof(T).IsEnum)
            {
                Type underlying = typeof(T).GetEnumUnderlyingType();
                Size = Marshal.SizeOf(underlying);
                realType = underlying;
            }
            else
            {
                Size = Marshal.SizeOf(typeof(T));
                realType = typeof(T);
            }

            // Basically, if any members of the type have a MarshalAs attributes, then we can't just pointer dereferenced. :(
            // This literally means any kind of MarshalAs. Strings, arrays, custom type sizes, etc.
            // Ideally, we want to avoid the Marshaler as much as possible. It causes a lot of overhead, and for a memory reading
            // lib where we need the best speed possible, we do things manually when possible!
            TypeRequiresMarshal =
                realType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any(
                    m => m.GetCustomAttributes(typeof(MarshalAsAttribute), true).Any());

            // Generate a method to get the address of a generic type. We'll be using this for RtlMoveMemory later for much faster structure reads.
            var method = new DynamicMethod(string.Format("GetPinnedPtr<{0}>", typeof(T).FullName.Replace(".", "<>")),
                typeof(void*),
                new[] { typeof(T).MakeByRefType() },
                typeof(SizeCache<>).Module);
            ILGenerator generator = method.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Conv_U);
            generator.Emit(OpCodes.Ret);
            GetUnsafePtr = (GetUnsafePtrDelegate)method.CreateDelegate(typeof(GetUnsafePtrDelegate));
        }

        #region Nested type: GetUnsafePtrDelegate

        internal unsafe delegate void* GetUnsafePtrDelegate(ref T value);

        #endregion
    }
}

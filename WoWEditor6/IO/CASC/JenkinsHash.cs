using System.Text;

namespace WoWEditor6.IO.CASC
{
    class JenkinsHash
    {
        private uint mA, mB, mC;

        public ulong Compute(string value)
        {
            mA = mB = mC = 0;

            return Calculate(Encoding.ASCII.GetBytes(value));
        }

        private unsafe ulong Calculate(byte[] data)
        {
            var length = (uint) data.Length;
            mA = mB = mC = 0xDEADBEEF + length;

            fixed(byte* ptr = data)
            {
                var u = (uint*) ptr;
                if ((*u & 0x3) == 0)
                {
                    var k = u;
                    while (length > 12)
                    {
                        mA += k[0];
                        mB += k[1];
                        mC += k[2];
                        Mix();
                        length -= 12;
                        k += 3;
                    }

                    switch (length)
                    {
                        case 12: mC += k[2]; mB += k[1]; mA += k[0]; break;
                        case 11: mC += k[2] & 0xffffff; mB += k[1]; mA += k[0]; break;
                        case 10: mC += k[2] & 0xffff; mB += k[1]; mA += k[0]; break;
                        case 9: mC += k[2] & 0xff; mB += k[1]; mA += k[0]; break;
                        case 8: mB += k[1]; mA += k[0]; break;
                        case 7: mB += k[1] & 0xffffff; mA += k[0]; break;
                        case 6: mB += k[1] & 0xffff; mA += k[0]; break;
                        case 5: mB += k[1] & 0xff; mA += k[0]; break;
                        case 4: mA += k[0]; break;
                        case 3: mA += k[0] & 0xffffff; break;
                        case 2: mA += k[0] & 0xffff; break;
                        case 1: mA += k[0] & 0xff; break;
                        case 0:
                            {
                                var val = ((ulong)mC << 32) | mB;
                                return val;
                            }
                    }
                }
                else if ((*u & 0x01) == 0)
                {
                    var k = (ushort*)u;
                    while (length > 12)
                    {
                        mA += k[0] + ((uint)k[1] << 16);
                        mB += k[2] + ((uint)k[3] << 16);
                        mC += k[3] + ((uint)k[5] << 16);
                        Mix();
                        length -= 12;
                        k += 6;
                    }

                    var k8 = (byte*)k;
                    switch (length)
                    {
                        case 12:
                            mC += k[4] + (((uint)k[5]) << 16);
                            mB += k[2] + (((uint)k[3]) << 16);
                            mA += k[0] + (((uint)k[1]) << 16);
                            break;
                        case 11:
                            mC += ((uint)k8[10]) << 16;
                            goto case 10;
                        case 10:
                            mC += k[4];
                            mB += k[2] + (((uint)k[3]) << 16);
                            mA += k[0] + (((uint)k[1]) << 16);
                            break;
                        case 9:
                            mC += k8[8];
                            goto case 8;
                        case 8:
                            mB += k[2] + (((uint)k[3]) << 16);
                            mA += k[0] + (((uint)k[1]) << 16);
                            break;
                        case 7:
                            mB += ((uint)k8[6]) << 16;
                            goto case 6;
                        case 6:
                            mB += k[2];
                            mA += k[0] + (((uint)k[1]) << 16);
                            break;
                        case 5:
                            mB += k8[4];
                            goto case 4;
                        case 4:
                            mA += k[0] + (((uint)k[1]) << 16);
                            break;
                        case 3:
                            mA += ((uint)k8[2]) << 16;
                            goto case 2;
                        case 2:
                            mA += k[0];
                            break;
                        case 1:
                            mA += k8[0];
                            break;

                        case 0:
                            return ((ulong)mC) | mB;
                    }
                }
                else
                {
                    var k = (byte*)u;
                    while (length > 12)
                    {
                        mA += k[0];
                        mA += (uint)k[1] << 8;
                        mA += (uint)k[2] << 16;
                        mA += (uint)k[3] << 24;
                        mB += k[4];
                        mB += (uint)k[5] << 8;
                        mB += (uint)k[6] << 16;
                        mB += (uint)k[7] << 24;
                        mC += k[8];
                        mC += (uint)k[9] << 8;
                        mC += (uint)k[10] << 16;
                        mC += (uint)k[11] << 24;
                        Mix();
                        length -= 12;
                        k += 12;
                    }

                    switch (length)
                    {
                        case 12:
                            mC += (((uint)k[11]) << 24);
                            goto case 11;
                        case 11:
                            mC += (((uint)k[10]) << 16);
                            goto case 10;
                        case 10:
                            mC += (((uint)k[9]) << 8);
                            goto case 9;
                        case 9:
                            mC += k[8];
                            goto case 8;
                        case 8:
                            mB += (((uint)k[7]) << 24);
                            goto case 7;
                        case 7:
                            mB += (((uint)k[6]) << 16);
                            goto case 6;
                        case 6:
                            mB += (((uint)k[5]) << 8);
                            goto case 5;
                        case 5:
                            mB += k[4];
                            goto case 4;
                        case 4:
                            mA += (((uint)k[3]) << 24);
                            goto case 3;
                        case 3:
                            mA += (((uint)k[2]) << 16);
                            goto case 2;
                        case 2:
                            mA += (((uint)k[1]) << 8);
                            goto case 1;
                        case 1:
                            mA += k[0]; break;
                        case 0:
                            return ((ulong)mC << 32) | mB;
                    }
                }
            }

            Final();
            return ((ulong) mC << 32) | mB;
        }

        private void Final()
        {
            mC ^= mB; mC -= Rot(mB, 14);
            mA ^= mC; mA -= Rot(mC, 11);
            mB ^= mA; mB -= Rot(mA, 25);
            mC ^= mB; mC -= Rot(mB, 16);
            mA ^= mC; mA -= Rot(mC, 4);
            mB ^= mA; mB -= Rot(mA, 14);
            mC ^= mB; mC -= Rot(mB, 24);
        }

        private void Mix()
        {
            mA -= mC; mA ^= Rot(mC, 4); mC += mB;
            mB -= mA; mB ^= Rot(mA, 6); mA += mC;
            mC -= mB; mC ^= Rot(mB, 8); mB += mA;
            mA -= mC; mA ^= Rot(mC, 16); mC += mB;
            mB -= mA; mB ^= Rot(mA, 19); mA += mC;
            mC -= mB; mC ^= Rot(mB, 4); mB += mA;
        }

        private static uint Rot(uint x, byte k)
        {
            return (x << k) | (x >> (32 - k));
        }
    }
}

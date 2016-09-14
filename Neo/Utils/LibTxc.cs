/*
 * Code is based on libtxc_dxtn, license:
 *
 * libtxc_dxtn
 * Version:  1.0
 *
 * Copyright (C) 2004  Roland Scheidegger   All Rights Reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL
 * BRIAN PAUL BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN
 * AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *
 *
 */

using System;
using GLenum = System.UInt32;
using GLint = System.Int32;
using GLshort = System.Int16;
using GLushort = System.UInt16;
using GLuint = System.UInt32;
using GLboolean = System.Byte;
using GLubyte = System.Byte;
using GLchan = System.Byte;

namespace Neo.Utils
{
    unsafe static class LibTxc
    {
        private const int Alphacut = 127;
        private const GLboolean GlFalse = 0;
        private const GLboolean GlTrue = 1;
        private const int Redweight = 4;
        private const int Greenweight = 16;
        private const int Blueweight = 1;

        public const GLenum GlCompressedRgbS3TcDxt1Ext = 0x83F0;
        public const GLenum GlCompressedRgbaS3TcDxt1Ext = 0x83F1;
        public const GLenum GlCompressedRgbaS3TcDxt3Ext = 0x83F2;
        public const GLenum GlCompressedRgbaS3TcDxt5Ext = 0x83F3;

        // ReSharper disable once FunctionComplexityOverflow
        public static void CompressDxtn(GLint width, GLint height,
            GLubyte* srcPixData, GLenum destFormat,
            GLubyte* dest, GLint dstRowStride)
        {
            const int srccomps = 4;
            GLubyte* blkaddr = dest;
            GLubyte*** srcpixels = stackalloc GLubyte**[4];
            GLubyte** scpp0 = stackalloc GLubyte*[4];
            GLubyte* scp0 = stackalloc GLubyte[4];
            GLubyte* scp1 = stackalloc GLubyte[4];
            GLubyte* scp2 = stackalloc GLubyte[4];
            GLubyte* scp3 = stackalloc GLubyte[4];
            scpp0[0] = scp0;
            scpp0[1] = scp1;
            scpp0[2] = scp2;
            scpp0[3] = scp3;
            GLubyte** scpp1 = stackalloc GLubyte*[4];
            GLubyte* scp10 = stackalloc GLubyte[4];
            GLubyte* scp11 = stackalloc GLubyte[4];
            GLubyte* scp12 = stackalloc GLubyte[4];
            GLubyte* scp13 = stackalloc GLubyte[4];
            scpp1[0] = scp10;
            scpp1[1] = scp11;
            scpp1[2] = scp12;
            scpp1[3] = scp13;
            GLubyte** scpp2 = stackalloc GLubyte*[4];
            GLubyte* scp20 = stackalloc GLubyte[4];
            GLubyte* scp21 = stackalloc GLubyte[4];
            GLubyte* scp22 = stackalloc GLubyte[4];
            GLubyte* scp23 = stackalloc GLubyte[4];
            scpp2[0] = scp20;
            scpp2[1] = scp21;
            scpp2[2] = scp22;
            scpp2[3] = scp23;
            GLubyte** scpp3 = stackalloc GLubyte*[4];
            GLubyte* scp30 = stackalloc GLubyte[4];
            GLubyte* scp31 = stackalloc GLubyte[4];
            GLubyte* scp32 = stackalloc GLubyte[4];
            GLubyte* scp33 = stackalloc GLubyte[4];
            scpp3[0] = scp30;
            scpp3[1] = scp31;
            scpp3[2] = scp32;
            scpp3[3] = scp33;
            srcpixels[0] = scpp0;
            srcpixels[1] = scpp1;
            srcpixels[2] = scpp2;
            srcpixels[3] = scpp3;
            GLchan* srcaddr;
            GLint numxpixels, numypixels;
            GLint i, j;
            GLint dstRowDiff;

            switch (destFormat)
            {
                case GlCompressedRgbS3TcDxt1Ext:
                case GlCompressedRgbaS3TcDxt1Ext:
                    dstRowDiff = dstRowStride >= (width * 2) ? dstRowStride - (((width + 3) & ~3) * 2) : 0;
                    for (j = 0; j < height; j += 4)
                    {
                        if (height > j + 3) numypixels = 4;
                        else numypixels = height - j;
                        srcaddr = srcPixData + j * width * srccomps;
                        for (i = 0; i < width; i += 4)
                        {
                            if (width > i + 3) numxpixels = 4;
                            else numxpixels = width - i;
                            Extractsrccolors(srcpixels, srcaddr, width, numxpixels, numypixels, srccomps);
                            Encodedxtcolorblockfaster(blkaddr, srcpixels, numxpixels, numypixels, destFormat);
                            srcaddr += srccomps * numxpixels;
                            blkaddr += 8;
                        }
                        blkaddr += dstRowDiff;
                    }
                    break;
                case GlCompressedRgbaS3TcDxt3Ext:
                    dstRowDiff = dstRowStride >= (width * 4) ? dstRowStride - (((width + 3) & ~3) * 4) : 0;
                    for (j = 0; j < height; j += 4)
                    {
                        if (height > j + 3) numypixels = 4;
                        else numypixels = height - j;
                        srcaddr = srcPixData + j * width * srccomps;
                        for (i = 0; i < width; i += 4)
                        {
                            if (width > i + 3) numxpixels = 4;
                            else numxpixels = width - i;
                            Extractsrccolors(srcpixels, srcaddr, width, numxpixels, numypixels, srccomps);
                            *blkaddr++ = (byte) ((srcpixels[0][0][3] >> 4) | (srcpixels[0][1][3] & 0xf0));
                            *blkaddr++ = (byte) ((srcpixels[0][2][3] >> 4) | (srcpixels[0][3][3] & 0xf0));
                            *blkaddr++ = (byte) ((srcpixels[1][0][3] >> 4) | (srcpixels[1][1][3] & 0xf0));
                            *blkaddr++ = (byte) ((srcpixels[1][2][3] >> 4) | (srcpixels[1][3][3] & 0xf0));
                            *blkaddr++ = (byte) ((srcpixels[2][0][3] >> 4) | (srcpixels[2][1][3] & 0xf0));
                            *blkaddr++ = (byte) ((srcpixels[2][2][3] >> 4) | (srcpixels[2][3][3] & 0xf0));
                            *blkaddr++ = (byte) ((srcpixels[3][0][3] >> 4) | (srcpixels[3][1][3] & 0xf0));
                            *blkaddr++ = (byte) ((srcpixels[3][2][3] >> 4) | (srcpixels[3][3][3] & 0xf0));
                            Encodedxtcolorblockfaster(blkaddr, srcpixels, numxpixels, numypixels, destFormat);
                            srcaddr += srccomps * numxpixels;
                            blkaddr += 8;
                        }
                        blkaddr += dstRowDiff;
                    }
                    break;
                case GlCompressedRgbaS3TcDxt5Ext:
                    dstRowDiff = dstRowStride >= (width * 4) ? dstRowStride - (((width + 3) & ~3) * 4) : 0;
                    for (j = 0; j < height; j += 4)
                    {
                        if (height > j + 3) numypixels = 4;
                        else numypixels = height - j;
                        srcaddr = srcPixData + j * width * srccomps;
                        for (i = 0; i < width; i += 4)
                        {
                            if (width > i + 3) numxpixels = 4;
                            else numxpixels = width - i;
                            Extractsrccolors(srcpixels, srcaddr, width, numxpixels, numypixels, srccomps);
                            Encodedxt5Alpha(blkaddr, srcpixels, numxpixels, numypixels);
                            Encodedxtcolorblockfaster(blkaddr + 8, srcpixels, numxpixels, numypixels, destFormat);
                            srcaddr += srccomps * numxpixels;
                            blkaddr += 16;
                        }
                        blkaddr += dstRowDiff;
                    }
                    break;
                default:
                    throw new ArgumentException(string.Format("libdxtn: Bad dstFormat {0} in tx_compress_dxtn\n",
                        destFormat));
            }
        }

        private static void Storedxtencodedblock(GLubyte* blkaddr, GLubyte*** srccolors, GLubyte** bestcolor,
            GLint numxpixels, GLint numypixels, GLuint type, GLboolean haveAlpha)
        {
            GLint i, j, colors;
            GLuint pixerror, pixerrorbest;
            GLint colordist;
            GLuint bits = 0, bits2 = 0;
            GLubyte enc = 0;
            GLubyte** cv = stackalloc GLubyte*[4];
            GLubyte* cv0 = stackalloc GLubyte[4];
            GLubyte* cv1 = stackalloc GLubyte[4];
            GLubyte* cv2 = stackalloc GLubyte[4];
            GLubyte* cv3 = stackalloc GLubyte[4];
            cv[0] = cv0;
            cv[1] = cv1;
            cv[2] = cv2;
            cv[3] = cv3;

            bestcolor[0][0] = (byte) (bestcolor[0][0] & 0xf8);
            bestcolor[0][1] = (byte) (bestcolor[0][1] & 0xfc);
            bestcolor[0][2] = (byte) (bestcolor[0][2] & 0xf8);
            bestcolor[1][0] = (byte) (bestcolor[1][0] & 0xf8);
            bestcolor[1][1] = (byte) (bestcolor[1][1] & 0xfc);
            bestcolor[1][2] = (byte) (bestcolor[1][2] & 0xf8);

            var color0 = (ushort) (bestcolor[0][0] << 8 | bestcolor[0][1] << 3 | bestcolor[0][2] >> 3);
            var color1 = (ushort) (bestcolor[1][0] << 8 | bestcolor[1][1] << 3 | bestcolor[1][2] >> 3);
            if (color0 < color1)
            {
                var tempcolor = color0;
                color0 = color1;
                color1 = tempcolor;
                var colorptr = bestcolor[0];
                bestcolor[0] = bestcolor[1];
                bestcolor[1] = colorptr;
            }


            for (i = 0; i < 3; i ++)
            {
                cv[0][i] = bestcolor[0][i];
                cv[1][i] = bestcolor[1][i];
                cv[2][i] = (byte) ((bestcolor[0][i] * 2 + bestcolor[1][i]) / 3);
                cv[3][i] = (byte) ((bestcolor[0][i] + bestcolor[1][i] * 2) / 3);
            }

            uint testerror = 0;
            for (j = 0; j < numypixels; j++)
            {
                for (i = 0; i < numxpixels; i++)
                {
                    pixerrorbest = 0xffffffff;
                    for (colors = 0; colors < 4; colors++)
                    {
                        colordist = srccolors[j][i][0] - cv[colors][0];
                        pixerror = (uint) (colordist * colordist * Redweight);
                        colordist = (srccolors[j][i][1] - cv[colors][1]);
                        pixerror += (uint) (colordist * colordist * Greenweight);
                        colordist = srccolors[j][i][2] - cv[colors][2];
                        pixerror += (uint) (colordist * colordist * Blueweight);
                        if (pixerror < pixerrorbest)
                        {
                            pixerrorbest = pixerror;
                            enc = (byte) colors;
                        }
                    }
                    testerror += pixerrorbest;
                    bits |= (uint) (enc << (2 * (j * 4 + i)));
                }
            }
            for (i = 0; i < 3; i ++)
            {
                cv[2][i] = (byte) ((bestcolor[0][i] + bestcolor[1][i]) / 2);
                /* this isn't used. Looks like the black color constant can only be used
         with RGB_DXT1 if I read the spec correctly (note though that the radeon gpu disagrees,
         it will decode 3 to black even with DXT3/5), and due to how the color searching works
         it won't get used even then */
                cv[3][i] = 0;
            }
            uint testerror2 = 0;
            for (j = 0; j < numypixels; j++)
            {
                for (i = 0; i < numxpixels; i++)
                {
                    pixerrorbest = 0xffffffff;
                    if ((type == GlCompressedRgbaS3TcDxt1Ext) && (srccolors[j][i][3] <= Alphacut))
                    {
                        enc = 3;
                        pixerrorbest = 0; /* don't calculate error */
                    }
                    else
                    {
                        /* we're calculating the same what we have done already for colors 0-1 above... */
                        for (colors = 0; colors < 3; colors++)
                        {
                            colordist = srccolors[j][i][0] - cv[colors][0];
                            pixerror = (uint) (colordist * colordist * Redweight);
                            colordist = srccolors[j][i][1] - cv[colors][1];
                            pixerror += (uint) (colordist * colordist * Greenweight);
                            colordist = srccolors[j][i][2] - cv[colors][2];
                            pixerror += (uint) (colordist * colordist * Blueweight);
                            if (pixerror < pixerrorbest)
                            {
                                pixerrorbest = pixerror;
                                /* need to exchange colors later */
                                if (colors > 1) enc = (byte) colors;
                                else enc = (byte) (colors ^ 1);
                            }
                        }
                    }
                    testerror2 += pixerrorbest;
                    bits2 |= (uint) (enc << (2 * (j * 4 + i)));
                }
            }


            /* finally we're finished, write back colors and bits */
            if ((testerror > testerror2) || (haveAlpha != GlFalse))
            {
                *blkaddr++ = (byte) (color1 & 0xff);
                *blkaddr++ = (byte) (color1 >> 8);
                *blkaddr++ = (byte) (color0 & 0xff);
                *blkaddr++ = (byte) (color0 >> 8);
                *blkaddr++ = (byte) (bits2 & 0xff);
                *blkaddr++ = (byte) ((bits2 >> 8) & 0xff);
                *blkaddr++ = (byte) ((bits2 >> 16) & 0xff);
                *blkaddr = (byte) (bits2 >> 24);
            }
            else
            {
                *blkaddr++ = (byte) (color0 & 0xff);
                *blkaddr++ = (byte) (color0 >> 8);
                *blkaddr++ = (byte) (color1 & 0xff);
                *blkaddr++ = (byte) (color1 >> 8);
                *blkaddr++ = (byte) (bits & 0xff);
                *blkaddr++ = (byte) ((bits >> 8) & 0xff);
                *blkaddr++ = (byte) ((bits >> 16) & 0xff);
                *blkaddr = (byte) (bits >> 24);
            }
        }

        // ReSharper disable once FunctionComplexityOverflow
        private static void Fancybasecolorsearch(GLubyte*** srccolors, GLubyte** bestcolor,
            GLint numxpixels, GLint numypixels)
        {
            GLint i, j;
            GLint* blockerrlin0 = stackalloc GLint[3];
            GLint* blockerrlin1 = stackalloc GLint[3];
            GLint** blockerrlin = stackalloc GLint*[2];
            blockerrlin[0] = blockerrlin0;
            blockerrlin[1] = blockerrlin1;
            GLubyte* nrcolor = stackalloc GLubyte[2];
            GLint* pixerrorcolorbest = stackalloc GLint[3];
            GLubyte enc = 0;
            GLubyte** cv = stackalloc GLubyte*[4];
            GLubyte* cv0 = stackalloc GLubyte[4];
            GLubyte* cv1 = stackalloc GLubyte[4];
            GLubyte* cv2 = stackalloc GLubyte[4];
            GLubyte* cv3 = stackalloc GLubyte[4];
            cv[0] = cv0;
            cv[1] = cv1;
            cv[2] = cv2;
            cv[3] = cv3;
            GLubyte** testcolor = stackalloc GLubyte*[2];
            GLubyte* tc0 = stackalloc GLubyte[3];
            GLubyte* tc1 = stackalloc GLubyte[3];
            testcolor[0] = tc0;
            testcolor[1] = tc1;

            if (((bestcolor[0][0] & 0xf8) << 8 | (bestcolor[0][1] & 0xfc) << 3 | bestcolor[0][2] >> 3) <
                ((bestcolor[1][0] & 0xf8) << 8 | (bestcolor[1][1] & 0xfc) << 3 | bestcolor[1][2] >> 3))
            {
                testcolor[0][0] = bestcolor[0][0];
                testcolor[0][1] = bestcolor[0][1];
                testcolor[0][2] = bestcolor[0][2];
                testcolor[1][0] = bestcolor[1][0];
                testcolor[1][1] = bestcolor[1][1];
                testcolor[1][2] = bestcolor[1][2];
            }
            else
            {
                testcolor[1][0] = bestcolor[0][0];
                testcolor[1][1] = bestcolor[0][1];
                testcolor[1][2] = bestcolor[0][2];
                testcolor[0][0] = bestcolor[1][0];
                testcolor[0][1] = bestcolor[1][1];
                testcolor[0][2] = bestcolor[1][2];
            }

            for (i = 0; i < 3; i ++)
            {
                cv[0][i] = testcolor[0][i];
                cv[1][i] = testcolor[1][i];
                cv[2][i] = (byte) ((testcolor[0][i] * 2 + testcolor[1][i]) / 3);
                cv[3][i] = (byte) ((testcolor[0][i] + testcolor[1][i] * 2) / 3);
            }

            blockerrlin[0][0] = 0;
            blockerrlin[0][1] = 0;
            blockerrlin[0][2] = 0;
            blockerrlin[1][0] = 0;
            blockerrlin[1][1] = 0;
            blockerrlin[1][2] = 0;

            nrcolor[0] = 0;
            nrcolor[1] = 0;

            for (j = 0; j < numypixels; j++)
            {
                for (i = 0; i < numxpixels; i++)
                {
                    var pixerrorbest = 0xffffffff;
                    GLint colors;
                    for (colors = 0; colors < 4; colors++)
                    {
                        var colordist = srccolors[j][i][0] - (cv[colors][0]);
                        var pixerror = (uint) (colordist * colordist * Redweight);
                        var pixerrorred = (uint) colordist;
                        colordist = srccolors[j][i][1] - (cv[colors][1]);
                        pixerror += (uint) (colordist * colordist * Greenweight);
                        var pixerrorgreen = (uint) colordist;
                        colordist = srccolors[j][i][2] - (cv[colors][2]);
                        pixerror += (uint) (colordist * colordist * Blueweight);
                        var pixerrorblue = (uint) colordist;
                        if (pixerror < pixerrorbest)
                        {
                            enc = (byte) colors;
                            pixerrorbest = pixerror;
                            pixerrorcolorbest[0] = (int) pixerrorred;
                            pixerrorcolorbest[1] = (int) pixerrorgreen;
                            pixerrorcolorbest[2] = (int) pixerrorblue;
                        }
                    }
                    GLint z;
                    if (enc == 0)
                    {
                        for (z = 0; z < 3; z++)
                        {
                            blockerrlin[0][z] += 3 * pixerrorcolorbest[z];
                        }
                        nrcolor[0] += 3;
                    }
                    else if (enc == 2)
                    {
                        for (z = 0; z < 3; z++)
                        {
                            blockerrlin[0][z] += 2 * pixerrorcolorbest[z];
                        }
                        nrcolor[0] += 2;
                        for (z = 0; z < 3; z++)
                        {
                            blockerrlin[1][z] += 1 * pixerrorcolorbest[z];
                        }
                        nrcolor[1] += 1;
                    }
                    else if (enc == 3)
                    {
                        for (z = 0; z < 3; z++)
                        {
                            blockerrlin[0][z] += 1 * pixerrorcolorbest[z];
                        }
                        nrcolor[0] += 1;
                        for (z = 0; z < 3; z++)
                        {
                            blockerrlin[1][z] += 2 * pixerrorcolorbest[z];
                        }
                        nrcolor[1] += 2;
                    }
                    else if (enc == 1)
                    {
                        for (z = 0; z < 3; z++)
                        {
                            blockerrlin[1][z] += 3 * pixerrorcolorbest[z];
                        }
                        nrcolor[1] += 3;
                    }
                }
            }
            if (nrcolor[0] == 0) nrcolor[0] = 1;
            if (nrcolor[1] == 0) nrcolor[1] = 1;
            for (j = 0; j < 2; j++)
            {
                for (i = 0; i < 3; i++)
                {
                    var newvalue = testcolor[j][i] + blockerrlin[j][i] / nrcolor[j];
                    if (newvalue <= 0)
                        testcolor[j][i] = 0;
                    else if (newvalue >= 255)
                        testcolor[j][i] = 255;
                    else testcolor[j][i] = (byte) newvalue;
                }
            }

            if ((Math.Abs(testcolor[0][0] - testcolor[1][0]) < 8) &&
                (Math.Abs(testcolor[0][1] - testcolor[1][1]) < 4) &&
                (Math.Abs(testcolor[0][2] - testcolor[1][2]) < 8))
            {
                var coldiffred = (byte) Math.Abs(testcolor[0][0] - testcolor[1][0]);
                var coldiffgreen = (byte) (2 * Math.Abs(testcolor[0][1] - testcolor[1][1]));
                var coldiffblue = (byte) Math.Abs(testcolor[0][2] - testcolor[1][2]);
                var coldiffmax = coldiffred;
                if (coldiffmax < coldiffgreen) coldiffmax = coldiffgreen;
                if (coldiffmax < coldiffblue) coldiffmax = coldiffblue;
                if (coldiffmax > 0)
                {
                    GLubyte factor;
                    if (coldiffmax > 4) factor = 2;
                    else if (coldiffmax > 2) factor = 3;
                    else factor = 4;
                    GLubyte ind0;
                    GLubyte ind1;
                    if (testcolor[1][1] >= testcolor[0][1])
                    {
                        ind1 = 1;
                        ind0 = 0;
                    }
                    else
                    {
                        ind1 = 0;
                        ind0 = 1;
                    }
                    if ((testcolor[ind1][1] + factor * coldiffgreen) <= 255)
                        testcolor[ind1][1] += (byte) (factor * coldiffgreen);
                    else testcolor[ind1][1] = 255;
                    if ((testcolor[ind1][0] - testcolor[ind0][1]) > 0)
                    {
                        if ((testcolor[ind1][0] + factor * coldiffred) <= 255)
                            testcolor[ind1][0] += (byte) (factor * coldiffred);
                        else testcolor[ind1][0] = 255;
                    }
                    else
                    {
                        if ((testcolor[ind0][0] + factor * coldiffred) <= 255)
                            testcolor[ind0][0] += (byte) (factor * coldiffred);
                        else testcolor[ind0][0] = 255;
                    }
                    if ((testcolor[ind1][2] - testcolor[ind0][2]) > 0)
                    {
                        if ((testcolor[ind1][2] + factor * coldiffblue) <= 255)
                            testcolor[ind1][2] += (byte) (factor * coldiffblue);
                        else testcolor[ind1][2] = 255;
                    }
                    else
                    {
                        if ((testcolor[ind0][2] + factor * coldiffblue) <= 255)
                            testcolor[ind0][2] += (byte) (factor * coldiffblue);
                        else testcolor[ind0][2] = 255;
                    }
                }
            }

            if (((testcolor[0][0] & 0xf8) << 8 | (testcolor[0][1] & 0xfc) << 3 | testcolor[0][2] >> 3) <
                ((testcolor[1][0] & 0xf8) << 8 | (testcolor[1][1] & 0xfc) << 3 | testcolor[1][2]) >> 3)
            {
                for (i = 0; i < 3; i++)
                {
                    bestcolor[0][i] = testcolor[0][i];
                    bestcolor[1][i] = testcolor[1][i];
                }
            }
            else
            {
                for (i = 0; i < 3; i++)
                {
                    bestcolor[0][i] = testcolor[1][i];
                    bestcolor[1][i] = testcolor[0][i];
                }
            }
        }

        private static void Encodedxtcolorblockfaster(GLubyte* blkaddr, GLubyte*** srccolors,
            GLint numxpixels, GLint numypixels, GLuint type)
        {
            GLubyte** bestcolor = stackalloc GLubyte*[2];
            GLubyte** basecolors = stackalloc GLubyte*[2];
            var bc1 = stackalloc GLubyte[3];
            var bc2 = stackalloc GLubyte[3];
            basecolors[0] = bc1;
            basecolors[1] = bc2;
            GLubyte i, j;
            GLuint highcv;
            var haveAlpha = GlFalse;

            var lowcv = highcv = (uint) (srccolors[0][0][0] * srccolors[0][0][0] * Redweight +
                                          srccolors[0][0][1] * srccolors[0][0][1] * Greenweight +
                                          srccolors[0][0][2] * srccolors[0][0][2] * Blueweight);
            bestcolor[0] = bestcolor[1] = srccolors[0][0];
            for (j = 0; j < numypixels; j++)
            {
                for (i = 0; i < numxpixels; i++)
                {
                    if ((type != GlCompressedRgbaS3TcDxt1Ext) || (srccolors[j][i][3] <= Alphacut))
                    {
                        var testcv = (uint) (srccolors[j][i][0] * srccolors[j][i][0] * Redweight +
                                                srccolors[j][i][1] * srccolors[j][i][1] * Greenweight +
                                                srccolors[j][i][2] * srccolors[j][i][2] * Blueweight);
                        if (testcv > highcv)
                        {
                            highcv = testcv;
                            bestcolor[1] = srccolors[j][i];
                        }
                        else if (testcv < lowcv)
                        {
                            lowcv = testcv;
                            bestcolor[0] = srccolors[j][i];
                        }
                    }
                    else haveAlpha = GlTrue;
                }
            }

            for (j = 0; j < 2; j++)
            {
                for (i = 0; i < 3; i++)
                {
                    basecolors[j][i] = bestcolor[j][i];
                }
            }
            bestcolor[0] = basecolors[0];
            bestcolor[1] = basecolors[1];

            Fancybasecolorsearch(srccolors, bestcolor, numxpixels, numypixels);
            Storedxtencodedblock(blkaddr, srccolors, bestcolor, numxpixels, numypixels, type, haveAlpha);
        }

        private static void Writedxt5Encodedalphablock(GLubyte* blkaddr, GLubyte alphabase1, GLubyte alphabase2,
            GLubyte* alphaenc)
        {
            *blkaddr++ = alphabase1;
            *blkaddr++ = alphabase2;
            *blkaddr++ = (byte) (alphaenc[0] | (alphaenc[1] << 3) | ((alphaenc[2] & 3) << 6));
            *blkaddr++ =
                (byte) ((alphaenc[2] >> 2) | (alphaenc[3] << 1) | (alphaenc[4] << 4) | ((alphaenc[5] & 1) << 7));
            *blkaddr++ = (byte) ((alphaenc[5] >> 1) | (alphaenc[6] << 2) | (alphaenc[7] << 5));
            *blkaddr++ = (byte) (alphaenc[8] | (alphaenc[9] << 3) | ((alphaenc[10] & 3) << 6));
            *blkaddr++ =
                (byte) ((alphaenc[10] >> 2) | (alphaenc[11] << 1) | (alphaenc[12] << 4) | ((alphaenc[13] & 1) << 7));
            *blkaddr = (byte) ((alphaenc[13] >> 1) | (alphaenc[14] << 2) | (alphaenc[15] << 5));
        }

        // ReSharper disable once FunctionComplexityOverflow
        private static void Encodedxt5Alpha(GLubyte* blkaddr, GLubyte*** srccolors,
            GLint numxpixels, GLint numypixels)
        {
            GLubyte* alphabase = stackalloc GLubyte[2];
            GLubyte* alphause = stackalloc GLubyte[2];
            GLshort* alphatest = stackalloc GLshort[2];
            GLubyte* alphaenc1 = stackalloc GLubyte[16];
            GLubyte* alphaenc2 = stackalloc GLubyte[16];
            GLubyte* alphaenc3 = stackalloc GLubyte[16];
            GLubyte* acutValues = stackalloc GLubyte[7];
            GLubyte i, j, aindex;
            var alphaabsmin = GlFalse;
            var alphaabsmax = GlFalse;
            GLshort alphadist;

            alphabase[0] = 0xff;
            alphabase[1] = 0x0;
            for (j = 0; j < numypixels; j++)
            {
                for (i = 0; i < numxpixels; i++)
                {
                    if (srccolors[j][i][3] == 0)
                        alphaabsmin = GlTrue;
                    else if (srccolors[j][i][3] == 255)
                        alphaabsmax = GlTrue;
                    else
                    {
                        if (srccolors[j][i][3] > alphabase[1])
                            alphabase[1] = srccolors[j][i][3];
                        if (srccolors[j][i][3] < alphabase[0])
                            alphabase[0] = srccolors[j][i][3];
                    }
                }
            }


            if ((alphabase[0] > alphabase[1]) && !(alphaabsmin != 0 && alphaabsmax != 0))
            {
                *blkaddr++ = srccolors[0][0][3];
                blkaddr++;
                *blkaddr++ = 0;
                *blkaddr++ = 0;
                *blkaddr++ = 0;
                *blkaddr++ = 0;
                *blkaddr++ = 0;
                *blkaddr = 0;
                return;
            }

            uint alphablockerror1 = 0x0;
            var alphablockerror2 = 0xffffffff;
            var alphablockerror3 = 0xffffffff;
            if (alphaabsmin != 0) alphause[0] = 0;
            else alphause[0] = alphabase[0];
            if (alphaabsmax != 0) alphause[1] = 255;
            else alphause[1] = alphabase[1];
            for (aindex = 0; aindex < 7; aindex++)
            {
                acutValues[aindex] =
                    (byte) ((alphause[0] * (2 * aindex + 1) + alphause[1] * (14 - (2 * aindex + 1))) / 14);
            }

            for (j = 0; j < numypixels; j++)
            {
                for (i = 0; i < numxpixels; i++)
                {
                    if (srccolors[j][i][3] > acutValues[0])
                    {
                        alphaenc1[4 * j + i] = 0;
                        alphadist = (short) (srccolors[j][i][3] - alphause[1]);
                    }
                    else if (srccolors[j][i][3] > acutValues[1])
                    {
                        alphaenc1[4 * j + i] = 2;
                        alphadist = (short) (srccolors[j][i][3] - (alphause[1] * 6 + alphause[0] * 1) / 7);
                    }
                    else if (srccolors[j][i][3] > acutValues[2])
                    {
                        alphaenc1[4 * j + i] = 3;
                        alphadist = (short) (srccolors[j][i][3] - (alphause[1] * 5 + alphause[0] * 2) / 7);
                    }
                    else if (srccolors[j][i][3] > acutValues[3])
                    {
                        alphaenc1[4 * j + i] = 4;
                        alphadist = (short) (srccolors[j][i][3] - (alphause[1] * 4 + alphause[0] * 3) / 7);
                    }
                    else if (srccolors[j][i][3] > acutValues[4])
                    {
                        alphaenc1[4 * j + i] = 5;
                        alphadist = (short) (srccolors[j][i][3] - (alphause[1] * 3 + alphause[0] * 4) / 7);
                    }
                    else if (srccolors[j][i][3] > acutValues[5])
                    {
                        alphaenc1[4 * j + i] = 6;
                        alphadist = (short) (srccolors[j][i][3] - (alphause[1] * 2 + alphause[0] * 5) / 7);
                    }
                    else if (srccolors[j][i][3] > acutValues[6])
                    {
                        alphaenc1[4 * j + i] = 7;
                        alphadist = (short) (srccolors[j][i][3] - (alphause[1] * 1 + alphause[0] * 6) / 7);
                    }
                    else
                    {
                        alphaenc1[4 * j + i] = 1;
                        alphadist = (short) (srccolors[j][i][3] - alphause[0]);
                    }
                    alphablockerror1 += (uint) (alphadist * alphadist);
                }
            }
            if (alphablockerror1 >= 32)
            {
                alphablockerror2 = 0;
                for (aindex = 0; aindex < 5; aindex++)
                {
                    acutValues[aindex] =
                        (byte) ((alphabase[0] * (10 - (2 * aindex + 1)) + alphabase[1] * (2 * aindex + 1)) / 10);
                }
                for (j = 0; j < numypixels; j++)
                {
                    for (i = 0; i < numxpixels; i++)
                    {
                        if (srccolors[j][i][3] == 0)
                        {
                            alphaenc2[4 * j + i] = 6;
                            alphadist = 0;
                        }
                        else if (srccolors[j][i][3] == 255)
                        {
                            alphaenc2[4 * j + i] = 7;
                            alphadist = 0;
                        }
                        else if (srccolors[j][i][3] <= acutValues[0])
                        {
                            alphaenc2[4 * j + i] = 0;
                            alphadist = (short) (srccolors[j][i][3] - alphabase[0]);
                        }
                        else if (srccolors[j][i][3] <= acutValues[1])
                        {
                            alphaenc2[4 * j + i] = 2;
                            alphadist = (short) (srccolors[j][i][3] - (alphabase[0] * 4 + alphabase[1] * 1) / 5);
                        }
                        else if (srccolors[j][i][3] <= acutValues[2])
                        {
                            alphaenc2[4 * j + i] = 3;
                            alphadist = (short) (srccolors[j][i][3] - (alphabase[0] * 3 + alphabase[1] * 2) / 5);
                        }
                        else if (srccolors[j][i][3] <= acutValues[3])
                        {
                            alphaenc2[4 * j + i] = 4;
                            alphadist = (short) (srccolors[j][i][3] - (alphabase[0] * 2 + alphabase[1] * 3) / 5);
                        }
                        else if (srccolors[j][i][3] <= acutValues[4])
                        {
                            alphaenc2[4 * j + i] = 5;
                            alphadist = (short) (srccolors[j][i][3] - (alphabase[0] * 1 + alphabase[1] * 4) / 5);
                        }
                        else
                        {
                            alphaenc2[4 * j + i] = 1;
                            alphadist = (short) (srccolors[j][i][3] - alphabase[1]);
                        }
                        alphablockerror2 += (uint) (alphadist * alphadist);
                    }
                }


                if ((alphablockerror2 > 96) && (alphablockerror1 > 96))
                {
                    GLshort blockerrlin1 = 0;
                    GLshort blockerrlin2 = 0;
                    GLubyte nralphainrangelow = 0;
                    GLubyte nralphainrangehigh = 0;
                    alphatest[0] = 0xff;
                    alphatest[1] = 0x0;
                    for (j = 0; j < numypixels; j++)
                    {
                        for (i = 0; i < numxpixels; i++)
                        {
                            if ((srccolors[j][i][3] > alphatest[1]) &&
                                (srccolors[j][i][3] < (255 - (alphabase[1] - alphabase[0]) / 28)))
                                alphatest[1] = srccolors[j][i][3];
                            if ((srccolors[j][i][3] < alphatest[0]) &&
                                (srccolors[j][i][3] > (alphabase[1] - alphabase[0]) / 28))
                                alphatest[0] = srccolors[j][i][3];
                        }
                    }
                    if (alphatest[1] <= alphatest[0])
                    {
                        alphatest[0] = 1;
                        alphatest[1] = 254;
                    }
                    for (aindex = 0; aindex < 5; aindex++)
                    {
                        acutValues[aindex] =
                            (byte) ((alphatest[0] * (10 - (2 * aindex + 1)) + alphatest[1] * (2 * aindex + 1)) / 10);
                    }

                    for (j = 0; j < numypixels; j++)
                    {
                        for (i = 0; i < numxpixels; i++)
                        {
                            if (srccolors[j][i][3] <= alphatest[0] / 2)
                            {
                            }
                            else if (srccolors[j][i][3] > ((255 + alphatest[1]) / 2))
                            {
                            }
                            else if (srccolors[j][i][3] <= acutValues[0])
                            {
                                blockerrlin1 += (short) ((srccolors[j][i][3] - alphatest[0]));
                                nralphainrangelow += 1;
                            }
                            else if (srccolors[j][i][3] <= acutValues[1])
                            {
                                blockerrlin1 +=
                                    (short) ((srccolors[j][i][3] - (alphatest[0] * 4 + alphatest[1] * 1) / 5));
                                blockerrlin2 +=
                                    (short) ((srccolors[j][i][3] - (alphatest[0] * 4 + alphatest[1] * 1) / 5));
                                nralphainrangelow += 1;
                                nralphainrangehigh += 1;
                            }
                            else if (srccolors[j][i][3] <= acutValues[2])
                            {
                                blockerrlin1 +=
                                    (short) ((srccolors[j][i][3] - (alphatest[0] * 3 + alphatest[1] * 2) / 5));
                                blockerrlin2 +=
                                    (short) ((srccolors[j][i][3] - (alphatest[0] * 3 + alphatest[1] * 2) / 5));
                                nralphainrangelow += 1;
                                nralphainrangehigh += 1;
                            }
                            else if (srccolors[j][i][3] <= acutValues[3])
                            {
                                blockerrlin1 +=
                                    (short) ((srccolors[j][i][3] - (alphatest[0] * 2 + alphatest[1] * 3) / 5));
                                blockerrlin2 +=
                                    (short) ((srccolors[j][i][3] - (alphatest[0] * 2 + alphatest[1] * 3) / 5));
                                nralphainrangelow += 1;
                                nralphainrangehigh += 1;
                            }
                            else if (srccolors[j][i][3] <= acutValues[4])
                            {
                                blockerrlin1 +=
                                    (short) ((srccolors[j][i][3] - (alphatest[0] * 1 + alphatest[1] * 4) / 5));
                                blockerrlin2 +=
                                    (short) ((srccolors[j][i][3] - (alphatest[0] * 1 + alphatest[1] * 4) / 5));
                                nralphainrangelow += 1;
                                nralphainrangehigh += 1;
                            }
                            else
                            {
                                blockerrlin2 += (short) ((srccolors[j][i][3] - alphatest[1]));
                                nralphainrangehigh += 1;
                            }
                        }
                    }

                    if (nralphainrangelow == 0) nralphainrangelow = 1;
                    if (nralphainrangehigh == 0) nralphainrangehigh = 1;
                    alphatest[0] = (short) (alphatest[0] + (blockerrlin1 / nralphainrangelow));
                    if (alphatest[0] < 0)
                    {
                        alphatest[0] = 0;
                    }
                    alphatest[1] = (short) (alphatest[1] + (blockerrlin2 / nralphainrangehigh));
                    if (alphatest[1] > 255)
                    {
                        alphatest[1] = 255;
                    }

                    alphablockerror3 = 0;
                    for (aindex = 0; aindex < 5; aindex++)
                    {
                        acutValues[aindex] =
                            (byte) ((alphatest[0] * (10 - (2 * aindex + 1)) + alphatest[1] * (2 * aindex + 1)) / 10);
                    }
                    for (j = 0; j < numypixels; j++)
                    {
                        for (i = 0; i < numxpixels; i++)
                        {
                            if (srccolors[j][i][3] <= alphatest[0] / 2)
                            {
                                alphaenc3[4 * j + i] = 6;
                                alphadist = srccolors[j][i][3];
                            }
                            else if (srccolors[j][i][3] > ((255 + alphatest[1]) / 2))
                            {
                                alphaenc3[4 * j + i] = 7;
                                alphadist = (short) (255 - srccolors[j][i][3]);
                            }
                            else if (srccolors[j][i][3] <= acutValues[0])
                            {
                                alphaenc3[4 * j + i] = 0;
                                alphadist = (short) (srccolors[j][i][3] - alphatest[0]);
                            }
                            else if (srccolors[j][i][3] <= acutValues[1])
                            {
                                alphaenc3[4 * j + i] = 2;
                                alphadist = (short) (srccolors[j][i][3] - (alphatest[0] * 4 + alphatest[1] * 1) / 5);
                            }
                            else if (srccolors[j][i][3] <= acutValues[2])
                            {
                                alphaenc3[4 * j + i] = 3;
                                alphadist = (short) (srccolors[j][i][3] - (alphatest[0] * 3 + alphatest[1] * 2) / 5);
                            }
                            else if (srccolors[j][i][3] <= acutValues[3])
                            {
                                alphaenc3[4 * j + i] = 4;
                                alphadist = (short) (srccolors[j][i][3] - (alphatest[0] * 2 + alphatest[1] * 3) / 5);
                            }
                            else if (srccolors[j][i][3] <= acutValues[4])
                            {
                                alphaenc3[4 * j + i] = 5;
                                alphadist = (short) (srccolors[j][i][3] - (alphatest[0] * 1 + alphatest[1] * 4) / 5);
                            }
                            else
                            {
                                alphaenc3[4 * j + i] = 1;
                                alphadist = (short) (srccolors[j][i][3] - alphatest[1]);
                            }
                            alphablockerror3 += (uint) (alphadist * alphadist);
                        }
                    }
                }
            }

            if ((alphablockerror1 <= alphablockerror2) && (alphablockerror1 <= alphablockerror3))
            {
                Writedxt5Encodedalphablock(blkaddr, alphause[1], alphause[0], alphaenc1);
            }
            else if (alphablockerror2 <= alphablockerror3)
            {
                Writedxt5Encodedalphablock(blkaddr, alphabase[0], alphabase[1], alphaenc2);
            }
            else
            {
                Writedxt5Encodedalphablock(blkaddr, (GLubyte) alphatest[0], (GLubyte) alphatest[1], alphaenc3);
            }
        }

        private static void Extractsrccolors(GLubyte*** srcpixels, GLchan* srcaddr,
            GLint srcRowStride, GLint numxpixels, GLint numypixels, GLint comps)
        {
            GLubyte j;
            for (j = 0; j < numypixels; j++)
            {
                var curaddr = srcaddr + j * srcRowStride * comps;
                GLubyte i;
                for (i = 0; i < numxpixels; i++)
                {
                    GLubyte c;
                    for (c = 0; c < comps; c++)
                    {
                        if (c < 3)
                            srcpixels[j][i][2 - c] = (*curaddr++);
                        else
                            srcpixels[j][i][c] = (*curaddr++);
                    }
                }
            }
        }
    }
}

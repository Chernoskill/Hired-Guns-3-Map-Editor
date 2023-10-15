/*
 * IMG_lbm.c
 * 
  SDL_image:  An example image loading library for use with SDL
  Copyright (C) 1997-2012 Sam Lantinga <slouken@libsdl.org>

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

/* This is a ILBM image file loading framework
   Load IFF pictures, PBM & ILBM packing methods, with or without stencil
   Written by Daniel Morais ( Daniel AT Morais DOT com ) in September 2001.
   24 bits ILBM files support added by Marc Le Douarain (http://www.multimania.com/mavati)
   in December 2002.
   EHB and HAM (specific Amiga graphic chip modes) support added by Marc Le Douarain
   (http://www.multimania.com/mavati) in December 2003.
   Stencil and colorkey fixes by David Raulo (david.raulo AT free DOT fr) in February 2004.
   Buffer overflow fix in RLE decompression by David Raulo in January 2008.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace PDNIFFPlugin
{
    public static class MyExtensions
    {
        public static int ReadBEInt32(this Stream input)
        {
            byte[] buffer;
            buffer = new byte[4];
            if (input.Read(buffer, 0, 4) == 0)
                throw new System.InvalidOperationException("EOF in int");
            return (buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3];
        }
        public static uint ReadBEUInt32(this Stream input)
        {
            byte[] buffer;
            buffer = new byte[4];
            if (input.Read(buffer, 0, 4) == 0)
                throw new System.InvalidOperationException("EOF in uint");
            return (uint)((buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3]);
        }
        public static short ReadBEInt16(this Stream input)
        {
            byte[] buffer;
            buffer = new byte[2];
            if (input.Read(buffer, 0, 2) == 0)
                throw new System.InvalidOperationException("EOF in short");
            return (short)((buffer[0] << 8) | buffer[1]);
        }
        public static ushort ReadBEUInt16(this Stream input)
        {
            byte[] buffer;
            buffer = new byte[2];
            if (input.Read(buffer, 0, 2) == 0)
                throw new System.InvalidOperationException("EOF in ushort");
            return (ushort)((buffer[0] << 8) | buffer[1]);
        }
    }

    /* Structure for an IFF picture ( BMHD = Bitmap Header ) */
    public struct BMHD
    {
        public BMHD(Stream input)
        {
            w = input.ReadBEUInt16();
            h = input.ReadBEUInt16();
            x = input.ReadBEInt16();
            y = input.ReadBEInt16();
            planes = (byte)input.ReadByte();
            mask = (byte)input.ReadByte();
            tcomp = (byte)input.ReadByte();
            pad1 = (byte)input.ReadByte();
            tcolor = input.ReadBEUInt16();
            xAspect = (byte)input.ReadByte();
            yAspect = (byte)input.ReadByte();
            Lpage = input.ReadBEInt16();
            Hpage = input.ReadBEInt16();
        }

        public ushort w, h;		/* width & height of the bitmap in pixels */
        public short x, y;		/* screen coordinates of the bitmap */
        public byte planes;		/* number of planes of the bitmap */
        public byte mask;			/* mask type ( 0 => no mask ) */
        public byte tcomp;		/* compression type */
        public byte pad1;			/* dummy value, for padding */
        public ushort tcolor;		/* transparent color */
        public byte xAspect,		/* pixel aspect ratio */
                    yAspect;
        public short Lpage;		/* width of the screen in pixels */
        public short Hpage;		/* height of the screen in pixels */
    };

    public class IFFFileType
    {
        public int MAXCOLORS = 256;

        /*
        public IFFFileType()
            : base("IFF",
                FileTypeFlags.SupportsLoading | FileTypeFlags.SupportsSaving,
                new String[] { ".iff" })
        {
        }
        */

        public Bitmap IFFToPNG(Stream input)
        {
            long start;
            // SDL_Surface *Image;
            byte[] id, colormap;
            byte pbm, msk;
            int color, count;
            long bytesloaded;
            uint size, nbcolors;
            uint i, j, bytesperline, nbplanes, stencil, plane, h;
            uint remainingbytes;
            uint width;
            BMHD bmhd;
            string error;
            bool flagHAM, flagEHB;
            System.Text.Encoding aenc = new System.Text.ASCIIEncoding();

            id = new byte[4];
            colormap = new byte[MAXCOLORS * 3];

            try
            {
                start = input.Position;

                if (4 != input.Read(id, 0, 4))
                {
                    error = "error reading IFF chunk";
                    goto done;
                }

                /* Should be the size of the file minus 4+4 ( 'FORM'+size ) */
                size = input.ReadBEUInt32();

                if (aenc.GetString(id) != "FORM")
                {
                    error = "not a IFF file " + aenc.GetString(id);
                    goto done;
                }

                if (input.Read(id, 0, 4) != 4)
                {
                    error = "error reading IFF chunk";
                    goto done;
                }

                pbm = 0;

                /* File format : PBM=Packed Bitmap, ILBM=Interleaved Bitmap */
                if (aenc.GetString(id) == "PBM ") pbm = 1;
                else if (aenc.GetString(id) != "ILBM")
                {
                    error = "not a IFF picture";
                    goto done;
                }

                nbcolors = 0;

                flagHAM = false;
                flagEHB = false;
                // Work around C# bug where it insists this variable has to be assigned.
                bmhd = new BMHD();

                while (aenc.GetString(id) != "BODY")
                {
                    if (input.Read(id, 0, 4) != 4)
                    {
                        error = "error reading IFF chunk";
                        goto done;
                    }

                    size = input.ReadBEUInt32();

                    bytesloaded = 0;

                    if (aenc.GetString(id) == "BMHD") /* Bitmap header */
                    {
                        long streamPos = input.Position;
                        bmhd = new BMHD(input);

                        bytesloaded = input.Position - streamPos;
                    }

                    if (aenc.GetString(id) == "CMAP") /* palette ( Color Map ) */
                    {
                        if (size != input.Read(colormap, 0, (int)size))
                        {
                            error = "error reading CMAP chunk";
                            goto done;
                        }

                        bytesloaded = size;
                        nbcolors = size / 3;
                    }

                    if (aenc.GetString(id) == "CAMG") /* Amiga ViewMode  */
                    {
                        uint viewmodes = input.ReadBEUInt32();

                        bytesloaded = size;
                        if ((viewmodes & 0x0800) > 0)
                            flagHAM = true;
                        if ((viewmodes & 0x0080) > 0)
                            flagEHB = true;
                    }

                    if (aenc.GetString(id) != "BODY")
                    {
                        if ((size & 1) == 1) ++size;    /* padding ! */
                        size -= (uint)bytesloaded;
                        /* skip the remaining bytes of this chunk */
                        if (size > 0) input.Seek(size, SeekOrigin.Current);
                    }
                }

                if (bmhd.w == 0)
                {
                    error = "bad data";
                    goto done;
                }
                /* compute some usefull values, based on the bitmap header */

                width = (uint)((bmhd.w + 15) & 0xFFFFFFF0);  /* Width in pixels modulo 16 */

                bytesperline = (uint)(((bmhd.w + 15) / 16) * 2);

                nbplanes = bmhd.planes;

                if (pbm == 1)                         /* File format : 'Packed Bitmap' */
                {
                    bytesperline *= 8;
                    nbplanes = 1;
                }

                stencil = (uint)(bmhd.mask & 1);   /* There is a mask ( 'stencil' ) */

                /* Allocate memory for a temporary buffer ( used for
                       decompression/deinterleaving ) */

                Bitmap b;
                if (((bmhd.planes == 24 || flagHAM) ? 24 : 8) == 8)
                    b = new Bitmap((int)width, bmhd.h, PixelFormat.Format8bppIndexed);
                else
                    b = new Bitmap((int)width, bmhd.h, PixelFormat.Format24bppRgb);

                /* Update palette informations */

                /* There is no palette in 24 bits ILBM file */
                if (nbcolors > 0 && !flagHAM)
                {
                    ColorPalette palette = b.Palette;
                    /* FIXME: Should this include the stencil? See comment below */
                    int nbrcolorsfinal = 1 << (int)(nbplanes + stencil);

                    for (i = 0; i < nbcolors; i++)
                    {
                        palette.Entries[i] = Color.FromArgb(colormap[i * 3 + 0], colormap[i * 3 + 1], colormap[i * 3 + 2]);
                    }

                    /* Amiga EHB mode (Extra-Half-Bright) */
                    /* 6 bitplanes mode with a 32 colors palette */
                    /* The 32 last colors are the same but divided by 2 */
                    /* Some Amiga pictures save 64 colors with 32 last wrong colors, */
                    /* they shouldn't !, and here we overwrite these 32 bad colors. */
                    if ((nbcolors == 32 || flagEHB) && (1 << bmhd.planes) == 64)
                    {
                        nbcolors = 64;
                        for (i = 32; i < 64; i++)
                        {
                            palette.Entries[i] = Color.FromArgb(colormap[(i - 32) * 3 + 0] / 2, colormap[(i - 32) * 3 + 1] / 2, colormap[(i - 32) * 3 + 2] / 2);
                        }
                    }

                    /* If nbcolors < 2^nbplanes, repeat the colormap */
                    /* This happens when pictures have a stencil mask */
                    if (nbrcolorsfinal > (1 << bmhd.planes))
                    {
                        nbrcolorsfinal = (1 << bmhd.planes);
                    }
                    for (i = nbcolors; i < (uint)nbrcolorsfinal; i++)
                    {
                        palette.Entries[i] = palette.Entries[i % nbcolors];
                    }
                    // TODO: Matters?
                    //if ( pbm == 0 )
                    //    Image->format->palette->ncolors = nbrcolorsfinal;
                    b.Palette = palette;

                    if ((bmhd.mask & 2) != 0)               /* There is a transparent color */
                        b.MakeTransparent(palette.Entries[bmhd.tcolor]);
                }

                /* Get the bitmap */

                for (h = 0; h < bmhd.h; h++)
                {
                    byte[] MiniBuf = new byte[bytesperline * (nbplanes + stencil)];
                    byte[] line;
                    /* uncompress the datas of each planes */

                    for (plane = 0; plane < (nbplanes + stencil); plane++)
                    {
                        uint mbIdx = plane * bytesperline;

                        remainingbytes = bytesperline;

                        if (bmhd.tcomp == 1)        /* Datas are compressed */
                        {
                            do
                            {
                                count = (byte)input.ReadByte();
                                if (count == -1)
                                {
                                    error = "error reading BODY chunk";
                                    goto done;
                                }

                                if ((count & 0x80) != 0)
                                {
                                    count ^= 0xFF;
                                    count += 2; /* now it */

                                    if (count > remainingbytes)
                                    {
                                        error = "error reading BODY chunk";
                                        goto done;
                                    }
                                    color = input.ReadByte();
                                    if (color == -1)
                                    {
                                        error = "error reading BODY chunk";
                                        goto done;
                                    }
                                    for (int ii = 0; ii < count; ii++)
                                        MiniBuf[mbIdx + ii] = (byte)color;
                                }
                                else
                                {
                                    ++count;

                                    if (count > remainingbytes)
                                    {
                                        error = "error reading BODY chunk";
                                        goto done;
                                    }
                                    line = new byte[count];
                                    if (input.Read(line, 0, count) != count)
                                    {
                                        error = "error reading BODY chunk";
                                        goto done;
                                    }
                                    line.CopyTo(MiniBuf, mbIdx);
                                }

                                mbIdx += (uint)count;
                                remainingbytes -= (uint)count;

                            } while (remainingbytes > 0);
                        }
                        else
                        {
                            line = new byte[bytesperline];
                            if (input.Read(line, 0, (int)bytesperline) != bytesperline)
                            {
                                error = "error reading BODY chunk";
                                goto done;
                            }
                            line.CopyTo(MiniBuf, mbIdx);
                        }
                    }

                    /* One line has been read, store it ! */
                    Rectangle rect = new Rectangle(0, (int)h, (int)width, 1); // (int)bmhd.h);
                    BitmapData bmpdata;
                    if (nbplanes == 24 || flagHAM)
                    {
                        bmpdata = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    }
                    else
                        bmpdata = b.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                    byte[] bdbits = new byte[bmpdata.Stride * bmpdata.Height];
                    int bdidx = 0;
                    byte[] null8 = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

                    if (pbm != 0)                 /* File format : 'Packed Bitmap' */
                    {
                        MiniBuf.CopyTo(bdbits,0);
                        //MiniBuf.CopyTo(0, bdbits, 0, (int)width);
                    }
                    else        /* We have to un-interlace the bits ! */
                    {
                        if (nbplanes != 24 && !flagHAM)
                        {
                            size = (width + 7) / 8;

                            for (i = 0; i < size; i++)
                            {
                                null8.CopyTo(bdbits, bdidx);
                                //null8.CopyTo(0, bdbits, bdidx, null8.Length);

                                for (plane = 0; plane < (nbplanes + stencil); plane++)
                                {
                                    color = MiniBuf[i + (plane * bytesperline)];
                                    msk = 0x80;

                                    for (j = 0; j < 8; j++)
                                    {
                                        if ((plane + j) <= 7)
                                            bdbits[bdidx + j] |= (byte)((color & msk) >> (int)(7 - plane - j));
                                        else
                                            bdbits[bdidx + j] |= (byte)((color & msk) << (int)(plane + j - 7));
                                        msk >>= 1;
                                    }
                                }
                                bdidx += 8;
                            }
                        }
                        else
                        {
                            uint finalcolor = 0;
                            size = (width + 7) / 8;
                            /* 24 bitplanes ILBM : R0...R7,G0...G7,B0...B7 */
                            /* or HAM (6 bitplanes) or HAM8 (8 bitplanes) modes */
                            for (i = 0; i < width; i = i + 8)
                            {
                                byte maskBit = 0x80;
                                for (j = 0; j < 8; j++)
                                {
                                    uint pixelcolor = 0;
                                    uint maskColor = 1;
                                    byte dataBody;
                                    for (plane = 0; plane < nbplanes; plane++)
                                    {
                                        dataBody = MiniBuf[plane * size + i / 8];
                                        if ((dataBody & maskBit) != 0)
                                            pixelcolor = pixelcolor | maskColor;
                                        maskColor = maskColor << 1;
                                    }
                                    /* HAM : 12 bits RGB image (4 bits per color component) */
                                    /* HAM8 : 18 bits RGB image (6 bits per color component) */
                                    if (flagHAM)
                                    {
                                        switch (pixelcolor >> (int)(nbplanes - 2))
                                        {
                                            case 0: /* take direct color from palette */
                                                finalcolor = (uint)(colormap[pixelcolor * 3] + (colormap[pixelcolor * 3 + 1] << 8) + (colormap[pixelcolor * 3 + 2] << 16));
                                                break;
                                            case 1: /* modify only blue component */
                                                finalcolor = finalcolor & 0x00FFFF;
                                                finalcolor = finalcolor | (pixelcolor << (int)(16 + (10 - nbplanes)));
                                                break;
                                            case 2: /* modify only red component */
                                                finalcolor = finalcolor & 0xFFFF00;
                                                finalcolor = finalcolor | pixelcolor << (int)(10 - nbplanes);
                                                break;
                                            case 3: /* modify only green component */
                                                finalcolor = finalcolor & 0xFF00FF;
                                                finalcolor = finalcolor | (pixelcolor << (int)(8 + (10 - nbplanes)));
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        finalcolor = pixelcolor;
                                    }
                                    bdbits[bdidx + 0] = (byte)(finalcolor >> 16);
                                    bdbits[bdidx + 1] = (byte)(finalcolor >> 8);
                                    bdbits[bdidx + 2] = (byte)(finalcolor);
                                    bdidx += 3;

                                    maskBit = (byte)(maskBit >> 1);
                                }
                            }
                        }
                    }
                    System.Runtime.InteropServices.Marshal.Copy(bdbits, 0, bmpdata.Scan0, bdbits.Length);
                    b.UnlockBits(bmpdata);
                }
                b = new Bitmap(500, 500);
                return b;

                done:
                MessageBox.Show("Problem Importing File: " + error);

                /* END ------- */
                b = new Bitmap(500, 500);
                return b;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
                string s = string.Format("{0}", e.Message);
                MessageBox.Show("Problem Importing File, " + s);

                Bitmap b = new Bitmap(500, 500);
                return b;
            }
        }
        /*
        protected override void OnSave(Document input, Stream output, SaveConfigToken token,
            Surface scratchSurface, ProgressEventHandler callback)
        {
            RenderArgs ra = new RenderArgs(new Surface(input.Size));
            input.Render(ra);

            ra.Bitmap.Save(output, ImageFormat.Bmp);
        }
         */
    }
    /*
    public class IFFFileTypeFactory : IFileTypeFactory
    {
        public FileType[] GetFileTypeInstances()
        {
            return new FileType[] { new IFFFileType() };
        }
    }
    */
}

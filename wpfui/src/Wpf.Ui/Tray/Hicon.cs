// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.


using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wpf.Ui.Tray;

// TODO: This class is the only reason for using System.Drawing.Common. It is worth looking for a way to get hIcon without using it.

/// <summary>
/// Facilitates the creation of a hIcon.
/// </summary>
internal static class Hicon
{
    /// <summary>
    /// Tries to take the icon pointer assigned to the application.
    /// </summary>
    /// <returns></returns>
    public static IntPtr FromApp()
    {
        try
        {
            var processName = Process.GetCurrentProcess().MainModule?.FileName;

            if (String.IsNullOrEmpty(processName))
                return IntPtr.Zero;

            var appIconsExtractIcon = System.Drawing.Icon.ExtractAssociatedIcon(processName);

            if (appIconsExtractIcon == null)
                return IntPtr.Zero;

            

            return appIconsExtractIcon.Handle;
        }
#if DEBUG
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR | Unable to get application hIcon - {e}", "Wpf.Ui.Hicon");
            throw;
#else
        catch
        {
            return IntPtr.Zero;
#endif
        }
    }

    /// <summary>
    /// Tries to allocate an icon to memory and fetch a pointer to it.
    /// </summary>
    /// <param name="source">Image source.</param>
    public static IntPtr FromSource(ImageSource source)
    {
        var hIcon = IntPtr.Zero;
        var bitmapFrame = source as BitmapFrame;

        if (bitmapFrame?.Decoder == null || bitmapFrame.Decoder.Frames.Count < 1)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"ERROR | Unable to allocate hIcon, Decoder source is empty", "Wpf.Ui.Hicon");
#endif

            return IntPtr.Zero;
        }

        
        bitmapFrame = bitmapFrame.Decoder.Frames[0];

        var stride = bitmapFrame.PixelWidth * ((bitmapFrame.Format.BitsPerPixel + 7) / 8);
        var pixels = new byte[bitmapFrame.PixelHeight * stride];

        bitmapFrame.CopyPixels(pixels, stride, 0);

        
        var gcHandle = GCHandle.Alloc(pixels, GCHandleType.Pinned);

        if (!gcHandle.IsAllocated)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"ERROR | Unable to allocate hIcon, allocation failed.", "Wpf.Ui.Hicon");
#endif

            return IntPtr.Zero;
        }


        
        
        var bitmap = new Bitmap(bitmapFrame.PixelWidth, bitmapFrame.PixelHeight, stride,
            System.Drawing.Imaging.PixelFormat.Format32bppPArgb, gcHandle.AddrOfPinnedObject());

        hIcon = bitmap.GetHicon();

        
        gcHandle.Free();

        return hIcon;
    }
}


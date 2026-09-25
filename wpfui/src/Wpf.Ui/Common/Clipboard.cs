// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.


using System;
using System.Threading;

namespace Wpf.Ui.Common;

/// <summary>
/// Provides methods to place data on and retrieve data from the system clipboard.
/// </summary>
public static class Clipboard
{
    /// <summary>
    /// Set the text data to Clipboard.
    /// </summary>
    public static void SetText(string text)
    {
        try
        {
            Thread thread = new(() => System.Windows.Clipboard.SetText(text));

            thread.SetApartmentState(ApartmentState.STA); 
            thread.Start();
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}


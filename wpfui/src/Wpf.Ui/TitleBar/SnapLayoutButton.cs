// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.


using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Media;

namespace Wpf.Ui.TitleBar;

/// <summary>
/// Represents a snap layout button.
/// </summary>
internal class SnapLayoutButton
{
    /// <summary>
    /// Visual controls of the button.
    /// </summary>
    private readonly Wpf.Ui.Controls.Button _visual;

    /// <summary>
    /// Type of the button.
    /// </summary>
    public readonly TitleBarButton Type;

    /// <summary>
    /// Rendered size of the button control.
    /// </summary>
    private Size _renderedSize;

    /// <summary>
    /// Whether the button is clicked.
    /// </summary>
    public bool IsClickedDown { get; set; }

    /// <summary>
    /// Whether the mouse is over the button.
    /// </summary>
    public bool IsHovered { get; private set; }

    /// <summary>
    /// Creates new instance and sets internals.
    /// </summary>
    public SnapLayoutButton(Wpf.Ui.Controls.Button button, TitleBarButton type, double dpiScale)
    {
        _visual = button ?? throw new InvalidOperationException($"Parameter button of the {typeof(SnapLayoutButton)} cannot be null.");

        // TODO: If application is DPI aware, the scale can vary depends on the screen
        

        if (button.IsLoaded)
            UpdateScale(dpiScale);
        else
            button.Loaded += (_, _) => UpdateScale(dpiScale);

        Type = type;
        IsHovered = false;
        IsClickedDown = false;
    }

    public void UpdateScale(double dpiScale)
    {
        
        var renderedWidth = _visual.ActualWidth * dpiScale;
        var renderedHeight = _visual.ActualHeight * dpiScale;

        
        if (renderedWidth < 1 || renderedHeight < 1)
            _renderedSize = new Size(0d, 0d);
        else
            _renderedSize = new Size(renderedWidth, renderedHeight);
    }

    /// <summary>
    /// Invokes click on the button.
    /// </summary>
    public void InvokeClick()
    {
        if (new ButtonAutomationPeer(_visual).GetPattern(PatternInterface.Invoke) is IInvokeProvider invokeProvider)
            invokeProvider.Invoke();

        IsClickedDown = false;
    }

    /// <summary>
    /// Forces button background to change.
    /// </summary>
    public void Hover(SolidColorBrush hoverBrush)
    {
        if (IsHovered)
            return;

        _visual.Background = hoverBrush;
        IsHovered = true;
    }

    /// <summary>
    /// Forces button background to change.
    /// </summary>
    public void RemoveHover(SolidColorBrush regularBrush)
    {
        if (!IsHovered)
            return;

        _visual.Background = regularBrush;

        IsHovered = false;
        IsClickedDown = false;
    }

    /// <summary>
    /// Indicates whether the mouse is over the button.
    /// </summary>
    public bool IsMouseOver(IntPtr positionPointer)
    {
        

        
        if (positionPointer == IntPtr.Zero)
            return false;

        
        if (_renderedSize.Height == 0 && _renderedSize.Width == 0)
            return false;

        var positionWords = positionPointer.ToInt32();

        if (positionWords < 1)
            return false;

        
        var positionX = positionWords & 0xffff;
        
        var positionY = positionWords >> 0x0010;

        
        if (positionX < 0 || positionY < 0)
            return false;

        
        Rect rect;

        try
        {
            
            rect = new Rect(
                _visual.PointToScreen(new Point()),
                _renderedSize);
        }
#if DEBUG
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR | {e}", "Wpf.Ui.SnapLayout");
#else
        catch
        {
#endif
            return false; 
        }

        
        return rect.Contains(new Point(positionX, positionY));
    }
}

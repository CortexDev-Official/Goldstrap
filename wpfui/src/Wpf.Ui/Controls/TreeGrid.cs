// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.


using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Wpf.Ui.Controls;

/// <summary>
/// Work in progress.
/// </summary>
public class TreeGrid : System.Windows.Controls.Primitives.Selector
{
    /// <summary>
    /// Property for <see cref="Headers"/>.
    /// </summary>
    public static readonly DependencyProperty HeadersProperty = DependencyProperty.Register(nameof(Headers),
        typeof(ObservableCollection<TreeGridHeader>), typeof(TreeGrid),
        new PropertyMetadata(new ObservableCollection<TreeGridHeader> { }, OnHeadersChanged));

    
    
    
    
    

    /// <summary>
    /// Content is the data used to generate the child elements of this control.
    /// </summary>
    [Bindable(true)]
    public ObservableCollection<TreeGridHeader> Headers
    {
        get => GetValue(HeadersProperty) as ObservableCollection<TreeGridHeader>;
        set => SetValue(HeadersProperty, value);
    }

    
    
    
    
    
    
    
    
    

    public TreeGrid()
    {
        var x = new System.Windows.Controls.ContentControl();
        var y = new System.Windows.Controls.ItemsControl();
        var z = new System.Windows.Controls.ListBox();
    }

    
    
    
    
    
    
    

    
    
    
    

    
    
    
    
    
    
    
    
    
    
    

    protected virtual void OnHeadersChanged()
    {
        
    }

    protected virtual void OnContentChanged()
    {
        
    }

    private static void OnHeadersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TreeGrid treeGrid)
            return;

        treeGrid.OnHeadersChanged();
    }

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TreeGrid treeGrid)
            return;

        treeGrid.OnContentChanged();
    }
}

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.IO;
using System.Windows.Input;

public static class ObservableHelpers
{
    public static IObservable<IEvent<RoutedEventArgs>> GetClick(this Button control)
    {
        return Observable.FromEvent<RoutedEventArgs>(control, "Click");
    }

    public static IObservable<IEvent<RoutedEventArgs>> GetLoaded(this FrameworkElement fe)
    {
        return Observable.FromEvent<RoutedEventArgs>(fe, "Loaded");
    }

    public static IObservable<IEvent<MouseEventArgs>> GetMouseEnter(this FrameworkElement fe)
    {
        return Observable.FromEvent<MouseEventArgs>(fe, "MouseEnter");
    }

    public static IObservable<IEvent<MouseEventArgs>> GetMouseLeave(this FrameworkElement fe)
    {
        return Observable.FromEvent<MouseEventArgs>(fe, "MouseLeave");
    }

    public static IObservable<IEvent<MouseButtonEventArgs>> GetMouseDown(this FrameworkElement fe)
    {
        return Observable.FromEvent<MouseButtonEventArgs>(fe, "MouseDown");
    }

    public static IObservable<IEvent<MouseButtonEventArgs>> GetMouseUp(this FrameworkElement fe)
    {
        return Observable.FromEvent<MouseButtonEventArgs>(fe, "MouseUp");
    }

#if !SILVERLIGHT
    public static IObservable<IEvent<EventArgs>> GetClosed(this Window x)
    {
        return Observable.FromEvent<EventArgs>(x, "Closed");
    }
#endif

    public static IObservable<WebResponse> GetResponseAsync(this WebRequest request)
    {
        return Observable.FromAsyncPattern<WebResponse>(
            request.BeginGetResponse,
            request.EndGetResponse)();
    }
}
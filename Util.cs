using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;
using System.Disposables;
using System.Windows;

[DataContract]
public class NotifyObject : INotifyPropertyChanged
{
    [IgnoreDataMember]
    PropertyChangedEventHandler m_propertyChanged;

    public event PropertyChangedEventHandler PropertyChanged
    {
        add { m_propertyChanged += value; }
        remove { m_propertyChanged -= value; }
    }

    protected void RaisePropertyChanged(params string[] props)
    {
        PropertyChangedEventHandler handler = m_propertyChanged;

        if (handler != null)
            foreach(var prop in props)
                handler(this, new PropertyChangedEventArgs(prop));
    }
}

public static class StreamExtensions
{
    const int BLOCK_SIZE = 100000;

    public static byte[] ReadToEnd(this Stream stream)
    {
        return stream.ReadBlocks().SelectMany(block => block).ToArray();
    }

    public static IEnumerable<byte[]> ReadBlocks(this Stream stream)
    {
        var buffer = new byte[BLOCK_SIZE];
        int bytesRead = 0;
        do
        {
            bytesRead = stream.Read(buffer, 0, BLOCK_SIZE);
            yield return buffer.Take(bytesRead).ToArray();
        }
        while (bytesRead != 0);
    }

    // ----- Async -----

    public static IObservable<byte[]> ReadToEndAsync(this Stream stream)
    {
        return stream.ReadBlocksAsync().Aggregate(new List<byte>(), (acc, bytes) => { acc.AddRange(bytes); return acc; })
            .Select(list => list.ToArray());
    }
    
    public static IObservable<byte[]> ReadBlocksAsync(this Stream stream)
    {
        return Observable.CreateWithDisposable<byte[]>(observer => {
            var buffer = new byte[BLOCK_SIZE];
            
            Action<int> onRead = null;
            onRead = bytesRead => {
                if (bytesRead == 0)
                {
                    observer.OnCompleted();
                    return;
                }
                observer.OnNext(buffer.Take(bytesRead).ToArray());
                stream.ReadAsync(buffer, 0, BLOCK_SIZE).Subscribe(onRead);
            };
            stream.ReadAsync(buffer, 0, BLOCK_SIZE).Subscribe(onRead);
            return Disposable.Empty;
        });
    }

#if !SILVERLIGHT
    public static IObservable<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count)
    {
        return Observable.FromAsyncPattern<byte[], int, int, int>(stream.BeginRead, stream.EndRead)
            (buffer, offset, count);
    }
#else
	public static IObservable<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count)
    {
        return Observable.FromAsyncPattern<int>(
            (cb, state) => stream.BeginRead(buffer, offset, count, cb, state), stream.EndRead)();
    }
#endif
}

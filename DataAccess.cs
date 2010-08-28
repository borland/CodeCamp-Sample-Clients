using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Threading;

public static class DataAccess
{
    public const string Server = "http://mac:3000";

    public static WebRequest Request(params object[] pathOptions)
    {
        Thread.Sleep(300);
        return WebRequest.Create( Server + "/" + String.Join("/", pathOptions.Select(p => p.ToString()).ToArray()) );
    }

#if !SILVERLIGHT
    public static IEnumerable<Person> LoadPeople()
    {
        return Request("person")
            .GetResponse()
            .GetResponseStream()
            .ReadToEnd()
            .ReadJson<Person[]>();
    }

    public static IEnumerable<Picture> LoadPictures(int personId)
    {
        return Request("person", personId, "picture")
            .GetResponse()
            .GetResponseStream()
            .ReadToEnd()
            .ReadJson<Picture[]>();
    }
    
    public static byte[] LoadRawImage(int personId, int pictureId)
    {
        return Request("person", personId, "picture", pictureId)
            .GetResponse()
            .GetResponseStream()
            .ReadToEnd();
    }
#endif
}
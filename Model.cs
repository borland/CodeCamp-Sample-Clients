using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;

[DataContract(Name = "person")]
public class Person : NotifyObject
{
    int m_id;
    string m_firstName, m_lastName, m_address, m_phoneNumber;

    [DataMember(Name = "id")]
    public int Id
    {
        get { return m_id; }
        set { m_id = value; RaisePropertyChanged("Id"); }
    }

    [DataMember(Name = "first_name")]
    public string FirstName
    {
        get { return m_firstName; }
        set { m_firstName = value; RaisePropertyChanged("FirstName", "Name"); }
    }

    [DataMember(Name = "last_name")]
    public string LastName
    {
        get { return m_lastName; }
        set { m_lastName = value; RaisePropertyChanged("LastName", "Name"); }
    }

    [DataMember(Name = "address")]
    public string Address
    {
        get { return m_address; }
        set { m_address = value; RaisePropertyChanged("Address"); }
    }

    [DataMember(Name = "phone_number")]
    public string PhoneNumber
    {
        get { return m_phoneNumber; }
        set { m_phoneNumber = value; RaisePropertyChanged("PhoneNumber"); }
    }

    [IgnoreDataMember]
    public string Name
    { get { return String.Format("{0} {1}", FirstName, LastName); } }
}

[DataContract(Name = "picture")]
public class Picture : NotifyObject
{
    int m_id, m_personId;
    string m_caption, m_mimeType;
    byte[] m_image;
    
    [DataMember(Name = "id")]
    public int Id
    {
        get { return m_id; }
        set { m_id = value; RaisePropertyChanged("Id"); }
    }

    [DataMember(Name = "person_id")]
    public int PersonId
    {
        get { return m_personId; }
        set { m_personId = value; RaisePropertyChanged("PersonId"); }
    }

    [DataMember(Name = "caption")]
    public string Caption
    {
        get { return m_caption; }
        set { m_caption = value; RaisePropertyChanged("Caption"); }
    }

    [DataMember(Name = "mime_type")]
    public string MimeType
    {
        get { return m_mimeType; }
        set { m_mimeType = value; RaisePropertyChanged("MimeType"); }
    }
    
    // seperate web request to get the binary data
    [IgnoreDataMember]
    public byte[] Image
    {
        get { return m_image; }
        set { m_image = value; RaisePropertyChanged("Image"); }
    }
}

public static class Ext
{
    public static T ReadJson<T>(this byte[] bytes)
    {
        using (var ms = new MemoryStream(bytes))
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(ms);
        }
    }

    public static byte[] WriteJson<T>(this T obj)
    {
        using(var ms = new MemoryStream())
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(ms, obj);
            return ms.GetBuffer();
        }
    }
}

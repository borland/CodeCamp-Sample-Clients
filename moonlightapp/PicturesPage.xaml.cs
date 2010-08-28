using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.IO;

namespace moonlightapp
{
	public partial class PicturesPage : UserControl
	{
		public PicturesPage()
		{
			this.InitializeComponent();
		}
		
		void Close_Click(object sender, RoutedEventArgs e)
		{
			((Grid)Parent).Children.Remove(this);
			IsEnabled = true;
			ClientLog.Log("Done");
		}
	}
	
    public class PicturesWindowVM : NotifyObject
    {
        Person m_person;
        ObservableCollection<Picture> m_pictures;

        public Person Person
        {
            get { return m_person; }
            set { m_person = value; RaisePropertyChanged("Person"); }
        }

        public ObservableCollection<Picture> Pictures
        { 
            get { return m_pictures; }
            set { m_pictures = value; RaisePropertyChanged("Pictures"); }
        }
    }

    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			ClientLog.Log("Converting byte array to bitmap");
            if (value != null && value is byte[])
            {
                var ByteArray = value as byte[];
                var bmp = new BitmapImage();
				bmp.SetSource(new MemoryStream(ByteArray));
                return bmp;
            }
            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new Exception("The method or operation is not implemented."); }
    }

    public class DebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}



﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Concurrency;

namespace FirstWpfApp
{
    /// <summary>
    /// Interaction logic for PicturesWindow.xaml
    /// </summary>
    public partial class PicturesWindow : Window
    {
        public PicturesWindow()
        {
            InitializeComponent();
        }
        
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void AnimateScale(Image image, double to, double milliseconds)
        {
            var current = ((ScaleTransform)image.LayoutTransform);
            var xf = new ScaleTransform(current.ScaleX, current.ScaleY);
            xf.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation { To = to, Duration = TimeSpan.FromMilliseconds(milliseconds) });
            xf.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation { To = to, Duration = TimeSpan.FromMilliseconds(milliseconds) });
            image.LayoutTransform = xf;
        }

        IObservable<Unit> GenerateSignal(int seconds)
        {
            return Observable.Return(new Unit()).Delay(TimeSpan.FromSeconds(seconds), Scheduler.Dispatcher);
        }

        IObservable<Unit> SignalAfter<T>(IObservable<T> start, IObservable<T> cancel, int seconds)
        {
            return start.SelectMany(GenerateSignal(1).TakeUntil(cancel));
        }
		
        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            var image = sender as Image;


            SignalAfter(image.GetMouseEnter(), image.GetMouseLeave(), 1)
                .Subscribe(ping => AnimateScale(image, 1, 250));

            SignalAfter(image.GetMouseLeave(), image.GetMouseEnter(), 1)
                .Subscribe(ping => AnimateScale(image, 0.3, 220));

            SignalAfter(image.GetMouseDown(), image.GetMouseUp(), 2)
             .Subscribe(ping => MessageBox.Show("Mouse held down continously for 2 seconds"));

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

    [ValueConversion(typeof(byte[]), typeof(BitmapImage))]
    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is byte[])
            {
                var ByteArray = value as byte[];
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(ByteArray);
                bmp.EndInit();
                return bmp;
            }
            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { throw new Exception("The method or operation is not implemented."); }
    }
}

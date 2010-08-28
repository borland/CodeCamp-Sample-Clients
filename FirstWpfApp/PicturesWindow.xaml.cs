using System;
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

        void AnimateScale(Image image, double target)
        {
            var current = ((ScaleTransform)image.LayoutTransform);
            var xf = new ScaleTransform(current.ScaleX, current.ScaleY);
            xf.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation { To = target, Duration = TimeSpan.FromMilliseconds(250) });
            xf.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation { To = target, Duration = TimeSpan.FromMilliseconds(250) });
            image.LayoutTransform = xf;
        }

        IObservable<Unit> Ping(int seconds)
        {
            return Observable.Return(new Unit()).Delay(TimeSpan.FromSeconds(seconds), Scheduler.Dispatcher);
        }

        IObservable<Unit> PingIfNotCanceled<T>(IObservable<T> signal, IObservable<T> cancellation, int seconds)
        {
            return signal.SelectMany(Ping(1).TakeUntil(cancellation));
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            var image = sender as Image;

            PingIfNotCanceled(image.GetMouseEnter(), image.GetMouseLeave(), 1)
                .Subscribe(ping => AnimateScale(image, 1));

            PingIfNotCanceled(image.GetMouseLeave(), image.GetMouseEnter(), 1)
                .Subscribe(ping => AnimateScale(image, 0.3));

            PingIfNotCanceled(image.GetMouseDown(), image.GetMouseUp(), 2)
                .Subscribe(ping => MessageBox.Show("Mouse held down continously for 2 seconds"));


            //var tEnter = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            //var tLeave = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            //image.MouseEnter += (s, e2) =>
            //{
            //    tLeave.Stop();
            //    tEnter.Tick += (s3, e3) =>
            //    {
            //        tEnter.Stop();
            //        if (!image.IsMouseOver)
            //            return;

            //        AnimateTo(image, 1);
            //    };
            //    tEnter.Start();
            //};
            //image.MouseLeave += (s, e2) =>
            //{
            //    tEnter.Stop();
            //    tLeave.Tick += (s3, e3) =>
            //    {
            //        tLeave.Stop();
            //        if (image.IsMouseOver)
            //            return;

            //        AnimateTo(image, 0.3);
            //    };
            //    tLeave.Start();                
            //};
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

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Net;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.IO;

namespace FirstWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
            
            FetchButton.Click += (s,e) => ReloadPeople();
        }

        public void ReloadPeople()
        {
            ViewModel.People = new ObservableCollection<Person>(DataAccess.LoadPeople());
        }

        private void ViewPictures_Click(object sender, RoutedEventArgs e)
        {
            var person = ((Button)sender).DataContext as Person;
            if(person == null)
                return;

            var picWindow = new PicturesWindow();
            var childViewModel = new PicturesWindowVM { 
                Person = person, 
                Pictures = new ObservableCollection<Picture>()
            };
                        
            var pictures = DataAccess.LoadPictures(person.Id);
            foreach(var tmp in pictures)
            {
                var picture = tmp;
                childViewModel.Pictures.Add(picture);
                var bytes = DataAccess.LoadRawImage(person.Id, picture.Id);
                picture.Image = bytes;
            }
            
            picWindow.DataContext = childViewModel;
            picWindow.ShowDialog();
        }
    }

    public class MainWindowViewModel : NotifyObject
    {
        ObservableCollection<Person> m_people;

        public ObservableCollection<Person> People
        { 
            get{ return m_people; }
            set{ m_people = value; RaisePropertyChanged("People"); }
        }
    }
}

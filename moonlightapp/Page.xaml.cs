using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading;
using System.ComponentModel;
using System.IO;

namespace moonlightapp
{
	public partial class Page : UserControl
	{
		public Page()
		{
			InitializeComponent();
			VM = new MainWindowViewModel();
            DataContext = VM;

			this.GetLoaded().Subscribe(_ => ReloadPeopleAsync());
            FetchButton.GetClick().Subscribe(_ => ReloadPeopleAsync());
		}
				
        MainWindowViewModel VM { get; set; }

        public void ReloadPeopleAsync()
        {
            VM.People = new ObservableCollection<Person>();
			DataAccess.LoadPeopleAsync().ObserveOnDispatcher().Subscribe(p => VM.People.Add(p));
        }

        private void ViewPictures_Click(object sender, RoutedEventArgs e)
        {
			ClientLog.Log("Viewing pictures");
			
            var person = ((Button)sender).DataContext as Person;
            if(person == null)
                return;

			var picWindow = new PicturesPage();
            var picWindowVM = new PicturesWindowVM { 
                Person = person, 
                Pictures = new ObservableCollection<Picture>()
            };
			
            DataAccess.LoadPicturesAsync(person.Id).ObserveOnDispatcher().Subscribe(picture =>
            {
                picWindowVM.Pictures.Add(picture);
				
                DataAccess.LoadImageAsync(person.Id, picture.Id).ObserveOnDispatcher().Subscribe(bytes => 
					picture.Image = bytes);
            });           
            
            picWindow.DataContext = picWindowVM;
			picWindow.HorizontalAlignment = HorizontalAlignment.Center;
			picWindow.VerticalAlignment = VerticalAlignment.Center;
			
			Container.Children.Add(picWindow);
			ClientLog.Log("Done");
        }
    }

    public class MainWindowViewModel : NotifyObject
    {
        ObservableCollection<Person> m_people;
		bool m_isEnabled = true;
		
        public ObservableCollection<Person> People
        { 
            get{ return m_people; }
            set{ m_people = value; RaisePropertyChanged("People"); }
        }
    }
}


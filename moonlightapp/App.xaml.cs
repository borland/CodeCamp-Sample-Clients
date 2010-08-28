
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace moonlightapp
{


	public partial class App : Application
	{

		public App()
		{
			this.Startup += new StartupEventHandler(OnStartup);
			this.InitializeComponent();
		}

		void OnStartup(object sender, StartupEventArgs args)
		{
			this.RootVisual = new Page();
		}
	}
}


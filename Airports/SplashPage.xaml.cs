// SplashPage.xaml.cs
using System;
using System.Threading.Tasks;
using System.Linq;
using Airports.Models;

namespace Airports
{
	public partial class SplashPage : ContentPage
	{

		//Test
		public SplashPage()
		{
			InitializeComponent();
			StartApp();
		}

		private async void StartApp()
		{
			await Task.Run(() =>
			{
				// save
			});

			if (Application.Current != null)
				Application.Current.MainPage = new AppShell();
		}
	}
}

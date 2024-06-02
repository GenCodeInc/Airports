using Microsoft.Maui.Controls;
using System;
using Microsoft.Maui.Devices;

namespace Airports
{
	public partial class ImagePopupPage : ContentPage
	{

		public ImagePopupPage(string imageUrl)
		{

			InitializeComponent();
			MyImage.Source = imageUrl;
		}

		private async void OnCloseButtonClicked(object sender, EventArgs e)
		{
			// Close the modal page or perform the desired action
			await Navigation.PopModalAsync();
		}
	}
}

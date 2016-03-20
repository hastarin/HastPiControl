// ***********************************************************************
// Assembly         : HastPiControl
// Author           : Jon Benson
// Created          : 14-11-2015
// 
// Last Modified By : Jon Benson
// Last Modified On : 28-11-2015
// ***********************************************************************

namespace HastPiControl
{
    using Windows.UI.Xaml;

    using HastPiControl.Models;

    /// <summary>An empty page that can be used on its own or navigated to within a Frame.</summary>
    public sealed partial class MainPage
    {
        /// <summary>Initializes a new instance of the <see cref="MainPage" /> class.</summary>
        public MainPage()
        {
            this.MainPageViewModel = new PiFaceDigital2ViewModel();
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await this.MainPageViewModel.InitializePiFace();
        }

        /// <summary>Gets or sets the main page view model.</summary>
        /// <value>The main page view model.</value>
        public PiFaceDigital2ViewModel MainPageViewModel { get; set; }
    }
}
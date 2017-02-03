using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Tdmts.WifiRelay
{
    //TDMTS 

    public sealed partial class MainPage : Page
    {

        private DateTime startedDateTime = DateTime.Now;
        private GpioPin pinButton;
        private GpioPin pinRelay1;
        private GpioPin pinRelay2;

        public MainPage()
        {
            this.InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private async void Timer_Tick(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                lblStartDateTime.Text = startedDateTime.ToString();
                lblRunningDateTime.Text = DateTime.Now.Subtract(startedDateTime).ToString();
                lblCurrentDateTime.Text = DateTime.Now.ToString();
            });
        }

        public async void InitializePins()
        {
            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(5000));

                pinButton = GpioController.GetDefault().OpenPin(12, GpioSharingMode.Exclusive);
                pinButton.DebounceTimeout = TimeSpan.FromMilliseconds(50);
                pinButton.SetDriveMode(GpioPinDriveMode.Input);
                
                pinButton.ValueChanged += PinButton_ValueChanged;

                pinRelay1 = GpioController.GetDefault().OpenPin(17, GpioSharingMode.Exclusive);
                pinRelay1.Write(GpioPinValue.Low);
                pinRelay1.SetDriveMode(GpioPinDriveMode.Output);
                
                pinRelay2 = GpioController.GetDefault().OpenPin(27, GpioSharingMode.Exclusive);
                pinRelay2.Write(GpioPinValue.Low);
                pinRelay2.SetDriveMode(GpioPinDriveMode.Output);

                await UpdateStatus();
            }
            catch (Exception x)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    lblException.Text = x.Message;
                });
            }
            
        }

        public async Task<int> UpdateStatus()
        {
            try
            {
                GpioPinValue valueButton = await Task.FromResult(pinButton.Read());
                if (valueButton == GpioPinValue.High)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        lblStatusButton.Text = "Active";
                    });
                }
                else
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        lblStatusButton.Text = "Inactive";
                    });
                }

                GpioPinValue valueRelay1 = await Task.FromResult(pinRelay1.Read());
                if (valueRelay1 == GpioPinValue.High)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        lblStatusRelay1.Text = "Active";
                    });
                }
                else
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        lblStatusRelay1.Text = "Inactive";
                    });
                }

                GpioPinValue valueRelay2 = await Task.FromResult(pinRelay2.Read());
                if (valueRelay2 == GpioPinValue.High)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        lblStatusRelay2.Text = "Active";
                    });
                }
                else
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        lblStatusRelay2.Text = "Inactive";
                    });
                }

                return 0;
            }
            catch (Exception x)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    lblException.Text = x.Message;
                });
                return -1;
            }
            
        }

        private async void PinButton_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            try
            {
                if (args.Edge == GpioPinEdge.FallingEdge)
                {
                    btnToggle_Click(sender, null);
                }
            }
            catch (Exception x)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    lblException.Text = x.Message;
                });
            }
        }

        private async void btnToggle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GpioPinValue valueRelay1 = await Task.FromResult(pinRelay1.Read());
                if (valueRelay1 == GpioPinValue.High)
                {
                    pinRelay1.Write(GpioPinValue.Low);
                }
                else
                {
                    pinRelay1.Write(GpioPinValue.High);
                }

                GpioPinValue valueRelay2 = await Task.FromResult(pinRelay2.Read());
                if (valueRelay2 == GpioPinValue.High)
                {
                    pinRelay2.Write(GpioPinValue.Low);
                }
                else
                {
                    pinRelay2.Write(GpioPinValue.High);
                }

                await UpdateStatus();
            }
            catch (Exception x)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    lblException.Text = x.Message;
                });
            }
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.InitializePins();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (pinButton != null)
                {
                    pinButton.Dispose();
                }
                if (pinRelay1 != null)
                {
                    pinRelay1.Dispose();
                }
                if (pinRelay2 != null)
                {
                    pinRelay2.Dispose();
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}

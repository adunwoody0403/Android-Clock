using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.IO;

namespace DesktopClock.ViewModels
{

    class MainPageViewModel : INotifyPropertyChanged
    {
        public string TimeString { get => timeString; set { timeString = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimeString))); } }
        public bool EnableDate { get => enableDate; set { enableDate = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableDate))); } }
        public string DateString { get => dateString; set { dateString = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DateString))); } }
        public Color Color { get => textColor; set { textColor = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color))); } }
        public double Hue { get => hue; set { hue = value; UpdateColorHSV(); PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hue))); } }
        public double Saturation { get => saturation; set { saturation = value; UpdateColorHSV(); PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Saturation))); } }
        public double Value { get => value; set { this.value = value; UpdateColorHSV(); PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value))); } }
        
        private DateTime time;
        private string timeString;
        private bool enableDate = true;
        private string dateString;
        private double hue;
        private double saturation = 1;
        private double value = 1;
        private Color textColor = Color.Red;

        public event PropertyChangedEventHandler PropertyChanged;

        const string saveFileName = "DesktopClockSettings";

        public enum ColorMode
        {
            Solid,
            Random,
            Daily
        }

        public MainPageViewModel()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(250), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    UpdateTime();
                });

                return true;
            });

            LoadSettingsFromFile();
        }

        private void UpdateTime()
        {
            time = DateTime.Now;
            TimeString = time.ToString("HH:mm:ss");

            if (enableDate)
            {
                DateString = time.ToString("dddd, dd MMMM yyyy");
            }
        }

        private void UpdateColorHSV()
        {
            Color = Color.FromHsv(Hue, Saturation, Value);
        }
        public void SaveSettings()
        {
            try
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), saveFileName);

                using (var stream = File.Open(fileName, FileMode.Create))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        writer.Write(Hue);
                        writer.Write(Saturation);
                        writer.Write(Value);
                        writer.Write(EnableDate);
                        Console.WriteLine($"Saved data to file: H:{Hue} S:{Saturation} V:{Value} Date:{EnableDate}");
                    }
                }
            }
            catch
            {
                Console.WriteLine("Failed to save settings.");
            }
        }

        public void LoadSettingsFromFile()
        {
            try
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), saveFileName);
                if (File.Exists(fileName))
                {
                    using (var stream = File.Open(fileName, FileMode.Open))
                    {
                        using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                        {
                            Hue = reader.ReadDouble();
                            Saturation = reader.ReadDouble();
                            Value = reader.ReadDouble();
                            EnableDate = reader.ReadBoolean();
                            UpdateColorHSV();
                            Console.WriteLine($"Loaded data from file: H:{Hue} S:{Saturation} V:{Value} Date:{EnableDate}");
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Failed to load settings.");
            }
        }
    }
}

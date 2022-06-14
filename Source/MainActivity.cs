using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Views;
using DesktopClock.Views;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace DesktopClock.Droid
{
    [Activity(Label = "DesktopClock", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, /*ScreenOrientation = ScreenOrientation.Portrait,*/ ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        bool showNavigationBar;
        double hideNavigationBarTimer;
        const double hideNavigationBarTimerInterval = 100;
        const double hideNavigationBarTimerDuration = 5000;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            TransparentStatusBar();
            HideNavigationBar();
            PreventScreenTimeout();
            Fullscreen();

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            MessagingCenter.Subscribe<Xamarin.Forms.Application>(this, "OnTap", OnTap);
            MessagingCenter.Subscribe<Xamarin.Forms.Application>(this, "OnDoubleTap", OnDoubleTap);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void Fullscreen()
        {
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
        }

        private void TransparentStatusBar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
            }
        }

        private void PreventScreenTimeout()
        {
            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
        }

        private void ShowNavigationBar()
        {
            Console.WriteLine("Main activity: Show navigation bar.");
            showNavigationBar = true;

            int uiOptions = (int)Window.DecorView.SystemUiVisibility;
            uiOptions |= (int)SystemUiFlags.LowProfile;
            uiOptions |= (int)SystemUiFlags.Fullscreen;
            uiOptions &= (int)~SystemUiFlags.HideNavigation;
            uiOptions |= (int)SystemUiFlags.ImmersiveSticky;
            uiOptions |= (int)SystemUiFlags.LayoutStable;
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;

            if (hideNavigationBarTimer <= 0)
            {
                hideNavigationBarTimer = hideNavigationBarTimerDuration;
                Device.StartTimer(TimeSpan.FromMilliseconds(hideNavigationBarTimerInterval), () =>
                {
                    if (showNavigationBar)
                    {
                        hideNavigationBarTimer -= hideNavigationBarTimerInterval;
                        if (hideNavigationBarTimer <= 0)
                        {
                            hideNavigationBarTimer = 0;
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                HideNavigationBar();
                            });
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            else
            {
                hideNavigationBarTimer = hideNavigationBarTimerDuration;
            }

        }

        private void HideNavigationBar()
        {
            Console.WriteLine("Main activity: Hide navigation bar.");
            showNavigationBar = false;
            hideNavigationBarTimer = 0;

            int uiOptions = (int)Window.DecorView.SystemUiVisibility;
            uiOptions |= (int)SystemUiFlags.LowProfile;
            uiOptions |= (int)SystemUiFlags.Fullscreen;
            uiOptions |= (int)SystemUiFlags.HideNavigation;
            uiOptions |= (int)SystemUiFlags.ImmersiveSticky;
            uiOptions |= (int)SystemUiFlags.LayoutStable;
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
        }

        private void OnTap(Xamarin.Forms.Application app)
        {
            Console.WriteLine("Main activity: Tap");

            if (showNavigationBar) HideNavigationBar();
            else ShowNavigationBar();
            
            PerformHapticFeedback();
        }

        private void OnDoubleTap(Xamarin.Forms.Application app)
        {
            Console.WriteLine("Main activity: Double tap");
            PerformHapticFeedback();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            QuitApplication();
        }

        private void QuitApplication()
        {
            Process.KillProcess(Process.MyPid());
        }

        private void PerformHapticFeedback()
        {
            try
            {
                HapticFeedback.Perform(HapticFeedbackType.Click);
            }
            catch (Exception e)
            {

            }
        }
    }
}
//using Plugin.LocalNotification;
using Android.App;
using Android.Content.PM;
//using Android.OS;
//using AndroidX.Core.App;
//using Microsoft.Maui.Controls.Compatibility;
//using AndroidX.Core.Content;
//using Android.Nfc;
//using Android.Util;
//using Android.Runtime;

//using Android.Content;


namespace EduTrack
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private const string TAG = "EduTrack.MainActivity";
        //protected override void OnCreate(Bundle savedInstanceState)
        //{
        //    LocalNotificationCenter.CreateNotificationChannel();
        //    base.OnCreate(savedInstanceState);
        //    Log.Debug(TAG, "OnCreate: Activity started.");

        //    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        //    {
        //        Log.Debug(TAG, "Creating notification channel...");


        //        //From Sample Code ====================================================================
        //        var channel = new NotificationChannel("default", "Default Channel", NotificationImportance.High)
        //        {
        //            Description = "General notifications"
        //        };

        //        var notificationManager = (NotificationManager)GetSystemService(NotificationService);
        //        notificationManager.CreateNotificationChannel(channel);
        //        Log.Debug(TAG, "Notification channel created.");
        //        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
        //        {
        //            Log.Debug(TAG, "Checking notification permission...");
        //            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.PostNotifications) != Permission.Granted)
        //            {
        //                Log.Debug(TAG, "Requesting notification permission...");
        //                ActivityCompat.RequestPermissions(this, new[] { Android.Manifest.Permission.PostNotifications }, 100);
        //            }
        //            else
        //            {
        //                Log.Debug(TAG, "Notification permission already granted.");
        //            }

        //        }
        //        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
        //        {
        //            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ScheduleExactAlarm) != Permission.Granted)
        //            {
        //                ActivityCompat.RequestPermissions(this, new[] { Android.Manifest.Permission.ScheduleExactAlarm }, 101);
        //            }
        //            else
        //            {
        //                Log.Debug(TAG, "ScheduleExactAlarm permission already granted.");
        //            }
        //        }
        //    }
        //}


        //        public void ShowImmediateNotification() 
        //        { 
        //            var context = Android.App.Application.Context;


        //            var intent = new Intent(context, typeof(MainActivity));
        //            intent.AddFlags(ActivityFlags.ClearTop);
        //            //var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.OneShot);
        //            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.OneShot | PendingIntentFlags.Immutable); // Add FLAG_IMMUTABLE


        //            var notificationBuilder = new NotificationCompat.Builder(context, "default") 
        //                .SetSmallIcon(Resource.Drawable.dotnet_bot_android) // Ensure you have an icon in your resources
        //                .SetContentTitle("Immediate Notification") 
        //                .SetContentText("This is a test notification") 
        //                .SetAutoCancel(true) .SetContentIntent(pendingIntent); 

        //            var notificationManager = NotificationManagerCompat.From(context); 
        //            notificationManager.Notify(0, notificationBuilder.Build()); 

        //            Log.Debug(TAG, "Immediate notification shown."); 
    }
}


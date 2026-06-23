using Android.Content;
using Android.Provider;
using Scheri.PETPanel.Interfaces;

namespace Scheri.PETPanel.Android {
    public class AndroidSettingService(Context context) : IPlatformSettingService {
        private readonly Context _context = context;

        public void RedirectToHomeSettings() {
            try {
                Intent intent = new Intent(Settings.ActionHomeSettings);
                intent.SetFlags(ActivityFlags.NewTask);
                _context.StartActivity(intent);
            } catch (System.Exception) {
                Intent intent = new Intent(Settings.ActionManageDefaultAppsSettings);
                intent.SetFlags(ActivityFlags.NewTask);
                _context.StartActivity(intent);
            }
        }
    }
}

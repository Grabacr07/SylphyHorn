using MetroTrilithon.Lifetime;
using SylphyHorn.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsDesktop;

namespace SylphyHorn.Models
{
    public class StickyWindowsManager : IDisposable
    {
        private static StickyWindowsManager instanceStickyWindowsManager;
        private IDisposable currentMovingOperation;
        public static HashSet<IntPtr> StickyWindows = new HashSet<IntPtr>();
        public static event EventHandler<string> ToggleStickyWindowEvent;
        private static Guid LastDesktopId;
        private StickyWindowsManager()
        {
            VirtualDesktop.CurrentChanged += this.VirtualDesktopOnCurrentChanged;
        }

        public static StickyWindowsManager Instance
        {
            get
            {
                if (instanceStickyWindowsManager == null)
                {
                    instanceStickyWindowsManager = new StickyWindowsManager();
                }
                return instanceStickyWindowsManager;
            }
        }


        public void ToggleStickyWindow()
        {
            var hWnd = InteropHelper.GetForegroundWindowEx();
            if (hWnd == null)
            {
                return;
            }
            var title = InteropHelper.GetWindowText((int)hWnd);

            if (StickyWindows.Contains(hWnd))
            {
                StickyWindows.Remove(hWnd);
                ToggleStickyWindowEvent.Invoke(this, title + ": Is remove from Pins windows");
            }
            else
            {
                StickyWindows.Add(hWnd);
                ToggleStickyWindowEvent.Invoke(this, title + ": Is now Pin");
            }

            Console.WriteLine(StickyWindows.ToString());
            Console.WriteLine("Process ID:" + InteropHelper.GetWindowThreadProcessId(hWnd));
        }

        private void VirtualDesktopOnCurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
        {
            VisualHelper.InvokeOnUIDispatcher(() =>
            {
                var desktops = VirtualDesktop.GetDesktops();
                var newIndex = Array.IndexOf(desktops, e.NewDesktop);
                var newDesktopId = desktops[newIndex].Id;

                if (!GeneralSettings.OverrideOSDefaultKeyCombination ||
                    LastDesktopId == null || LastDesktopId != newDesktopId)
                {
                    MoveSticky(newDesktopId);
                }
            });
        }

        public void MoveSticky(Guid id)
        {
            LastDesktopId = id;
            var tasks = CreateMovingTask(id);

            currentMovingOperation?.Dispose();
            currentMovingOperation = Iterate(tasks);
        }

        /*
        * This create multiple task that will ve executed in sequence order. This is needed to prevent a concurrent probleme that
        * occur when we move multiple windows of a save process ( multiple chrome window for exemple )
        */
        public IEnumerable<Task> CreateMovingTask(Guid id)
        {
            var invalids = new HashSet<IntPtr>();
            foreach (var sticky in StickyWindows)
            {
                if (InteropHelper.IsWindow(sticky))
                {
                    yield return new Task(() =>
                    {
                        VDMHelper.helper.MoveWindowToDesktop(sticky, id);
                    });
                }
                else
                {
                    invalids.Add(sticky);
                }
            }
            StickyWindows.ExceptWith(invalids);
        }
        public IDisposable Iterate(IEnumerable<Task> asyncIterator)
        {
            if (asyncIterator == null) throw new ArgumentNullException("asyncIterator");

            var enumerator = asyncIterator.GetEnumerator();
            if (enumerator == null) throw new InvalidOperationException("Invalid enumerable - GetEnumerator returned null");

            var source = new CancellationTokenSource();
            var tcs = new TaskCompletionSource<object>();
            tcs.Task.ContinueWith(_ => enumerator.Dispose(), TaskContinuationOptions.ExecuteSynchronously);

            Action<Task> recursiveBody = null;
            recursiveBody = delegate
           {
               try
               {
                   if (enumerator.MoveNext())
                   {
                       Task.Delay(TimeSpan.FromMilliseconds(15), source.Token)
                                               .ContinueWith(_ =>
                                               {
                                                   enumerator.Current.Start();
                                                   enumerator.Current.ContinueWith(recursiveBody, TaskContinuationOptions.ExecuteSynchronously);
                                               }, TaskScheduler.FromCurrentSynchronizationContext());
                   }
                   else tcs.TrySetResult(null);
               }
               catch (Exception exc) { tcs.TrySetException(exc); }
           };

            recursiveBody(null);
            return Disposable.Create(() => source.Cancel());
        }

        public void Dispose()
        {
            VirtualDesktop.CurrentChanged -= this.VirtualDesktopOnCurrentChanged;
        }
    }

    public static class VirtualDesktopExtention
    {
        public static VirtualDesktop MoveSticky(this VirtualDesktop desktop)
        {
            StickyWindowsManager.Instance.MoveSticky(desktop.Id);
            return desktop;
        }
    }
}

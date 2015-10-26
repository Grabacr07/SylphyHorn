using System;

namespace SylphyHorn.Models
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using WindowsDesktop;

    using MetroTrilithon.Lifetime;

    public class StickyWindowsManager : IDisposable
    {
        #region Static Fields

        public static HashSet<IntPtr> StickyWindows = new HashSet<IntPtr>();

        private static StickyWindowsManager instanceStickyWindowsManager;

        private static Guid lastDesktopId;

        #endregion

        #region Fields

        private IDisposable currentMovingOperation;

        #endregion

        #region Constructors and Destructors

        private StickyWindowsManager()
        {
            VirtualDesktop.CurrentChanged += this.VirtualDesktopOnCurrentChanged;
        }

        #endregion

        #region Public Events

        public static event EventHandler<string> ToggleStickyWindowEvent;

        #endregion

        #region Public Properties

        public static StickyWindowsManager Instance => instanceStickyWindowsManager ?? (instanceStickyWindowsManager = new StickyWindowsManager());

        #endregion

        #region Public Methods and Operators

        /*
        * This create multiple task that will be executed in sequence order. This is needed to prevent a concurrent probleme that
        * occur when we move multiple windows of a same process ( multiple chrome window for exemple )
        */

        public IEnumerable<Task> CreateMovingTask(Guid id)
        {
            var invalids = new HashSet<IntPtr>();
            foreach (var sticky in StickyWindows)
            {
                if (InteropHelper.IsWindow(sticky))
                {
                    yield return new Task(() => { VDMHelper.helper.MoveWindowToDesktop(sticky, id); });
                }
                else
                {
                    invalids.Add(sticky);
                }
            }
            StickyWindows.ExceptWith(invalids);
        }

        public void Dispose()
        {
            VirtualDesktop.CurrentChanged -= this.VirtualDesktopOnCurrentChanged;
        }

        public IDisposable Iterate(IEnumerable<Task> asyncIterator)
        {
            if (asyncIterator == null)
            {
                throw new ArgumentNullException("asyncIterator");
            }

            var enumerator = asyncIterator.GetEnumerator();
            if (enumerator == null)
            {
                throw new InvalidOperationException("Invalid enumerable - GetEnumerator returned null");
            }

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
                            Task.Delay(TimeSpan.FromMilliseconds(15), source.Token).ContinueWith(
                                _ =>
                                    {
                                        enumerator.Current.Start();
                                        enumerator.Current.ContinueWith(recursiveBody, TaskContinuationOptions.ExecuteSynchronously);
                                    },
                                TaskScheduler.FromCurrentSynchronizationContext());
                        }
                        else
                        {
                            tcs.TrySetResult(null);
                        }
                    }
                    catch (Exception exc)
                    {
                        tcs.TrySetException(exc);
                    }
                };

            recursiveBody(null);
            return Disposable.Create(() => source.Cancel());
        }

        public void MoveSticky(Guid id)
        {
            lastDesktopId = id;
            var tasks = this.CreateMovingTask(id);

            this.currentMovingOperation?.Dispose();
            this.currentMovingOperation = this.Iterate(tasks);
        }

        public void ToggleStickyWindow()
        {
            var hWnd = InteropHelper.GetForegroundWindowEx();
            var title = InteropHelper.GetWindowText((int)hWnd);

            if (StickyWindows.Contains(hWnd))
            {
                StickyWindows.Remove(hWnd);
                ToggleStickyWindowEvent?.Invoke(this, title + ": Is remove from pinned windows");
            }
            else
            {
                StickyWindows.Add(hWnd);
                ToggleStickyWindowEvent?.Invoke(this, title + ": Is now pinned");
            }

            Console.WriteLine(StickyWindows.ToString());
            Console.WriteLine("Process ID:" + InteropHelper.GetWindowThreadProcessId(hWnd));
        }

        #endregion

        #region Methods

        private void VirtualDesktopOnCurrentChanged(object sender, VirtualDesktopChangedEventArgs e)
        {
            VisualHelper.InvokeOnUIDispatcher(
                () =>
                    {
                        var desktops = VirtualDesktop.GetDesktops();
                        var newIndex = Array.IndexOf(desktops, e.NewDesktop);
                        var newDesktopId = desktops[newIndex].Id;

                        if (!GeneralSettings.OverrideOSDefaultKeyCombination || lastDesktopId != newDesktopId)
                        {
                            this.MoveSticky(newDesktopId);
                        }
                    });
        }

        #endregion
    }

    public static class VirtualDesktopExtention
    {
        #region Public Methods and Operators

        public static VirtualDesktop MoveSticky(this VirtualDesktop desktop)
        {
            StickyWindowsManager.Instance.MoveSticky(desktop.Id);
            return desktop;
        }

        #endregion
    }
}
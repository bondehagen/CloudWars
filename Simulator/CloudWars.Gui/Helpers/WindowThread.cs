using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace CloudWars.Helpers
{
    public class WindowThread<TWindow> where TWindow : Window, new()
    {
        private bool canAbortThread = false;
        private Thread thread;
        private TWindow window;

        public WindowThread() {}

        public bool IsClosed()
        {
            return canAbortThread;
        }

        public void Start()
        {
            thread = new Thread(Run) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void Stop()
        {
            if (window != null)
            {
                window.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action) (() => window.Close()));
                while (!canAbortThread) {}
            }
            thread.Abort();
        }

        private void Run()
        {
            window = new TWindow();
            window.Closed += OnClosed;
            window.ShowDialog();
        }

        public void Update(Action<TWindow> action)
        {
            if (window != null)
                window.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action, window);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Dispatcher.CurrentDispatcher.InvokeShutdown();
            canAbortThread = true;
            //TODO: call close event
        }
    }
}
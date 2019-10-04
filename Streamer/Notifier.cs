using System;
using System.Collections;
using System.Threading;
using TWLib.Streamer.Models;

namespace TWLib.Streamer
{
    internal class Notifier : IDisposable
    {
        private volatile bool _enabled;
        private ManualResetEvent _exited;
        private readonly object _sync;
        System.Collections.Generic.Queue<string> _queue;

        public Action<string> OnMessage = null;

        public Notifier()
        {
            _enabled = true;
            _exited = new ManualResetEvent(false);
            _queue = new System.Collections.Generic.Queue<string>();
            _sync = ((ICollection)_queue).SyncRoot;

            ThreadPool.QueueUserWorkItem(
              state =>
              {
                  while (_enabled || Count > 0)
                  {
                      var msg = dequeue();
                      if (msg != null)
                      {
#if UBUNTU
              var nf = new Notification (msg.Summary, msg.Body, msg.Icon);
              nf.AddHint ("append", "allowed");
              nf.Show ();
#else
                          OnMessage(msg);
#endif
                      }
                      else
                      {
                          Thread.Sleep(500);
                      }
                  }

                  _exited.Set();
              }
            );
        }

        public int Count
        {
            get
            {
                lock (_sync)
                    return _queue.Count;
            }
        }

        public string dequeue()
        {
            lock (_sync)
                return _queue.Count > 0 ? _queue.Dequeue() : null;
        }

        public void Close()
        {
            _enabled = false;
            _exited.WaitOne();
            _exited.Close();
        }

        public void Notify(string message)
        {
            lock (_sync)
            {
                if (_enabled)
                    _queue.Enqueue(message);
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }
    }

}


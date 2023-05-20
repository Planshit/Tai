using Core.Librarys.SQLite;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Servicers.Instances
{
    public class Database : IDatabase
    {
        private TaiDbContext _writerContext;
        private readonly object _writeLocker = new object();
        private readonly object _readLocker = new object();
        private readonly object _closeLocker = new object();
        private int _outTime = 60;
        private int _readerNum = 0;
        private bool _isWriting = false;
        private bool _isReading = false;
        public TaiDbContext GetReaderContext()
        {
            lock (_readLocker)
            {
                _isReading = true;
                int duration = 0;
                while (_isWriting)
                {
                    Thread.Sleep(1000);
                    duration++;
                    if (duration >= _outTime)
                    {
                        break;
                    }
                }
                _writerContext?.Dispose();
                _readerNum++;
                var db = new TaiDbContext();
                db.Database.Connection.StateChange += Connection_StateChange;
                return db;
            }
        }

        private void Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            if (e.CurrentState == System.Data.ConnectionState.Closed)
            {
                _readerNum--;
                _isReading = _readerNum > 0;
            }
        }
       
        public void CloseReader()
        {
        }

        public TaiDbContext GetWriterContext()
        {
            lock (_writeLocker)
            {
                int duration = 0;
                while (_isWriting || _isReading)
                {
                    Thread.Sleep(1000);
                    duration++;
                    if (duration >= _outTime)
                    {
                        break;
                    }
                }
                _isWriting = true;
                _writerContext = new TaiDbContext();
                return _writerContext;
            }
        }

        public void CloseWriter()
        {
            lock (_closeLocker)
            {
                _writerContext?.Dispose();
                _isWriting = false;
            }
        }
    }
}

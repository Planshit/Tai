using Core.Librarys.SQLite;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Servicers.Instances
{
    public class Database : IDatabase
    {
        //private TaiDbContext _readerContext;
        private TaiDbContext _writerContext;
        private readonly object _locker = new object();
        private bool _locked = false;
        private int _outTime = 10;
        private bool _isReading = false;
        public TaiDbContext GetReaderContext()
        {
            lock (_locker)
            {
                int lockTime = 0;
                while (_locked)
                {
                    Thread.Sleep(100);
                    lockTime += 100;
                    if (lockTime >= _outTime * 1000)
                    {
                        _locked = false;
                    }
                }
                //_locked = true;

                //_readerContext = new TaiDbContext();
                //return _readerContext;
                return new TaiDbContext();
            }
        }

        public void CloseReader()
        {
            //_readerContext?.Dispose();
        }

        public TaiDbContext GetWriterContext()
        {
            lock (_locker)
            {
                int lockTime = 0;
                while (_locked)
                {
                    Thread.Sleep(100);
                    lockTime += 100;
                    if (lockTime >= _outTime * 1000)
                    {
                        _locked = false;
                    }
                }
                _locked = true;

                _writerContext = new TaiDbContext();
                return _writerContext;
            }
        }

        public void CloseWriter()
        {
            _writerContext?.Dispose();
            _locked = false;
        }
    }
}

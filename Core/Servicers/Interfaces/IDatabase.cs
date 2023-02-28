using Core.Librarys.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Interfaces
{
    public interface IDatabase
    {
        TaiDbContext GetReaderContext();
        //void CloseReader();
        TaiDbContext GetWriterContext();
        void CloseWriter();
    }
}

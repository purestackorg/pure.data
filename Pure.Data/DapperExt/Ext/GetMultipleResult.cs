using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;

namespace Pure.Data
{
    public interface IMultipleResultReader:IDisposable
    {
        IEnumerable<T> Read<T>();
    }

    public class GridReaderResultReader : IMultipleResultReader
    {
        private readonly SqlMapper.GridReader _reader;

        public GridReaderResultReader(SqlMapper.GridReader reader)
        {
            _reader = reader;
        }

        public IEnumerable<T> Read<T>()
        {
            return _reader.Read<T>();
        }

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Dispose();
            }
        }
    }

    public class SequenceReaderResultReader : IMultipleResultReader
    {
        private readonly Queue<SqlMapper.GridReader> _items;

        public SequenceReaderResultReader(IEnumerable<SqlMapper.GridReader> items)
        {
            _items = new Queue<SqlMapper.GridReader>(items);
        }

        public IEnumerable<T> Read<T>()
        {
            SqlMapper.GridReader reader = _items.Dequeue();
            return reader.Read<T>();
        }
        public void Dispose()
        {
            foreach (var item in _items)
            {
                if (item != null)
                {
                    item.Dispose();
                } 
            }
            
        }
    }
}
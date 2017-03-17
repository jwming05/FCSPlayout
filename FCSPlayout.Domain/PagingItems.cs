using System;
using System.Collections.Generic;

namespace FCSPlayout.Domain
{
    public class PagingItems : PagingInfo
    {
        private readonly int _pageCount;
        public PagingItems(int pageIndex, int rowsPerPage, int rowCount)
            : base(pageIndex, rowsPerPage)
        {
            if (rowCount < 0)
            {
                throw new ArgumentOutOfRangeException("rowCount");
            }
            this.RowCount = rowCount;

            _pageCount = (this.RowCount + this.RowsPerPage - 1) / this.RowsPerPage;
        }

        public PagingItems(PagingInfo pagingInfo, int rowCount) :
            this(pagingInfo.PageIndex, pagingInfo.RowsPerPage, rowCount)
        {
        }

        //public int PageIndex { get; set; }
        public int PageCount
        {
            get
            {
                return _pageCount;
            }
        }

        public IEnumerable<object> Items { get; set; }
        public int RowCount
        { get; private set; }
    }

    public class PagingItems<TItem> : PagingItems
    {
        private readonly int _pageCount;
        public PagingItems(int pageIndex, int rowsPerPage, int rowCount)
            : base(pageIndex, rowsPerPage, rowCount)
        {
        }

        public PagingItems(PagingInfo pagingInfo, int rowCount) :
            base(pagingInfo, rowCount)
        {
        }

        public new IEnumerable<TItem> Items { get; set; }
    }

    
}

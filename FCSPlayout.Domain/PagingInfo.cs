using System;

namespace FCSPlayout.Domain
{
    public class PagingInfo
    {
        private readonly int _rowIndex;

        /// <summary>
        /// 创建一个PagingInfo实例。
        /// </summary>
        /// <param name="pageIndex">基于0的页面索引。</param>
        /// <param name="rowsPerPage">每页的行数。</param>
        public PagingInfo(int pageIndex, int rowsPerPage)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            if (rowsPerPage <= 0)
            {
                throw new ArgumentOutOfRangeException("rowsPerPage");
            }

            this.PageIndex = pageIndex;
            this.RowsPerPage = rowsPerPage;

            _rowIndex = this.PageIndex * RowsPerPage;
        }

        /// <summary>
        /// 获取基于0的页面索引。
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// 获取每页的行数。
        /// </summary>
        public int RowsPerPage { get; private set; }

        /// <summary>
        /// 获取当前页面的基于0的行索引。
        /// </summary>
        public int RowIndex
        {
            get { return _rowIndex; }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    public partial class Matrix
    {
        private double[][] _matrix;
        private MatrixRowCollection _rowCollection;
        private MatrixColumnCollection _columnCollection;

        public Matrix(int rowCount,int columnCount)
        {
            if (rowCount < 1)
            {
                throw new ArgumentOutOfRangeException("rowCount");
            }

            if (columnCount < 1)
            {
                throw new ArgumentOutOfRangeException("columnCount");
            }

            this.RowCount = rowCount;
            this.ColumnCount = columnCount;

            _matrix = new double[rowCount][];
            for(int i = 0; i < _matrix.Length; i++)
            {
                _matrix[i] = new double[columnCount];
            }
        }

        public int ColumnCount { get; private set; }
        public int RowCount { get; private set; }

        public MatrixRowCollection Rows
        {
            get
            {
                if (_rowCollection == null)
                {
                    _rowCollection = new MatrixRowCollection(_matrix);
                }
                return _rowCollection;
            }
        }

        public MatrixColumnCollection Columns
        {
            get
            {
                if (_columnCollection == null)
                {
                    _columnCollection = new MatrixColumnCollection(_matrix);
                }
                return _columnCollection;
            }
        }
    }

    public partial class Matrix
    {
        public class MatrixRowCollection
        {
            private MatrixRow[] _rows;
            internal MatrixRowCollection(double[][] matrix)
            {
                _rows = new MatrixRow[matrix.Length];
                for(int i = 0; i < _rows.Length; i++)
                {
                    _rows[i] = new MatrixRow(matrix[i]);
                }
            }

            public MatrixRow this[int index]
            {
                get { return _rows[index]; }
                //set
                //{
                //    _row[index] = value;
                //}
            }

            public int Count
            {
                get { return _rows.Length; }
            }
        }


        public class MatrixColumnCollection
        {
            private MatrixColumn[] _columns;
            internal MatrixColumnCollection(double[][] matrix)
            {
                this.Count = matrix[0].Length;

                _columns = new MatrixColumn[this.Count];
                for (int i = 0; i < _columns.Length; i++)
                {
                    _columns[i] = new MatrixColumn(matrix,i);
                }
            }

            public MatrixColumn this[int index]
            {
                get { return _columns[index]; }
            }

            public int Count
            {
                get;private set;
            }
        }

        public class MatrixRow : IEnumerable<double>
        {
            private double[] _row;

            internal MatrixRow(double[] row)
            {
                _row = row;
            }

            public double this[int index]
            {
                get { return _row[index]; }
                set
                {
                    _row[index] = value;
                }
            }

            public int Count
            {
                get { return _row.Length; }
            }

            public IEnumerator<double> GetEnumerator()
            {
                foreach(var d in _row)
                {
                    yield return d;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        public class MatrixColumn : IEnumerable<double>
        {
            private int _column;
            private double[][] _matrix;

            internal MatrixColumn(double[][] matrix,int column)
            {
                _matrix = matrix;
                _column = column;
            }

            public double this[int index]
            {
                get { return _matrix[index][_column]; }
                set
                {
                    _matrix[index][_column] = value;
                }
            }

            public int Count
            {
                get { return _matrix.Length; }
            }

            public IEnumerator<double> GetEnumerator()
            {
                for (int i=0;i<this.Count;i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}

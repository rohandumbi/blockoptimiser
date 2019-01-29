using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Services.DataImport
{
    public class CSVReader
    {
        public int DATA_TYPE_GROUP_BY = 1;
        public int DATA_TYPE_ADDITIVE = 2;
        public int DATA_TYPE_GRADE = 3;

        public String[] Headers { get; private set; }
        public int[] DataTypes { get; private set; }
        public int FieldCount { get; private set; }

        private StreamReader _file;
        private int _rowsRead;
        private String[] _lineArr;

        public void SetFile(String FileName)
        {
            _lineArr = null;
            _rowsRead = 0;
            _file = File.OpenText(FileName);
            FetchHeaders();
            FetchDataTypes();
        }

        public void ImportData()
        {
            if(_rowsRead == 1 && _lineArr != null) { 
}
        }

        private void FetchHeaders()
        {
            if (_rowsRead == 0)
            {
                ReadLine();
                Headers = _lineArr;
                FieldCount = _lineArr.Length;
            }

        }
        private void FetchDataTypes()
        {
            ReadLine();
            DataTypes = new int[_lineArr.Length];
            for (int i = 0; i < _lineArr.Length; i++)
            {
                try
                {
                    int val = Int32.Parse(_lineArr[i]);
                    if(val < 100)
                    {
                        DataTypes[i] = DATA_TYPE_GRADE;
                    } else
                    {
                        DataTypes[i] = DATA_TYPE_ADDITIVE;
                    }
                } catch (Exception e)
                {
                    DataTypes[i] = DATA_TYPE_GROUP_BY;
                }
                
            }
        }
        public bool ReadLine()
        {
            var result = !_file.EndOfStream;
            if (result == true)
            {
                _lineArr = _file.ReadLine().Split(',');
                _rowsRead++;
            }
            return result;
        }
    }
}

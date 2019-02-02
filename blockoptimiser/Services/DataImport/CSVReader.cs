using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockoptimiser.Services.DataImport
{
    public class CSVReader : IDataReader, IDisposable
    {
        public int DATA_TYPE_GROUP_BY = 1;
        public int DATA_TYPE_ADDITIVE = 2;
        public int DATA_TYPE_GRADE = 3;

        private StreamReader _file;
        private char _delimiter;
        private string  _csvHeaderstring = "", _csvlinestring = "";
        private string[] _header;
        public int[] _dataTypes;
        private int recordsaffected;
        private bool _iscolumnlocked = false;
        private System.Collections.Specialized.OrderedDictionary headercollection =
            new System.Collections.Specialized.OrderedDictionary();
        private string[] _line;

        public string[] Header
        {
            get { return _header; }
        }
        public string[] Line
        {
            get { return _line; }
        }
        public int[] DataTypes
        {
            get { return _dataTypes; }
        }

        public CSVReader(string filePath, bool fetchDataTypes = false, char delimiter = ',')
        {
            _file = File.OpenText(filePath);
            _delimiter = delimiter;
            Read();
            _csvHeaderstring = _csvlinestring;
            _header = ReadRow(_csvHeaderstring);
            int count = 1;
            foreach (var item in _header) 
            {
                if (headercollection.Contains(item) == true)
                    throw new Exception("Duplicate found in CSV header. Cannot import file with duplicate header");
                headercollection.Add($"COL_{count}", null);
                count++;
            }
            
            if(fetchDataTypes)
            {
                FetchDataTypes();
                Close(); //close and repoen to get the record position to beginning.
                _file = File.OpenText(filePath);
                Read(); // Making one read so that header row is read
            }
            _iscolumnlocked = false; //setting this to false since above read is called 
                                     //internally during constructor and actual user read() didnot start.
            _csvlinestring = "";
            _line = null;
            recordsaffected = 0;
        }

        private void FetchDataTypes()
        {
            Read();
            _dataTypes = new int[_line.Length];

            for (int i = 0; i < _line.Length; i++)
            {
                try
                {
                    int val = Int32.Parse(_line[i]);
                    if (val < 100)
                    {
                        DataTypes[i] = DATA_TYPE_GRADE;
                    }
                    else
                    {
                        DataTypes[i] = DATA_TYPE_ADDITIVE;
                    }
                }
                catch (Exception e)
                {
                    DataTypes[i] = DATA_TYPE_GROUP_BY;
                }
            }
        }

        public bool Read()
        {
            var result = !_file.EndOfStream;
            if (result == true)
            {
                _csvlinestring = _file.ReadLine();
                _line = ReadRow(_csvlinestring);
                recordsaffected++;
            }
            if (_iscolumnlocked == false)
                _iscolumnlocked = true;
            return result;
        }

        private string[] ReadRow(string line)
        {
            List<string> lines = new List<string>();
            if (String.IsNullOrEmpty(line) == true)
                return null;

            int pos = 0;
            int rows = 0;
            while (pos < line.Length)
            {
                string value;

                // Special handling for quoted field
                if (line[pos] == '"')
                {
                    // Skip initial quote
                    pos++;

                    // Parse quoted value
                    int start = pos;
                    while (pos < line.Length)
                    {
                        // Test for quote character
                        if (line[pos] == '"')
                        {
                            // Found one
                            pos++;

                            // If two quotes together, keep one
                            // Otherwise, indicates end of value
                            if (pos >= line.Length || line[pos] != '"')
                            {
                                pos--;
                                break;
                            }
                        }
                        pos++;
                    }
                    value = line.Substring(start, pos - start);
                    value = value.Replace("\"\"", "\"");
                }
                else
                {
                    // Parse unquoted value
                    int start = pos;
                    while (pos < line.Length && line[pos] != _delimiter)
                        pos++;
                    value = line.Substring(start, pos - start);
                }
                // Add field to list
                if (rows < lines.Count)
                    lines[rows] = value;
                else
                    lines.Add(value);
                rows++;

                // Eat up to and including next comma
                while (pos < line.Length && line[pos] != _delimiter)
                    pos++;
                if (pos < line.Length)
                    pos++;
            }
            return lines.ToArray();
        }

        public void Close()
        {
            _file.Close();
            _file.Dispose();
            _file = null;
        }

        /// <summary>
        /// Gets a value that indicates the depth of nesting for the current row.
        /// </summary>
        public int Depth
        {
            get { return 1; }
        }

        public DataTable GetSchemaTable()
        {
            DataTable t = new DataTable();
            t.Rows.Add(Header);
            return t;
        }

        public bool IsClosed
        {
            get { return _file == null; }
        }

        public bool NextResult()
        {
            return Read();
        }

        /// <summary>
        /// Returns how many records read so far.
        /// </summary>
        public int RecordsAffected
        {
            get { return recordsaffected; }
        }

        public void Dispose()
        {
            if (_file != null)
            {
                _file.Dispose();
                _file = null;
            }
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public int FieldCount
        {
            get { return Header.Length; }
        }

        public bool GetBoolean(int i)
        {
            return Boolean.Parse(Line[i]);
        }

        public byte GetByte(int i)
        {
            return Byte.Parse(Line[i]);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return Char.Parse(Line[i]);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            return (IDataReader)this;
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            return DateTime.Parse(Line[i]);
        }

        public decimal GetDecimal(int i)
        {
            return Decimal.Parse(Line[i]);
        }

        public double GetDouble(int i)
        {
            return Double.Parse(Line[i]);
        }

        public Type GetFieldType(int i)
        {
            return typeof(String);
        }

        public float GetFloat(int i)
        {
            return float.Parse(Line[i]);
        }

        public Guid GetGuid(int i)
        {
            return Guid.Parse(Line[i]);
        }

        public short GetInt16(int i)
        {
            return Int16.Parse(Line[i]);
        }

        public int GetInt32(int i)
        {
            return Int32.Parse(Line[i]);
        }

        public long GetInt64(int i)
        {
            return Int64.Parse(Line[i]);
        }

        public string GetName(int i)
        {
            return Header[i];
        }

        public int GetOrdinal(string name)
        {
            int result = -1;
            for (int i = 0; i < Header.Length; i++)
                if (Header[i] == name)
                {
                    result = i;
                    break;
                }
            return result;
        }

        public string GetString(int i)
        {
            return Line[i];
        }

        public object GetValue(int i)
        {
            return Line[i];
        }

        public int GetValues(object[] values)
        {
            values = Line;
            return 1;
        }

        public bool IsDBNull(int i)
        {
            return string.IsNullOrWhiteSpace(Line[i]);
        }

        public object this[string name]
        {
            get { return Line[GetOrdinal(name)]; }
        }

        public object this[int i]
        {
            get { return GetValue(i); }
        }
    }
}

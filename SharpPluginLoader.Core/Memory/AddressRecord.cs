using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Memory
{
    internal class AddressRecord
    {
        public AddressRecord(Pattern pattern, int offset = 0)
        {
            _pattern = pattern;
            _offset = offset;
            _address = 0;
        }

        public AddressRecord(string pattern, int offset = 0)
        {
            _pattern = Pattern.FromString(pattern);
            _offset = offset;
            _address = 0;
        }

        public nint Address
        {
            get
            {
                if (_address == 0)
                {
                    var addr = PatternScanner.Scan(_pattern).FirstOrDefault();
                    if (addr == 0) throw new Exception("Failed to find address");
                    _address = addr + _offset;
                }

                return _address;
            }
        }

        private readonly Pattern _pattern;
        private readonly int _offset;
        private nint _address;
    }
}

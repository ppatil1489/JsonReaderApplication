using JsonMonitorService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonReaderApplication
{
    public class BookEqualityComparer : IEqualityComparer<Book>
    {
        public bool Equals(Book b1, Book b2)
        {
            if (b2 == null && b1 == null)
                return true;
            else if (b1 == null || b2 == null)
                return false;
            else if (b1.Description == b2.Description && b1.Name == b2.Name
                                && b1.Rack == b2.Rack)
                return true;
            else
                return false;
        }

        public int GetHashCode(Book bx)
        {
            return (bx.Description + bx.Name + bx.Rack).GetHashCode();
        }
    }
}

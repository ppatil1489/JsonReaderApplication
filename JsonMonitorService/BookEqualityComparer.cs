using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonMonitorService
{
    /// <summary>
    /// This class used to compare object properties using IEqualityComparer.
    /// </summary>
    public class BookEqualityComparer : IEqualityComparer<Book>
    {
        public bool Equals(Book? book1, Book? book2)
        {
            if (book2 == null && book1 == null)
                return true;
            else if (book1 == null || book2 == null)
                return false;
            else if (book1.Description == book2.Description && book1.Name == book2.Name
                                && book1.Rack == book2.Rack)
                return true;
            else
                return false;
        }

        public int GetHashCode(Book book)
        {
            return (book.Description + book.Name + book.Rack).GetHashCode();
        }
    }
}

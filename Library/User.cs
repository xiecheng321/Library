using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    //=========== User 类 ==========
    class User
    {
        public string username { get; set; }
        public int age { get; set; }
        public string email { get; set; }
        public List<Guid> borrowedBookIds { get; set; } = new List<Guid>();

        public User() { } // 默认构造函数

        public User(string username, int age, string email)
        {
            this.username = username;
            this.age = age;
            this.email = email;
        }
        public void PrintBorrowedBooks(List<Book> library)
        {
            if (borrowedBookIds.Count == 0)
            {
                Console.WriteLine("尚未借阅任何书籍");
                return;
            }

            Console.WriteLine($"{username}当前借阅书籍列表：");
            foreach (var id in borrowedBookIds)
            {
                var book = library.FirstOrDefault(b => b.Id == id);
                if (book != null)
                    Console.WriteLine($"-{book.bookname}");
            }
        }
    }
}

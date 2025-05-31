using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    //========== Book 类 ==========
    class Book
    {
        public string bookname { get; set; }
        public string author { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid(); // 唯一标识符
        public bool isAvailable { get; set; } // 是否可借

        public Book() { } // 默认构造函数
        public Book(string bookname, string author)
        {
            this.bookname = bookname;
            this.author = author;
            this.Id = Guid.NewGuid();
            this.isAvailable = true; //默认上架
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    //========== LibraryApp 类 ==========
    public class LibraryApp
    {
        private Dictionary<string, User> users;
        private List<Book> library;
        private User currentUser;
        private LibraryService service;

        public LibraryApp()
        {
            service = new LibraryService();
            users = service.LoadUsers("users.json");
            library = service.LoadBooks("books.json");
            if (library.Count == 0)      //如果字典中没有图书数据，添加默认书籍
            {
                library = new List<Book>
                {
                    new Book("c#编程入门", "张三"),
                    new Book("面向对象编程", "李四"),
                    new Book("数据结构与算法", "王五"),
                    new Book("计算机网络", "赵六"),
                };
                service.SaveBooks(library, "books.json");
            }
            currentUser = null;    //当前用户默认为空
        }

        public void Run()
        {
            while (true)
            {
                ShowMenu();
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": RegisterOrLogin(); break;
                    case "2": BorrowBook(); break;
                    case "3": ReturnBook(); break;
                    case "4": AddBook(); break;
                    case "5": DeleteBook(); break;
                    case "6": ExitApp(); break;
                    default: Console.WriteLine("无效选择，请重试。"); break;
                }
                service.SaveUsers(users, "users.json"); // 操作完成后，自动保存数据
                service.SaveBooks(library, "books.json");
            }
        }

        private void ShowMenu()
        {
            Console.WriteLine("\n====图书借阅系统====");
            if (currentUser == null)
                Console.WriteLine("当前未登录");
            else
                Console.WriteLine($"当前用户：{currentUser.username}");
            Console.WriteLine("1. 登录/注册");
            Console.WriteLine("2. 借书");
            Console.WriteLine("3. 还书");
            Console.WriteLine("4. 添加书籍");
            Console.WriteLine("5. 删除书籍");
            Console.WriteLine("6. 退出");
            Console.Write("请输入操作序号：");
        }

        private void RegisterOrLogin()
        {
            if (currentUser != null)
            {
                Console.WriteLine("你已登录，如需切换用户请先注销。");
                return;
            }
            currentUser = service.LoginOrRegisterUser(users);
        }


        private void BorrowBook()
        {
            if (currentUser == null)
            {
                Console.WriteLine("请先登录或注册。");
                return;
            }
            var availableBooks = new List<Book>();
            for (int i = 0; i < library.Count; i++)
            {
                if (library[i].isAvailable)
                {
                    availableBooks.Add(library[i]);
                    // 编号从1开始
                    Console.WriteLine($"{availableBooks.Count},{library[i].bookname}");
                }
            }
            if (availableBooks.Count == 0)
            {
                Console.WriteLine("暂无可借图书。");
                return;
            }
            Console.Write("请输入要借阅的书籍编号：");
            int bookIndex;
            if (int.TryParse(Console.ReadLine(), out bookIndex) && bookIndex >= 1 && bookIndex < availableBooks.Count)
            {
                service.BorrowBook(currentUser, availableBooks[bookIndex - 1]);
            }
            else
                Console.WriteLine("无效编号。");
        }

        private void ReturnBook()
        {
            if (currentUser == null)
            {
                Console.WriteLine("请先登录或注册。");
                return;
            }
            service.ReturnBook(currentUser, library);
        }

        private void AddBook()
        {
            service.AddBook(library);
        }

        private void DeleteBook()
        {
            service.DeleteBook(library);
        }

        private void ExitApp()
        {
            //退出前最后保存一次
            service.SaveUsers(users, "users.json");
            service.SaveBooks(library, "books.json");
            Console.WriteLine("感谢使用，再见！");
            Environment.Exit(0);
        }


    }
}

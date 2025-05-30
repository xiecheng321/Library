using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Library
{
    class Program
    {
        static void Main(string[] args)
        {
            new LibraryApp().Run();
        }
    }

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
                service.SaveBooks(library,"books.json");
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
                    case "2":BorrowBook(); break;
                    case "3":ReturnBook(); break;
                    case "4":AddBook(); break;
                    case "5":DeleteBook(); break;
                    case "6":ExitApp(); break;
                    default: Console.WriteLine("无效选择，请重试。"); break;
                }
                service.SaveUsers(users,"users.json"); // 操作完成后，自动保存数据
                service.SaveBooks(library,"books.json");
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
                    Console.WriteLine($"{availableBooks.Count - 1},{library[i].bookname}");
                }
            }
            if (availableBooks.Count == 0) 
            {
                Console.WriteLine("暂无可借图书。");
                return;
            }
            Console.Write("请输入要借阅的书籍编号：");
            int bookIndex;
            if (int.TryParse(Console.ReadLine(), out bookIndex) && bookIndex >= 0 && bookIndex < availableBooks.Count)
            {
                service.BorrowBook(currentUser, availableBooks[bookIndex]);
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

    //=========== LibraryService 类 ==========
    class LibraryService
    {
        public User LoginOrRegisterUser(Dictionary<string, User> users)
        {
            Console.WriteLine("请输入用户名：");
            string username = Console.ReadLine();
            if (users.ContainsKey(username))
            {
                Console.WriteLine($"欢迎回来，{username}！");
                return users[username];
            }
            else
            {
                int age;
                while (true)
                {
                    Console.WriteLine("新用户注册，请输入年龄：");
                    if (int.TryParse(Console.ReadLine(), out age)) break;
                    Console.WriteLine("年龄输入无效，请重新输入。");
                }
                Console.WriteLine("请输入邮箱：");
                string email = Console.ReadLine();
                var user = new User(username, age, email);
                users[username] = user;
                Console.WriteLine($"用户{username}注册并登录成功！");
                return user;
            }
        }


        public void BorrowBook(User user, Book book)     //借书方法
        {
            if (book.isAvailable)
            {
                book.isAvailable = false;
                user.borrowedBookIds.Add(book.Id);
                Console.WriteLine($"{user.username}成功借阅《{book.bookname}》");
            }
            else
            {
                Console.WriteLine("这本书已被借出。");
            }
        }

        public void ReturnBook(User user, List<Book> library)       //还书方法
        {
            if (user.borrowedBookIds.Count == 0)
            {
                Console.WriteLine("你当前没有借书。");
                return;
            }
            Console.WriteLine("你当前借了这些书：");
            for (int i = 0; i < user.borrowedBookIds.Count; i++)
            {
                var book = library.FirstOrDefault(b => b.Id == user.borrowedBookIds[i]);
                if (book != null)
                    Console.WriteLine($"{i}.{book.bookname}");
            }
            Console.WriteLine("请输入你要归还的书籍的编号:");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < user.borrowedBookIds.Count)
            {
                Guid toReturnId = user.borrowedBookIds[index];
                user.borrowedBookIds.RemoveAt(index);
                var book = library.FirstOrDefault(b => b.Id == toReturnId);
                if (book != null)
                {
                    book.isAvailable = true;
                    Console.WriteLine($"你已归还《{book.bookname}》");
                }
            }
            else
            {
                Console.WriteLine("输入了无效编号。");
            }
        }

        public void SaveUsers(Dictionary<string, User> users, string path)     // 保存用户数据到文件
        {
            var json = JsonSerializer.Serialize(users);
            File.WriteAllText(path, json);
        }

        public Dictionary<string, User> LoadUsers(string path)              // 从文件加载用户数据
        {
            if (!File.Exists(path)) return new Dictionary<string, User>();
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Dictionary<string, User>>(json) ?? new Dictionary<string, User>();
        }

        public void SaveBooks(List<Book> books, string path)       // 保存书籍数据到文件
        {
            var json = JsonSerializer.Serialize(books);
            File.WriteAllText(path, json);
        }

        public List<Book> LoadBooks(string path)              // 从文件加载书籍数据
        {
            if (!File.Exists(path)) return new List<Book>();
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }

        public void AddBook(List<Book> library)           //添加书籍方法
        {
            string name, author;
            do
            {
                Console.WriteLine("请输入书名：");
                name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                    Console.WriteLine("书名不能为空，请重新输入。");
            } while (string.IsNullOrWhiteSpace(name));

            do
            {
                Console.WriteLine("请输入作者：");
                author = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(author))
                    Console.WriteLine("作者不能为空，请重新输入。");
            } while (string.IsNullOrWhiteSpace(author));

            library.Add(new Book(name, author));
            Console.WriteLine($"书籍《{name}》添加成功！");
        }


        public void DeleteBook(List<Book> library)
        {
            Console.WriteLine("当前图书列表：");
            for (int i = 0; i < library.Count; i++)
            {
                Console.WriteLine($"{i}, {library[i].bookname}（{library[i].author}）");
            }
            Console.WriteLine("请输入要删除的书籍编号：");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < library.Count)
            {
                Console.WriteLine($"已删除《{library[index].bookname}》");
                library.RemoveAt(index);
            }

            else
            {
                Console.WriteLine("无效编号，删除失败。");
            }
        }
    }
}


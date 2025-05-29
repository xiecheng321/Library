using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Library
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LibraryService service = new LibraryService();

            Dictionary<string, User> users = service.LoadUsers("users.json");
            List<Book> library = service.LoadBooks("books.json");
            if (library.Count == 0) // 如果没有书籍数据，则添加一些默认书籍
            {
                library = new List<Book>
                {
                    new Book("c#编程入门", "张三"),
                    new Book("面向对象编程", "李四"),
                    new Book("数据结构与算法", "王五"),
                    new Book("计算机网络", "赵六"),
                };
            }
            service.SaveBooks(library, "books.json"); // 立即保存

            User currentUser = null;

            while (true)
            {
                Console.WriteLine("\n欢迎来到图书馆借阅系统,请选择操作：");
                if (currentUser == null)
                    Console.WriteLine("1. 登录/注册");
                else
                    Console.WriteLine($"当前用户：{currentUser.username}");
                Console.WriteLine("2. 借书");
                Console.WriteLine("3. 还书");
                Console.WriteLine("4. 添加书籍");
                Console.WriteLine("5. 删除书籍");
                Console.WriteLine("6. 退出");
                Console.Write("请输入操作编号：");
                string choice = Console.ReadLine();

                if (choice == "1" && currentUser == null)
                {
                    Console.WriteLine("请输入用户名：");
                    string username = Console.ReadLine();
                    if (users.ContainsKey(username))
                    {
                        currentUser = users[username];
                        Console.WriteLine($"欢迎回来，{username}！");
                    }
                    else
                    {
                        Console.WriteLine("新用户注册，请输入年龄：");
                        int age = int.Parse(Console.ReadLine());
                        Console.WriteLine("请输入邮箱：");
                        string email = Console.ReadLine();
                        currentUser = new User(username, age, email);
                        users[username] = currentUser;
                        Console.WriteLine($"用户{username}注册并登录成功！");
                    }
                    service.SaveUsers(users, "users.json");  //立即保存

                }

                else if (choice == "2")
                {
                    if (currentUser == null)
                    {
                        Console.WriteLine("请先登录或注册。");
                        continue;
                    }
                    // 只显示可借图书，并用新编号
                    var availableBooks = new List<Book>();
                    for (int i = 0; i < library.Count; i++)
                    {
                        if (library[i].isAvailable)
                        {
                            availableBooks.Add(library[i]);
                            Console.WriteLine($"{availableBooks.Count - 1}.{library[i].bookname}");
                        }
                    }
                    if (availableBooks.Count == 0)
                    {
                        Console.WriteLine("暂无可借图书。");
                        continue;
                    }
                    Console.Write("请输入要借阅的书籍编号：");
                    int bookIndex;
                    if (int.TryParse(Console.ReadLine(), out bookIndex) && bookIndex >= 0 && bookIndex < availableBooks.Count)
                        service.BorrowBook(currentUser, availableBooks[bookIndex]);
                    else
                        Console.WriteLine("无效编号。");
                    // 借书后立即保存
                    service.SaveBooks(library, "books.json");
                    service.SaveUsers(users, "users.json");
                }

                else if (choice == "3")
                {
                    if (currentUser == null)
                    {
                        Console.WriteLine("请先登录或注册。");
                        continue;
                    }
                    service.ReturnBook(currentUser,library);
                    // 还书后立即保存
                    service.SaveBooks(library, "books.json");
                    service.SaveUsers(users, "users.json");
                }
                else if (choice == "4")
                {
                    service.AddBook(library);
                    // 添加书后立即保存
                    service.SaveBooks(library, "books.json");
                }
                else if (choice == "5")
                {
                    service.DeleteBook(library);
                    // 删除书后立即保存
                    service.SaveBooks(library, "books.json");
                }
                else if (choice == "6")
                {
                    Console.WriteLine("感谢使用，再见！");
                    break;
                }
                else
                {
                    Console.WriteLine("无效输入，请重试。");
                }
            }
            //程序退出前保存
            service.SaveBooks(library, "books.json");
            service.SaveUsers(users, "users.json");
        }
    }
}
//========== Book 类 ==========
class Book 
{
    public string bookname {  get; set; }
    public string author { get; set; }
    public bool isAvailable { get; set; } // 是否可借

    public Book() { } // 默认构造函数
    public Book(string bookname, string author) 
    {
        this.bookname = bookname;
        this.author = author;
        this.isAvailable = true; //默认上架
    }
}
//=========== User 类 ==========
class User
{
    public string username { get; set; }
    public int age { get; set; }
    public string email { get; set; }
    public List<Book> borrowedBooks { get; set; } = new List<Book>();

    public User() { } // 默认构造函数

    public User(string username, int age, string email)
    {
        this.username = username;
        this.age = age;
        this.email = email;
    }
    public void PrintBorrowedBooks()
    {
        if (borrowedBooks.Count == 0)
        {
            Console.WriteLine("尚未借阅任何书籍");
            return;
        }

        Console.WriteLine($"{username}当前借阅书籍列表：");
        foreach (var book in borrowedBooks)
        {
            Console.WriteLine($"-{book.bookname}");
        }
    }
}

//=========== LibraryService 类 ==========
class LibraryService
{
    public void RegisterUser(Dictionary<string, User> users)    //用户注册方法
    {
        Console.WriteLine("请注册用户名：");
        string username = Console.ReadLine();
        Console.WriteLine("请输入年龄：");
        int age = int.Parse(Console.ReadLine());
        Console.WriteLine("请输入邮箱：");
        string email = Console.ReadLine();

        if (!users.ContainsKey(username))
        {
            users[username] = new User(username, age, email);
            Console.WriteLine($"用户{username}注册成功！");
        }
        else
        {
            Console.WriteLine("用户名已存在！");
        }
    }

    public void BorrowBook(User user, Book book)     //借书方法
    {
        if (book.isAvailable)
        {
            book.isAvailable = false;
            user.borrowedBooks.Add(book);
            Console.WriteLine($"{user.username}成功借阅《{book.bookname}》");
        }
        else
        {
            Console.WriteLine("这本书已被借出。");
        }
    }

    public void ReturnBook(User user, List<Book> library)       //还书方法
    {
        if (user.borrowedBooks.Count == 0)
        {
            Console.WriteLine("你当前没有借书。");
            return;
        }
        Console.WriteLine("你当前借了这些书：");
        for (int i = 0; i < user.borrowedBooks.Count; i++)
        {
            Console.WriteLine($"{i}.{user.borrowedBooks[i].bookname}");
        }
        Console.WriteLine("请输入你要归还的书籍的编号:");
        int index = int.Parse(Console.ReadLine());

        if (index >= 0 && index < user.borrowedBooks.Count)
        {
            Book toReturn = user.borrowedBooks[index];
            user.borrowedBooks.RemoveAt(index);
            //toReturn.isAvailable = true;
            // 关键：在library中查找同名同作者的书，并设置isAvailable=true
            foreach (var book in library)
            {
                if (book.bookname == toReturn.bookname && book.author == toReturn.author)
                {
                    book.isAvailable = true;
                    break;
                }
            }
            Console.WriteLine($"你已归还《{toReturn.bookname}》");
        }
        else
        {
            Console.WriteLine("输入了无效编号。");
        }
    }

    public void SaveUsers(Dictionary<string, User> users, string path)     // 保存用户数据到文件
    {
        var json = JsonSerializer.Serialize(users);
        File.WriteAllText(path,json);
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


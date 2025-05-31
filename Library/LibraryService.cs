using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Library
{
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
                    // 编号从1开始
                    Console.WriteLine($"{i +1}.{book.bookname}");
            }
            Console.WriteLine("请输入你要归还的书籍的编号:");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index < user.borrowedBookIds.Count)
            {
                Guid toReturnId = user.borrowedBookIds[index - 1];
                user.borrowedBookIds.RemoveAt(index - 1);
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

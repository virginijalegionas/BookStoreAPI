using System.Net;
using System.Net.Http.Json;

namespace BookStoreAPI;

[TestClass]
public class ApiTests : TestBase
{
    [TestMethod]
    public async Task GetBooks_Success()
    {
        Books actualBooks = await anonymousClient.GetFromJsonAsync<Books>("BookStore/v1/Books");
        //will need this list in the other tests        
        Assert.AreEqual(8, actualBooks.books.Length);
        //Validate first book in the list
        Book expectedBook1 = new Book
        {
            isbn = "9781449325862",
            title = "Git Pocket Guide",
            subTitle = "A Working Introduction",
            author = "Richard E. Silverman",
            publish_date = new DateTime(2020, 06, 04, 08, 48, 39),
            publisher = "O'Reilly Media",
            pages = 234,
            description = "This pocket guide is the perfect on-the-job companion to Git",
            website = "http://chimera.labs.oreilly.com/books/1230000000561/index.html"
        };
        Book actualBook1 = actualBooks.books.SingleOrDefault(x => x.title == expectedBook1.title);
        BooksAreEaqual(expectedBook1, actualBook1);
        //Validate second book in the list
        Book expectedBook2 = new Book
        {
            isbn = "9781449331818",
            title = "Learning JavaScript Design Patterns",
            subTitle = "A JavaScript and jQuery Developer's Guide",
            author = "Addy Osmani",
            publish_date = new DateTime(2020, 06, 04, 09, 11, 40),
            publisher = "O'Reilly Media",
            pages = 254,
            description = "With Learning JavaScript Design Patterns, you'll learn how",
            website = "http://www.addyosmani.com/resources/essentialjsdesignpatterns/book/"
        };
        Book actualBook2 = actualBooks.books.SingleOrDefault(x => x.title == expectedBook2.title);
        BooksAreEaqual(expectedBook2, actualBook2);
        //Validate third book in the list
        Book expectedBook3 = new Book
        {
            isbn = "9781449337711",
            title = "Designing Evolvable Web APIs with ASP.NET",
            subTitle = "Harnessing the Power of the Web",
            author = "Glenn Block et al.",
            publish_date = new DateTime(2020, 06, 04, 09, 12, 43),
            publisher = "O'Reilly Media",
            pages = 238,
            description = "Design and build Web APIs for a broad range of clients—including",
            website = "http://chimera.labs.oreilly.com/books/1234000001708/index.html"
        };
        Book actualBook3 = actualBooks.books.SingleOrDefault(x => x.title == expectedBook3.title);
        BooksAreEaqual(expectedBook3, actualBook3);
    }

    [TestMethod]
    public async Task AddDeleteListOfBooksToUser_Success()
    {
        //Will be adding some Books for User List and deleting them
        AddListOfBooks listOfBooks = new AddListOfBooks
        {
            userId = userId,
            collectionOfIsbns = [
                new Isbn{ isbn = "9781491950296"},
                new Isbn{ isbn = "9781593275846"},
            ]
        };
        //First need to Check if there are any books in user list
        UserBooks userBooks = await GetUserBooks(userId);
        if (userBooks.books.Length > 0)
        {
            //delete list of books if list is not empty
            DeleteAllBooksFromUserList(userId);
        }
        //adding books
        AddedListOfBooks addedListOfBooks = await AddBooksToUserList(listOfBooks);
        Assert.AreEqual(2, addedListOfBooks.books.Length);
        Assert.AreEqual("9781491950296", addedListOfBooks.books[0].isbn);
        Assert.AreEqual("9781593275846", addedListOfBooks.books[1].isbn);
        //deleting books
        DeleteAllBooksFromUserList(userId);
    }

    [TestMethod]
    public async Task AddListOfBooksToUser_NonExistingIsbn_400()
    {
        AddListOfBooks listOfBooks = new AddListOfBooks
        {
            userId = userId,
            collectionOfIsbns = [
                new Isbn{ isbn = "nonExisting"},
            ]
        };
        using HttpResponseMessage response = await authorizedClient.PostAsJsonAsync("BookStore/v1/Books", listOfBooks);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task AddBookToUser_BookIsAlreadyAdded_400()
    {
        //First need to Check if there are any books in user list        
        UserBooks userBooks = await GetUserBooks(userId);
        string isbnToAdd;
        if (userBooks.books.Length > 0)
        {
            isbnToAdd = userBooks.books[0].isbn;
        }
        else
        {
            //adding new book in the list
            AddListOfBooks listOfBooks = new AddListOfBooks
            {
                userId = userId,
                collectionOfIsbns = [
                    new Isbn{ isbn = "9781593275846"},
                ]
            };
            AddedListOfBooks addedListOfBooks = await AddBooksToUserList(listOfBooks);
            Assert.AreEqual(1, addedListOfBooks.books.Length);
            Assert.AreEqual("9781593275846", addedListOfBooks.books[0].isbn);
            isbnToAdd = listOfBooks.collectionOfIsbns[0].isbn;
        }
        //adding the same book for the second time
        AddListOfBooks addingSameBooks = new AddListOfBooks
        {
            userId = userId,
            collectionOfIsbns = [
                new Isbn{ isbn = isbnToAdd},
            ]
        };
        using HttpResponseMessage response = await authorizedClient.PostAsJsonAsync("BookStore/v1/Books", addingSameBooks);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        //test cleanup
        DeleteAllBooksFromUserList(userId);
    }

    [TestMethod]
    public async Task AddListOfBooksToUser_InvalidAuthorizationHeader_401()
    {
        AddListOfBooks listOfBooks = new AddListOfBooks
        {
            userId = userId,
            collectionOfIsbns = [
                new Isbn{ isbn = "9781593275846"},
            ]
        };
        using HttpResponseMessage response = await unauthorizedClient.PostAsJsonAsync("BookStore/v1/Books", listOfBooks);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task AddListOfBooksToUser_InvalidUserId_401()
    {
        AddListOfBooks listOfBooks = new AddListOfBooks
        {
            userId = "invalidUserId",
            collectionOfIsbns = [
                new Isbn{ isbn = "9781593275846"},
            ]
        };
        using HttpResponseMessage response = await authorizedClient.PostAsJsonAsync("BookStore/v1/Books", listOfBooks);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task GetBookByIsbn_Success()
    {
        //Get first book and validate
        Book expectedBook1 = new Book
        {
            isbn = "9781491950296",
            title = "Programming JavaScript Applications",
            subTitle = "Robust Web Architecture with Node, HTML5, and Modern JS Libraries",
            author = "Eric Elliott",
            publish_date = new DateTime(2014, 07, 01, 00, 00, 00),
            publisher = "O'Reilly Media",
            pages = 254,
            description = "Take advantage of JavaScript's power to build robust web-scale",
            website = "http://chimera.labs.oreilly.com/books/1234000000262/index.html"
        };
        Book actualBook1 = await anonymousClient.GetFromJsonAsync<Book>($"BookStore/v1/Book?ISBN={expectedBook1.isbn}");
        BooksAreEaqual(expectedBook1, actualBook1);

        //Get second book and validate
        Book expectedBook2 = new Book
        {
            isbn = "9781449365035",
            title = "Speaking JavaScript",
            subTitle = "An In-Depth Guide for Programmers",
            author = "Axel Rauschmayer",
            publish_date = new DateTime(2014, 02, 01, 00, 00, 00),
            publisher = "O'Reilly Media",
            pages = 460,
            description = "Like it or not, JavaScript is everywhere these days-from browser",
            website = "http://speakingjs.com/"
        };
        Book actualBook2 = await anonymousClient.GetFromJsonAsync<Book>($"BookStore/v1/Book?ISBN={expectedBook2.isbn}");
        BooksAreEaqual(expectedBook2, actualBook2);
    }

    [TestMethod]
    public async Task GetBookByIsbn_IsbnNotFound_400()
    {
        //try to get a book by non existing ISBN        
        using HttpResponseMessage response = await anonymousClient.GetAsync("BookStore/v1/Book?ISBN=NonExistingISBN");
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task DeleteAllBooksFromUserList_InvalidAuthorizationHeader_401()
    {
        //trying to delete books
        using HttpResponseMessage response = await unauthorizedClient.DeleteAsync($"BookStore/v1/Books?UserId={userId}");
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task DeleteAllBooksFromUserList_InvalidUserId_401()
    {
        //trying to delete books
        using HttpResponseMessage response = await authorizedClient.DeleteAsync("BookStore/v1/Books?UserId=invalidId");
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task DeleteOneBookFromUserList_Success()
    {
        //First need to Check if there are any books in user list        
        UserBooks userBooks = await GetUserBooks(userId);
        string isbnToDelete;
        if (userBooks.books.Length > 0)
        {
            isbnToDelete = userBooks.books[0].isbn;
        }
        else
        {
            AddListOfBooks listOfBooks = new AddListOfBooks
            {
                userId = userId,
                collectionOfIsbns = [
                    new Isbn{ isbn = "9781491950296"},
                    new Isbn{ isbn = "9781593275846"},
                ]
            };
            //add some books
            AddedListOfBooks addedListOfBooks = await AddBooksToUserList(listOfBooks);
            Assert.AreEqual(2, addedListOfBooks.books.Length);
            isbnToDelete = listOfBooks.collectionOfIsbns[0].isbn;
        }
        BookToDelete bookToDelete = new BookToDelete
        {
            userId = userId,
            isbn = isbnToDelete,
        };
        //trying to delete one book
        //need to have sprecific request to delete one book
        using HttpRequestMessage request = new HttpRequestMessage();
        request.RequestUri = new Uri(pageUrl, "BookStore/v1/Book");
        request.Method = HttpMethod.Delete;
        request.Content = JsonContent.Create(bookToDelete);
        using HttpResponseMessage response = await authorizedClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        //test cleanup
        DeleteAllBooksFromUserList(userId);
    }

    [TestMethod]
    public async Task ReplaceBookInUserList_Success()
    {
        //first check if user have books in his list
        UserBooks userBooks = await GetUserBooks(userId);
        if (userBooks.books.Length > 0)
        {
            //delete all books from user
            DeleteAllBooksFromUserList(userId);
        }
        //add some books        
        AddListOfBooks listOfBooks = new AddListOfBooks
        {
            userId = userId,
            collectionOfIsbns = [
                new Isbn{ isbn = "9781491950296"},
                new Isbn{ isbn = "9781593275846"},
            ]
        };
        AddedListOfBooks addedListOfBooks = await AddBooksToUserList(listOfBooks);
        Assert.AreEqual(2, addedListOfBooks.books.Length);
        //now will replace one of the books
        string isbnToReplace = "9781491950296";
        BookForReplace bookForReplace = new BookForReplace
        {
            userId = userId,
            isbn = "9781449337711",
        };
        UserBooks userBooksAfterReplace = await ReplaceBookUserList(isbnToReplace, bookForReplace);
        Assert.AreEqual(2, userBooksAfterReplace.books.Length);
        Assert.IsTrue(userBooksAfterReplace.books.Any(x => x.isbn == "9781449337711"));
        Assert.IsTrue(userBooksAfterReplace.books.Any(x => x.isbn == "9781593275846"));

        //test cleanup
        DeleteAllBooksFromUserList(userId);
    }

    [TestMethod]
    public async Task ReplaceBookInUserList_InvalidAuthorizationHeader_401()
    {
        //first check if user have books in his list
        UserBooks userBooks = await GetUserBooks(userId);
        if (userBooks.books.Length > 0)
        {
            //delete all books from user
            DeleteAllBooksFromUserList(userId);
        }
        //add some books        
        AddListOfBooks listOfBooks = new AddListOfBooks
        {
            userId = userId,
            collectionOfIsbns = [
                new Isbn{ isbn = "9781491950296"},
                new Isbn{ isbn = "9781593275846"},
            ]
        };
        AddedListOfBooks addedListOfBooks = await AddBooksToUserList(listOfBooks);
        Assert.AreEqual(2, addedListOfBooks.books.Length);
        //now will replace one of the books
        string isbnToReplace = "9781491950296";
        BookForReplace bookForReplace = new BookForReplace
        {
            userId = userId,
            isbn = "9781449337711",
        };
        using HttpResponseMessage response = await unauthorizedClient.PutAsJsonAsync($"BookStore/v1/Books/{isbnToReplace}", bookForReplace);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);

        //test cleanup
        DeleteAllBooksFromUserList(userId);
    }

    [TestMethod]
    public async Task ReplaceBookInUserList_InvalidUserId_401()
    {
        //first check if user have books in his list
        UserBooks userBooks = await GetUserBooks(userId);
        if (userBooks.books.Length > 0)
        {
            //delete all books from user
            DeleteAllBooksFromUserList(userId);
        }
        //add some books        
        AddListOfBooks listOfBooks = new AddListOfBooks
        {
            userId = userId,
            collectionOfIsbns = [
                new Isbn{ isbn = "9781491950296"},
                new Isbn{ isbn = "9781593275846"},
            ]
        };
        AddedListOfBooks addedListOfBooks = await AddBooksToUserList(listOfBooks);
        Assert.AreEqual(2, addedListOfBooks.books.Length);
        //now will replace one of the books
        string isbnToReplace = "9781491950296";
        BookForReplace bookForReplace = new BookForReplace
        {
            userId = "invalidUserId",
            isbn = "9781449337711",
        };
        using HttpResponseMessage response = await authorizedClient.PutAsJsonAsync($"BookStore/v1/Books/{isbnToReplace}", bookForReplace);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);

        //test cleanup
        DeleteAllBooksFromUserList(userId);
    }

    [TestMethod]
    public async Task ReplaceBookInUserList_IsbnForReplaceNotFound_400()
    {
        //first check if user have books in his list
        UserBooks userBooks = await GetUserBooks(userId);
        if (userBooks.books.Length > 0)
        {
            //delete all books from user
            DeleteAllBooksFromUserList(userId);
        }
        //add some books        
        AddListOfBooks listOfBooks = new AddListOfBooks
        {
            userId = userId,
            collectionOfIsbns = [
                new Isbn{ isbn = "9781491950296"},
                new Isbn{ isbn = "9781593275846"},
            ]
        };
        AddedListOfBooks addedListOfBooks = await AddBooksToUserList(listOfBooks);
        Assert.AreEqual(2, addedListOfBooks.books.Length);
        //now will replace one of the books
        string isbnToReplace = "9781491950296";
        BookForReplace bookForReplace = new BookForReplace
        {
            userId = userId,
            isbn = "invalidIsbn",
        };
        using HttpResponseMessage response = await authorizedClient.PutAsJsonAsync($"BookStore/v1/Books/{isbnToReplace}", bookForReplace);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        //test cleanup
        DeleteAllBooksFromUserList(userId);
    }

    [TestMethod]
    public async Task ReplaceBookInUserList_IsbnToReplaceNotFound_400()
    {
        //first check if user have books in his list
        UserBooks userBooks = await GetUserBooks(userId);
        if (userBooks.books.Length > 0)
        {
            //delete all books from user
            DeleteAllBooksFromUserList(userId);
        }
        //add some books        
        AddListOfBooks listOfBooks = new AddListOfBooks
        {
            userId = userId,
            collectionOfIsbns = [
                new Isbn{ isbn = "9781491950296"},
                new Isbn{ isbn = "9781593275846"},
            ]
        };
        AddedListOfBooks addedListOfBooks = await AddBooksToUserList(listOfBooks);
        Assert.AreEqual(2, addedListOfBooks.books.Length);
        //now will replace one of the books
        string isbnToReplace = "invalidIsbn";
        BookForReplace bookForReplace = new BookForReplace
        {
            userId = userId,
            isbn = "9781449337711",
        };
        using HttpResponseMessage response = await authorizedClient.PutAsJsonAsync($"BookStore/v1/Books/{isbnToReplace}", bookForReplace);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        //test cleanup
        DeleteAllBooksFromUserList(userId);
    }

    public void BooksAreEaqual(Book expectedBook, Book actualBook)
    {
        Assert.IsNotNull(actualBook, $"Expected that book: {actualBook.title} appears in the book list");
        Assert.AreEqual(expectedBook.isbn, actualBook.isbn);
        Assert.AreEqual(expectedBook.subTitle, actualBook.subTitle);
        Assert.AreEqual(expectedBook.author, actualBook.author);
        Assert.AreEqual(expectedBook.publish_date, actualBook.publish_date);
        Assert.AreEqual(expectedBook.publisher, actualBook.publisher);
        Assert.AreEqual(expectedBook.pages, actualBook.pages);
        StringAssert.Contains(actualBook.description, expectedBook.description, $"Expected book: {actualBook.title} to have this text in description: {expectedBook.description}");
        Assert.AreEqual(expectedBook.website, actualBook.website);
    }

    public async Task<UserBooks> GetUserBooks(string userId)
    {
        UserBooks userBooks = await authorizedClient.GetFromJsonAsync<UserBooks>($"Account/v1/User/{userId}");
        return userBooks;
    }

    public async void DeleteAllBooksFromUserList(string userId)
    {
        using HttpResponseMessage response = await authorizedClient.DeleteAsync($"BookStore/v1/Books?UserId={userId}");
        response.EnsureSuccessStatusCode();
        UserBooks userBooks = await GetUserBooks(userId);
        Assert.AreEqual(0, userBooks.books.Length, "Expected userBooks.books to be empty");
    }

    public async Task<UserBooks> ReplaceBookUserList(string isbnToReplace, BookForReplace bookForReplace)
    {
        using HttpResponseMessage response = await authorizedClient.PutAsJsonAsync($"BookStore/v1/Books/{isbnToReplace}", bookForReplace);
        response.EnsureSuccessStatusCode();
        UserBooks userBooksAfterReplace = await response.Content.ReadFromJsonAsync<UserBooks>();
        return userBooksAfterReplace;
    }

    public async Task<AddedListOfBooks> AddBooksToUserList(AddListOfBooks listOfBooks)
    {
        using HttpResponseMessage response = await authorizedClient.PostAsJsonAsync("BookStore/v1/Books", listOfBooks);
        response.EnsureSuccessStatusCode();
        AddedListOfBooks addedListOfBooks = await response.Content.ReadFromJsonAsync<AddedListOfBooks>();
        return addedListOfBooks;
    }
}
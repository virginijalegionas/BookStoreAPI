using System.Net.Http.Json;

namespace BookStoreAPI;

[TestClass]
public class ApiTests : TestBase
{
    [TestMethod]
    public async Task GetBooks()
    {
        Books actualBooks = await anonymousClient.GetFromJsonAsync<Books>("BookStore/v1/Books");
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
    public async Task AddListOfBooksToUser()
    {
        AddListOfBooks listOfBooks = new AddListOfBooks
        {
            userId = "d4e2fd50-cc12-42d8-8e93-1a1b0ce693c0",
            collectionOfIsbns = [
                new Isbn{ isbn = "9781491950296"},
                new Isbn{ isbn = "9781593275846"},
            ]
        };
        using HttpResponseMessage response = await authorizedClient.PostAsJsonAsync("BookStore/v1/Books", listOfBooks);
        response.EnsureSuccessStatusCode();
        AddedListOfBooks addedListOfBooks = await response.Content.ReadFromJsonAsync<AddedListOfBooks>();
        Assert.AreEqual(2, addedListOfBooks.books.Length);
        Assert.AreEqual("9781491950296", addedListOfBooks.books[0].isbn);
        Assert.AreEqual("9781593275846", addedListOfBooks.books[1].isbn);
    }

    [TestMethod]
    public async Task AddListOfBooksToUser_400()
    {
        AddListOfBooks listOfBooks = new AddListOfBooks
        {
            userId = "d4e2fd50-cc12-42d8-8e93-1a1b0ce693c0",
            collectionOfIsbns = [
                new Isbn{ isbn = "nonExisting"},
            ]
        };
        using HttpResponseMessage response = await authorizedClient.PostAsJsonAsync("BookStore/v1/Books", listOfBooks);
        Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task AddListOfBooksToUser_InvalidAuthorizationHeader_401()
    {
        AddListOfBooks listOfBooks = new AddListOfBooks
        {
            userId = "d4e2fd50-cc12-42d8-8e93-1a1b0ce693c0",
            collectionOfIsbns = [
                new Isbn{ isbn = "9781593275846"},
            ]
        };
        using HttpResponseMessage response = await unauthorizedClient.PostAsJsonAsync("BookStore/v1/Books", listOfBooks);
        Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
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
        Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
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
}
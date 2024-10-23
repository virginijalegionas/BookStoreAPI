namespace BookStoreAPI;

public class UserBooks
{
    public string userId { get; set; }
    public string userName { get; set; }

    public Book[] books { get; set; }
}

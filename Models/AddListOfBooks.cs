namespace BookStoreAPI;

public class AddListOfBooks
{
    public string userId { get; set; }
    public Isbn[] collectionOfIsbns { get; set; }
}

public class Isbn
{
    public string isbn { get; set; }
}
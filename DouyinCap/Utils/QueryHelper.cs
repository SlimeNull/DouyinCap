
//Console.WriteLine(await page.Frames[page.Frames.Length - 1].GetContentAsync());
//Console.ReadLine();

static class QueryHelper
{
    public static string AllChatMessages()
    {
        return ".webcast-chatroom___messages .webcast-chatroom___items > div > .webcast-chatroom___item.webcast-chatroom___enter-done";
    }

    public static string ChatMessage(string? dataId)
    {
        return $".webcast-chatroom___messages .webcast-chatroom___items > div > .webcast-chatroom___item.webcast-chatroom___enter-done[data-id=\"{dataId}\"]";
    }

    public static string AllChatMessagesAfter(string? dataId)
    {
        if (dataId == null)
            return AllChatMessages();
        
        return $".webcast-chatroom___messages .webcast-chatroom___items > div > .webcast-chatroom___item.webcast-chatroom___enter-done[data-id=\"{dataId}\"] ~ .webcast-chatroom___item.webcast-chatroom___enter-done";
    }
}
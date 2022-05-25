using PuppeteerSharp;

static class ElementHandleExtensions
{
    public static Task<string> GetAttributeAsync(this ElementHandle element, string attributeName)
    {
        return element.EvaluateFunctionAsync<string>($"(item)=>item.getAttribute('{attributeName}')");
    }

    public static async Task<string> GetOuterHtmlAsync(this ElementHandle element)
    {
        var outerHtmlHandle = await element.GetPropertyAsync("outerHTML");
        return await outerHtmlHandle.JsonValueAsync<string>();
    }

    public static async Task<string> GetInnerTextAsync(this ElementHandle element)
    {
        var outerHtmlHandle = await element.GetPropertyAsync("innerText");
        return await outerHtmlHandle.JsonValueAsync<string>();
    }
}
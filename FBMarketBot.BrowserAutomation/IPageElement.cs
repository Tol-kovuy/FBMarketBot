using FBMarketBot.IoC;
using System.Threading.Tasks;

namespace FBMarketBot.BrowserAutomation
{
    /// <summary>
    /// Defines the contract for a page element in the browser automation system, 
    /// providing methods for interacting with the element asynchronously.
    /// </summary>
    public interface IPageElement : ITransientDependency
    {
        /// <summary>
        /// Clicks the element asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ClickAsync();

        /// <summary>
        /// Types the specified text into the element asynchronously.
        /// </summary>
        /// <param name="text">The text to type into the element.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task TypeAsync(string text);

        /// <summary>
        /// Uploads a file to the element asynchronously (typically used for file input elements).
        /// </summary>
        /// <param name="filePath">The file path to upload.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UploadFileAsync(string filePath);

        /// <summary>
        /// Finds a child element inside the current element by its CSS selector asynchronously.
        /// </summary>
        /// <param name="selector">The CSS selector of the element to find.</param>
        /// <returns>A task representing the asynchronous operation, 
        /// containing the found element as an `IPageElement` or null if not found.</returns>
        Task<IPageElement> FindElementAsync(string selector);

        Task<string> EvaluateFunctionAsync<T>(string script);
    }
}

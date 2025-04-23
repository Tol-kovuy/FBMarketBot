using FBMarketBot.GoLogin.ProfileDTOs;
using FBMarketBot.IoC;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace FBMarketBot.BrowserAutomation
{
    /// <summary>
    /// Interface for browser automation tasks, providing methods to interact with web pages asynchronously.
    /// </summary>
    public interface IBrowserAutomation : ISingletonDependency
    {
        /// <summary>
        /// Navigates to the specified URL in the browser asynchronously.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task GoToAsync(string url);

        /// <summary>
        /// Retrieves the current URL of the page asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the current URL as a string.</returns>
        Task<string> GetCurrentUrl();

        /// <summary>
        /// Finds a single element on the page by its CSS selector asynchronously.
        /// </summary>
        /// <param name="selector">The CSS selector of the element to find.</param>
        /// <returns>A task representing the asynchronous operation, containing the found element as an IPageElement.</returns>
        Task<IPageElement> FindElementAsync(string selector);

        /// <summary>
        /// Waits for an element to appear on the page that matches the provided XPath expression asynchronously.
        /// </summary>
        /// <param name="selector">The XPath selector to wait for.</param>
        /// <returns>A task representing the asynchronous operation, containing the found element as an IPageElement.</returns>
        Task<IPageElement> WaitForXPathAsync(string selector);

        /// <summary>
        /// Finds all elements on the page matching the given CSS selector asynchronously.
        /// </summary>
        /// <param name="selector">The CSS selector of the elements to find.</param>
        /// <returns>A task representing the asynchronous operation, containing an array of IPageElement objects for the matching elements.</returns>
        Task<IPageElement[]> FindAllElementsAsync(string selector);

        /// <summary>
        /// Finds all elements on the page that match the provided XPath expression asynchronously.
        /// </summary>
        /// <param name="selector">The XPath selector to find elements by.</param>
        /// <returns>A task representing the asynchronous operation, containing an array of IPageElement objects for the matching elements.</returns>
        Task<IPageElement[]> FindAllXPathAsync(string selector);

        /// <summary>
        /// Retrieves all cookies associated with the current page asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a collection of cookies.</returns>
        Task<IEnumerable<Cookie>> GetCookiesAsync();
        Task<IPageElement> WaitForSelectorAsync(string selector);
        Task InitializeAsync(Profile profile);
    }
}

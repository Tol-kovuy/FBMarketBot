using System;
using System.Threading.Tasks;

namespace FBMarketBot.BrowserAutomation
{
    public static class InputSimulatorExtensions
    {
        /// <summary>
        /// Simulates text input with a delay between characters
        /// </summary>
        public static async Task TypeTextLikeHumanAsync(this IPageElement element, string text, int minDelay = 5, int maxDelay = 2000)
        {
            var random = new Random();

            foreach (char c in text)
            {
                await element.TypeAsync(c.ToString());
                await Task.Delay(random.Next(minDelay, maxDelay));
            }
        }
    }
}

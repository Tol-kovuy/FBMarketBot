using FBMarketBot.IoC;

namespace FBMarketBot.Settings.Selectors.AccessorSlctrs
{
    /// <summary>
    /// Represents an interface for accessing selectors within the application.
    /// This interface inherits from ITransientDependency, indicating it is registered as a transient dependency in the IoC container.
    /// </summary>
    public interface ISelectorsAccessors : ITransientDependency
    {
        /// <summary>
        /// Retrieves an instance of ISelectors. Get selector from Json file
        /// </summary>
        /// <returns>An implementation of the ISelectors interface.</returns>
        ISelectors Get();
    }
}
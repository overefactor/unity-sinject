namespace Overefactor.DI.Runtime.Interfaces
{
    /// <summary>
    /// Interface for handling the injection event in Sapo DI.
    /// </summary>
    public interface ISInjectorInjectHandler
    {
        /// <summary>
        /// Method that is called when the injector is in injection phase.
        /// </summary>
        /// <param name="injector">The injector where the injection is happening.</param>
        public void OnInject(ISInjector injector);
    }
}
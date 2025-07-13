namespace Overefactor.DI.Runtime.Interfaces
{
    /// <summary>
    /// Interface for handling the registration event in Sapo DI.
    /// </summary>
    public interface ISInjectorRegisterHandler
    {
        /// <summary>
        /// Method that is called when the injector is in registration phase.
        /// </summary>
        /// <param name="injector">The injector where the registration is happening.</param>
        public void OnRegister(ISInjector injector);
    }
}
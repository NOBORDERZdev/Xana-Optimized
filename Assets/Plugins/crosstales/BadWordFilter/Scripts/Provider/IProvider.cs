namespace Crosstales.BWF.Provider
{
   /// <summary>Interface for all providers.</summary>
   public interface IProvider
   {
      #region Properties

      /// <summary>Checks the readiness status of the provider.</summary>
      /// <returns>True if the provider is ready.</returns>
      bool isReady { get; set; }

      #endregion


      #region Methods

      /// <summary>Loads all sources.</summary>
      void Load();

      /// <summary>Saves all sources.</summary>
      void Save();

      /// <summary>Verify a source.</summary>
      /// <returns>An empty list if no errors are found, otherwise all the failed regexes.</returns>
      System.Collections.Generic.List<string> Verify(Crosstales.BWF.Data.Source source);

      #endregion
   }
}
// © 2018-2024 crosstales LLC (https://www.crosstales.com)
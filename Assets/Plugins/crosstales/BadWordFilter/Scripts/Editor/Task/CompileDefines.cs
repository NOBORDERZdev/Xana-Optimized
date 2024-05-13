#if UNITY_EDITOR
using UnityEditor;

namespace Crosstales.BWF.EditorTask
{
   /// <summary>Adds the given define symbols to PlayerSettings define symbols.</summary>
   [InitializeOnLoad]
   public class CompileDefines : Crosstales.Common.EditorTask.BaseCompileDefines
   {
      private const string SYMBOL = "CT_BWF";

      static CompileDefines()
      {
         if (Crosstales.BWF.EditorUtil.EditorConfig.COMPILE_DEFINES)
         {
            addSymbolsToAllTargets(SYMBOL);
         }
         else
         {
            removeSymbolsFromAllTargets(SYMBOL);
         }
      }
   }
}
#endif
// © 2017-2024 crosstales LLC (https://www.crosstales.com)
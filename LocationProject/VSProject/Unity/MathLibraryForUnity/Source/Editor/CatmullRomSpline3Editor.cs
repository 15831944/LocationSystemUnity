#if UNITY_EDITOR
using UnityEditor;

namespace Dest.Math
{
	[CustomEditor(typeof(CatmullRomSpline3))]
	public class CatmullRomSpline3Editor : SplineBaseEditor<CatmullRomSpline3>
	{
	}
}
#endif

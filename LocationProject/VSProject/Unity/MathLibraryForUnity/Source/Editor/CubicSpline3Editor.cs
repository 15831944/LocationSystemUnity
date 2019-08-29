#if UNITY_EDITOR
using UnityEditor;

namespace Dest.Math
{
	[CustomEditor(typeof(CubicSpline3))]
	public class CubicSpline3Editor : SplineBaseEditor<CubicSpline3>
	{
	}
}
#endif

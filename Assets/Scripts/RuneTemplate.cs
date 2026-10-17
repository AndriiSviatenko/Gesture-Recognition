using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	[CreateAssetMenu(fileName = "New Rune Template", menuName = "Rune Template", order = 1)]
    public class RuneTemplate : ScriptableObject
	{
		public string runeName;
		public List<Vector2> points = new List<Vector2>();
    }
}
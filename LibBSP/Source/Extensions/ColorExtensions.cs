#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
#define UNITY
#endif

namespace LibBSP {
#if UNITY
	using Color = UnityEngine.Color32;
#elif GODOT
	using Color = Godot.Color;
#else
	using Color = System.Drawing.Color;
#endif

	/// <summary>
	/// Static class containing helper methods for <c>Color</c> objects.
	/// </summary>
	public static class ColorExtensions {
		
		/// <summary>
		/// Constructs a new <c>Color</c> from the passed values.
		/// </summary>
		/// <param name="a">Alpha component of the color.</param>
		/// <param name="r">Red component of the color.</param>
		/// <param name="g">Green component of the color.</param>
		/// <param name="b">Blue component of the color.</param>
		/// <returns>The resulting <c>Color</c> object.</returns>
		public static Color FromArgb(int a, int r, int g, int b) {
#if UNITY
			return new Color((byte)r, (byte)g, (byte)b, (byte)a);
#elif GODOT
			return new Color(r << 24 | g << 16 | b << 8 | a);
#else
			return Color.FromArgb(a, r, g, b);
#endif
		}

		/// <summary>
		/// Gets this <c>Color</c> as R8G8B8A8.
		/// </summary>
		/// <param name="color">This <c>Color</c>.</param>
		/// <returns>A <c>byte</c> array with four members, RGBA.</returns>
		public static byte[] GetBytes(this Color color) {
			byte[] bytes = new byte[4];
#if UNITY
			bytes[0] = color.r;
			bytes[1] = color.g;
			bytes[2] = color.b;
			bytes[3] = color.a;
#elif GODOT
			bytes[0] = (byte)color.r8;
			bytes[1] = (byte)color.g8;
			bytes[2] = (byte)color.b8;
			bytes[3] = (byte)color.a8;
#else
			bytes[0] = color.R;
			bytes[1] = color.G;
			bytes[2] = color.B;
			bytes[3] = color.A;
#endif
			return bytes;
		}

	}
}

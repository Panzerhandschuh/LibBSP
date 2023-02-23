#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
#define UNITY
#endif

using System;
using System.Collections.Generic;
using System.Text;

namespace LibBSP {
#if UNITY
	using Vector2 = UnityEngine.Vector2;
	using Vector3 = UnityEngine.Vector3;
#elif GODOT
	using Vector2 = Godot.Vector2;
	using Vector3 = Godot.Vector3;
#elif NEOAXIS
	using Vector2 = NeoAxis.Vector2F;
	using Vector3 = NeoAxis.Vector3F;
#else
	using Vector2 = System.Numerics.Vector2;
	using Vector3 = System.Numerics.Vector3;
#endif

	/// <summary>
	/// Handles the data needed for a primitive texture info object.
	/// </summary>
	public struct PrimitiveTextureInfo : ILumpObject {

		/// <summary>
		/// The <see cref="ILump"/> this <see cref="ILumpObject"/> came from.
		/// </summary>
		public ILump Parent { get; private set; }

		/// <summary>
		/// Array of <c>byte</c>s used as the data source for this <see cref="ILumpObject"/>.
		/// </summary>
		public byte[] Data { get; private set; }

		/// <summary>
		/// The <see cref="LibBSP.MapType"/> to use to interpret <see cref="Data"/>.
		/// </summary>
		public MapType MapType {
			get {
				if (Parent == null || Parent.Bsp == null) {
					return MapType.Undefined;
				}
				return Parent.Bsp.MapType;
			}
		}

		/// <summary>
		/// The version number of the <see cref="ILump"/> this <see cref="ILumpObject"/> came from.
		/// </summary>
		public int LumpVersion {
			get {
				if (Parent == null) {
					return 0;
				}
				return Parent.LumpInfo.version;
			}
		}

		/// <summary>
		/// Gets or sets the texture coordinates of this <see cref="PrimitiveTextureInfo"/>.
		/// </summary>
		public Vector2 TexCoord {
			get {
				return Vector2Extensions.ToVector2(Data, 0);
			}
			set {
				value.GetBytes().CopyTo(Data, 0);
			}
		}

		/// <summary>
		/// Gets or sets the lightmap texture coordinates of this <see cref="PrimitiveTextureInfo"/>.
		/// </summary>
		public Vector2 LightmapCoord {
			get {
				return Vector2Extensions.ToVector2(Data, 8);
			}
			set {
				value.GetBytes().CopyTo(Data, 8);
			}
		}

		/// <summary>
		/// Creates a new <see cref="PrimitiveTextureInfo"/> object from a <c>byte</c> array.
		/// </summary>
		/// <param name="data"><c>byte</c> array to parse.</param>
		/// <param name="parent">The <see cref="ILump"/> this <see cref="StaticProp"/> came from.</param>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> was <c>null</c>.</exception>
		public PrimitiveTextureInfo(byte[] data, ILump parent = null) {
			if (data == null) {
				throw new ArgumentNullException();
			}

			Data = data;
			Parent = parent;
		}

		/// <summary>
		/// Creates a new <see cref="PrimitiveTextureInfo"/> by copying the fields in <paramref name="source"/>, using
		/// <paramref name="parent"/> to get <see cref="LibBSP.MapType"/> and <see cref="LumpInfo.version"/>
		/// to use when creating the new <see cref="StaticProp"/>.
		/// If the <paramref name="parent"/>'s <see cref="BSP"/>'s <see cref="LibBSP.MapType"/> is different from
		/// the one from <paramref name="source"/>, it does not matter, because fields are copied by name.
		/// </summary>
		/// <param name="source">The <see cref="Model"/> to copy.</param>
		/// <param name="parent">
		/// The <see cref="ILump"/> to use as the <see cref="Parent"/> of the new <see cref="Model"/>.
		/// Use <c>null</c> to use the <paramref name="source"/>'s <see cref="Parent"/> instead.
		/// </param>
		public PrimitiveTextureInfo(PrimitiveTextureInfo source, ILump parent) {
			Parent = parent;

			if (parent != null && parent.Bsp != null) {
				if (source.Parent != null && source.Parent.Bsp != null && source.Parent.Bsp.MapType == parent.Bsp.MapType && source.LumpVersion == parent.LumpInfo.version) {
					Data = new byte[source.Data.Length];
					Array.Copy(source.Data, Data, source.Data.Length);
					return;
				} else {
					Data = new byte[GetStructLength(parent.Bsp.MapType, parent.LumpInfo.version)];
				}
			} else {
				if (source.Parent != null && source.Parent.Bsp != null) {
					Data = new byte[GetStructLength(source.Parent.Bsp.MapType, source.Parent.LumpInfo.version)];
				} else {
					Data = new byte[GetStructLength(MapType.Undefined, 0)];
				}
			}

			TexCoord = source.TexCoord;
			LightmapCoord = source.LightmapCoord;
		}

		/// <summary>
		/// Factory method to parse a <c>byte</c> array into <see cref="PrimitiveTextureInfo"/> objects.
		/// </summary>
		/// <param name="data">The data to parse.</param>
		/// <param name="bsp">The <see cref="BSP"/> this lump came from.</param>
		/// <param name="lumpInfo">The <see cref="LumpInfo"/> associated with this lump.</param>
		/// <returns><see cref="PrimitiveTextureInfo"/> objects.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> parameter was <c>null</c>.</exception>
		public static Lump<PrimitiveTextureInfo> LumpFactory(byte[] data, BSP bsp, LumpInfo lumpInfo) {
			if (data == null) {
				throw new ArgumentNullException();
			}

			return new Lump<PrimitiveTextureInfo>(data, GetStructLength(bsp.MapType, lumpInfo.version), bsp, lumpInfo);
		}

		/// <summary>
		/// This is a variable length structure. Return -1. The <see cref="PrimitiveTextureInfo"/> class will handle object creation.
		/// </summary>
		/// <param name="mapType">The <see cref="LibBSP.MapType"/> of the BSP.</param>
		/// <param name="lumpVersion">The version number for the lump.</param>
		/// <returns>-1</returns>
		public static int GetStructLength(MapType mapType, int lumpVersion = 0) {
			return 16;
		}

	}
}

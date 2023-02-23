#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
#define UNITY
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibBSP
{
#if UNITY
	using Vector3 = UnityEngine.Vector3;
#elif GODOT
	using Vector3 = Godot.Vector3;
#elif NEOAXIS
	using Vector3 = NeoAxis.Vector3F;
#else
	using Vector3 = System.Numerics.Vector3;
#endif

	/// <summary>
	/// Holds the data used by the world light structures of all formats of BSP.
	/// </summary>
	public struct WorldLight : ILumpObject {
		
		public enum EmitType {
			EmitSurface,       // 90 degree spotlight
			EmitPoint,         // simple point light source
			EmitSpotlight,     // spotlight with penumbra
			EmitSkyLight,      // directional light with no falloff (surface must trace to SKY texture)
			EmitQuakeLight,    // linear falloff, non-lambertian
			EmitSkyAmbient,    // spherical light source with no falloff (surface must trace to SKY texture)
		}

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
		/// Gets or sets the origin for this <see cref="WorldLight"/>.
		/// </summary>
		public Vector3 Origin {
			get {
				return Vector3Extensions.ToVector3(Data, 0);
			}
			set {
				value.GetBytes().CopyTo(Data, 0);
			}
		}

		/// <summary>
		/// Gets or sets the intensity for this <see cref="WorldLight"/>.
		/// </summary>
		public Vector3 Intensity {
			get {
				return Vector3Extensions.ToVector3(Data, 12);
			}
			set {
				value.GetBytes().CopyTo(Data, 12);
			}
		}

		/// <summary>
		/// Gets or sets the normal for this <see cref="WorldLight"/>.
		/// </summary>
		public Vector3 Normal {
			get {
				return Vector3Extensions.ToVector3(Data, 24);
			}
			set {
				value.GetBytes().CopyTo(Data, 24);
			}
		}

		/// <summary>
		/// Gets or sets the shadow cast offset for this <see cref="WorldLight"/>.
		/// </summary>
		public Vector3 ShadowCastOffset {
			get {
				return Vector3Extensions.ToVector3(Data, 36);
			}
			set {
				value.GetBytes().CopyTo(Data, 36);
			}
		}

		/// <summary>
		/// Gets or sets the cluster for this <see cref="WorldLight"/>.
		/// </summary>
		public int Cluster {
			get {
				return BitConverter.ToInt32(Data, 48);
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);
				bytes.CopyTo(Data, 48);
			}
		}

		/// <summary>
		/// Gets or sets the type for this <see cref="WorldLight"/>.
		/// </summary>
		public EmitType Type {
			get {
				return (EmitType)BitConverter.ToInt32(Data, 52);
			}
			set {
				byte[] bytes = BitConverter.GetBytes((int)value);
				bytes.CopyTo(Data, 52);
			}
		}

		/// <summary>
		/// Gets or sets the style for this <see cref="WorldLight"/>.
		/// </summary>
		public int Style {
			get {
				return BitConverter.ToInt32(Data, 56);
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);
				bytes.CopyTo(Data, 56);
			}
		}

		/// <summary>
		/// Gets or sets the stop dot for this <see cref="WorldLight"/>.
		/// </summary>
		public float StopDot {
			get {
				return BitConverter.ToSingle(Data, 60);
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);
				bytes.CopyTo(Data, 60);
			}
		}

		/// <summary>
		/// Gets or sets the stop dot 2 for this <see cref="WorldLight"/>.
		/// </summary>
		public float StopDot2 {
			get {
				return BitConverter.ToSingle(Data, 64);
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);
				bytes.CopyTo(Data, 64);
			}
		}

		/// <summary>
		/// Gets or sets the exponent for this <see cref="WorldLight"/>.
		/// </summary>
		public float Exponent {
			get {
				return BitConverter.ToSingle(Data, 68);
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);
				bytes.CopyTo(Data, 68);
			}
		}

		/// <summary>
		/// Gets or sets the radius for this <see cref="WorldLight"/>.
		/// </summary>
		public float Radius {
			get {
				return BitConverter.ToSingle(Data, 72);
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);
				bytes.CopyTo(Data, 72);
			}
		}

		/// <summary>
		/// Gets or sets the constant attenuation for this <see cref="WorldLight"/>.
		/// </summary>
		public float ConstantAttenuation {
			get {
				return BitConverter.ToSingle(Data, 76);
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);
				bytes.CopyTo(Data, 76);
			}
		}

		/// <summary>
		/// Gets or sets the linear attenuation for this <see cref="WorldLight"/>.
		/// </summary>
		public float LinearAttenuation {
			get {
				return BitConverter.ToSingle(Data, 80);
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);
				bytes.CopyTo(Data, 80);
			}
		}

		/// <summary>
		/// Gets or sets the quadratic attenuation for this <see cref="WorldLight"/>.
		/// </summary>
		public float QuadraticAttenuation {
			get {
				return BitConverter.ToSingle(Data, 84);
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);
				bytes.CopyTo(Data, 84);
			}
		}

		/// <summary>
		/// Gets or sets the flags for this <see cref="WorldLight"/>.
		/// </summary>
		public int Flags {
			get {
				return BitConverter.ToInt32(Data, 88);
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);
				bytes.CopyTo(Data, 88);
			}
		}

		/// <summary>
		/// Gets or sets the texture info for this <see cref="WorldLight"/>.
		/// </summary>
		public int TexInfo {
			get {
				return BitConverter.ToInt32(Data, 92);
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);
				bytes.CopyTo(Data, 92);
			}
		}

		/// <summary>
		/// Gets or sets the owner for this <see cref="WorldLight"/>.
		/// </summary>
		public int Owner {
			get {
				return BitConverter.ToInt32(Data, 96);
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);
				bytes.CopyTo(Data, 96);
			}
		}

		/// <summary>
		/// Creates a new <see cref="PlaneBSP"/> object from a <c>byte</c> array.
		/// </summary>
		/// <param name="data"><c>byte</c> array to parse.</param>
		/// <param name="parent">The <see cref="ILump"/> this <see cref="PlaneBSP"/> came from.</param>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> was <c>null</c>.</exception>
		public WorldLight(byte[] data, ILump parent = null) {
			if (data == null) {
				throw new ArgumentNullException();
			}

			Data = data;
			Parent = parent;
		}

		/// <summary>
		/// Creates a new <see cref="PlaneBSP"/> by copying the fields in <paramref name="source"/>, using
		/// <paramref name="parent"/> to get <see cref="LibBSP.MapType"/> and <see cref="LumpInfo.version"/>
		/// to use when creating the new <see cref="PlaneBSP"/>.
		/// If the <paramref name="parent"/>'s <see cref="BSP"/>'s <see cref="LibBSP.MapType"/> is different from
		/// the one from <paramref name="source"/>, it does not matter, because fields are copied by name.
		/// </summary>
		/// <param name="source">The <see cref="PlaneBSP"/> to copy.</param>
		/// <param name="parent">
		/// The <see cref="ILump"/> to use as the <see cref="Parent"/> of the new <see cref="PlaneBSP"/>.
		/// Use <c>null</c> to use the <paramref name="source"/>'s <see cref="Parent"/> instead.
		/// </param>
		public WorldLight(WorldLight source, ILump parent) {
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

			Origin = source.Origin;
			Intensity = source.Intensity;
			Normal = source.Normal;
			ShadowCastOffset = source.ShadowCastOffset;
			Cluster = source.Cluster;
			Type = source.Type;
			Style = source.Style;
			StopDot = source.StopDot;
			StopDot2 = source.StopDot2;
			Exponent = source.Exponent;
			Radius = source.Radius;
			ConstantAttenuation = source.ConstantAttenuation;
			LinearAttenuation = source.LinearAttenuation;
			QuadraticAttenuation = source.QuadraticAttenuation;
			Flags = source.Flags;
			TexInfo = source.TexInfo;
			Owner = source.Owner;
		}

		/// <summary>
		/// Factory method to parse a <c>byte</c> array into a <see cref="Lump{PlaneEx}"/>.
		/// </summary>
		/// <param name="data">The data to parse.</param>
		/// <param name="bsp">The <see cref="BSP"/> this lump came from.</param>
		/// <param name="lumpInfo">The <see cref="LumpInfo"/> associated with this lump.</param>
		/// <returns>A <see cref="Lump{PlaneEx}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> parameter was <c>null</c>.</exception>
		public static Lump<WorldLight> LumpFactory(byte[] data, BSP bsp, LumpInfo lumpInfo) {
			if (data == null) {
				throw new ArgumentNullException();
			}

			return new Lump<WorldLight>(data, GetStructLength(bsp.MapType, lumpInfo.version), bsp, lumpInfo);
		}

		/// <summary>
		/// Gets the length of this struct's data for the given <paramref name="mapType"/> and <paramref name="lumpVersion"/>.
		/// </summary>
		/// <param name="mapType">The <see cref="LibBSP.MapType"/> of the BSP.</param>
		/// <param name="lumpVersion">The version number for the lump.</param>
		/// <returns>The length, in <c>byte</c>s, of this struct.</returns>
		/// <exception cref="ArgumentException">This struct is not valid or is not implemented for the given <paramref name="mapType"/> and <paramref name="lumpVersion"/>.</exception>
		public static int GetStructLength(MapType mapType, int lumpVersion = 0) {
			if (mapType.IsSubtypeOf(MapType.Source)) {
				return 100;
			}

			throw new ArgumentException("Lump object " + MethodBase.GetCurrentMethod().DeclaringType.Name + " does not exist in map type " + mapType + " or has not been implemented.");
		}

		/// <summary>
		/// Gets the index for this lump in the BSP file for a specific map format.
		/// </summary>
		/// <param name="type">The map type.</param>
		/// <returns>Index for this lump, or -1 if the format doesn't have this lump or it's not implemented.</returns>
		public static int GetIndexForLump(MapType type) {
			if (type.IsSubtypeOf(MapType.Source)) {
				return 15;
			}

			return -1;
		}
	}
}

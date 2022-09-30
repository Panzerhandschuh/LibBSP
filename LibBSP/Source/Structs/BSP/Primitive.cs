#if UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
#define UNITY
#endif

using System;
using System.Collections.Generic;
using System.Reflection;

namespace LibBSP {
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
	/// Holds the data used by the primitive structures of all formats of BSP.
	/// </summary>
	public struct Primitive : ILumpObject {

		public enum PrimitiveType {
			PRIM_TRILIST = 0,
			PRIM_TRISTRIP = 1
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
		/// Gets or sets the type of this <see cref="Primitive"/>.
		/// </summary>
		public PrimitiveType Type {
			get {
				if (MapType.IsSubtypeOf(MapType.Source)) {
					return (PrimitiveType)Data[0];
				}

				return PrimitiveType.PRIM_TRILIST;
			}
			set {
				if (MapType.IsSubtypeOf(MapType.Source)) {
					Data[0] = (byte)value;
				}
			}
		}

		/// <summary>
		/// Enumerates the indices referenced by this <see cref="Primitive"/>.
		/// </summary>
		public IEnumerable<int> Indices {
			get {
				for (int i = 0; i < IndexCount; ++i) {
					yield return (int)Parent.Bsp.PrimitiveIndices[FirstIndex + i];
				}
			}
		}

		/// <summary>
		/// Gets or sets the index of the first triangle of this <see cref="Primitive"/>.
		/// </summary>
		[Index("Indices")] public int FirstIndex {
			get {
				if (MapType.IsSubtypeOf(MapType.Source)) {
					return BitConverter.ToUInt16(Data, 1);
				}

				return -1;
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);

				if (MapType.IsSubtypeOf(MapType.Source)) {
					Data[1] = bytes[0];
					Data[2] = bytes[1];
				}
			}
		}

		/// <summary>
		/// Gets or sets the count of triangles in this <see cref="Primitive"/>.
		/// </summary>
		[Count("Indices")] public int IndexCount {
			get {
				if (MapType.IsSubtypeOf(MapType.Source)) {
					return BitConverter.ToUInt16(Data, 3);
				}

				return -1;
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);

				if (MapType.IsSubtypeOf(MapType.Source)) {
					Data[3] = bytes[0];
					Data[4] = bytes[1];
				}
			}
		}

		/// <summary>
		/// Enumerates the indices referenced by this <see cref="Primitive"/>.
		/// </summary>
		public IEnumerable<Vector3> Vertices {
			get {
				for (int i = 0; i < VertexCount; ++i) {
					yield return Parent.Bsp.PrimitiveVertices[FirstVertex + i];
				}
			}
		}

		/// <summary>
		/// Gets or sets the index of the first vertex of this <see cref="Primitive"/>.
		/// </summary>
		[Index("Vertices")] public int FirstVertex {
			get {
				if (MapType.IsSubtypeOf(MapType.Source)) {
					return BitConverter.ToUInt16(Data, 5);
				}

				return -1;
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);

				if (MapType.IsSubtypeOf(MapType.Source)) {
					Data[5] = bytes[0];
					Data[6] = bytes[1];
				}
			}
		}

		/// <summary>
		/// Gets or sets the count of vertices in this <see cref="Primitive"/>.
		/// </summary>
		[Count("Vertices")] public int VertexCount {
			get {
				if (MapType.IsSubtypeOf(MapType.Source)) {
					return BitConverter.ToUInt16(Data, 7);
				}

				return -1;
			}
			set {
				byte[] bytes = BitConverter.GetBytes(value);

				if (MapType.IsSubtypeOf(MapType.Source)) {
					Data[7] = bytes[0];
					Data[8] = bytes[1];
				}
			}
		}

		/// <summary>
		/// Creates a new <see cref="Primitive"/> object from a <c>byte</c> array.
		/// </summary>
		/// <param name="data"><c>byte</c> array to parse.</param>
		/// <param name="parent">The <see cref="ILump"/> this <see cref="Primitive"/> came from.</param>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> was <c>null</c>.</exception>
		public Primitive(byte[] data, ILump parent = null) {
			if (data == null) {
				throw new ArgumentNullException();
			}

			Data = data;
			Parent = parent;
		}

		/// <summary>
		/// Creates a new <see cref="Primitive"/> by copying the fields in <paramref name="source"/>, using
		/// <paramref name="parent"/> to get <see cref="LibBSP.MapType"/> and <see cref="LumpInfo.version"/>
		/// to use when creating the new <see cref="Primitive"/>.
		/// If the <paramref name="parent"/>'s <see cref="BSP"/>'s <see cref="LibBSP.MapType"/> is different from
		/// the one from <paramref name="source"/>, it does not matter, because fields are copied by name.
		/// </summary>
		/// <param name="source">The <see cref="Primitive"/> to copy.</param>
		/// <param name="parent">
		/// The <see cref="ILump"/> to use as the <see cref="Parent"/> of the new <see cref="Primitive"/>.
		/// Use <c>null</c> to use the <paramref name="source"/>'s <see cref="Parent"/> instead.
		/// </param>
		public Primitive(Primitive source, ILump parent) {
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

			Type = source.Type;
			FirstIndex = source.FirstIndex;
			IndexCount = source.IndexCount;
			FirstVertex = source.FirstVertex;
			VertexCount = source.VertexCount;
		}

		/// <summary>
		/// Factory method to parse a <c>byte</c> array into a <see cref="Lump{Primitive}"/>.
		/// </summary>
		/// <param name="data">The data to parse.</param>
		/// <param name="bsp">The <see cref="BSP"/> this lump came from.</param>
		/// <param name="lumpInfo">The <see cref="LumpInfo"/> associated with this lump.</param>
		/// <returns>A <see cref="Lump{Primitive}"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> parameter was <c>null</c>.</exception>
		public static Lump<Primitive> LumpFactory(byte[] data, BSP bsp, LumpInfo lumpInfo) {
			if (data == null) {
				throw new ArgumentNullException();
			}

			return new Lump<Primitive>(data, GetStructLength(bsp.MapType, lumpInfo.version), bsp, lumpInfo);
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
				return 10; // Note: This is 10 bytes instead of 9 due to data structure alignment
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
				return 37;
			}

			return -1;
		}

	}
}

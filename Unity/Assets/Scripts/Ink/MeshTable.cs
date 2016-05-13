using System;
using System.Collections.Generic;
using UnityEngine;

namespace Painter
{
	public class MeshTable
	{
		readonly int AREA_LIMIT;
		List<Vector3> _Points;
		MeshBlock[] _Blocks;
		float _Around = 0;

		public MeshTable(int area, Vector3[] list)
		{
			if (area < 2) area = 2;
			AREA_LIMIT = area;
			_Points = new List<Vector3>(list);

			Vector3 first = _Points[0];
			float minx = first.x, maxx = first.x, minz = first.z, maxz = first.z;
			foreach(Vector3 p in _Points)
			{
				if (minx > p.x) minx = p.x;
				if (maxx < p.x) maxx = p.x;
				if (minz > p.z) minz = p.z;
				if (maxz < p.z) maxz = p.z;
			}
			Debug.Log("[MeshTable] {" + minx + ">" + maxx + "," + minz + ">" + maxz + "}");

			float xstep = (maxx - minx) / AREA_LIMIT;
			float zstep = (maxz - minz) / AREA_LIMIT;
			_Blocks = new MeshBlock[AREA_LIMIT * AREA_LIMIT];
			for(int xi = 0; xi < AREA_LIMIT; xi++)
			{
				float xstart = minx + xstep * xi;
				float xend = xstart + xstep;
				for(int zi = 0; zi < AREA_LIMIT; zi++)
				{
					float zstart = minz + zstep * zi;
					float zend = zstart + zstep;
					int index = xi * AREA_LIMIT + zi;
					_Blocks[index] = new MeshBlock(this, xstart, xend, zstart, zend);
				}
			}

			for(int i = 0; i < _Points.Count; i++)
			{
				Vector3 point = _Points[i];
				int xi = (int)((point.x - minx) / xstep);
				int zi = (int)((point.z - minz) / zstep);
				if (xi < 0) xi = 0;
				if (xi >= AREA_LIMIT) xi = AREA_LIMIT - 1;
				if (zi < 0) zi = 0;
				if (zi >= AREA_LIMIT) zi = AREA_LIMIT - 1;
				int index = xi * AREA_LIMIT + zi;
				_Blocks[index].AddIndex(i);
			}

#if UNITY_EDITOR
			string msg = "";
			for (int b = 0; b < _Blocks.Length; b++)
			{
				msg += " " + _Blocks[b].Count;
				if ((b + 1) % AREA_LIMIT == 0) msg += "\n";
				else msg += ",";
			}
			Debug.Log("[MeshTable]\n" + msg);
#endif
			if (xstep > zstep) _Around = xstep / 10.0f; else _Around = zstep / 10.0f;
		}
		
		public bool Nearest(Vector3 point, out int index)
		{
			index = -1;
			foreach(MeshBlock block in _Blocks)
			{
				if (block.Contact(point, _Around, float.MaxValue, out index)) return true;
			}
			return false;
		}

		public bool Contact(Vector3 point, float border, out int index)
		{
			index = -1;
			foreach (MeshBlock block in _Blocks)
			{
				if (block.Contact(point, 0, border, out index)) return true;
			}
			return false;
		}

		public List<int> Contacts(Vector3 point, float border)
		{
			List<int> list = new List<int>();
			foreach(MeshBlock block in _Blocks)
			{
				var contacts = block.Contacts(point, border);
				if (contacts != null && contacts.Count > 0) list.AddRange(contacts);
			}
			// どこにも接していないなら、近傍を計算
			if(list.Count == 0)
			{
				int index;
				if (Nearest(point, out index)) list.Add(index);
			}
			return list;
		}

		class MeshBlock
		{
			MeshTable _Table;
			float MinX, MaxX, MinZ, MaxZ;
			List<int> Indexes = new List<int>();
			public int Count { get { return Indexes.Count; } }

			public MeshBlock(MeshTable table, float minx, float maxx, float minz, float maxz)
			{
				_Table = table;
				MinX = minx;
				MaxX = maxx;
				MinZ = minz;
				MaxZ = maxz;
			}

			public void AddIndex(int i) { Indexes.Add(i); }

			public bool Contact(Vector3 point, float around, float border, out int index)
			{
				index = -1;
				if (point.x < MinX - around || MaxX + around < point.x || point.z < MinZ - around || MaxZ + around < point.z) return false;

				float distance = border;
				foreach(int i in Indexes)
				{
					Vector3 tmpPoint = _Table._Points[i];
					float tmpDistance = Vector3.Distance(point, tmpPoint);
					if(tmpDistance < distance)
					{
						index = i;
						distance = tmpDistance;
					}
				}
				return index >= 0;
			}

			public List<int> Contacts(Vector3 point, float border)
			{
				if (point.x < MinX - border || MaxX + border < point.x || point.z < MinZ - border || MaxZ + border < point.z) return null;

				List<int> list = new List<int>();
				foreach (int i in Indexes)
				{
					float tmpDistance = Vector3.Distance(point, _Table._Points[i]);
					if (border > tmpDistance) list.Add(i);
				}
				return list;
			}
		}
	}
}
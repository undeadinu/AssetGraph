using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;

namespace AssetBundleGraph {

	[Serializable] 
	public class SerializableMultiTargetInt {

		[Serializable]
		public class Entry {
			[SerializeField] public BuildTargetGroup targetGroup;
			[SerializeField] public int value;

			public Entry(BuildTargetGroup g, int v) {
				targetGroup = g;
				value = v;
			}
		}

		[SerializeField] private List<Entry> m_values;

		public SerializableMultiTargetInt(int value) {
			m_values = new List<Entry>();
			this[AssetBundleGraphPlatformSettings.DefaultTarget] = value;
		}

		public SerializableMultiTargetInt() {
			m_values = new List<Entry>();
		}

		public SerializableMultiTargetInt(MultiTargetProperty<int> property) {
			m_values = new List<Entry>();
			foreach(var k in property.Keys) {
				m_values.Add(new Entry(k, property[k]));
			}
		}

		public int this[BuildTargetGroup g] {
			get {
				int i = m_values.FindIndex(v => v.targetGroup == g);
				if(i >= 0) {
					return m_values[i].value;
				} else {
					return 0;
				}
			}
			set {
				int i = m_values.FindIndex(v => v.targetGroup == g);
				if(i >= 0) {
					m_values[i].value = value;
				} else {
					m_values.Add(new Entry(g, value));
				}
			}
		}

		public int this[BuildTarget index] {
			get {
				return this[AssetBundleGraphPlatformSettings.BuildTargetToBuildTargetGroup(index)];
			}
			set {
				this[AssetBundleGraphPlatformSettings.BuildTargetToBuildTargetGroup(index)] = value;
			}
		}

		public int DefaultValue {
			get {
				return this[AssetBundleGraphPlatformSettings.DefaultTarget];
			}
			set {
				this[AssetBundleGraphPlatformSettings.DefaultTarget] = value;
			}
		}

		public int CurrentPlatformValue {
			get {
				return this[EditorUserBuildSettings.selectedBuildTargetGroup];
			}
		}

		public bool ContainsValueOf (BuildTargetGroup group) {
			return m_values.FindIndex(v => v.targetGroup == group) >= 0;
		}

		public void Remove (BuildTargetGroup group) {
			int index = m_values.FindIndex(v => v.targetGroup == group);
			if(index >= 0) {
				m_values.RemoveAt(index);
			}
		}

		public MultiTargetProperty<int> ToProperty () {
			MultiTargetProperty<int> p = new MultiTargetProperty<int>();

			foreach(Entry e in m_values) {
				p.Set(e.targetGroup, e.value);
			}

			return p;
		}
	}
}
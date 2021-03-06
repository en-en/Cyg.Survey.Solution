//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//     Website: http://www.freesql.net
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;
using NpgsqlTypes;
using Npgsql.LegacyPostgis;

namespace GisModel {

	[JsonObject(MemberSerialization.OptIn), Table(Name = "design_user_line")]
	public partial class Design_user_line {

		/// <summary>
		/// 线路ID
		/// </summary>
		[JsonProperty, Column(Name = "id", DbType = "varchar(32)", IsPrimary = true)]
		public string Id { get; set; } = string.Empty;

		/// <summary>
		/// 方位角
		/// </summary>
		[JsonProperty, Column(Name = "azimuth")]
		public double? Azimuth { get; set; }

		/// <summary>
		/// 符号编码别名
		/// </summary>
		[JsonProperty, Column(Name = "code_alias", DbType = "varchar(50)")]
		public string Code_alias { get; set; } = string.Empty;

		/// <summary>
		/// 线路终点杆塔ID
		/// </summary>
		[JsonProperty, Column(Name = "end_id", DbType = "varchar(32)")]
		public string End_id { get; set; } = string.Empty;

		/// <summary>
		/// 终点节点类型
		/// </summary>
		[JsonProperty, Column(Name = "end_node_type")]
		public int? End_node_type { get; set; }

		/// <summary>
		/// 下户类型对应的ID
		/// </summary>
		[JsonProperty, Column(Name = "entry_id", DbType = "varchar(50)")]
		public string Entry_id { get; set; } = string.Empty;

		/// <summary>
		/// 下户方式
		/// </summary>
		[JsonProperty, Column(Name = "entry_mode", DbType = "varchar(64)")]
		public string Entry_mode { get; set; } = string.Empty;

		/// <summary>
		/// 下户类型
		/// </summary>
		[JsonProperty, Column(Name = "entry_type", DbType = "varchar(64)")]
		public string Entry_type { get; set; } = string.Empty;

		/// <summary>
		/// 几何列，类型为线
		/// </summary>
		[JsonProperty, Column(Name = "geom")]
		public PostgisGeometry Geom { get; set; }

		/// <summary>
		/// 是否电缆
		/// </summary>
		[JsonProperty, Column(Name = "is_cable")]
		public bool? Is_cable { get; set; }

		/// <summary>
		/// 电压等级
		/// </summary>
		[JsonProperty, Column(Name = "kv_level", DbType = "varchar(15)")]
		public string Kv_level { get; set; } = string.Empty;

		/// <summary>
		/// 线路长度
		/// </summary>
		[JsonProperty, Column(Name = "length")]
		public double? Length { get; set; }

		/// <summary>
		/// 线路型号
		/// </summary>
		[JsonProperty, Column(Name = "mode", DbType = "varchar(60)")]
		public string Mode { get; set; } = string.Empty;

		/// <summary>
		/// 型号ID
		/// </summary>
		[JsonProperty, Column(Name = "mode_id", DbType = "varchar(50)")]
		public string Mode_id { get; set; } = string.Empty;

		/// <summary>
		/// 线路名称
		/// </summary>
		[JsonProperty, Column(Name = "name", DbType = "varchar(100)")]
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// 所属要素Id
		/// </summary>
		[JsonProperty, Column(Name = "parent_id", DbType = "varchar(32)")]
		public string Parent_id { get; set; } = string.Empty;

		/// <summary>
		/// 所属图层名称
		/// </summary>
		[JsonProperty, Column(Name = "parent_name", DbType = "varchar(32)")]
		public string Parent_name { get; set; } = string.Empty;

		/// <summary>
		/// 所属工程编号
		/// </summary>
		[JsonProperty, Column(Name = "project_id", DbType = "varchar(64)")]
		public string Project_id { get; set; } = string.Empty;

		/// <summary>
		/// 备注
		/// </summary>
		[JsonProperty, Column(Name = "remark")]
		public string Remark { get; set; } = string.Empty;

		/// <summary>
		/// 线路起始杆塔ID
		/// </summary>
		[JsonProperty, Column(Name = "start_id", DbType = "varchar(32)")]
		public string Start_id { get; set; } = string.Empty;

		/// <summary>
		/// 起始节点类型
		/// </summary>
		[JsonProperty, Column(Name = "start_node_type")]
		public int? Start_node_type { get; set; }

		/// <summary>
		/// 线路状态
		/// </summary>
		[JsonProperty, Column(Name = "state")]
		public int? State { get; set; }

		/// <summary>
		/// 勘测时间
		/// </summary>
		[JsonProperty, Column(Name = "survey_time")]
		public DateTime? Survey_time { get; set; }

		/// <summary>
		/// 勘测人员
		/// </summary>
		[JsonProperty, Column(Name = "surveyor", DbType = "varchar(64)")]
		public string Surveyor { get; set; } = string.Empty;

		/// <summary>
		/// 符号ID
		/// </summary>
		[JsonProperty, Column(Name = "symbol_id")]
		public int? Symbol_id { get; set; }

		[JsonProperty, Column(Name = "turning_points", DbType = "text")]
		public string Turning_points { get; set; } = string.Empty;

		/// <summary>
		/// 线路类型
		/// </summary>
		[JsonProperty, Column(Name = "type", DbType = "varchar(15)")]
		public string Type { get; set; } = string.Empty;

	}

}

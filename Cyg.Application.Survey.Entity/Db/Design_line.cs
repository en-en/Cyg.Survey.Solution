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

	[JsonObject(MemberSerialization.OptIn), Table(Name = "design_line")]
	public partial class Design_line {

		/// <summary>
		/// 勘测线路ID，主键，UUID
		/// </summary>
		[JsonProperty, Column(Name = "id", DbType = "varchar(64)", IsPrimary = true)]
		public string Id { get; set; } = string.Empty;

		/// <summary>
		/// 方位角
		/// </summary>
		[JsonProperty, Column(Name = "azimuth")]
		public double? Azimuth { get; set; }

		/// <summary>
		/// 电缆回数
		/// </summary>
		[JsonProperty, Column(Name = "cable_number")]
		public int? Cable_number { get; set; }

		/// <summary>
		/// 终节点
		/// </summary>
		[JsonProperty, Column(Name = "end_id", DbType = "varchar(64)")]
		public string End_id { get; set; } = string.Empty;

		/// <summary>
		/// 终节点类型
		/// </summary>
		[JsonProperty, Column(Name = "end_node_type")]
		public int? End_node_type { get; set; }

		/// <summary>
		/// 几何列，类型为线
		/// </summary>
		[JsonProperty, Column(Name = "geom")]
		public PostgisGeometry Geom { get; set; }

		/// <summary>
		/// 线路组编号
		/// </summary>
		[JsonProperty, Column(Name = "group_id", DbType = "varchar(64)")]
		public string Group_id { get; set; } = string.Empty;

		/// <summary>
		/// 线路索引
		/// </summary>
		[JsonProperty, Column(Name = "index")]
		public short? Index { get; set; }

		/// <summary>
		/// 是否电缆，0：否；1：是
		/// </summary>
		[JsonProperty, Column(Name = "is_cable")]
		public bool? Is_cable { get; set; }

		/// <summary>
		/// 电压等级，枚举
		/// </summary>
		[JsonProperty, Column(Name = "kv_level")]
		public int? Kv_level { get; set; }

		/// <summary>
		/// 线路长度
		/// </summary>
		[JsonProperty, Column(Name = "length")]
		public double? Length { get; set; }

		[JsonProperty, Column(Name = "loop_direction")]
		public int? Loop_direction { get; set; }

		/// <summary>
		/// 回路层级：0：主干道 1：一级分支 2：二级分支等
		/// </summary>
		[JsonProperty, Column(Name = "loop_level")]
		public int? Loop_level { get; set; }

		/// <summary>
		/// 回路名称
		/// </summary>
		[JsonProperty, Column(Name = "loop_name")]
		public string Loop_name { get; set; } = string.Empty;

		/// <summary>
		/// 标示线路段在回路中的逻辑顺序
		/// </summary>
		[JsonProperty, Column(Name = "loop_seq")]
		public int? Loop_seq { get; set; }

		/// <summary>
		/// 回路序列号
		/// </summary>
		[JsonProperty, Column(Name = "loop_serial")]
		public int? Loop_serial { get; set; }

		/// <summary>
		/// 线路型号，枚举
		/// </summary>
		[JsonProperty, Column(Name = "mode")]
		public string Mode { get; set; } = string.Empty;

		/// <summary>
		/// 型号ID
		/// </summary>
		[JsonProperty, Column(Name = "mode_id", DbType = "varchar(64)")]
		public string Mode_id { get; set; } = string.Empty;

		/// <summary>
		/// 线路名称
		/// </summary>
		[JsonProperty, Column(Name = "name")]
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// 父级回路名称
		/// </summary>
		[JsonProperty, Column(Name = "parent_loop_name")]
		public string Parent_loop_name { get; set; } = string.Empty;

		/// <summary>
		/// 所属工程ID
		/// </summary>
		[JsonProperty, Column(Name = "project_id", DbType = "varchar(64)")]
		public string Project_id { get; set; } = string.Empty;

		/// <summary>
		/// 备注
		/// </summary>
		[JsonProperty, Column(Name = "remark")]
		public string Remark { get; set; } = string.Empty;

		/// <summary>
		/// 始节点
		/// </summary>
		[JsonProperty, Column(Name = "start_id", DbType = "varchar(64)")]
		public string Start_id { get; set; } = string.Empty;

		/// <summary>
		/// 始节点类型
		/// </summary>
		[JsonProperty, Column(Name = "start_node_type")]
		public int? Start_node_type { get; set; }

		/// <summary>
		/// 状态：1-新建，2-利旧，3-原有，4-换装。枚举。在勘测表中，默认写入 新建
		/// </summary>
		[JsonProperty, Column(Name = "state")]
		public int? State { get; set; }

		/// <summary>
		/// 勘测时间
		/// </summary>
		[JsonProperty, Column(Name = "survey_time")]
		public DateTime? Survey_time { get; set; }

		/// <summary>
		/// 勘测人ID
		/// </summary>
		[JsonProperty, Column(Name = "surveyor", DbType = "varchar(64)")]
		public string Surveyor { get; set; } = string.Empty;

		/// <summary>
		/// 符号ID
		/// </summary>
		[JsonProperty, Column(Name = "symbol_id")]
		public short? Symbol_id { get; set; }

		/// <summary>
		/// 折点
		/// </summary>
		[JsonProperty, Column(Name = "turning_points", DbType = "text")]
		public string Turning_points { get; set; } = string.Empty;

		/// <summary>
		/// 线路类型，预留
		/// </summary>
		[JsonProperty, Column(Name = "type")]
		public string Type { get; set; } = string.Empty;

	}

}
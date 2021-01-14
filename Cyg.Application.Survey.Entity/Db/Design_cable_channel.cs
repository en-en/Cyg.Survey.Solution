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

	[JsonObject(MemberSerialization.OptIn), Table(Name = "design_cable_channel")]
	public partial class Design_cable_channel {

		/// <summary>
		/// 勘测电缆井ID，主键，UUID
		/// </summary>
		[JsonProperty, Column(Name = "id", DbType = "varchar(64)", IsPrimary = true)]
		public string Id { get; set; } = string.Empty;

		/// <summary>
		/// 排列方式
		/// </summary>
		[JsonProperty, Column(Name = "arrangement", DbType = "varchar(15)")]
		public string Arrangement { get; set; } = string.Empty;

		/// <summary>
		/// 电缆通道编号
		/// </summary>
		[JsonProperty, Column(Name = "code")]
		public string Code { get; set; } = string.Empty;

		/// <summary>
		/// 电缆管材质ID
		/// </summary>
		[JsonProperty, Column(Name = "duct_id", DbType = "varchar(64)")]
		public string Duct_id { get; set; } = string.Empty;

		/// <summary>
		/// 电缆管材质型号
		/// </summary>
		[JsonProperty, Column(Name = "duct_spec", DbType = "varchar(64)")]
		public string Duct_spec { get; set; } = string.Empty;

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
		/// 是否交底
		/// </summary>
		[JsonProperty, Column(Name = "is_disclosure")]
		public bool? Is_disclosure { get; set; }

		/// <summary>
		/// 敷设方式：无、排管、直埋、电缆沟、隧道
		/// </summary>
		[JsonProperty, Column(Name = "lay_mode")]
		public int? Lay_mode { get; set; }

		/// <summary>
		/// 长度
		/// </summary>
		[JsonProperty, Column(Name = "length")]
		public double? Length { get; set; }

		/// <summary>
		/// 电缆通道型号
		/// </summary>
		[JsonProperty, Column(Name = "mode")]
		public string Mode { get; set; } = string.Empty;

		/// <summary>
		/// 电缆通道类型ID
		/// </summary>
		[JsonProperty, Column(Name = "mode_id", DbType = "varchar(64)")]
		public string Mode_id { get; set; } = string.Empty;

		/// <summary>
		/// 工程编号
		/// </summary>
		[JsonProperty, Column(Name = "project_id", DbType = "varchar(64)")]
		public string Project_id { get; set; } = string.Empty;

		/// <summary>
		/// 备注
		/// </summary>
		[JsonProperty, Column(Name = "remark")]
		public string Remark { get; set; } = string.Empty;

		/// <summary>
		/// 预留宽度
		/// </summary>
		[JsonProperty, Column(Name = "rsv_width")]
		public double? Rsv_width { get; set; }

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
		/// 通道状态：1-新建，2-利旧，3-原有，4-换装。枚举。在勘测表中，默认写入 新建
		/// </summary>
		[JsonProperty, Column(Name = "state")]
		public int? State { get; set; }

		/// <summary>
		/// 勘测时间
		/// </summary>
		[JsonProperty, Column(Name = "survey_time")]
		public DateTime? Survey_time { get; set; }

		/// <summary>
		/// 勘测人员ID
		/// </summary>
		[JsonProperty, Column(Name = "surveyor", DbType = "varchar(64)")]
		public string Surveyor { get; set; } = string.Empty;

		/// <summary>
		/// 符号ID
		/// </summary>
		[JsonProperty, Column(Name = "symbol_id")]
		public short? Symbol_id { get; set; }

		/// <summary>
		/// 电压等级
		/// </summary>
		[JsonProperty, Column(Name = "voltage", DbType = "varchar(15)")]
		public string Voltage { get; set; } = string.Empty;

	}

}

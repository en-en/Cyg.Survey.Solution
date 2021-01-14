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

	[JsonObject(MemberSerialization.OptIn), Table(Name = "design_electric_meter")]
	public partial class Design_electric_meter {

		/// <summary>
		/// 户表ID，主键，UUID
		/// </summary>
		[JsonProperty, Column(Name = "id", DbType = "varchar(64)", IsPrimary = true)]
		public string Id { get; set; } = string.Empty;

		/// <summary>
		/// 方位角，两侧线的角平分线上
		/// </summary>
		[JsonProperty, Column(Name = "azimuth")]
		public double? Azimuth { get; set; }

		/// <summary>
		/// 表数
		/// </summary>
		[JsonProperty, Column(Name = "count")]
		public int? Count { get; set; }

		/// <summary>
		/// 几何列，类型为点
		/// </summary>
		[JsonProperty, Column(Name = "geom")]
		public PostgisGeometry Geom { get; set; }

		/// <summary>
		/// 电压等级，枚举
		/// </summary>
		[JsonProperty, Column(Name = "kv_level")]
		public int Kv_level { get; set; }

		/// <summary>
		/// 纬度
		/// </summary>
		[JsonProperty, Column(Name = "lat")]
		public double Lat { get; set; }

		/// <summary>
		/// 经度
		/// </summary>
		[JsonProperty, Column(Name = "lon")]
		public double Lon { get; set; }

		/// <summary>
		/// 型号
		/// </summary>
		[JsonProperty, Column(Name = "mode", DbType = "varchar(64)")]
		public string Mode { get; set; } = string.Empty;

		/// <summary>
		/// 型号ID
		/// </summary>
		[JsonProperty, Column(Name = "mode_id", DbType = "varchar(64)")]
		public string Mode_id { get; set; } = string.Empty;

		/// <summary>
		/// 名称
		/// </summary>
		[JsonProperty, Column(Name = "name")]
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
		/// 表位
		/// </summary>
		[JsonProperty, Column(Name = "total_count")]
		public int? Total_count { get; set; }

		/// <summary>
		/// 户表类型：单相表箱、户表、集中表箱、三相表箱。枚举
		/// </summary>
		[JsonProperty, Column(Name = "type")]
		public int? Type { get; set; }

	}

}

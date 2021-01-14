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

	[JsonObject(MemberSerialization.OptIn), Table(Name = "dismantle_mark")]
	public partial class Dismantle_mark {

		/// <summary>
		/// 勘测地物ID，主键，UUID
		/// </summary>
		[JsonProperty, Column(Name = "id", DbType = "varchar(64)", IsPrimary = true)]
		public string Id { get; set; } = string.Empty;

		/// <summary>
		/// 方位角
		/// </summary>
		[JsonProperty, Column(Name = "azimuth", DbType = "varchar(30)")]
		public string Azimuth { get; set; } = string.Empty;

		/// <summary>
		/// 楼层数（m）
		/// </summary>
		[JsonProperty, Column(Name = "floors", DbType = "varchar(10)")]
		public string Floors { get; set; } = string.Empty;

		/// <summary>
		/// 几何列，类型为点
		/// </summary>
		[JsonProperty, Column(Name = "geom")]
		public PostgisGeometry Geom { get; set; }

		/// <summary>
		/// 高
		/// </summary>
		[JsonProperty, Column(Name = "height", DbType = "varchar(30)")]
		public string Height { get; set; } = string.Empty;

		/// <summary>
		/// 纬度
		/// </summary>
		[JsonProperty, Column(Name = "lat")]
		public double Lat { get; set; }

		/// <summary>
		/// 电力线电压等级：无、110kV及以上、35kV、10kV、0.4kV及以下。枚举
		/// </summary>
		[JsonProperty, Column(Name = "line_kv_level")]
		public int? Line_kv_level { get; set; }

		/// <summary>
		/// 带电跨越
		/// </summary>
		[JsonProperty, Column(Name = "live_cross", DbType = "varchar(64)")]
		public string Live_cross { get; set; } = string.Empty;

		/// <summary>
		/// 经度
		/// </summary>
		[JsonProperty, Column(Name = "lon")]
		public double Lon { get; set; }

		/// <summary>
		/// 地物名称
		/// </summary>
		[JsonProperty, Column(Name = "name")]
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// 所属工程ID
		/// </summary>
		[JsonProperty, Column(Name = "project_id", DbType = "varchar(64)")]
		public string Project_id { get; set; } = string.Empty;

		/// <summary>
		/// 运营商
		/// </summary>
		[JsonProperty, Column(Name = "provider")]
		public string Provider { get; set; } = string.Empty;

		/// <summary>
		/// 备注
		/// </summary>
		[JsonProperty, Column(Name = "remark")]
		public string Remark { get; set; } = string.Empty;

		/// <summary>
		/// 道路等级：无、一级公路、二级公路、三级公路、省道、县道、普通硬化道路、机耕道路。枚举
		/// </summary>
		[JsonProperty, Column(Name = "road_level")]
		public int? Road_level { get; set; }

		/// <summary>
		/// 状态：1-新建，2-利旧，3-原有，4-换装。枚举。在勘测表中，写入的数据只能是 原有
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
		/// 类型：无、道路、河沟、湖塘、房屋、经济作物、电力线、通讯线、其它。枚举
		/// </summary>
		[JsonProperty, Column(Name = "type")]
		public int Type { get; set; }

		/// <summary>
		/// 宽
		/// </summary>
		[JsonProperty, Column(Name = "width", DbType = "varchar(30)")]
		public string Width { get; set; } = string.Empty;

	}

}

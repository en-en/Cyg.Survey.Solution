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

	[JsonObject(MemberSerialization.OptIn), Table(Name = "survey_material_info")]
	public partial class Survey_material_info {

		/// <summary>
		/// 自增ID
		/// </summary>
		[JsonProperty, Column(Name = "id", DbType = "varchar(64)", IsPrimary = true)]
		public string Id { get; set; } = string.Empty;

		/// <summary>
		/// 组件ID
		/// </summary>
		[JsonProperty, Column(Name = "component_id", DbType = "varchar(64)")]
		public string Component_id { get; set; } = string.Empty;

		/// <summary>
		/// 所属设备ID
		/// </summary>
		[JsonProperty, Column(Name = "device_id", DbType = "varchar(64)")]
		public string Device_id { get; set; } = string.Empty;

		/// <summary>
		/// 所属设备类型：杆塔、电缆井、线路、地物、拉线、变压器、门架
		/// </summary>
		[JsonProperty, Column(Name = "device_type")]
		public string Device_type { get; set; } = string.Empty;

		/// <summary>
		/// 物料ID
		/// </summary>
		[JsonProperty, Column(Name = "material_id", DbType = "varchar(64)")]
		public string Material_id { get; set; } = string.Empty;

		/// <summary>
		/// 数量
		/// </summary>
		[JsonProperty, Column(Name = "number")]
		public int? Number { get; set; }

		/// <summary>
		/// 操作类型：0=删除、1=增加、2=修改
		/// </summary>
		[JsonProperty, Column(Name = "operation_type")]
		public int? Operation_type { get; set; }

		/// <summary>
		/// 所属部件：主杆，横担，绝缘子，耐张线夹，接续线夹，拉线，基础，防雷，杆上设备，接地，其他。枚举
		/// </summary>
		[JsonProperty, Column(Name = "part")]
		public int Part { get; set; }

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

	}

}

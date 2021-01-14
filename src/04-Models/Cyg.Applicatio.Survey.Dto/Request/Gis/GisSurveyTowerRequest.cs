using Cyg.Applicatio.Dto;
using Cyg.Resource.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cyg.Applicatio.Survey.Dto
{
    public class GisSurveyTowerRequest
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DisplayName("主键")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string Id { get; set; }

        /// <summary>
        /// 上一节点
        /// </summary>
        public string ParentId { get; set; }


        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 杆塔逻辑编号，即回路逻辑序号
        /// </summary>
        public string LogicCode { get; set; }

        /// <summary>
        /// 杆型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 杆高
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 杆梢径
        /// </summary>
        public double Rod { get; set; }

        /// <summary>
        /// 杆材质
        /// </summary>
        public string Material { get; set; }

        /// <summary>
        /// 分段方式
        /// </summary>
        [DefineEnum(typeof(SegmentMode))]
        public SegmentMode Segment { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Lon { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DefineEnum(typeof(SurveyState))]
        public SurveyState State { get; set; }

        /// <summary>
        /// 排列方式
        /// </summary>
        [DefineEnum(typeof(Arrangement))]
        public Arrangement Sort { get; set; }

        /// <summary>
        /// 电压等级
        /// </summary>
        [DefineEnum(typeof(KVLevel))]
        public KVLevel KvLevel { get; set; }

        /// <summary>
        /// 勘测时间
        /// </summary>
        public DateTime? SurveyTime { get; set; }

        /// <summary>
        /// 方位角
        /// </summary>
        public double Azimuth { get; set; }

        /// <summary>
        /// 杆型方案
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 杆塔所属的回路名称
        /// </summary>
        public string LoopName { get; set; }

        /// <summary>
        /// 杆塔所属的回路层级
        /// </summary>
        public string LoopLevel { get; set; }

        /// <summary>
        /// 是否耐张杆
        /// </summary>
        public short IsTension { get; set; }

        /// <summary>
        /// 埋深
        /// </summary>
        public double Depth { get; set; }

        /// <summary>
        /// 杆型方案编号
        /// </summary>
        public string ModeId { get; set; }

        /// <summary>
        /// 杆型编号
        /// </summary>
        public string PoleTypeCode { get; set; }

        /// <summary>
        /// 上一节点类型
        /// </summary>
        [DefineEnum(typeof(NodeType))]
        public NodeType PreNodeType { get; set; }

        /// <summary>
        /// 勘测人员
        /// </summary>
        public string Surveyor { get; set; }

        /// <summary>
        /// 符号Id
        /// </summary>
        public short SymbolId { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        [DisplayName("项目编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string ProjectId { get; set; }

        /// <summary>
        /// 带电作业
        /// </summary>
        public bool ElectrifiedWork { get; set; }

        /// <summary>
        /// 回路数量
        /// </summary>
        public string LoopNum { get; set; }

        /// <summary>
        /// 联动回路数
        /// </summary>
        public string LinkNum { get; set; }

        /// <summary>
        /// 几何列，类型为点
        /// </summary>
        public string Geom { get; set; }
    }
}

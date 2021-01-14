namespace Cyg.Applicatio.Survey.Dto
{
    public class ProjectDetailResponse
    {
        /// <summary>
        /// 工程基本信息
        /// </summary>
        public ProjectResponse Project { get; set; }

        /// <summary>
        /// 勘察数据
        /// </summary>
        public CollectionDataResponse SurveyData { get; set; }

        /// <summary>
        /// 设计数据
        /// </summary>
        public CollectionDataResponse DesignData { get; set; }

        /// <summary>
        /// 交底数据
        /// </summary>
        public DisclosureDataResponse DisclosureData { get; set; }
    }
}

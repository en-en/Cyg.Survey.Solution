
using Cyg.Applicatio.Entity;
using Cyg.Applicatio.Survey.Dto;
using Cyg.Applicatio.Survey.Dto.Request.Gis;
using Cyg.Extensions.Service;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Repository.Impl
{
    /// <summary>
    /// MarialDb数据转存postgis
    /// </summary>
    public class GisTransformRepository : BaseRepository, IGisTransformRepository
    {
        public readonly string PostgisConnectionString;

        public GisTransformRepository()
        {
            PostgisConnectionString = Ioc.GetService<IConfiguration>().GetSection("ConnectionStrings").GetValue<string>("postgis");
        }

        /// <summary>
        /// 按项目MarialDb数据转存postgis
        /// </summary>
        /// <param name="projectIds"></param>
        /// <returns></returns>
        public async Task GisTransformProjectAsync(List<string> projectIds)
        {
            var query = new SqlBuilder();
            #region Gis待转换数据查询sql

            #region 勘察
            //轨迹
            query.AddSql("SELECT tr.ID Id,tr.RecordTime RecordDate,u.ID Recorder,u.CompanyID Company,tr.RecordType Type,tr.ProjectID ProjectId,CONCAT('POINT(',tr.Lon_WGS84,' ',tr.Lat_WGS84,')') Geom FROM pdd_t_trackrecord tr LEFT JOIN pdd_t_project_allot_user pau ON pau.ProjectId=tr.ProjectID AND pau.ArrangeType=1 LEFT JOIN pdd_t_users u ON pau.UserId=u.ID WHERE tr.RecordType=1 AND tr.ProjectID IN @ProjectIds;");
            //电缆井
            query.AddSql("SELECT c.ID Id,c.ParentID ParentId,ce.SymbolId,c.`Code` `Code`,c.C_Type Type,c.State State,c.Remark Remark,c.Surveyor Surveyor,c.SurveyTime SurveyTime,c.C_LayMode LayMode,c.Lat_WGS84 Lat,c.Lon_WGS84 Lon,c.ProjectID ProjectId,c.PreNodeType PreNodeType,c.ElectrifiedWork ElectrifiedWork,NULL LogicCode,NULL `Mode`,NULL ModeId,ce.SymbolId,NULL Azimuth,CONCAT('POINT(',c.Lon_WGS84,' ',c.Lat_WGS84,')') Geom FROM pdd_t_survey_cable c LEFT JOIN pdd_t_survey_cable_ext ce ON ce.ID=c.ID WHERE c.ProjectID IN @ProjectIds;");
            //电缆通道
            query.AddSql("SELECT cc.ID Id,cc.`Code` `Code`,cc.Start_ID StartId,cc.End_ID EndId,cc.C_LayMode LayMode,cc.State State,cc.DuctID DuctId,cc.DuctSpec DuctSpec,cc.C_Mode `Mode`,cc.C_ModeID ModeId,cc.Remark Remark,cc.StartNodeType StartNodeType,cc.EndNodeType EndNodeType,cc.Surveyor Surveyor,cc.SurveyTime SurveyTime,NULL Arrangement,NULL SymbolId,cc.Length Length,NULL RsvWidth,cc.ProjectID ProjectId,NULL Voltage,NULL IsDisclosure,CONCAT('LINESTRING(',IFNULL(t_start.Lon_WGS84,0)+IFNULL(c_start.Lon_WGS84,0)+IFNULL(cd_start.Lon_WGS84,0),' ',IFNULL(t_start.Lat_WGS84,0)+IFNULL(c_start.Lat_WGS84,0)+IFNULL(cd_start.Lat_WGS84,0),',',IFNULL(t_end.Lon_WGS84,0)+IFNULL(c_end.Lon_WGS84,0)+IFNULL(cd_end.Lon_WGS84,0),' ',IFNULL(t_end.Lat_WGS84,0)+IFNULL(c_end.Lat_WGS84,0)+IFNULL(cd_end.Lat_WGS84,0),')') Geom FROM pdd_t_survey_cablechannel cc LEFT JOIN pdd_t_survey_tower t_start ON t_start.ID=cc.Start_ID AND cc.StartNodeType=1 LEFT JOIN pdd_t_survey_tower t_end ON t_end.ID=cc.End_ID AND cc.EndNodeType=1 LEFT JOIN pdd_t_survey_cable c_start ON c_start.ID=cc.Start_ID AND cc.StartNodeType=2 LEFT JOIN pdd_t_survey_cable c_end ON c_end.ID=cc.End_ID AND cc.EndNodeType=2 LEFT JOIN pdd_t_survey_cabledevice cd_start ON cd_start.ID=cc.Start_ID AND cc.StartNodeType=3 LEFT JOIN pdd_t_survey_cabledevice cd_end ON cd_end.ID=cc.End_ID AND cc.EndNodeType=3 WHERE cc.Start_ID IS NOT NULL AND cc.StartNodeType IN (1,2,3) AND cc.End_ID IS NOT NULL AND cc.EndNodeType IN (1,2,3) AND cc.ProjectID IN @ProjectIds;");
            //电缆设备
            query.AddSql("SELECT cd.ID Id,ext.SymbolId,cd.`Name` `Name`,cd.Type Type,cd.ParentId ParentId,cd.EquipModel EquipModel,NULL EquipModelID,NULL Capacity,NULL Length,NULL Width,NULL Height,cd.State State,NULL SymbolId,cd.Remark Remark,cd.PreNodeType PreNodeType,cd.`Code` `Code`,cd.ProjectID ProjectId,cd.Surveyor Surveyor,cd.SurveyTime SurveyTime,NULL Azimuth,NULL LogicCode,CONCAT('POINT(',cd.Lon_WGS84,' ',cd.Lat_WGS84,')') Geom FROM pdd_t_survey_cabledevice cd LEFT JOIN pdd_t_survey_cabledevice_ext ext  on cd.ID=ext.ID WHERE cd.ProjectID IN @ProjectIds;");
            //线路
            query.AddSql("SELECT l.ID Id,l.`Name` `Name`,l.GroupId GroupId,l.Start_ID StartId,l.End_ID EndId,l.L_Type Type,l.L_Mode `Mode`,l.Length Length,l.KVLevel KvLevel,l.LoopSerial LoopSerial,l.State State,l.IsCable IsCable,l.ProjectID ProjectId,l.Surveyor Surveyor,l.SurveyTime SurveyTime,l.Remark Remark,l.StartNodeType StartNodeType,l.EndNodeType EndNodeType,l.LoopName LoopName,le.L_ModeID ModeID,NULL CableNumber,NULL Azimuth,NULL LoopSeq,NULL LoopLevel,NULL ParentLoopName,NULL TurningPoints,CONCAT('LINESTRING(',IFNULL(t_start.Lon_WGS84,0)+IFNULL(c_start.Lon_WGS84,0)+IFNULL(cd_start.Lon_WGS84,0),' ',IFNULL(t_start.Lat_WGS84,0)+IFNULL(c_start.Lat_WGS84,0)+IFNULL(cd_start.Lat_WGS84,0),',',IFNULL(t_end.Lon_WGS84,0)+IFNULL(c_end.Lon_WGS84,0)+IFNULL(cd_end.Lon_WGS84,0),' ',IFNULL(t_end.Lat_WGS84,0)+IFNULL(c_end.Lat_WGS84,0)+IFNULL(cd_end.Lat_WGS84,0),')') Geom,le.SymbolId FROM pdd_t_survey_line l LEFT JOIN pdd_t_survey_line_ext le ON le.Id=l.ID LEFT JOIN pdd_t_survey_tower t_start ON t_start.ID=l.Start_ID AND l.StartNodeType=1 LEFT JOIN pdd_t_survey_tower t_end ON t_end.ID=l.End_ID AND l.EndNodeType=1 LEFT JOIN pdd_t_survey_cable c_start ON c_start.ID=l.Start_ID AND l.StartNodeType=2 LEFT JOIN pdd_t_survey_cable c_end ON c_end.ID=l.End_ID AND l.EndNodeType=2 LEFT JOIN pdd_t_survey_cabledevice cd_start ON cd_start.ID=l.Start_ID AND l.StartNodeType=3 LEFT JOIN pdd_t_survey_cabledevice cd_end ON cd_end.ID=l.End_ID AND l.EndNodeType=3 WHERE l.Start_ID IS NOT NULL AND l.StartNodeType IN (1,2,3) AND l.End_ID IS NOT NULL AND l.EndNodeType IN (1,2,3) AND l.ProjectID IN @ProjectIds;");
            //地物
            query.AddSql("SELECT m.ID Id,m.`Name` `Name`,m.Type Type,m.Width Width,m.Height Height,NULL Azimuth,m.Lat_WGS84 Lat,m.Lon_WGS84 Lon,m.RoadLevel RoadLevel,m.LineKvLevel LineKvLevel,m.Floors Floors,m.Provider Provider,m.ProjectID ProjectId,m.State State,m.Surveyor Surveyor,m.SurveyTime SurveyTime,m.Remark Remark,NULL LiveCross,CONCAT('POINT(',m.Lon_WGS84,' ',m.Lat_WGS84,')') Geom FROM pdd_t_survey_mark m WHERE m.ProjectID IN @ProjectIds;");
            //架空杆上设备
            query.AddSql(@"SELECT ohd.ID Id,ohdext.SymbolId,ohd.`Name` `Name`,ohd.Type Type,ohd.Main_ID MainId,ohd.Sub_ID SubId,ohd.Capacity Capacity,ohd.FixMode FixMode,ohd.State State,ohd.Lat_WGS84 Lat,ohd.Lon_WGS84 Lon,ohd.ProjectID ProjectId,ohd.Surveyor Surveyor,ohd.SurveyTime SurveyTime,ohd.Remark Remark,NULL `Mode`,NULL ModeId,NULL Azimuth,CONCAT('POINT(',ohd.Lon_WGS84,' ',ohd.Lat_WGS84,')') Geom FROM pdd_t_survey_overheaddevice ohd LEFT JOIN pdd_t_survey_overheaddevice_ext  ohdext
on ohd.ID = ohdext.ID WHERE ohd.ProjectID IN @ProjectIds;");
            //杆塔
            query.AddSql("SELECT t.ID Id,t.ParentID ParentId,ext.SymbolId,t.`Code` `Code`,NULL LogicCode,t.T_Type Type,t.T_Height Height,t.T_Rod Rod,t.T_Material Material,t.T_Segment Segment,t.Lat_WGS84 Lat,t.Lon_WGS84 Lon,t.State State,t.T_Sort Sort,t.KVLevel KvLevel,t.SurveyTime SurveyTime,NULL Azimuth,t.T_Mode `Mode`,t.Remark Remark,NULL LoopName,NULL LoopLevel,NULL IsTension,NULL Depth,NULL ModeId,NULL PoleTypeCode,t.PreNodeType PreNodeType,t.Surveyor Surveyor,NULL SymbolId,t.ProjectID ProjectId,t.ElectrifiedWork ElectrifiedWork,NULL LoopNum,NULL LinkNum,CONCAT('POINT(',t.Lon_WGS84,' ',t.Lat_WGS84,')') Geom FROM pdd_t_survey_tower t LEFT JOIN pdd_t_survey_tower_ext ext on t.ID=ext.ID WHERE t.ProjectID IN @ProjectIds;");
            //设计拉线
            query.AddSql(@"SELECT
                              	pullLine.ID Id,
                              	pullLine.Main_Id,
                              	pullLine.Azimuth,
                              	pullLine.D_Mode MODE,
                              	pullLine.Remark,
                              	pullLine.D_ModeID mode_id,
                              	pullLine.State,
                              	pullLine.ProjectId,
                                CONCAT(
                              		'POINT(',
                              		pullLine.Lon_WGS84,
                              		' ',
                              		pullLine.Lat_WGS84,
                              		')'
                              	) Geom
                              FROM
                              	pdd_t_survey_pullline pullLine 
                              WHERE pullLine.ProjectID IN @ProjectIds;");

            //拆除拉线
            query.AddSql(@"SELECT
                              	pullLine.ID Id,
                              	pullLine.Main_Id,
                              	pullLine.Azimuth,
                              	pullLine.D_Mode MODE,
                              	pullLine.Remark,
                              	pullLine.D_ModeID mode_id,
                              	pullLine.State,
                              	pullLine.ProjectId,
                                CONCAT(
                              		'POINT(',
                              		pullLine.Lon_WGS84,
                              		' ',
                              		pullLine.Lat_WGS84,
                              		')'
                              	) Geom
                              FROM
                              	pdd_t_dismantle_pullline pullLine 
                               WHERE pullLine.ProjectID IN @ProjectIds;");   
            #endregion

            #endregion
            query.AddParam("ProjectIds", projectIds);
            using (var reader = await DbHelper.QueryMultipleAsync(query))
            {

                #region 勘察
                var trackList = (await reader.ReadAsync<GisSurveyTrackRequest>()).Where(x=>x.Recorder==CurrentUser.Id);
                var surveyCableList = await reader.ReadAsync<GisSurveyCableRequest>();
                var surveyChannelList = await reader.ReadAsync<GisSurveyCableChannelRequest>();
                var surveyCableDeviceList = await reader.ReadAsync<GisSurveyCableEquipmentRequest>();
                var surveyLineList = await reader.ReadAsync<GisSurveyLineRequest>();
                var surveyMarkList = await reader.ReadAsync<GisSurveyMarkRequest>();
                var surveyOverheadDeviceList = await reader.ReadAsync<GisSurveyOverheadDeviceRequest>();
                var surveyTowerList = await reader.ReadAsync<GisSurveyTowerRequest>();
                var designPullLineList = await reader.ReadAsync<GisPullLineRequest>();
                var dismantlePullLineList = await reader.ReadAsync<GisPullLineRequest>();
                #endregion

                using (IDbConnection connection = new NpgsqlConnection(PostgisConnectionString))
                {
                    connection.Open();
                        using (var tran = connection.BeginTransaction())
                        {
                            try
                            {
                                var sql = string.Empty;
                                #region 删除旧数据
                                //删除勘察数据
                                sql += "delete from survey_track t where t.project_id=any(@ProjectIds);";
                                sql += "delete from survey_cable c where c.project_id=any(@ProjectIds);";
                                sql += "delete from survey_cable_channel cc where cc.project_id=any(@ProjectIds);";
                                sql += "delete from survey_cable_equipment ce where ce.project_id=any(@ProjectIds);";
                                sql += "delete from survey_line l where l.project_id=any(@ProjectIds);";
                                sql += "delete from survey_mark m where m.project_id=any(@ProjectIds);";
                                sql += "delete from survey_over_head_device ohd where ohd.project_id=any(@ProjectIds);";
                                sql += "delete from survey_tower t where t.project_id=any(@ProjectIds);";
                                sql += "delete from design_pull_line t where t.project_id=any(@ProjectIds);";
                                sql += "delete from dismantle_pull_line t where t.project_id=any(@ProjectIds);";
                                var rtn= await connection.ExecuteAsync(sql, new { ProjectIds = projectIds }, transaction: tran);
                                #endregion


                                #region 勘察数据
                                //勘察轨迹
                                sql = "insert into survey_track(id, record_date, recorder, company, type, project_id, geom) values (@id, @RecordDate, @Recorder, @Company, @Type, @ProjectId,ST_GeomFromText(@Geom));";
                                await connection.ExecuteAsync(sql, trackList);
                                //电缆井
                                sql = "insert into survey_cable(id, parent_id, code, type, state, remark, surveyor, survey_time, lay_mode, lat, lon, project_id, pre_node_type, electrified_work, logic_code, mode, mode_id, symbol_id, azimuth, geom) values (@Id,@ParentId,@Code,@Type,@State,@Remark,@surveyor,@SurveyTime,@LayMode,@Lat,@Lon,@ProjectId,@PreNodeType,@ElectrifiedWork,@LogicCode,@Mode,@ModeId,@SymbolId,@Azimuth,ST_GeomFromText(@Geom));";
                                await connection.ExecuteAsync(sql, surveyCableList);
                                //电缆通道
                                sql = "insert into survey_cable_channel(id, code, start_id, end_id, lay_mode, state, duct_id, duct_spec, mode, mode_id, remark, start_node_type, end_node_type, surveyor, survey_time, arrangement, symbol_id, length, rsv_width, project_id, voltage, is_disclosure, geom) values(@id,@code,@startid,@endid,@laymode,@state,@ductid,@ductspec,@mode,@modeid,@remark,@startnodetype,@endnodetype,@surveyor,@surveytime,@arrangement,@SymbolId,@length,@rsvwidth,@projectid,@voltage,@isdisclosure,ST_GeomFromText(@Geom));";
                                await connection.ExecuteAsync(sql, surveyChannelList);
                                //电缆设备
                                sql = "insert into survey_cable_equipment(id, name, type, parent_id, equip_model, equip_model_id, capacity, length, width, height, state, symbol_id, remark, pre_node_type, code, project_id, surveyor, survey_time, azimuth, logic_code, geom) values (@id,@name,@type,@parentid,@equipmodel,@equipmodelid,@capacity,@length,@width,@height,@state,@SymbolId,@remark,@prenodetype,@code,@projectid,@surveyor,@surveytime,@azimuth,@logiccode,ST_GeomFromText(@Geom));";
                                await connection.ExecuteAsync(sql, surveyCableDeviceList);
                                //线路
                                sql = "insert into survey_line(id, name, group_id, start_id, end_id, type, mode, length, kv_level, loop_serial, state, is_cable, project_id, surveyor, survey_time, remark, start_node_type, end_node_type, loop_name, mode_id, cable_number, azimuth, loop_seq, loop_level, parent_loop_name, turning_points,index, geom,loop_direction,symbol_id) values (@id,@name,@groupid,@startid,@endid,@type,@mode,@length,@kvlevel,@loopserial,@state,@iscable,@projectid,@surveyor,@surveytime,@remark,@startnodetype,@endnodetype,@loopname,@modeid,@cablenumber,@azimuth,@loopseq,@looplevel,@parentloopname,@turningpoints,@index,ST_GeomFromText(@geom),0,@SymbolId);";
                                await connection.ExecuteAsync(sql, surveyLineList);
                                //地物
                                sql = "insert into survey_mark(id, name, type, width, height, azimuth, lat, lon, road_level, line_kv_level, floors, provider, project_id, state, surveyor, survey_time, remark, live_cross, geom) values (@id,@name,@type,@width,@height,@azimuth,@lat,@lon,@roadlevel,@linekvlevel,@floors,@provider,@projectid,@state,@surveyor,@surveytime,@remark,@livecross,ST_GeomFromText(@geom));";
                                await connection.ExecuteAsync(sql, surveyMarkList);

                                //架空杆上设备
                                sql = "insert into survey_over_head_device(id, name, type, main_id, sub_id, capacity, fix_mode, state, lat, lon, project_id, surveyor, survey_time, remark, mode, mode_id, azimuth, geom,symbol_id) values (@id,@name,@type,@mainid,@subid,@capacity,@fixmode,@state,@lat,@lon,@projectid,@surveyor,@surveytime,@remark,@mode,@modeid,@azimuth,ST_GeomFromText(@geom),@SymbolId);";
                                await connection.ExecuteAsync(sql, surveyOverheadDeviceList);
                                //杆塔4
                                sql = "insert into survey_tower(id, parent_id, code, logic_code, type, height, rod, material, segment, lat, lon, state, sort, kv_level, survey_time, azimuth, mode, remark, loop_name, loop_level, is_tension, depth, mode_id, pole_type_code, pre_node_type, surveyor, symbol_id, project_id, electrified_work, loop_num, link_num, geom) values (@id,@parentid,@code,@logiccode,@type,@height,@rod,@material,@segment,@lat,@lon,@state,@sort,@kvlevel,@surveytime,@azimuth,@mode,@remark,@loopname,@looplevel,@istension,@depth,@modeid,@poletypecode,@prenodetype,@surveyor,@SymbolId,@projectid,@electrifiedwork,@loopnum,@linknum,ST_GeomFromText(@geom));";
                                await connection.ExecuteAsync(sql, surveyTowerList);
                                //设计拉线
                                //designPullLineList.ToList().ForEach(x => x.Geom = "POINT(0 0)");
                                sql = "insert into design_pull_line(id, main_id, azimuth, mode, remark, mode_id, state, project_id,geom) values (@id,@main_id,@azimuth,@mode,@remark,@mode_id,@state,@ProjectId,ST_GeomFromText(@geom));";
                                await connection.ExecuteAsync(sql, designPullLineList);
                                //拆除拉线
                                sql = "insert into dismantle_pull_line(id, main_id, azimuth, mode, remark, mode_id, state, project_id,geom) values (@id,@main_id,@azimuth,@mode,@remark,@mode_id,@state,@ProjectId,ST_GeomFromText(@geom));";
                                //dismantlePullLineList.ToList().ForEach(x => x.Geom = "POINT(0 0)");
                                await connection.ExecuteAsync(sql, dismantlePullLineList);
                                #endregion
                                tran.Commit();
                            }
                            catch (Exception e)
                            {
                                tran.Rollback();
                                tran.Dispose();
                                throw e;
                            }

                        }
                }
            }
            
        }


        /// <summary>
        /// 按项目转换marialdb交底轨迹数据到postgis
        /// </summary>
        /// <param name="projectIds"></param>
        /// <returns></returns>
        public async Task GisTransformDisclosureTrack(List<string> projectIds)
        {
            var query = new SqlBuilder();
            query.AddSql("SELECT tr.ID Id,tr.RecordTime RecordDate,u.ID Recorder,u.CompanyID Company,tr.RecordType Type,tr.ProjectID ProjectId,CONCAT('POINT(',tr.Lon_WGS84,' ',tr.Lat_WGS84,')') Geom FROM pdd_t_trackrecord tr LEFT JOIN pdd_t_project_allot_user pau ON pau.ProjectId=tr.ProjectID AND pau.ArrangeType=1 LEFT JOIN pdd_t_users u ON pau.UserId=u.ID WHERE tr.RecordType=2 AND tr.ProjectID IN @ProjectIds;");
            query.AddParam("ProjectIds",projectIds);
            var trackList = DbHelper.Query<GisDisclosureTrackRequest>(query);

            using (IDbConnection connection = new NpgsqlConnection(PostgisConnectionString))
            {
                connection.Open();
                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = string.Empty;
                        #region 删除旧数据
                        //删除轨迹数据
                        sql += "delete from disclosure_track t where t.project_id=any(@ProjectIds);";
                        await connection.ExecuteAsync(sql, new { ProjectIds = projectIds }, transaction: tran);
                        #endregion

                        #region 轨迹数据
                        //交底轨迹
                        sql = "insert into disclosure_track(id, record_date, recorder, company, type, project_id, geom) values (@id, @RecordDate, @Recorder, @Company, @Type, @ProjectId,ST_GeomFromText(@Geom));";
                        await connection.ExecuteAsync(sql, trackList);
                        #endregion

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }

                }
            }
        }


    }
}

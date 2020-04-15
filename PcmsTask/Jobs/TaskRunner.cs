using PcmsTask.Commons;
using PcmsTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static PcmsTask.Commons.Status;

namespace PcmsTask.Jobs
{
    class TaskRunner
    {
        public void TaskApi()
        {
            // 네트워크드라이브 연결
            NetworkDriveHelper.Connect();

            //CommonUtil.LogWrite("Start PcmsTask");
            //CommonUtil.LogWrite("connection string" + System.Configuration.ConfigurationManager.ConnectionStrings["PUBLISH"].ConnectionString);
            // 디비 연결
            PCMSDBContext db = new PCMSDBContext();
            
            string curHour = DateTime.UtcNow.ToString("HH");
            //CommonUtil.LogWrite("현재시간:" + curHour);
            List<Channel> channels = new List<Channel>();
            try
            {
                channels = db.Channels.ToList();
            }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (Exception e) {
#pragma warning restore CS0168 // The variable 'e' is declared but never used
                //CommonUtil.LogWrite(e.Message);
            }

            //CommonUtil.LogWrite("Channel Count:" + channels.Count());

            foreach (Channel c in channels)
            {
                //CommonUtil.LogWrite("Channel:"+c.name + "=" + c.type);
                // 사용여부
                if (c.usage == false && !c.Instantrun) continue;
                // 입력 시간 -> UTC 화
                int athour = c.athour -9 < 0 ? c.athour - 9 + 24: c.athour - 9;
                if (string.Format("{0:D2}", athour) != curHour && !c.Instantrun) continue;
                if (!c.Instantrun)
                {
                    //CommonUtil.LogWrite("Time Trigger");
                    // 최근실행 내역 비교
                    var bs = db.Batches.Where(b => b.name == c.name && b.bound == c.bound);
                    if (bs.Count() > 0) { 
                        Batch lastrun = bs.OrderByDescending(b => b.ID).First();
                        string cur = DateTime.UtcNow.ToString("yyyyMMddHH");
                        string old = lastrun.createdate.ToString("yyyyMMddHH");
                        if (cur == old) continue;
                    }
                }
                else
                {
                    //CommonUtil.LogWrite("Instant Run");
                    c.Instantrun = false;
                    db.Entry(c).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                // 실행
                if (c.name == @"ODSM" && c.bound == BoundType.Inbound)
                {
                    ODSMInbound bound = new ODSMInbound(c);
                    bound.Execute();
                }
                else if (c.name == @"N360" && c.bound == BoundType.Outbound)
                {
                    N360Outbound bound = new N360Outbound(c);
                    bound.Execute();
                }
                else if (c.name == @"PforceRX" && c.bound == BoundType.Inbound)
                {
                    PforceRXInbound bound = new PforceRXInbound(c);
                    bound.Execute();
                }
                else if (c.name == @"PforceRX" && c.bound == BoundType.Outbound)
                {
                    PforceRXOutbound bound = new PforceRXOutbound(c);
                    bound.Execute();
                }
                else if (c.name == @"MMS" && c.bound == BoundType.Inbound)
                {
                    MMSInbound bound = new MMSInbound(c);
                    bound.Execute();
                }                
            } // for channel

            // 삭제/만료 설정 추가
            try
            {
                if (db.Settings.Where(s => s.type == SettingType.ExpiredBatch).Count() < 1) {
                    Setting s = new Setting();
                    s.name = @"만료배치";
                    s.type = SettingType.ExpiredBatch;
                    s.value = @"N";
                    db.Settings.Add(s);
                    db.SaveChanges();
                }
                if (db.Settings.Where(s => s.type == SettingType.RemoveBatch).Count() < 1)
                {
                    Setting s = new Setting();
                    s.name = @"삭제배치";
                    s.type = SettingType.RemoveBatch;
                    s.value = @"N";
                    db.Settings.Add(s);
                    db.SaveChanges();
                }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            } catch (Exception e) {
#pragma warning restore CS0168 // The variable 'e' is declared but never used

            }
            // 삭제처리
            if (curHour == "00")
            {
                Setting ss = db.Settings.Where(s => s.type == SettingType.RemoveBatch).First();
                if (ss.value != @"N")
                {
                    DeleteTask dt = new DeleteTask();
                    dt.Execute();
                }
            }

            // 기간 만료 하기
            if (curHour == "00")
            {
                Setting ss = db.Settings.Where(s => s.type == SettingType.ExpiredBatch).First();
                if (ss.value != @"N")
                {
                    ExpirationTask et = new ExpirationTask();
                    et.Execute();
                }
            }
            
            NetworkDriveHelper.Disconnect();
            //CommonUtil.LogWrite("PcmsTask End.");
        }

    }
}
/*
INSERT INTO  
*/
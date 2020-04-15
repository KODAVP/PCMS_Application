using System;
using System.ComponentModel.DataAnnotations;

namespace PrivacyConsentDB.Commons
{
    public enum ChannelType { SFTP, API, XFER, NAS, DB }
    public enum BoundType { Inbound, Outbound }
    public enum ActionStatus { Waiting, Running }
    public enum BatchStatus
    {
        [Display(Name = "시작")]
        Begin,
        [Display(Name = "완료")]
        Completed,
        [Display(Name = "오류")]
        Error,
        [Display(Name = "없음")]
        Empty,
        [Display(Name = "실행중")]
        Running,
        [Display(Name = "실행완료")]
        Run
    }
    public enum CollectionStatus
    {
        ERROR = 0,
        FIND,
        DOWNLOAD,
        COMPLETED,
        UPLOAD
    }
    public class Status
    {
        [Flags]
        public enum SendChannel : int
        {
            NONE = 0,
            N360 = 1,
            PFORCERX = 2,
            MMS = 4,
            OTHER1 = 8,
            OTHER2 = 16,
            OTHER3 = 32,
            PFORCERX_ONEKEY_DUP = 64,
            PFORCERX_INVALID = 128
        }
        public enum ApprovalStatus
        {
            [Display(Name = "요청")]
            Request,
            [Display(Name = "반려")]
            Rejected,
            [Display(Name = "승인완료")]
            Approved
        }
        public enum ConsentStatus
        {
            [Display(Name = "동의안함")]
            OptOut,
            [Display(Name = "동의함")]
            OptIn,
            [Display(Name = "만료됨")]
            Expired
        }

        public enum PrivacyStatus
        {
            REGISTED // 등록된 상태
            , IMPORTED // 벌크업로드 또는 N360이 아닌 곳에서 가져온 상태
            , GRANTED // N360을 통해 키가 생성된 상태
            , ERASED // 개인정보 삭제
            , DELETED // 만료되어 삭제됨
            , INACTIVED // 비활성화. 아무 인터페이스로도 돌지 않음
        }

        public enum SettingType
        {
            [Display(Name = "승인 발생시 알림 이메일")]
            NotificationEmail
        , [Display(Name = "동의 유효기간(년)")]
            ConsentTerm
        , [Display(Name = "동의 만료 알림(일)")]
            ConsentExpiredAlert
        , [Display(Name = "화이자링크 알림")]
            PfizerLinkAlert
        , [Display(Name = "만료배치동작")]
            ExpiredBatch
        , [Display(Name = "삭제배치동작")]
            RemoveBatch
        , [Display(Name = "인터페이스 알람 메일")]
            InterfaceAlertMail
        }

    }
}
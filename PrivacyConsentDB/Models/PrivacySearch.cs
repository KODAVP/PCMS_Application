using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrivacyConsentDB.Models
{
    public class PrivacySearch
    {
        [DisplayName("담당자")]
        public string owner { get; set; } // 담당자
        [DisplayName("고객명")]
        public string name { get; set; } // 고객명
        [DisplayName("진료과")]
        public string depart { get; set; } // 진료과
        [DisplayName("근무처")]
        public string wkpname { get; set; } // 병원명
        [DisplayName("PCMS Code")]
        public string PCMS_ID { get; set; } // PCMS Code
        public bool CONSENT_TOTAL { get; set; } // 동의여부 전체
        public bool CONSENT_USE { get; set; }   // 동의여부 수집/이용
        public bool CONSENT_MARKETING_AGREEMENT { get; set; }
        public bool CONSENT_TRUST { get; set; } // 동의여부 위탁
        public bool CONSENT_ABROAD { get; set; }// 동의여부 국외이전

        public string subscribe { get; set; }     // 수신거부 허용

        public string active { get; set; }        // 활성화 여부


        [DisplayName("채널(주)")]
        public string consentsource { get; set; } // 동의 채널 / 채널(주)
        [DisplayName("동의일자")]
        public DateTime? consentbegindt { get; set; }
        [DisplayName("동의일자")]
        public DateTime? consentenddt { get; set; }
        [DisplayName("수집일자")]
        public DateTime? collectbegindt { get; set; }
        [DisplayName("수집일자")]
        public DateTime? collectenddt { get; set; }

        [DisplayName("만료됨")]
        public string EXPIRED { get; set; }


        [DisplayName("승인여부")]
        public string ApprovalStatus { get; set; }

        public List<SelectListItem> aprovalstatusList {
            get {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "", Value = "" });
                items.Add(new SelectListItem { Text = "요청", Value = "0" });
                items.Add(new SelectListItem { Text = "반려", Value = "1" });
                items.Add(new SelectListItem { Text = "승인", Value = "2"});
                return items;
            }
        }

        public int startIndex { get; set; }
        public int pageSize { get; set; }

        public int totalCount { get; set; }

        //public string COMP_CODE { get; set; }

        public PrivacySearch() {
            this.CONSENT_TOTAL = true;
            this.EXPIRED = "total";
            this.subscribe = "total";
            this.active = "total";
            startIndex = 0;
            pageSize = 20;
        }

        public PrivacySearch(PrivacySearch o)
        {
            this.CONSENT_TOTAL = o.CONSENT_TOTAL;
            this.CONSENT_USE = o.CONSENT_USE;
            this.CONSENT_MARKETING_AGREEMENT = o.CONSENT_MARKETING_AGREEMENT;
            this.CONSENT_TRUST = o.CONSENT_TRUST;
            this.CONSENT_ABROAD = o.CONSENT_ABROAD;
            this.owner = o.owner;
            this.name = o.name;
            this.depart = o.depart;
            this.wkpname = o.wkpname;
            this.PCMS_ID = o.PCMS_ID;

            this.subscribe = o.subscribe;
            this.consentsource = o.consentsource;
            this.consentbegindt = o.consentbegindt;
            this.consentenddt = o.consentenddt;
            this.collectbegindt = o.collectbegindt;
            this.collectenddt = o.collectenddt;
            this.startIndex = o.startIndex;
            this.pageSize = o.pageSize;
            this.totalCount = o.totalCount;

            this.EXPIRED = o.EXPIRED;

            this.active = o.active;
            //this.COMP_CODE = o.COMP_CODE;
        }
    }
}
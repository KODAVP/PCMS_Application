using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrivacyConsentDB.Models
{
    public class GetCompanyList
    {
        public static List<SelectListItem> GetCompany()
        {
            GetCompany company = new GetCompany();
            List<SelectListItem> lstCompany = (from obj in company.GetCompanyList().AsEnumerable()
                                               select new SelectListItem
                                               {
                                                   Text = obj.COMP_NAME,
                                                   Value = obj.COMP_CODE
                                               }).ToList();
            lstCompany.Insert(0, new SelectListItem { Text = "Select Company", Value = "" });

            return lstCompany;
        }


    }
}
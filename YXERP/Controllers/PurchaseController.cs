﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using CloudSalesEnum;
using CloudSalesBusiness;
using CloudSalesEntity;

namespace YXERP.Controllers
{
    public class PurchaseController : BaseController
    {
        //
        // GET: /Purchase/

        public ActionResult Index()
        {
            return View("Purchase");
        }
        /// <summary>
        /// 我的采购
        /// </summary>
        /// <returns></returns>
        public ActionResult Purchase()
        {
            ViewBag.Type = (int)EnumDocType.RK;
            ViewBag.Title = "采购入库";
            return View("FilterProducts");
        }

        public ActionResult MyPurchase()
        {
            return View();
        }
        /// <summary>
        /// 我的采购详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DocDetail(string id)
        {
            ViewBag.Model = OrdersBusiness.GetStorageDetail(id, CurrentUser.ClientID);
            return View();
        }

        /// <summary>
        /// 采购单确认页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfirmPurchase()
        {
            ViewBag.Items = OrdersBusiness.GetShoppingCart(EnumDocType.RK, CurrentUser.UserID);
            return View();
        }
        /// <summary>
        /// 采购审核页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AuditDetail(string id)
        {
            ViewBag.Wares = new WarehouseBusiness().GetWareHouses(CurrentUser.ClientID); 
            ViewBag.Model = OrdersBusiness.GetStorageDetail(id, CurrentUser.ClientID);
            return View();
        }

        public ActionResult PurchaseAudit()
        {
            return View();
        }

        #region Ajax

        /// <summary>
        /// 保存采购单
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public JsonResult SubmitPurchase(string doc)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<CloudSalesEntity.StorageDoc>(doc);
            model.DocType = (int)EnumDocType.RK;

            var id = OrdersBusiness.CreateStorageDoc(model, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("ID", id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取我的采购单
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult GetMyPurchase(string keyWords, int pageIndex, int totalCount, int status = -1, bool isAll = false)
        {
            int pageCount = 0;
            List<StorageDoc> list = OrdersBusiness.GetStorageDocList(isAll ? string.Empty : CurrentUser.UserID, EnumDocType.RK, (EnumDocStatus)status, keyWords, PageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        ///  删除单据
        /// </summary>
        /// <param name="docid"></param>
        /// <returns></returns>
        public JsonResult DeletePurchase(string docid)
        {
            var bl = new OrdersBusiness().DeleteDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 作废单据
        /// </summary>
        /// <param name="docid"></param>
        /// <returns></returns>
        public JsonResult InvalidPurchase(string docid)
        {
            var bl = new OrdersBusiness().InvalidDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 更换入库仓库和货位
        /// </summary>
        /// <param name="autoid"></param>
        /// <param name="wareid"></param>
        /// <param name="depotid"></param>
        /// <returns></returns>
        public JsonResult UpdateStorageDetailWare(string autoid, string wareid, string depotid)
        {
            var bl = new OrdersBusiness().UpdateStorageDetailWare(autoid, wareid, depotid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 审核上架
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public JsonResult AuditPurchase(string ids)
        {
            bool bl = new OrdersBusiness().AuditStorageIn(ids, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        } 

        #endregion

    }
}

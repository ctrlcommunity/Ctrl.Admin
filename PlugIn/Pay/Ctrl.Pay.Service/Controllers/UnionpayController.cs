using Ctrl.Plugin.PayCore.Gateways;
using Ctrl.Plugin.PayCore.Response;
using Ctrl.Plugin.Unionpay;
using Ctrl.Plugin.Unionpay.Domain;
using Ctrl.Plugin.Unionpay.Request;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Ctrl.Pay.Service.Controllers
{
    /// <summary>
    ///    银联支付操作接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UnionpayController : Controller
    {
        private readonly IGateway _gateway;
        public UnionpayController(IGateways gateways)
        {
            _gateway = gateways.Get<UnionpayGateway>();
        }
        /// <summary>
        ///     电脑网站支付
        /// </summary>
        /// <param name="order_id">订单号</param>
        /// <param name="total_amount">金额（单位：分）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WebPay(string order_id, int total_amount)
        {
            var request = new WebPayRequest();
            request.AddGatewayData(new WebPayModel()
            {
                TotalAmount = total_amount,
                OrderId = order_id
            });

            var response = _gateway.Execute(request);
            return Content(response.Html, "text/html", Encoding.UTF8);
        }

        /// <summary>
        ///     手机网站支付
        /// </summary>
        /// <param name="order_id">订单号</param>
        /// <param name="total_amount">金额（单位：分）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WapPay(string order_id, int total_amount)
        {
            var request = new WapPayRequest();
            request.AddGatewayData(new WapPayModel()
            {
                TotalAmount = total_amount,
                OrderId = order_id
            });

            var response = _gateway.Execute(request);
            return Content(response.Html, "text/html", Encoding.UTF8);
        }
        /// <summary>
        /// APP支付 
        /// </summary>
        /// <param name="order_id">订单号</param>
        /// <param name="total_amount">金额（单位：分）</param>
        /// <param name="body">描述</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AppPay(string order_id, int total_amount, string body)
        {
            var request = new AppPayRequest();
            request.AddGatewayData(new AppPayModel()
            {
                Body = body,
                TotalAmount = total_amount,
                OrderId = order_id
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     扫码支付
        /// </summary>
        /// <param name="order_id">订单号</param>
        /// <param name="total_amount">金额</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ScanPay(string order_id, int total_amount)
        {
            var request = new ScanPayRequest();
            request.AddGatewayData(new ScanPayModel()
            {
                TotalAmount = total_amount,
                OrderId = order_id
            });

            var response = _gateway.Execute(request);

            return Json(response);
        }
        /// <summary>
        ///     条码支付
        /// </summary>
        /// <param name="order_id">订单号</param>
        /// <param name="qr_no">二维码编码</param>
        /// <param name="total_amount">金额</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BarcodePay(string order_id, string qr_no, int total_amount)
        {
            var request = new BarcodePayRequest();
            request.AddGatewayData(new BarcodePayModel()
            {
                TotalAmount = total_amount,
                OrderId = order_id,
                QrNo = qr_no
            });
            request.PaySucceed += BarcodePay_PaySucceed;
            request.PayFailed += BarcodePay_PayFaild;

            var response = _gateway.Execute(request);

            return Json(response);
        }
        /// <summary>
        ///     撤销订单
        /// </summary>
        /// <param name="order_id">订单号</param>
        /// <param name="cancel_amount">金额</param>
        /// <param name="orig_qry_id">原始流水号</param>
        /// <param name="orig_order_id">原始订单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Cancel(string order_id, int cancel_amount, string orig_qry_id, string orig_order_id)
        {
            var request = new CancelRequest();
            request.AddGatewayData(new CancelModel()
            {
                OrderId = order_id,
                CancelAmount = cancel_amount,
                OrigOrderId = orig_order_id,
                OrigQryId = orig_qry_id
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     申请退款
        /// </summary>
        /// <param name="order_id">订单号</param>
        /// <param name="refund_amount">金额</param>
        /// <param name="orig_qry_id">原始流水号</param>
        /// <param name="orig_order_id">原始订单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Refund(string order_id, int refund_amount, string orig_qry_id, string orig_order_id)
        {
            var request = new RefundRequest();
            request.AddGatewayData(new RefundModel()
            {
                OrderId = order_id,
                RefundAmount = refund_amount,
                OrigOrderId = orig_order_id,
                OrigQryId = orig_qry_id
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     查询订单
        /// </summary>
        /// <param name="order_id">订单号</param>
        /// <param name="query_id">流水号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Query(string order_id, string query_id)
        {
            var request = new QueryRequest();
            request.AddGatewayData(new QueryModel()
            {
                OrderId = order_id,
                QueryId = query_id
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     下载对账单
        /// </summary>
        /// <param name="bill_date">MMDD</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BillDownload(string bill_date)
        {
            var request = new BillDownloadRequest();
            request.AddGatewayData(new BillDownloadModel()
            {
                BillDate = bill_date
            });

            var response = _gateway.Execute(request);
            return File(response.GetBillFile(), "application/zip");
        }

        /// <summary>
        /// 支付成功事件
        /// </summary>
        /// <param name="response">返回结果</param>
        /// <param name="message">提示信息</param>
        private void BarcodePay_PaySucceed(IResponse response, string message)
        {
        }

        /// <summary>
        /// 支付失败事件
        /// </summary>
        /// <param name="response">返回结果,可能是BarcodePayResponse/QueryResponse</param>
        /// <param name="message">提示信息</param>
        private void BarcodePay_PayFaild(IResponse response, string message)
        {
        }




    }
}
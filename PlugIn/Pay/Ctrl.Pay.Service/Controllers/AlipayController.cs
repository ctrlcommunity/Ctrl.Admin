using Ctrl.Plugin.Ali;
using Ctrl.Plugin.Ali.Domain;
using Ctrl.Plugin.Ali.Request;
using Ctrl.Plugin.PayCore.Gateways;
using Ctrl.Plugin.PayCore.Response;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;

namespace Ctrl.Pay.Service.Controllers
{
    /// <summary>
    ///  支付宝操作接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AlipayController : Controller
    {
        private readonly IGateway _gateway;

        public AlipayController(IGateways gateways)
        {
            _gateway = gateways.Get<AlipayGateway>();
        }

        /// <summary>
        ///     电脑网站支付
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="subject">订单标题</param>
        /// <param name="total_amount">总金额（单位为元精确到小数点后两位）</param>
        /// <param name="body">订单描述</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WebPay(string out_trade_no, string subject, double total_amount, string body)
        {
            var request = new WebPayRequest();
            request.AddGatewayData(new WebPayModel()
            {
                Body = body,
                TotalAmount = total_amount,
                Subject = subject,
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);
            return Content(response.Html, "text/html", Encoding.UTF8);
        }
        /// <summary>
        ///     手机网站支付
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="subject">订单标题</param>
        /// <param name="total_amount">总金额（单位为元精确到小数点后两位）</param>
        /// <param name="body">订单描述</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WapPay(string out_trade_no, string subject, double total_amount, string body)
        {
            var request = new WapPayRequest();
            request.AddGatewayData(new WapPayModel()
            {
                Body = body,
                TotalAmount = total_amount,
                Subject = subject,
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);
            return Redirect(response.Url);
        }
        /// <summary>
        ///     APP支付
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="subject">订单标题</param>
        /// <param name="total_amount">总金额（单位为元精确到小数点后两位）</param>
        /// <param name="body">订单描述</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AppPay(string out_trade_no, string subject, double total_amount, string body)
        {
            var request = new AppPayRequest();
            request.AddGatewayData(new AppPayModel()
            {
                Body = body,
                TotalAmount = total_amount,
                Subject = subject,
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     扫码支付
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="subject">订单标题</param>
        /// <param name="total_amount">总金额（单位为元精确到小数点后两位）</param>
        /// <param name="body">订单描述</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ScanPay(string out_trade_no, string subject, double total_amount, string body)
        {
            var request = new ScanPayRequest();
            request.AddGatewayData(new ScanPayModel()
            {
                Body = body,
                TotalAmount = total_amount,
                Subject = subject,
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);

            return Json(response);
        }
        /// <summary>
        ///     条码支付
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="auth_code">授权码</param>
        /// <param name="subject">订单标题</param>
        /// <param name="total_amount">总金额（单位为精确到小数点后两位）</param>
        /// <param name="body">订单描述</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BarcodePay(string out_trade_no, string auth_code, string subject, double total_amount, string body)
        {
            var request = new BarcodePayRequest();
            request.AddGatewayData(new BarcodePayModel()
            {
                Body = body,
                TotalAmount = total_amount,
                Subject = subject,
                OutTradeNo = out_trade_no,
                AuthCode = auth_code
            });
            request.PaySucceed += BarcodePay_PaySucceed;
            request.PayFailed += BarcodePay_PayFaild;

            var response = _gateway.Execute(request);

            return Json(response);
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
        /// <summary>
        ///     交易查询
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="trade_no">支付宝订单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Query(string out_trade_no, string trade_no)
        {
            var request = new QueryRequest();
            request.AddGatewayData(new QueryModel()
            {
                TradeNo = trade_no,
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     交易退款
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="trade_no">支付宝订单号</param>
        /// <param name="refund_amount">退款金额</param>
        /// <param name="refund_reason">退款原因</param>
        /// <param name="out_request_no">退款号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Refund(string out_trade_no, string trade_no, double refund_amount, string refund_reason, string out_request_no)
        {
            var request = new RefundRequest();
            request.AddGatewayData(new RefundModel()
            {
                TradeNo = trade_no,
                OutTradeNo = out_trade_no,
                RefundAmount = refund_amount,
                RefundReason = refund_reason,
                OutRefundNo = out_request_no
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     退款查询
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="trade_no">支付宝订单号</param>
        /// <param name="out_request_no">退款号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RefundQuery(string out_trade_no, string trade_no, string out_request_no)
        {
            var request = new RefundQueryRequest();
            request.AddGatewayData(new RefundQueryModel()
            {
                TradeNo = trade_no,
                OutTradeNo = out_trade_no,
                OutRefundNo = out_request_no
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     交易撤销
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="trade_no">支付宝订单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Cancel(string out_trade_no, string trade_no)
        {
            var request = new CancelRequest();
            request.AddGatewayData(new CancelModel()
            {
                TradeNo = trade_no,
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///  交易关闭
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="trade_no">支付宝订单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Close(string out_trade_no, string trade_no)
        {
            var request = new CloseRequest();
            request.AddGatewayData(new CloseModel()
            {
                TradeNo = trade_no,
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     单笔转账
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="payee_account">收款方账户</param>
        /// <param name="payee_type">首款方账户类型</param>
        /// <param name="amount">转账金额</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Transfer(string out_trade_no, string payee_account, string payee_type, double amount, string remark)
        {
            var request = new TransferRequest();
            request.AddGatewayData(new TransferModel()
            {
                OutTradeNo = out_trade_no,
                PayeeAccount = payee_account,
                Amount = amount,
                Remark = remark,
                PayeeType = payee_type
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }

        /// <summary>
        ///     转账查询
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="trade_no">支付宝订单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TransferQuery(string out_trade_no, string trade_no)
        {
            var request = new TransferQueryRequest();
            request.AddGatewayData(new TransferQueryModel()
            {
                TradeNo = trade_no,
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     下载对账单
        /// </summary>
        /// <param name="bill_date">账单日期 日账单格式为yyyy-MM-dd，月账单格式为yyyy-MM</param>
        /// <param name="bill_type">账单类型</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> BillDownload(string bill_date, string bill_type)
        {
            var request = new BillDownloadRequest();
            request.AddGatewayData(new BillDownloadModel()
            {
                BillDate = bill_date,
                BillType = bill_type
            });

            var response = _gateway.Execute(request);
            return File(await response.GetBillFileAsync(), "application/zip");
        }
    }
}

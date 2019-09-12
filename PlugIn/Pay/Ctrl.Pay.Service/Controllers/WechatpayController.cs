using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ctrl.Plugin.PayCore.Gateways;
using Ctrl.Plugin.PayCore.Response;
using Ctrl.Plugin.Wx;
using Ctrl.Plugin.Wx.Domain;
using Ctrl.Plugin.Wx.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ctrl.Pay.Service.Controllers
{
    /// <summary>
    ///     微信接口操作
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WechatpayController : Controller
    {
        private readonly IGateway _gateway;

        public WechatpayController(IGateways gateways)
        {
            _gateway = gateways.Get<WechatpayGateway>();
        }
        /// <summary>
        ///     扫码支付
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="body">订单描述</param>
        /// <param name="total_amount">金额（单位为分）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ScanPay(string out_trade_no, string body, int total_amount)
        {
            var request = new ScanPayRequest();
            request.AddGatewayData(new ScanPayModel()
            {
                Body = body,
                TotalAmount = total_amount,
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);

            return Json(response);
        }
        /// <summary>
        ///     条码支付
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="auth_code">授权码(扫码支付授权码，设备读取用户微信中的条码或者二维码信息（注：用户刷卡条形码规则：18位纯数字，以10、11、12、13、14、15开头）)</param>
        /// <param name="total_amount">金额（单位为分）</param>
        /// <param name="body">订单描述</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BarcodePay(string out_trade_no, string auth_code, int total_amount, string body)
        {
            var request = new BarcodePayRequest();
            request.AddGatewayData(new BarcodePayModel()
            {
                Body = body,
                TotalAmount = total_amount,
                OutTradeNo = out_trade_no,
                AuthCode = auth_code
            });
            request.PaySucceed += BarcodePay_PaySucceed;
            request.PayFailed += BarcodePay_PayFaild;

            var response = _gateway.Execute(request);

            return Json(response);
        }
        /// <summary>
        ///     公众号支付
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="total_amount">金额(单位为分)</param>
        /// <param name="body">订单描述</param>
        /// <param name="open_id">用户标识</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PublicPay(string out_trade_no, int total_amount, string body, string open_id)
        {
            var request = new PublicPayRequest();
            request.AddGatewayData(new PublicPayModel()
            {
                Body = body,
                OutTradeNo = out_trade_no,
                TotalAmount = total_amount,
                OpenId = open_id
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     微信APP支付
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="total_amount">金额（单位为分）</param>
        /// <param name="body">订单描述</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AppPay(string out_trade_no, int total_amount, string body)
        {
            var request = new AppPayRequest();
            request.AddGatewayData(new AppPayModel()
            {
                Body = body,
                TotalAmount = total_amount,
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     微信小程序支付
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="total_amount">金额（单位为分）</param>
        /// <param name="body">订单描述</param>
        /// <param name="open_id">用户标识</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AppletPay(string out_trade_no, int total_amount, string body, string open_id)
        {
            var request = new AppletPayRequest();
            request.AddGatewayData(new AppletPayModel()
            {
                Body = body,
                OutTradeNo = out_trade_no,
                TotalAmount = total_amount,
                OpenId = open_id
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     微信H5支付
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="total_amount">金额（单位为分）</param>
        /// <param name="body">订单描述</param>
        /// <param name="scene_info">场景信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult WapPay(string out_trade_no, int total_amount, string body, string scene_info)
        {
            var request = new WapPayRequest();
            request.AddGatewayData(new WapPayModel()
            {
                Body = body,
                TotalAmount = total_amount,
                OutTradeNo = out_trade_no,
                SceneInfo = scene_info
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     查询订单
        /// </summary>
        /// <param name="out_trade_no">微信订单号</param>
        /// <param name="trade_no">订单号</param>
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
        ///     关闭订单
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Close(string out_trade_no)
        {
            var request = new CloseRequest();
            request.AddGatewayData(new CloseModel()
            {
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     撤销订单
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Cancel(string out_trade_no)
        {
            var request = new CancelRequest();
            request.AddGatewayData(new CancelModel()
            {
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     申请退款
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="trade_no">微信订单号</param>
        /// <param name="total_amount">金额（单位为分）</param>
        /// <param name="refund_amount">退款金额（单位为分）</param>
        /// <param name="refund_desc">退款描述</param>
        /// <param name="out_refund_no">退款号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Refund(string out_trade_no, string trade_no, int total_amount, int refund_amount, string refund_desc, string out_refund_no)
        {
            var request = new RefundRequest();
            request.AddGatewayData(new RefundModel()
            {
                TradeNo = trade_no,
                RefundAmount = refund_amount,
                RefundDesc = refund_desc,
                OutRefundNo = out_refund_no,
                TotalAmount = total_amount,
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     查询退款
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="trade_no">微信订单号</param>
        /// <param name="out_refund_no">退货订单号</param>
        /// <param name="refund_no">微信退款订单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RefundQuery(string out_trade_no, string trade_no, string out_refund_no, string refund_no)
        {
            var request = new RefundQueryRequest();
            request.AddGatewayData(new RefundQueryModel()
            {
                TradeNo = trade_no,
                OutTradeNo = out_trade_no,
                OutRefundNo = out_refund_no,
                RefundNo = refund_no
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     下载对账单
        /// </summary>
        /// <param name="bill_date">下载对账单的日期(格式：yyyyMMdd)</param>
        /// <param name="bill_type">账单类型(ALL，返回当日所有订单信息，默认值;SUCCESS，返回当日成功支付的订单;REFUND，返回当日退款订单;RECHARGE_REFUND，返回当日充值退款订单)</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BillDownload(string bill_date, string bill_type)
        {
            var request = new BillDownloadRequest();
            request.AddGatewayData(new BillDownloadModel()
            {
                BillDate = bill_date,
                BillType = bill_type
            });

            var response = _gateway.Execute(request);
            return File(response.GetBillFile(), "text/csv", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv");
        }
        /// <summary>
        ///     下载资金账单
        /// </summary>
        /// <param name="bill_date">下载资金账单日期（格式：yyyyMMdd）</param>
        /// <param name="account_type">资金账户类型（账单的资金来源账户：Basic  基本账户 Operation 运营账户 Fees 手续费账户）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FundFlowDownload(string bill_date, string account_type)
        {
            var request = new FundFlowDownloadRequest();
            request.AddGatewayData(new FundFlowDownloadModel()
            {
                BillDate = bill_date,
                AccountType = account_type
            });

            var response = _gateway.Execute(request);
            return File(response.GetBillFile(), "text/csv", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv");
        }
        /// <summary>
        ///     企业付款到零钱
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="openid">用户openid</param>
        /// <param name="check_name">校验用户姓名选项（NO_CHECK：不校验真实姓名 FORCE_CHECK：强校验真实姓名）</param>
        /// <param name="true_name">收款用户姓名</param>
        /// <param name="amount">付款金额</param>
        /// <param name="desc">付款说明</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Transfer(string out_trade_no, string openid, string check_name, string true_name, int amount, string desc)
        {
            var request = new TransferRequest();
            request.AddGatewayData(new TransferModel()
            {
                OutTradeNo = out_trade_no,
                OpenId = openid,
                Amount = amount,
                Desc = desc,
                CheckName = check_name,
                TrueName = true_name
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     查询企业付款零钱
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TransferQuery(string out_trade_no)
        {
            var request = new TransferQueryRequest();
            request.AddGatewayData(new TransferQueryModel()
            {
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     获取RSA公钥
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PublicKey()
        {
            var request = new PublicKeyRequest();

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     企业付款到银行卡
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <param name="bank_no">首款方银行卡号</param>
        /// <param name="true_name">收款方用户名</param>
        /// <param name="bank_code">收款方开户行（银行卡所在开户行编号,详见银行编号列表）</param>
        /// <param name="amount">付款金额</param>
        /// <param name="desc">付款说明</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TransferToBank(string out_trade_no, string bank_no, string true_name, string bank_code, int amount, string desc)
        {
            var request = new TransferToBankRequest();
            request.AddGatewayData(new TransferToBankModel()
            {
                OutTradeNo = out_trade_no,
                BankNo = bank_no,
                Amount = amount,
                Desc = desc,
                BankCode = bank_code,
                TrueName = true_name
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     查询企业付款银行卡
        /// </summary>
        /// <param name="out_trade_no">订单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TransferToBankQuery(string out_trade_no)
        {
            var request = new TransferToBankQueryRequest();
            request.AddGatewayData(new TransferToBankQueryModel()
            {
                OutTradeNo = out_trade_no
            });

            var response = _gateway.Execute(request);
            return Json(response);
        }
        /// <summary>
        ///     获取OepnId
        /// </summary>
        /// <param name="code">微信Code</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OAuth(string code)
        {
            var request = new OAuthRequest();
            request.AddGatewayData(new OAuthModel()
            {
                Code = code
            });

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

    }
}

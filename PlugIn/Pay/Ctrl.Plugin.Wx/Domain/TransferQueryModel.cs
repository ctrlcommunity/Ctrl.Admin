﻿using Ctrl.Plugin.PayCore;
using Ctrl.Plugin.PayCore.Utils;
using System.ComponentModel.DataAnnotations;

namespace Ctrl.Plugin.Wx.Domain
{
    public class TransferQueryModel
    {
        /// <summary>
        /// 商户订单号
        /// </summary>
        [ReName(Name = "partner_trade_no")]
        [Required(ErrorMessage = "请设置商户订单号")]
        [StringLength(32, ErrorMessage = "商户订单号最大长度为32位")]
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        public string NonceStr { get; } = Util.GenerateNonceStr();
    }
}
